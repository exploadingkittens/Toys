using toysRus.DAL;
using toysRus.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace toysRus.Controllers
{
    public class ProductQueryParams
    {
        [Display(Name ="Product name like")]
        public string ProductName { get; set; }
        [Display(Name ="User name like")]
        public string UserName { get; set; }
        [Display(Name ="Max price")]
        public int? MaxPrice { get; set; }
    }

    public class AvailableProductsInCategoryParams
    {
        [Display(Name = "Product name like")]
        public string ProductName { get; set; }
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }
    }

    public class QueriesController : BaseController
    {
        private UserManager<User> _userManager;
        private FinalContext _context;

        public QueriesController(FinalContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult ProductQuery()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> ProductQuery(ProductQueryParams prms)
        {
            if (string.IsNullOrWhiteSpace(prms.ProductName) &&
                string.IsNullOrWhiteSpace(prms.UserName) &&
                prms.MaxPrice == null)
            {
                AddError("There must be at least one parameter for query");
                return View(prms);
            }

            IQueryable<Product> query = _context.Products;

            if (!string.IsNullOrWhiteSpace(prms.ProductName))
            {
                query = query.Where(p => p.Name.Contains(prms.ProductName));
            }

            if (!string.IsNullOrWhiteSpace(prms.UserName))
            {
                query = query.Where(p => p.Seller.UserName.Contains(prms.UserName));
            }

            if (prms.MaxPrice != null)
            {
                query = query.Where(p => p.Price < prms.MaxPrice.Value);
            }

            query = query.Include(p => p.Seller);

            return View("FilteredProducts", await query.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> AvailableProductQuery()
        {
            var categoryList = await _context.Categories.ToListAsync();
            categoryList.Insert(0, new Category()
            {
                ID = -1,
                Name = "Not chosen"
            });

            ViewBag.Categories = categoryList;

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AvailableProductQuery(AvailableProductsInCategoryParams prms)
        {
            bool isCategoryChosen = prms.CategoryId != null && prms.CategoryId.Value != -1;

            if (string.IsNullOrWhiteSpace(prms.ProductName) &&
                !isCategoryChosen)
            {
                AddError("There must be at least one parameter for query");
                return View(prms);
            }

            IQueryable<Product> query = _context.Products;

            if (!string.IsNullOrWhiteSpace(prms.ProductName))
            {
                query = query.Where(p => p.Name.Contains(prms.ProductName));
            }

            if (isCategoryChosen)
            {
                var category = await _context.Categories.FirstOrDefaultAsync(cat => cat.ID == prms.CategoryId.Value);

                if (category == null)
                {
                   AddError( $"Category with id {prms.CategoryId.Value} doesn't exist");
                    return View(prms);
                }

                query = query.Where(p => p.Category.ID == prms.CategoryId.Value);
            }

            query = query.Include(p => p.Seller);

            return View("FilteredProducts", await query.ToListAsync());
        }
    }
}
