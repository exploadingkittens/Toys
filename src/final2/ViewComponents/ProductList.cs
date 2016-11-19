using Toys.DAL;
using Toys.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toys.ViewComponents
{
    public class ToyListByCategory : ViewComponent
    {
        public ToyListByCategory(FinalContext context)
        {
            Context = context;
        }

        private FinalContext Context { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(int categoryId)
        {
            if (!Context.Categories.Any(c => c.ID == categoryId))
            {
                throw new Exception("Category does not exist");
            }

            IQueryable<Toy> query = Context.Toys
                        .Where(p => p.Available > 0 &&
                                    p.Category.ID == categoryId)
                        .OrderBy(p => p.Name);

            query = query.Include(p => p.Seller);


            return View(await query.ToListAsync());
        }
    }

    public class AllToysList : ViewComponent
    {
        public AllToysList(FinalContext context)
        {
            Context = context;
        }

        private FinalContext Context { get; set; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IQueryable<Toy> query = Context.Toys
                        .Where(p => p.Available > 0)
                        .OrderBy(p => p.Name);

            query = query.Include(p => p.Seller);

            return View(await query.ToListAsync());
        }
    }

    public class ToyListFromList : ViewComponent
    {
        public ToyListFromList(FinalContext context)
        {
            Context = context;
        }

        private FinalContext Context { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<Toy> toys)
        {
            var toyList = await Task.Run(() => toys.ToList());
            return View(toyList);
        }
    }
}
