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
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }
            var userManager = serviceProvider.GetService<UserManager<AppUser>>() ?? throw new Exception("The userManager returned a null.");
            var user = await userManager.FindByIdAsync(uid) ?? throw new Exception("The testUserPw password was probably not strong enough!");
            IR = await userManager.AddToRoleAsync(user, role);
            return IR;
        }
        public static async Task SeedDataOnDb(AppDbContext context)
        {
            if (context.Category.Any()) { return; }
            else
            {
                await context.Category.AddRangeAsync(new Category
                {
                    CategoryName = "Men",
                    Imageurl = "images/Categories/shirt.png",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                }, new Category
                {
                    CategoryName = "Women",
                    Imageurl = "images/Categories/dress.png",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                });
            }
            //await context.SaveChangesAsync();
            if (context.Product.Any()) { return; }
            else
            {
                await context.Product.AddRangeAsync(new Product
                {
                    ProductName = "Shirt",
                    Description = "Grey men's shirt size S",
                    Price = Math.Round(((decimal)50), 2),
                    Quantity = 1,
                    Imageurl = "images/Products/shirt.png",
                    CategoryId = 1,
                    IsActive = true,
                    ClaimStatus = true,
                    CreatedDate = DateTime.Now
                }, new Product
                {
                    ProductName = "Shirt",
                    Description = "Blue men's shirt size 9",
                    Price = Math.Round(((decimal)50), 2),
                    Quantity = 5,
                    Imageurl = "images/Products/shirt1.png",
                    CategoryId = 1,
                    IsActive = true,
                    ClaimStatus = false,
                    CreatedDate = DateTime.Now
                }, new Product
                {
                    ProductName = "Shirt",
                    Description = "black men's shirt size M",
                    Price = Math.Round(((decimal)50), 2),
                    Quantity = 1,
                    Imageurl = "images/Products/shirt2.png",
                    CategoryId = 1,
                    IsActive = false,
                    ClaimStatus = false,
                    CreatedDate = DateTime.Now
                }, new Product
                {
                    ProductName = "Shirt",
                    Description = "Strong blue men's shirt size 14",
                    Price = Math.Round(((decimal)60), 2),
                    Quantity = 7,
                    Imageurl = "images/Products/shirt3.png",
                    CategoryId = 1,
                    IsActive = true,
                    ClaimStatus = false,
                    CreatedDate = DateTime.Now
                }, new Product
                {
                    ProductName = "Shirt",
                    Description = "Sky blue men's shrit size XL",
                    Price = Math.Round(((decimal)50), 2),
                    Quantity = 1,
                    Imageurl = "images/Products/shirt4.png",
                    CategoryId = 1,
                    IsActive = false,
                    ClaimStatus = false,
                    CreatedDate = DateTime.Now
                }, new Product
                {
                    ProductName = "Shirt",
                    Description = "Red and white stripe men's shirt size 12",
                    Price = Math.Round(((decimal)60), 2),
                    Quantity = 2,
                    Imageurl = "images/Products/shirt5.png",
                    CategoryId = 1,
                    IsActive = false,
                    ClaimStatus = true,
                    CreatedDate = DateTime.Now
                }, new Product
                {
                    ProductName = "Dress",
                    Description = "Red short dress",
                    Price = Math.Round(((decimal)50), 2),
                    Quantity = 3,
                    Imageurl = "images/Products/dress.png",
                    CategoryId = 2,
                    IsActive = true,
                    ClaimStatus = true,
                    CreatedDate = DateTime.Now
                }, new Product
                {
                    ProductName = "Dress",
                    Description = "Purple long dress",
                    Price = Math.Round(((decimal)60), 2),
                    Quantity = 2,
                    Imageurl = "images/Products/dress1.png",
                    CategoryId = 2,
                    IsActive = true,
                    ClaimStatus = true,
                    CreatedDate = DateTime.Now
                }, new Product
                {
                    ProductName = "Dress",
                    Description = "Black long dress",
                    Price = Math.Round(((decimal)50), 2),
                    Quantity = 10,
                    Imageurl = "images/Products/dress2.png",
                    CategoryId = 2,
                    IsActive = true,
                    ClaimStatus = true,
                    CreatedDate = DateTime.Now
                }, new Product
                {
                    ProductName = "Dress",
                    Description = "Grey long dress",
                    Price = Math.Round(((decimal)85), 2),
                    Quantity = 2,
                    Imageurl = "images/Products/dress3.png",
                    CategoryId = 2,
                    IsActive = false,
                    ClaimStatus = false,
                    CreatedDate = DateTime.Now
                }, new Product
                {
                    ProductName = "Dress",
                    Description = "long Black wedding dress",
                    Price = Math.Round(((decimal)50), 2),
                    Quantity = 2,
                    Imageurl = "images/Products/dress4.png",
                    CategoryId = 2,
                    IsActive = true,
                    ClaimStatus = true,
                    CreatedDate = DateTime.Now
                }, new Product
                {
                    ProductName = "Dress",
                    Description = "lite purple dress size 34",
                    Price = Math.Round(((decimal)60), 2),
                    Quantity = 9,
                    Imageurl = "images/Products/dress5.png",
                    CategoryId = 2,
                    IsActive = true,
                    ClaimStatus = true,
                    CreatedDate = DateTime.Now
                });
            }
            //await context.SaveChangesAsync();
            if (context.Gender.Any()) { return; }
            else
            {
                await context.Gender.AddRangeAsync(new Gender
                {
                    GenderName = "Select gender"
                }, new Gender
                {
                    GenderName = "Female"
                }, new Gender
                {
                    GenderName = "Male"
                }, new Gender
                {
                    GenderName = "Prefer not to say"
                });
            }
            //await context.SaveChangesAsync();
            if (context.Province.Any()) { return; }
            else
            {
                await context.Province.AddRangeAsync(new Province
                {
                    ProvinceName = "Gauteng"
                }, new Province
                {
                    ProvinceName = "limpopo"
                }, new Province
                {
                    ProvinceName = "North west"
                }, new Province
                {
                    ProvinceName = "Free state"
                }, new Province
                {
                    ProvinceName = "Northern Cape"
                }, new Province
                {
                    ProvinceName = "Western Cape"
                }, new Province
                {
                    ProvinceName = "Mpumalanga"
                }, new Province
                {
                    ProvinceName = "Eastern Cape"
                }, new Province
                {
                    ProvinceName = "Kwazulu Natal"
                });
            }
            //await context.SaveChangesAsync();
            if (context.City.Any()) { return; }
            else
            {
                await context.City.AddRangeAsync(new City
                {
                    city = "Johannesburg",
                    provinceId = 1
                }, new City
                {
                    city = "Pretoria",
                    provinceId = 1
                }, new City
                {
                    city = "Soweto",
                    provinceId = 1
                }, new City
                {
                    city = "Centurion",
                    provinceId = 1
                }, new City
                {
                    city = "Krugersdorp",
                    provinceId = 1
                }, new City
                {
                    city = "Randburg",
                    provinceId = 1
                }, new City
                {
                    city = "Roodepoort",
                    provinceId = 1
                }, new City
                {
                    city = "Kempton Park",
                    provinceId = 1
                }, new City
                {
                    city = "Boksburg",
                    provinceId = 1
                }, new City
                {
                    city = "Benoni",
                    provinceId = 1
                }, new City
                {
                    city = "Germiston",
                    provinceId = 1
                }, new City
                {
                    city = "Brakpan",
                    provinceId = 1
                }, new City
                {
                    city = "Springs",
                    provinceId = 1
                }, new City
                {
                    city = "Vanderbijlpark",
                    provinceId = 1
                }, new City
                {
                    city = "Vereeniging",
                    provinceId = 1
                }, new City
                {
                    city = "Midrand",
                    provinceId = 1
                }, new City
                {
                    city = "Alberton",
                    provinceId = 1
                }, new City
                {
                    city = "Nigel",
                    provinceId = 1
                }, new City
                {
                    city = "Heidelberg",
                    provinceId = 1
                }, new City
                {
                    city = "Edenvale",
                    provinceId = 1
                }, new City
                {
                    city = "Sandton",
                    provinceId = 1
                }, new City
                {
                    city = "Randfontein",
                    provinceId = 1
                }, new City
                {
                    city = "Westonaria",
                    provinceId = 1
                }, new City
                {
                    city = "Cullinan",
                    provinceId = 1
                }, new City
                {
                    city = "Bronkhorstspruit",
                    provinceId = 1
                }, new City
                {
                    city = "Ekurhuleni",
                    provinceId = 1
                }, new City
                {
                    city = "Tshwane",
                    provinceId = 1
                }, new City
                {
                    city = "Mabopane",
                    provinceId = 1
                }, new City
                {
                    city = "Polokwane (Pietersburg)",
                    provinceId = 2
                }, new City
                {
                    city = "Thohoyandou",
                    provinceId = 2
                }, new City
                {
                    city = "Mokopane (Potgietersrus)",
                    provinceId = 2
                }, new City
                {
                    city = "Musina (Messina)",
                    provinceId = 2
                }, new City
                {
                    city = "Louis Trichardt (Makhado)",
                    provinceId = 2
                }, new City
                {
                    city = "Phalaborwa",
                    provinceId = 2
                }, new City
                {
                    city = "Tzaneen",
                    provinceId = 2
                }, new City
                {
                    city = "Giyani",
                    provinceId = 2
                }, new City
                {
                    city = "Modimolle (Nylstroom)",
                    provinceId = 2
                }, new City
                {
                    city = "Bela-Bela (Warmbaths)",
                    provinceId = 2
                }, new City
                {
                    city = "Lephalale (Ellisras)",
                    provinceId = 2
                }, new City
                {
                    city = "Modjadjiskloof (Duiwelskloof)",
                    provinceId = 2
                }, new City
                {
                    city = "Mookgophong (Naboomspruit)",
                    provinceId = 2
                }, new City
                {
                    city = "Jane Furse",
                    provinceId = 2
                }, new City
                {
                    city = "Burgersfort",
                    provinceId = 2
                }, new City
                {
                    city = "Groblersdal",
                    provinceId = 2
                }, new City
                {
                    city = "Steelpoort",
                    provinceId = 2
                }, new City
                {
                    city = "Lebowakgomo",
                    provinceId = 2
                }, new City
                {
                    city = "Thabazimbi",
                    provinceId = 2
                }, new City
                {
                    city = "Marble Hall",
                    provinceId = 2
                }, new City
                {
                    city = "Vaalwater",
                    provinceId = 2
                }, new City
                {
                    city = "Letaba",
                    provinceId = 2
                }, new City
                {
                    city = "Alldays",
                    provinceId = 2
                }, new City
                {
                    city = "Makhado Air Force Base",
                    provinceId = 2
                }, new City
                {
                    city = "Bloemfontein",
                    provinceId = 3
                }, new City
                {
                    city = "Bethlehem",
                    provinceId = 3
                }, new City
                {
                    city = "Welkom",
                    provinceId = 3
                }, new City
                {
                    city = "Sasolburg",
                    provinceId = 3
                }, new City
                {
                    city = "Parys",
                    provinceId = 3
                }, new City
                {
                    city = "Kroonstad",
                    provinceId = 3
                }, new City
                {
                    city = "Ficksburg",
                    provinceId = 3
                }, new City
                {
                    city = "Phuthaditjhaba",
                    provinceId = 3
                }, new City
                {
                    city = "Harrismith",
                    provinceId = 3
                }, new City
                {
                    city = "Virginia",
                    provinceId = 3
                }, new City
                {
                    city = "Thaba Nchu",
                    provinceId = 3
                }, new City
                {
                    city = "Koppies",
                    provinceId = 3
                }, new City
                {
                    city = "Ladybrand",
                    provinceId = 3
                }, new City
                {
                    city = "Wesselsbron",
                    provinceId = 3
                }, new City
                {
                    city = "Odendaalsrus",
                    provinceId = 3
                }, new City
                {
                    city = "Viljoenskroon",
                    provinceId = 3
                }, new City
                {
                    city = "Reitz",
                    provinceId = 3
                }, new City
                {
                    city = "Heilbron",
                    provinceId = 3
                }, new City
                {
                    city = "Hennenman",
                    provinceId = 3
                }, new City
                {
                    city = "Bothaville",
                    provinceId = 3
                }, new City
                {
                    city = "Senekal",
                    provinceId = 3
                }, new City
                {
                    city = "Ventersburg",
                    provinceId = 3
                }, new City
                {
                    city = "Virginia",
                    provinceId = 3
                }, new City
                {
                    city = "Winburg",
                    provinceId = 3
                }, new City
                {
                    city = "Zastron",
                    provinceId = 3
                }, new City
                {
                    city = "Clocolan",
                    provinceId = 3
                }, new City
                {
                    city = "Clarens",
                    provinceId = 3
                }, new City
                {
                    city = "Fouriesburg",
                    provinceId = 3
                }, new City
                {
                    city = "Marquard",
                    provinceId = 3
                }, new City
                {
                    city = "Memel",
                    provinceId = 3
                }, new City
                {
                    city = "Paul Roux",
                    provinceId = 3
                }, new City
                {
                    city = "Rosendal",
                    provinceId = 3
                }, new City
                {
                    city = "Soutpan",
                    provinceId = 3
                }, new City
                {
                    city = "Caledonspoort",
                    provinceId = 3
                }, new City
                {
                    city = "Tweespruit",
                    provinceId = 3
                }, new City
                {
                    city = "Bloemfontein",
                    provinceId = 4
                }, new City
                {
                    city = "Bethlehem",
                    provinceId = 4
                }, new City
                {
                    city = "Welkom",
                    provinceId = 4
                }, new City
                {
                    city = "Sasolburg",
                    provinceId = 4
                }, new City
                {
                    city = "Parys",
                    provinceId = 4
                }, new City
                {
                    city = "Kroonstad",
                    provinceId = 4
                }, new City
                {
                    city = "Ficksburg",
                    provinceId = 4
                }, new City
                {
                    city = "Phuthaditjhaba",
                    provinceId = 4
                }, new City
                {
                    city = "Harrismith",
                    provinceId = 4
                }, new City
                {
                    city = "Virginia",
                    provinceId = 4
                }, new City
                {
                    city = "Thaba Nchu",
                    provinceId = 4
                }, new City
                {
                    city = "Koppies",
                    provinceId = 4
                }, new City
                {
                    city = "Ladybrand",
                    provinceId = 4
                }, new City
                {
                    city = "Wesselsbron",
                    provinceId = 4
                }, new City
                {
                    city = "Odendaalsrus",
                    provinceId = 4
                }, new City
                {
                    city = "Viljoenskroon",
                    provinceId = 4
                }, new City
                {
                    city = "Reitz",
                    provinceId = 4
                }, new City
                {
                    city = "Heilbron",
                    provinceId = 4
                }, new City
                {
                    city = "Hennenman",
                    provinceId = 4
                }, new City
                {
                    city = "Bothaville",
                    provinceId = 4
                }, new City
                {
                    city = "Senekal",
                    provinceId = 4
                }, new City
                {
                    city = "Ventersburg",
                    provinceId = 4
                }, new City
                {
                    city = "Virginia",
                    provinceId = 4
                }, new City
                {
                    city = "Winburg",
                    provinceId = 4
                }, new City
                {
                    city = "Zastron",
                    provinceId = 4
                }, new City
                {
                    city = "Clocolan",
                    provinceId = 4
                }, new City
                {
                    city = "Clarens",
                    provinceId = 4
                }, new City
                {
                    city = "Fouriesburg",
                    provinceId = 4
                }, new City
                {
                    city = "Marquard",
                    provinceId = 4
                }, new City
                {
                    city = "Memel",
                    provinceId = 4
                }, new City
                {
                    city = "Paul Roux",
                    provinceId = 4
                }, new City
                {
                    city = "Rosendal",
                    provinceId = 4
                }, new City
                {
                    city = "Soutpan",
                    provinceId = 4
                }, new City
                {
                    city = "Caledonspoort",
                    provinceId = 4
                }, new City
                {
                    city = "Tweespruit",
                    provinceId = 4
                }, new City
                {
                    city = "Kimberley",
                    provinceId = 5
                }, new City
                {
                    city = "Upington",
                    provinceId = 5
                }, new City
                {
                    city = "Kathu",
                    provinceId = 5
                }, new City
                {
                    city = "Kuruman",
                    provinceId = 5
                }, new City
                {
                    city = "Springbok",
                    provinceId = 5
                }, new City
                {
                    city = "De Aar",
                    provinceId = 5
                }, new City
                {
                    city = "Postmasburg",
                    provinceId = 5
                }, new City
                {
                    city = "Prieska",
                    provinceId = 5
                }, new City
                {
                    city = "Danielskuil",
                    provinceId = 5
                }, new City
                {
                    city = "Jan Kempdorp",
                    provinceId = 5
                }, new City
                {
                    city = "Warrenton",
                    provinceId = 5
                }, new City
                {
                    city = "Keimoes",
                    provinceId = 5
                }, new City
                {
                    city = "Ritchie",
                    provinceId = 5
                }, new City
                {
                    city = "Hartswater",
                    provinceId = 5
                }, new City
                {
                    city = "Kakamas",
                    provinceId = 5
                }, new City
                {
                    city = "Strydenburg",
                    provinceId = 5
                }, new City
                {
                    city = "Colesberg",
                    provinceId = 5
                }, new City
                {
                    city = "Groblershoop",
                    provinceId = 5
                }, new City
                {
                    city = "Victoria West",
                    provinceId = 5
                }, new City
                {
                    city = "Carnarvon",
                    provinceId = 5
                }, new City
                {
                    city = "Williston",
                    provinceId = 5
                }, new City
                {
                    city = "Calvinia",
                    provinceId = 5
                }, new City
                {
                    city = "Nieuwoudtville",
                    provinceId = 5
                }, new City
                {
                    city = "Douglas",
                    provinceId = 5
                }, new City
                {
                    city = "Kenhardt",
                    provinceId = 5
                }, new City
                {
                    city = "Sutherland",
                    provinceId = 5
                }, new City
                {
                    city = "Andriesvale",
                    provinceId = 5
                }, new City
                {
                    city = "Askham",
                    provinceId = 5
                }, new City
                {
                    city = "Augrabies",
                    provinceId = 5
                }, new City
                {
                    city = "Pofadder",
                    provinceId = 5
                }, new City
                {
                    city = "Griekwastad",
                    provinceId = 5
                }, new City
                {
                    city = "Hopetown",
                    provinceId = 5
                }, new City
                {
                    city = "Brandvlei",
                    provinceId = 5
                }, new City
                {
                    city = "Cape Town",
                    provinceId = 6
                }, new City
                {
                    city = "Stellenbosch",
                    provinceId = 6
                }, new City
                {
                    city = "Paarl",
                    provinceId = 6
                }, new City
                {
                    city = "Franschhoek",
                    provinceId = 6
                }, new City
                {
                    city = "Somerset West",
                    provinceId = 6
                }, new City
                {
                    city = "Worcester",
                    provinceId = 6
                }, new City
                {
                    city = "Hermanus",
                    provinceId = 6
                }, new City
                {
                    city = "George",
                    provinceId = 6
                }, new City
                {
                    city = "Knysna",
                    provinceId = 6
                }, new City
                {
                    city = "Mossel Bay",
                    provinceId = 6
                }, new City
                {
                    city = "Oudtshoorn",
                    provinceId = 6
                }, new City
                {
                    city = "Beaufort West",
                    provinceId = 6
                }, new City
                {
                    city = "Saldanha",
                    provinceId = 6
                }, new City
                {
                    city = "Vredenburg",
                    provinceId = 6
                }, new City
                {
                    city = "Langebaan",
                    provinceId = 6
                }, new City
                {
                    city = "Swellendam",
                    provinceId = 6
                }, new City
                {
                    city = "Ceres",
                    provinceId = 6
                }, new City
                {
                    city = "Robertson",
                    provinceId = 6
                }, new City
                {
                    city = "Montagu",
                    provinceId = 6
                }, new City
                {
                    city = "Wellington",
                    provinceId = 6
                }, new City
                {
                    city = "Malmesbury",
                    provinceId = 6
                }, new City
                {
                    city = "Darling",
                    provinceId = 6
                }, new City
                {
                    city = "Clanwilliam",
                    provinceId = 6
                }, new City
                {
                    city = "Tulbagh",
                    provinceId = 6
                }, new City
                {
                    city = "McGregor",
                    provinceId = 6
                }, new City
                {
                    city = "Paternoster",
                    provinceId = 6
                }, new City
                {
                    city = "Arniston",
                    provinceId = 6
                }, new City
                {
                    city = "De Rust",
                    provinceId = 6
                }, new City
                {
                    city = "De Rust",
                    provinceId = 6
                }, new City
                {
                    city = "Montague",
                    provinceId = 6
                }, new City
                {
                    city = "Calitzdorp",
                    provinceId = 6
                }, new City
                {
                    city = "Nelspruit (Mbombela)",
                    provinceId = 7
                }, new City
                {
                    city = "Witbank (eMalahleni)",
                    provinceId = 7
                }, new City
                {
                    city = "Middelburg",
                    provinceId = 7
                }, new City
                {
                    city = "Secunda",
                    provinceId = 7
                }, new City
                {
                    city = "Ermelo",
                    provinceId = 7
                }, new City
                {
                    city = "Barberton",
                    provinceId = 7
                }, new City
                {
                    city = "Standerton",
                    provinceId = 7
                }, new City
                {
                    city = "Hazyview",
                    provinceId = 7
                }, new City
                {
                    city = "Lydenburg",
                    provinceId = 7
                }, new City
                {
                    city = "Piet Retief",
                    provinceId = 7
                }, new City
                {
                    city = "Malelane",
                    provinceId = 7
                }, new City
                {
                    city = "Komatipoort",
                    provinceId = 7
                }, new City
                {
                    city = "Sabie",
                    provinceId = 7
                }, new City
                {
                    city = "White River",
                    provinceId = 7
                }, new City
                {
                    city = "Belfast",
                    provinceId = 7
                }, new City
                {
                    city = "Carolina",
                    provinceId = 7
                }, new City
                {
                    city = "Graskop",
                    provinceId = 7
                }, new City
                {
                    city = "Dullstroom",
                    provinceId = 7
                }, new City
                {
                    city = "Kwamhlanga",
                    provinceId = 7
                }, new City
                {
                    city = "Balfour",
                    provinceId = 7
                }, new City
                {
                    city = "Badplaas",
                    provinceId = 7
                }, new City
                {
                    city = "Volksrust",
                    provinceId = 7
                }, new City
                {
                    city = "Machadodorp",
                    provinceId = 7
                }, new City
                {
                    city = "Bethal",
                    provinceId = 7
                }, new City
                {
                    city = "Greylingstad",
                    provinceId = 7
                }, new City
                {
                    city = "Marloth Park",
                    provinceId = 7
                }, new City
                {
                    city = "Delmas",
                    provinceId = 7
                }, new City
                {
                    city = "Amersfoort",
                    provinceId = 7
                }, new City
                {
                    city = "Morgenzon",
                    provinceId = 7
                }, new City
                {
                    city = "Wakkerstroom",
                    provinceId = 7
                }, new City
                {
                    city = "Trichardt",
                    provinceId = 7
                }, new City
                {
                    city = "Chrissiesmeer",
                    provinceId = 7
                }, new City
                {
                    city = "East London",
                    provinceId = 8
                }, new City
                {
                    city = "Port Elizabeth",
                    provinceId = 8
                }, new City
                {
                    city = "Mthatha",
                    provinceId = 8
                }, new City
                {
                    city = "Bhisho",
                    provinceId = 8
                }, new City
                {
                    city = "Queenstown",
                    provinceId = 8
                }, new City
                {
                    city = "King William's Town",
                    provinceId = 8
                }, new City
                {
                    city = "Grahamstown",
                    provinceId = 8
                }, new City
                {
                    city = "Graaff-Reinet",
                    provinceId = 8
                }, new City
                {
                    city = "Port St. Johns",
                    provinceId = 8
                }, new City
                {
                    city = "Butterworth",
                    provinceId = 8
                }, new City
                {
                    city = "Aliwal North",
                    provinceId = 8
                }, new City
                {
                    city = "Somerset East",
                    provinceId = 8
                }, new City
                {
                    city = "Fort Beaufort",
                    provinceId = 8
                }, new City
                {
                    city = "Cradock",
                    provinceId = 8
                }, new City
                {
                    city = "Sterkspruit",
                    provinceId = 8
                }, new City
                {
                    city = "Mount Frere",
                    provinceId = 8
                }, new City
                {
                    city = "Dutywa",
                    provinceId = 8
                }, new City
                {
                    city = "Lady Frere",
                    provinceId = 8
                }, new City
                {
                    city = "Maclear",
                    provinceId = 8
                }, new City
                {
                    city = "Elliot",
                    provinceId = 8
                }, new City
                {
                    city = "Cofimvaba",
                    provinceId = 8
                }, new City
                {
                    city = "Flagstaff",
                    provinceId = 8
                }, new City
                {
                    city = "Keiskammahoek",
                    provinceId = 8
                }, new City
                {
                    city = "Matatiele",
                    provinceId = 8
                }, new City
                {
                    city = "Nqamakwe",
                    provinceId = 8
                }, new City
                {
                    city = "Libode",
                    provinceId = 8
                }, new City
                {
                    city = "Qumbu",
                    provinceId = 8
                }, new City
                {
                    city = "Lusikisiki",
                    provinceId = 8
                }, new City
                {
                    city = "Willowvale",
                    provinceId = 8
                }, new City
                {
                    city = "Ngqamakhwe",
                    provinceId = 8
                }, new City
                {
                    city = "Centane",
                    provinceId = 8
                }, new City
                {
                    city = "Tsolo",
                    provinceId = 8
                }, new City
                {
                    city = "Stutterheim",
                    provinceId = 8
                }, new City
                {
                    city = "Peddie",
                    provinceId = 8
                }, new City
                {
                    city = "Idutywa",
                    provinceId = 8
                }, new City
                {
                    city = "Komga",
                    provinceId = 8
                }, new City
                {
                    city = "Whittlesea",
                    provinceId = 8
                }, new City
                {
                    city = "Mount Ayliff",
                    provinceId = 8
                }, new City
                {
                    city = "Hogsback",
                    provinceId = 8
                }, new City
                {
                    city = "Bisho",
                    provinceId = 8
                }, new City
                {
                    city = "Durban",
                    provinceId = 9
                }, new City
                {
                    city = "Pietermaritzburg",
                    provinceId = 9
                }, new City
                {
                    city = "Newcastle",
                    provinceId = 9
                }, new City
                {
                    city = "Richards Bay",
                    provinceId = 9
                }, new City
                {
                    city = "Ladysmith",
                    provinceId = 9
                }, new City
                {
                    city = "Amanzimtoti",
                    provinceId = 9
                }, new City
                {
                    city = "Ballito",
                    provinceId = 9
                }, new City
                {
                    city = "Port Shepstone",
                    provinceId = 9
                }, new City
                {
                    city = "Margate",
                    provinceId = 9
                }, new City
                {
                    city = "Howick",
                    provinceId = 9
                }, new City
                {
                    city = "Umhlanga",
                    provinceId = 9
                }, new City
                {
                    city = "Dundee",
                    provinceId = 9
                }, new City
                {
                    city = "Vryheid",
                    provinceId = 9
                }, new City
                {
                    city = "Estcourt",
                    provinceId = 9
                }, new City
                {
                    city = "Scottburgh",
                    provinceId = 9
                }, new City
                {
                    city = "Kokstad",
                    provinceId = 9
                }, new City
                {
                    city = "Verulam",
                    provinceId = 9
                }, new City
                {
                    city = "Stanger (KwaDukuza)",
                    provinceId = 9
                }, new City
                {
                    city = "Eshowe",
                    provinceId = 9
                }, new City
                {
                    city = "Ixopo",
                    provinceId = 9
                }, new City
                {
                    city = "Port Edward",
                    provinceId = 9
                }, new City
                {
                    city = "Mooi River",
                    provinceId = 9
                }, new City
                {
                    city = "Empangeni",
                    provinceId = 9
                }, new City
                {
                    city = "Bergville",
                    provinceId = 9
                }, new City
                {
                    city = "Hluhluwe",
                    provinceId = 9
                }, new City
                {
                    city = "Melmoth",
                    provinceId = 9
                }, new City
                {
                    city = "Mount Edgecombe",
                    provinceId = 9
                }, new City
                {
                    city = "Tongaat",
                    provinceId = 9
                }, new City
                {
                    city = "Ulundi",
                    provinceId = 9
                }, new City
                {
                    city = "Gingindlovu",
                    provinceId = 9
                });
            }     
            await context.SaveChangesAsync();
        }
    }
}