#nullable disable
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using A_Little_Source_Of_Hope.Data;
using A_Little_Source_Of_Hope.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace A_Little_Source_Of_Hope.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _AppDb;

        public IndexModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            AppDbContext AppDb)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _AppDb = AppDb; ;
        }

        public string Username { get; set; }
        public string ImageSrc { get; set; }
        public IEnumerable<SelectListItem> GenderList { get; set; }
        public IEnumerable<SelectListItem> CityList { get; set; }
        public IEnumerable<SelectListItem> ProvinceList { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Please enter an email address.")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Please enter a first name.")]
            [DataType(DataType.Text)]
            [Display(Name = "First name")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Please enter a last name.")]
            [DataType(DataType.Text)]
            [Display(Name = "Last name")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Please enter a phone number.")]
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            //[Required(ErrorMessage = "Please select a gender.")]
            [Display(Name = "Gender")]
            public string GenderId { get; set; }
            public IFormFile ImageFile { get; set; }

            [Required(ErrorMessage = "Please enter a city.")]
            [DataType(DataType.Text)]
            [Display(Name = "Address line 1")]
            public string AddressLine1 { get; set; }

            [Required(ErrorMessage = "Please enter a city.")]
            [DataType(DataType.Text)]
            [Display(Name = "Address line 2")]
            public string AddressLine2 { get; set; }

            //[Required(ErrorMessage = "Please select a city.")]
            [Display(Name = "City")]
            public string CityId { get; set; }
            [Required]
            [Display(Name = "Province")]
            public string ProvinceId { get; set; }

            //[Required(ErrorMessage = "Please select a city.")]
            [DataType(DataType.PostalCode)]
            [Display(Name = "Postal code")]
            public string PostalCode { get; set; }
        }

        private async Task LoadAsync(AppUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            //var Cities = await _AppDb.City.ToListAsync();
            //var Cities = await _AppDb.City.ToListAsync();
            //var Cities = await _AppDb.City.ToListAsync();
            Username = userName;
            GenderList = _AppDb.Gender.Select(x => new SelectListItem() { Text = x.GenderName, Value = x.Id.ToString() }).AsEnumerable();
            CityList = _AppDb.City.Select(x => new SelectListItem() { Text = x.CityName, Value = x.Id.ToString() }).AsEnumerable();
            ProvinceList = _AppDb.Province.Select(x => new SelectListItem() { Text = x.ProvinceName, Value = x.Id.ToString() }).AsEnumerable();
            ImageSrc = user.ImageUrl;

            Input = new InputModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = phoneNumber,
                AddressLine1 = user.AddressLine1,
                AddressLine2 = user.AddressLine2,
                PostalCode = user.PostalCode
            };
            if (user.Gender == null && Input.GenderId == null)
            {
                Input.GenderId = "1";
            }
            else if (user.Gender == null && Input.GenderId != null)
            {
                Input.GenderId = GenderList.FirstOrDefault(x => x.Value == Input.GenderId).Value;
            }
            else if (user.Gender != null && Input.GenderId == null)
            {
                Input.GenderId = GenderList.FirstOrDefault(x => x.Text == user.Gender).Value;
            }
            else
            {
                Input.GenderId = GenderList.FirstOrDefault(x => x.Value == Input.GenderId).Value;
            }

            if (user.City == null && Input.CityId == null)
            {
                Input.CityId = "1";
            }
            else if (user.City == null && Input.CityId != null)
            {
                Input.CityId = CityList.FirstOrDefault(x => x.Value == Input.CityId).Value;
            }
            else if (user.City != null && Input.CityId == null)
            {
                Input.CityId = CityList.FirstOrDefault(x => x.Text == user.City).Value;
            }
            else
            {
                Input.CityId = CityList.FirstOrDefault(x => x.Value == Input.CityId).Value;
            }
            if (user.Province == null && Input.ProvinceId == null)
            {
                Input.ProvinceId = "1";
            }
            else if (user.Province == null && Input.ProvinceId != null)
            {
                Input.ProvinceId = ProvinceList.FirstOrDefault(x => x.Value == Input.ProvinceId).Value;
            }
            else if (user.Province != null && Input.ProvinceId == null)
            {
                Input.ProvinceId = ProvinceList.FirstOrDefault(x => x.Text == user.Province).Value;
            }
            else
            {
                Input.ProvinceId = ProvinceList.FirstOrDefault(x => x.Value == Input.ProvinceId).Value;
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            if (Input.Email != user.Email)
            {
                user.Email = Input.Email;
            }
            if (Input.FirstName != user.FirstName)
            {
                user.FirstName = Input.FirstName;
            }
            if (Input.LastName != user.LastName)
            {
                user.LastName = Input.LastName;
            }
            if (Input.ImageFile != null)
            {
                var filename = Input.ImageFile.FileName;
                var fileExt = Path.GetExtension(filename);
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(Input.ImageFile.FileName);
                string myfile = fileNameWithoutExt + "_" + user.Id + fileExt;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\User");
                string fileNameWithPath = Path.Combine(path, myfile);
                if (fileNameWithPath != user.ImageUrl)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    //if (System.IO.File.Exists(fileNameWithPath))
                    //{
                    //    System.IO.File.Delete(fileNameWithPath);
                    //}
                    user.ImageUrl = $"images/User/{myfile}";
                    //using (var stream = new FileStream(path, FileMode.Create))
                    //{
                    //    Input.ImageFile.CopyTo(stream);
                    //}
                }
            }
            GenderList = _AppDb.Gender.Select(x => new SelectListItem() { Text = x.GenderName, Value = x.Id.ToString() }).AsEnumerable();
            var selectedGender = GenderList.FirstOrDefault(x => x.Value == Input.GenderId);
            if (user.Gender == null)
            {
                user.Gender = selectedGender.Text;
            }
            else
            {
                if (selectedGender.Text != user.Gender)
                {
                    user.Gender = selectedGender.Text;
                }
            }

            CityList = _AppDb.City.Select(x => new SelectListItem() { Text = x.CityName, Value = x.Id.ToString() }).AsEnumerable();
            var selectedCity = CityList.FirstOrDefault(x => x.Value == Input.CityId);
            if (user.City == null)
            {
                user.City = selectedCity.Text;
            }
            else
            {
                if (selectedCity.Text != user.City)
                {
                    user.City = selectedCity.Text;
                }
            }

            ProvinceList = _AppDb.Province.Select(x => new SelectListItem() { Text = x.ProvinceName, Value = x.Id.ToString() }).AsEnumerable();
            var selectedProvince = ProvinceList.FirstOrDefault(x => x.Value == Input.ProvinceId);
            if (user.Province == null)
            {
                user.Province = selectedProvince.Text;
            }
            else
            {
                if (selectedProvince.Text != user.Province)
                {
                    user.Province = selectedProvince.Text;
                }
            }

            if (Input.AddressLine1 != user.AddressLine1)
            {
                user.AddressLine1 = Input.AddressLine1;
            }
            if (Input.AddressLine2 != user.AddressLine2)
            {
                user.AddressLine2 = Input.AddressLine2;
            }
            if (Input.PostalCode != user.PostalCode)
            {
                user.PostalCode = Input.PostalCode;
            }

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
