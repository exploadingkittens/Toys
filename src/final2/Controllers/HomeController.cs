using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using toysRus.DAL;
using Microsoft.Data.Entity;

namespace toysRus.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(FinalContext context)
        {
            Context = context;
        }

        private FinalContext Context { get; set; }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("ItemsInCategory/{categoryId}")]
        public async Task<IActionResult> ItemsInCategory(int categoryId)
        {
            var category = await Context.Categories.FirstAsync(cat => cat.ID == categoryId);

            if (category == null)
            {
                AddError($"Category with id {categoryId} doesn't exist");
            }

            ViewBag.ChosenCategory = category;
            return View("Index");
        }

        [HttpGet]
        [Route("ClearCategory")]
        public IActionResult ClearCategory()
        {
            return RedirectToAction("Index");
        }
    }
}