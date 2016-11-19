using Toys.DAL;
using Toys.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Toys.HelperClasses;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using Microsoft.Net.Http.Headers;

namespace Toys.Controllers
{
    public class ProductPostData : Product
    {
        public ProductPostData()
        { }

        public ProductPostData(Product data) : base(data)
        { }

        public ProductPostData(ProductPostData data) : this((Product)data)
        {
            this.categoryID = data.categoryID;
        }

        [Required]
        [Display(Name = "Product category")]
        public int categoryID { get; set; }
    }

    public class AddCategoryPostData
    {
        [Required]
        [Display(Name = "Product category")]
        public string CategoryName { get; set; }
    }

    public class ProductsController : BaseController
    {
        private FinalContext _context;
        private UserManager<User> _userManager;

        private const string USER_IMAGES_FOLDER_NAME = "UserImages";
        private readonly string USER_IMAGES_DIR;

        public ProductsController(FinalContext context, UserManager<User> userManager, IApplicationEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;

            USER_IMAGES_DIR = Path.Combine(hostingEnvironment.ApplicationBasePath, "wwwroot", USER_IMAGES_FOLDER_NAME);
            Directory.CreateDirectory(USER_IMAGES_DIR);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyProducts()
        {
            var user = await UserRoles.GetUser(User, _userManager);

            var userProducts = _context.Products
                                       .Include(p => p.Seller)
                                       .Where(p => p.Seller.Id == user.Id);

            return View(await userProducts.ToListAsync());
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProduct(int productID)
        {
            var productFromDb = await _context.Products
                                        .Include(p => p.Seller)
                                        .FirstOrDefaultAsync(p => p.ID == productID);

            if (productFromDb == null)
            {
                AddError($"Product with ID {productID} Doesn't exist");
                return RedirectToAction("Index", "Home");
            }

            if (productFromDb.Seller.Id != User.GetUserId() &&
                !User.IsAdmin())
            {
                AddError($"Only the seller of the item or an admin can update a product's details");
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();

            return View(new ProductPostData(productFromDb));
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProduct(ProductPostData product)
        {
            var productFromDb = await _context.Products
                                           .Include(p => p.Seller)
                                           .FirstOrDefaultAsync(p => p.ID == product.ID);

            if (productFromDb == null)
            {
                AddError($"Product with ID {product.ID} Doesn't exist");
                return RedirectToAction("Index", "Home");
            }

            var productCategory = await _context.Categories.FirstOrDefaultAsync(c => c.ID == product.categoryID);

            if (productCategory == null)
            {
                AddError($"Category with ID {product.categoryID} Doesn't exist");
                return RedirectToAction("Index", "Home");
            }

            if (productFromDb.Seller.Id != User.GetUserId() &&
                !User.IsAdmin())
            {
                AddError($"Only the seller of the item or an admin can update a product's details");
                return RedirectToAction("Index", "Home");
            }

            productFromDb.ImageUrl = product.ImageUrl;
            productFromDb.Name = product.Name;
            productFromDb.Price = product.Price;
            productFromDb.Available = product.Available;
            productFromDb.Category = productCategory;

            await _context.SaveChangesAsync();

            AddInfo($"Product {product.Name} succefully updated");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadImage()
        {
            var files = HttpContext.Request.Form.Files;
            if (!files.Any())
            {
                return HttpBadRequest();
            }

            IFormFile pic = files["productImage"];

            return Ok(await SaveImage(pic));
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            var fileName = Path.GetRandomFileName();
            string inputFileName = ContentDispositionHeaderValue.Parse(image.ContentDisposition).FileName.Trim('"');

            fileName = Path.ChangeExtension(fileName, Path.GetExtension(inputFileName));

            await image.SaveAsAsync(Path.Combine(USER_IMAGES_DIR, fileName));

            return Url.Content(Path.Combine("~", USER_IMAGES_FOLDER_NAME, fileName).Replace('\\', '/'));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddNew()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddNew(ProductPostData prod)
        {
            if (!ModelState.IsValid)
            {
                AddError("Model not valid");
                return View(prod);
            }

            prod.Seller = await UserRoles.GetUser(User, _userManager);
            prod.Category = await _context.Categories.FirstAsync(c => c.ID == prod.categoryID);

            _context.Products.Add(new Product(prod));

            await _context.SaveChangesAsync();

            AddInfo($"Product {prod.Name} added");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult AddNewCategory()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> AddNewCategory(AddCategoryPostData prms)
        {
            var categoryName = prms.CategoryName;

            if (string.IsNullOrEmpty(categoryName))
            {
                AddError("Category name can't be empty!");
                return View(prms);
            }

            var existingCategory = await _context.Categories.FirstOrDefaultAsync(cat => cat.Name == categoryName);
            if (existingCategory != null)
            {
                AddError($"Category name {categoryName} already exists!");
                return View(prms);
            }

            _context.Categories.Add(new Category
            {
                Name = categoryName
            });

            await _context.SaveChangesAsync();

            AddInfo($"Category {prms.CategoryName} added");

            return RedirectToAction("ItemsInCategory", "Home", new
            {
                categoryId = (await _context.Categories.FirstAsync(cat => cat.Name == categoryName)).ID
            });
        }
    }
}
