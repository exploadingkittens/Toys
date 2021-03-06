﻿using Toys.DAL;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
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
                        .Include(c => c.Toys);
            

            var list = await query.ToListAsync();

            return View(list);
        }
    }
}
