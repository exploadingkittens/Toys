using Toys.DAL;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toys.ViewComponents
{
    public class CategoryList : ViewComponent
    {
        public CategoryList(FinalContext context)
        {
            Context = context;
        }

        private FinalContext Context { get; set; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var query = Context.Categories
                        .OrderBy(c => c.Name)
                        .Include(c => c.Products);

            var list = await query.ToListAsync();

            return View(list);
        }
    }
}
