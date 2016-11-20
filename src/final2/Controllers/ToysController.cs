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
    public class ToyPostData : Toy
    {
        public ToyPostData()
        { }

        public ToyPostData(Toy data) : base(data)
        { }

        public ToyPostData(ToyPostData data) : this((Toy)data)
        {
            this.categoryID = data.categoryID;
        }

        [Required]
        [Display(Name = "Toy category")]
        public int categoryID { get; set; }
    }

    public class AddCategoryPostData
    {
        [Required]
        [Display(Name = "Toy category")]
        public string CategoryName { get; set; }
    }

    public class ToysController : BaseController
    {
        private FinalContext _context;
        private UserManager<User> _userManager;

        private const string USER_IMAGES_FOLDER_NAME = "UserImages";
        private readonly string USER_IMAGES_DIR;

        public ToysController(FinalContext context, UserManager<User> userManager, IApplicationEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;

            USER_IMAGES_DIR = Path.Combine(hostingEnvironment.ApplicationBasePath, "wwwroot", USER_IMAGES_FOLDER_NAME);
            Directory.CreateDirectory(USER_IMAGES_DIR);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyToys()
        {
            var user = await UserRoles.GetUser(User, _userManager);

            var userToys = _context.Toys
                                       .Include(p => p.Seller)
                                       .Where(p => p.Seller.Id == user.Id);

            return View(await userToys.ToListAsync());
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditToy(int toyID)
        {
            var toyFromDb = await _context.Toys
                                        .Include(p => p.Seller)
                                        .FirstOrDefaultAsync(p => p.ID == toyID);

            if (toyFromDb == null)
            {
                AddError($"Toy with ID {toyID} Doesn't exist");
                return RedirectToAction("Index", "Home");
            }

            if (toyFromDb.Seller.Id != User.GetUserId() &&
                !User.IsAdmin())
            {
                AddError($"Only the seller of the item or an admin can update a toy's details");
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();

            return View(new ToyPostData(toyFromDb));
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditToy(ToyPostData toy)
        {
            var toyFromDb = await _context.Toys
                                           .Include(p => p.Seller)
                                           .FirstOrDefaultAsync(p => p.ID == toy.ID);

            if (toyFromDb == null)
            {
                AddError($"Toy with ID {toy.ID} Doesn't exist");
                return RedirectToAction("Index", "Home");
            }

            var toyCategory = await _context.Categories.FirstOrDefaultAsync(c => c.ID == toy.categoryID);

            if (toyCategory == null)
            {
                AddError($"Category with ID {toy.categoryID} Doesn't exist");
                return RedirectToAction("Index", "Home");
            }

            if (toyFromDb.Seller.Id != User.GetUserId() &&
                !User.IsAdmin())
            {
                AddError($"Only the seller of the item or an admin can update a toy's details");
                return RedirectToAction("Index", "Home");
            }

            toyFromDb.Description = toy.Description;
            toyFromDb.ImageUrl = toy.ImageUrl;
            toyFromDb.Name = toy.Name;
            toyFromDb.Price = toy.Price;
            toyFromDb.Available = toy.Available;
            toyFromDb.Category = toyCategory;

            await _context.SaveChangesAsync();

            AddInfo($"Toy {toy.Name} succefully updated");

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
        public async Task<IActionResult> AddNew(ToyPostData toyPost)
        {
            if (!ModelState.IsValid)
            {
                AddError("Model not valid");
                return View(toyPost);
            }

            toyPost.Seller = await UserRoles.GetUser(User, _userManager);
            toyPost.Category = await _context.Categories.FirstAsync(c => c.ID == toyPost.categoryID);

            _context.Toys.Add(new Toy(toyPost));

            await _context.SaveChangesAsync();

            AddInfo($"Toy {toyPost.Name} added");

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
