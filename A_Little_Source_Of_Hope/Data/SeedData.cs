using A_Little_Source_Of_Hope.Areas.Identity.Data;
using A_Little_Source_Of_Hope.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace A_Little_Source_Of_Hope.Data
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new AppDbContext(
             serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>());
            var adminID = await EnsureUser(serviceProvider, "@littl3Sourc3", "admin@outlook.com", "Employee");
            await EnsureRole(serviceProvider, adminID, Constants.ProductAdministratorsRole);
            var managerID = await EnsureUser(serviceProvider, "@littl3Sourc3", "manager@outlook.com", " Employee");
            await EnsureRole(serviceProvider, managerID, Constants.ProductAdministratorsRole);
            var OrphanageManagerID = await EnsureUser(serviceProvider, "@littl3Sourc3", "OrphanageManager@outlook.com", "Orphanage Manager");
            var CustomerID = await EnsureUser(serviceProvider, "@littl3Sourc3", "Customer@outlook.com", "Customer");
            await SeedDataOnDb(context);
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string testUserPw, string UserName, string UserType)
        {
            var userManager = serviceProvider.GetService<UserManager<AppUser>>() ?? throw new Exception("The userManager returned a null.");
            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = UserName,
                    EmailConfirmed = true,
                    Email = UserName,
                    UserType = UserType
                };
                await userManager.CreateAsync(user, testUserPw);
            }
            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }
            return user.Id;
        }
        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string uid, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>() ?? throw new Exception("roleManager null");
            IdentityResult IR;
            if (!await roleManager.RoleExistsAsync(role))
            {
                _ = await roleManager.CreateAsync(new IdentityRole(role));
            }
            var userManager = serviceProvider.GetService<UserManager<AppUser>>() ?? throw new Exception("The userManager returned a null.");
            var user = await userManager.FindByIdAsync(uid) ?? throw new Exception("The testUserPw password was probably not strong enough!");
            IR = await userManager.AddToRoleAsync(user, role);
            return IR;
        }
        public static async Task SeedDataOnDb(AppDbContext context)
        {
            if (context.Product.Any()) { return; }
            if (context.Category.Any()) { return; }
            await context.Category.AddRangeAsync(new Category
            {
                CategoryName = "Men",
                Imageurl = "images/Categories/shirt.png",
                IsActive = true,
                CreatedDate = DateTime.Now
            },new Category
            {
                CategoryName = "Women",
                Imageurl = "images/Categories/dress.png",
                IsActive = true,
                CreatedDate = DateTime.Now
            });
            await context.Product.AddRangeAsync(new Product
            {
                ProductName = "Shirt",
                Description = "Grey men's shirt size S",
                Price = Math.Round(((decimal)50),2),
                Quantity = 1,
                Imageurl = "images/Products/shirt.png",
                CategoryId = 1,
                IsActive = true,
                ClaimStatus = true,
                CreatedDate = DateTime.Now
            },new Product
            {
                ProductName = "Shirt",
                Description = "Blue men's shirt size 9",
                Price = Math.Round(((decimal)50),2),
                Quantity = 5,
                Imageurl = "images/Products/shirt1.png",
                CategoryId = 1,
                IsActive = true,
                ClaimStatus = false,
                CreatedDate = DateTime.Now
            },new Product
            {
                ProductName = "Shirt",
                Description = "black men's shirt size M",
                Price = Math.Round(((decimal)50),2),
                Quantity = 1,
                Imageurl = "images/Products/shirt2.png",
                CategoryId = 1,
                IsActive = false,
                ClaimStatus = false,
                CreatedDate = DateTime.Now
            },new Product
            {
                ProductName = "Shirt",
                Description = "Strong blue men's shirt size 14",
                Price = Math.Round(((decimal)60),2),
                Quantity = 7,
                Imageurl = "images/Products/shirt3.png",
                CategoryId = 1,
                IsActive = true,
                ClaimStatus = false,
                CreatedDate = DateTime.Now
            },new Product
            {
                ProductName = "Shirt",
                Description = "Sky blue men's shrit size XL",
                Price = Math.Round(((decimal)50),2),
                Quantity = 1,
                Imageurl = "images/Products/shirt4.png",
                CategoryId = 1,
                IsActive = false,
                ClaimStatus = false,
                CreatedDate = DateTime.Now
            },new Product
            {
                ProductName = "Shirt",
                Description = "Red and white stripe men's shirt size 12",
                Price = Math.Round(((decimal)60),2),
                Quantity = 2,
                Imageurl = "images/Products/shirt5.png",
                CategoryId = 1,
                IsActive = false,
                ClaimStatus = true,
                CreatedDate = DateTime.Now
            },new Product
            {
                ProductName = "Dress",
                Description = "Red short dress",
                Price = Math.Round(((decimal)50),2),
                Quantity = 3,
                Imageurl = "images/Products/dress.png",
                CategoryId = 2,
                IsActive = true,
                ClaimStatus = true,
                CreatedDate = DateTime.Now
            },new Product
            {
                ProductName = "Dress",
                Description = "Purple long dress",
                Price = Math.Round(((decimal)60),2),
                Quantity = 2,
                Imageurl = "images/Products/dress1.png",
                CategoryId = 2,
                IsActive = true,
                ClaimStatus = true,
                CreatedDate = DateTime.Now
            },new Product
            {
                ProductName = "Dress",
                Description = "Black long dress",
                Price = Math.Round(((decimal)50),2),
                Quantity = 10,
                Imageurl = "images/Products/dress2.png",
                CategoryId = 2,
                IsActive = true,
                ClaimStatus = true,
                CreatedDate = DateTime.Now
            },new Product
            {
                ProductName = "Dress",
                Description = "Grey long dress",
                Price = Math.Round(((decimal)85),2),
                Quantity = 2,
                Imageurl = "images/Products/dress3.png",
                CategoryId = 2,
                IsActive = false,
                ClaimStatus = false,
                CreatedDate = DateTime.Now
            },new Product
            {
                ProductName = "Dress",
                Description = "long Black wedding dress",
                Price = Math.Round(((decimal)50),2),
                Quantity = 2,
                Imageurl = "images/Products/dress4.png",
                CategoryId = 2,
                IsActive = true,
                ClaimStatus = true,
                CreatedDate = DateTime.Now
            },new Product
            {
                ProductName = "Dress",
                Description = "lite purple dress size 34",
                Price = Math.Round(((decimal)60),2),
                Quantity = 9,
                Imageurl = "images/Products/dress5.png",
                CategoryId = 2,
                IsActive = true,
                ClaimStatus = true,
                CreatedDate = DateTime.Now
            });
            await context.Gender.AddRangeAsync(new Gender
            {
                GenderName = "Select gender"
            },new Gender
            {
                GenderName = "Female"
            },new Gender
            {
                GenderName = "Male"
            },new Gender
            {
                GenderName = "Prefer not to say"
            });
            await context.SaveChangesAsync();
        }
    }
}