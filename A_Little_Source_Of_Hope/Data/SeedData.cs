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
            var adminID = await EnsureUser(serviceProvider, "@littl3Sourc3", "Admin@outlook.com", "Employee");
            await EnsureRole(serviceProvider, adminID, Constants.AdministratorsRole);
            var CaregoryAdminID = await EnsureUser(serviceProvider, "@littl3Sourc3", "CategoryAdmin@outlook.com", "Employee");
            await EnsureRole(serviceProvider, CaregoryAdminID, Constants.CategoryAdministratorsRole);
            var NewsletterAdminID = await EnsureUser(serviceProvider, "@littl3Sourc3", "NewsletterAdmin@outlook.com", "Employee");
            await EnsureRole(serviceProvider, NewsletterAdminID, Constants.NewsLetterAdministratorsRole);
            var OrphanageAdminID = await EnsureUser(serviceProvider, "@littl3Sourc3", "OrphanageAdmin@outlook.com", "Employee");
            await EnsureRole(serviceProvider, OrphanageAdminID, Constants.OrphanageAdministratorsRole);
            var ProductAdminID = await EnsureUser(serviceProvider, "@littl3Sourc3", "ProductAdmin@outlook.com", "Employee");
            await EnsureRole(serviceProvider, ProductAdminID, Constants.ProductAdministratorsRole);
            var VolunteerAdminID = await EnsureUser(serviceProvider, "@littl3Sourc3", "VolunteerAdmin@outlook.com", "Employee");
            await EnsureRole(serviceProvider, VolunteerAdminID, Constants.VolunteerAdministratorsRole);
            var managerID = await EnsureUser(serviceProvider, "@littl3Sourc3", "manager@outlook.com", " Employee");
            await EnsureRole(serviceProvider, managerID, Constants.ProductAdministratorsRole);
            var OrphanageManagerID = await EnsureUser(serviceProvider, "@littl3Sourc3", "OrphanageManager@outlook.com", "Orphanage Manager");
            await EnsureRole(serviceProvider, OrphanageManagerID, Constants.OrphanageManagersRole);
            var OrphanageManagerID2 = await EnsureUser(serviceProvider, "@littl3Sourc3", "OrphanageManager1@outlook.com", "Orphanage Manager");
            await EnsureRole(serviceProvider, OrphanageManagerID2, Constants.OrphanageManagersRole);
            var CustomerID = await EnsureUser(serviceProvider, "@littl3Sourc3", "Customer@outlook.com", "Customer");
            await EnsureRole(serviceProvider, CustomerID, Constants.CustomersRole);
            await SeedDataOnDb(context);
            await SDOrphanage(context, OrphanageManagerID, OrphanageManagerID2);
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
                    UserType = UserType,
                    ImageUrl = "https://datapicsblobact.blob.core.windows.net/datapicscontainer1/images.png"
                };
                await userManager.CreateAsync(user, testUserPw);
            }
            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }
            return user.Id;
        }
        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string userId, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>() ?? throw new Exception("roleManager null");
            IdentityResult IR;
            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }
            var userManager = serviceProvider.GetService<UserManager<AppUser>>() ?? throw new Exception("The userManager returned a null.");
            var user = await userManager.FindByIdAsync(userId) ?? throw new Exception("The testUserPw password was probably not strong enough!");
            IR = await userManager.AddToRoleAsync(user, role);
            return IR;
        }
        public static async Task SeedDataOnDb(AppDbContext context)
        {
            if (await context.Category.AnyAsync()) { return; }
            else
            {
                await context.Category.AddRangeAsync(new Category
                {
                    CategoryName = "Men",
                    Imageurl = "https://datapicsblobact.blob.core.windows.net/datapicscontainer1/ white shirt.png",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                }, new Category
                {
                    CategoryName = "Women",
                    Imageurl = "https://datapicsblobact.blob.core.windows.net/datapicscontainer1/ red dress.png",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                });
                await context.SaveChangesAsync();
            }
            if (await context.Product.AnyAsync()) { return; }
            else
            {
                await context.Product.AddRangeAsync(new Product
                {
                    ProductName = "Shirt",
                    Description = "Grey men's shirt size S",
                    Price = Math.Round(((decimal)50), 2),
                    Quantity = 1,
                    Imageurl = "https://datapicsblobact.blob.core.windows.net/datapicscontainer1/shirt.png",
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
                    Imageurl = "https://datapicsblobact.blob.core.windows.net/datapicscontainer1/shirt1.png",
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
                    Imageurl = "https://datapicsblobact.blob.core.windows.net/datapicscontainer1/shirt2.png",
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
                    Imageurl = "https://datapicsblobact.blob.core.windows.net/datapicscontainer1/shirt3.png",
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
                    Imageurl = "https://datapicsblobact.blob.core.windows.net/datapicscontainer1/shirt4.png",
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
                    Imageurl = "https://datapicsblobact.blob.core.windows.net/datapicscontainer1/shirt5.png",
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
                    Imageurl = "https://datapicsblobact.blob.core.windows.net/datapicscontainer1/dress.png",
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
                    Imageurl = "https://datapicsblobact.blob.core.windows.net/datapicscontainer1/dress1.png",
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
                    Imageurl = "https://datapicsblobact.blob.core.windows.net/datapicscontainer1/dress2.png",
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
                    Imageurl = "https://datapicsblobact.blob.core.windows.net/datapicscontainer1/dress3.png",
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
                    Imageurl = "https://datapicsblobact.blob.core.windows.net/datapicscontainer1/dress4.png",
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
                    Imageurl = "https://datapicsblobact.blob.core.windows.net/datapicscontainer1/dress5.png",
                    CategoryId = 2,
                    IsActive = true,
                    ClaimStatus = true,
                    CreatedDate = DateTime.Now
                }); await context.SaveChangesAsync();
            }
            if (await context.Gender.AnyAsync()) { return; }
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
                }); await context.SaveChangesAsync();
            }
            if (await context.Province.AnyAsync()) { return; }
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
                }); await context.SaveChangesAsync();
            }
            if (await context.City.AnyAsync()) { return; }
            else
            {
                await context.City.AddRangeAsync(new City
                {
                    CityName = "Johannesburg",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Pretoria",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Soweto",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Centurion",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Krugersdorp",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Randburg",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Roodepoort",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Kempton Park",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Boksburg",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Benoni",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Germiston",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Brakpan",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Springs",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Vanderbijlpark",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Vereeniging",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Midrand",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Alberton",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Nigel",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Heidelberg",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Edenvale",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Sandton",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Randfontein",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Westonaria",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Cullinan",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Bronkhorstspruit",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Ekurhuleni",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Tshwane",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Mabopane",
                    ProvinceId = 1
                }, new City
                {
                    CityName = "Polokwane (Pietersburg)",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Thohoyandou",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Mokopane (Potgietersrus)",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Musina (Messina)",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Louis Trichardt (Makhado)",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Phalaborwa",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Tzaneen",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Giyani",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Modimolle (Nylstroom)",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Bela-Bela (Warmbaths)",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Lephalale (Ellisras)",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Modjadjiskloof (Duiwelskloof)",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Mookgophong (Naboomspruit)",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Jane Furse",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Burgersfort",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Groblersdal",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Steelpoort",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Lebowakgomo",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Thabazimbi",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Marble Hall",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Vaalwater",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Letaba",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Alldays",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Makhado Air Force Base",
                    ProvinceId = 2
                }, new City
                {
                    CityName = "Bloemfontein",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Bethlehem",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Welkom",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Sasolburg",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Parys",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Kroonstad",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Ficksburg",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Phuthaditjhaba",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Harrismith",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Virginia",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Thaba Nchu",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Koppies",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Ladybrand",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Wesselsbron",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Odendaalsrus",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Viljoenskroon",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Reitz",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Heilbron",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Hennenman",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Bothaville",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Senekal",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Ventersburg",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Virginia",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Winburg",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Zastron",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Clocolan",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Clarens",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Fouriesburg",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Marquard",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Memel",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Paul Roux",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Rosendal",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Soutpan",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Caledonspoort",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Tweespruit",
                    ProvinceId = 3
                }, new City
                {
                    CityName = "Bloemfontein",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Bethlehem",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Welkom",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Sasolburg",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Parys",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Kroonstad",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Ficksburg",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Phuthaditjhaba",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Harrismith",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Virginia",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Thaba Nchu",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Koppies",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Ladybrand",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Wesselsbron",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Odendaalsrus",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Viljoenskroon",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Reitz",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Heilbron",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Hennenman",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Bothaville",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Senekal",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Ventersburg",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Virginia",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Winburg",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Zastron",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Clocolan",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Clarens",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Fouriesburg",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Marquard",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Memel",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Paul Roux",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Rosendal",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Soutpan",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Caledonspoort",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Tweespruit",
                    ProvinceId = 4
                }, new City
                {
                    CityName = "Kimberley",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Upington",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Kathu",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Kuruman",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Springbok",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "De Aar",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Postmasburg",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Prieska",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Danielskuil",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Jan Kempdorp",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Warrenton",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Keimoes",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Ritchie",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Hartswater",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Kakamas",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Strydenburg",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Colesberg",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Groblershoop",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Victoria West",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Carnarvon",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Williston",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Calvinia",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Nieuwoudtville",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Douglas",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Kenhardt",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Sutherland",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Andriesvale",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Askham",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Augrabies",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Pofadder",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Griekwastad",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Hopetown",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Brandvlei",
                    ProvinceId = 5
                }, new City
                {
                    CityName = "Cape Town",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Stellenbosch",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Paarl",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Franschhoek",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Somerset West",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Worcester",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Hermanus",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "George",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Knysna",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Mossel Bay",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Oudtshoorn",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Beaufort West",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Saldanha",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Vredenburg",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Langebaan",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Swellendam",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Ceres",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Robertson",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Montagu",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Wellington",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Malmesbury",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Darling",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Clanwilliam",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Tulbagh",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "McGregor",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Paternoster",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Arniston",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "De Rust",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "De Rust",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Montague",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Calitzdorp",
                    ProvinceId = 6
                }, new City
                {
                    CityName = "Nelspruit (Mbombela)",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Witbank (eMalahleni)",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Middelburg",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Secunda",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Ermelo",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Barberton",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Standerton",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Hazyview",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Lydenburg",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Piet Retief",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Malelane",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Komatipoort",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Sabie",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "White River",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Belfast",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Carolina",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Graskop",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Dullstroom",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Kwamhlanga",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Balfour",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Badplaas",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Volksrust",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Machadodorp",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Bethal",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Greylingstad",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Marloth Park",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Delmas",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Amersfoort",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Morgenzon",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Wakkerstroom",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Trichardt",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "Chrissiesmeer",
                    ProvinceId = 7
                }, new City
                {
                    CityName = "East London",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Port Elizabeth",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Mthatha",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Bhisho",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Queenstown",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "King William's Town",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Grahamstown",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Graaff-Reinet",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Port St. Johns",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Butterworth",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Aliwal North",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Somerset East",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Fort Beaufort",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Cradock",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Sterkspruit",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Mount Frere",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Dutywa",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Lady Frere",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Maclear",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Elliot",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Cofimvaba",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Flagstaff",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Keiskammahoek",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Matatiele",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Nqamakwe",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Libode",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Qumbu",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Lusikisiki",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Willowvale",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Ngqamakhwe",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Centane",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Tsolo",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Stutterheim",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Peddie",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Idutywa",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Komga",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Whittlesea",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Mount Ayliff",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Hogsback",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Bisho",
                    ProvinceId = 8
                }, new City
                {
                    CityName = "Durban",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Pietermaritzburg",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Newcastle",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Richards Bay",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Ladysmith",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Amanzimtoti",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Ballito",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Port Shepstone",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Margate",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Howick",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Umhlanga",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Dundee",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Vryheid",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Estcourt",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Scottburgh",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Kokstad",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Verulam",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Stanger (KwaDukuza)",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Eshowe",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Ixopo",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Port Edward",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Mooi River",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Empangeni",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Bergville",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Hluhluwe",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Melmoth",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Mount Edgecombe",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Tongaat",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Ulundi",
                    ProvinceId = 9
                }, new City
                {
                    CityName = "Gingindlovu",
                    ProvinceId = 9
                });
            }
            await context.SaveChangesAsync();
        }

        public static async Task SDOrphanage(AppDbContext context, string UserId, string UserId2)
        {
            if (await context.Orphanage.AnyAsync()) { return; }
            else
            {
                await context.Orphanage.AddRangeAsync(new Orphanage
                {
                    OrphanageName = "The Hope",
                    OrphanageEmail = "Hope2023@gmail.com",
                    OrphanageAddress = "5 Avenue, Johannesburg,8130",
                    Manager = "Sheldon Cooper",
                    PhoneNumber = "0786178662",
                    AppUserId = UserId,


                }, new Orphanage
                {
                    OrphanageName = "The Home",
                    OrphanageEmail = "home23@outlook.com",
                    OrphanageAddress = "5 Ave Botha, Pretoria,0081",
                    Manager = "Rajesh Kothrappoli",
                    PhoneNumber = "0715566997",
                    AppUserId = UserId2,
                });
                await context.SaveChangesAsync();
            }

        }
    }
}