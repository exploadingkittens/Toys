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
    public class ProductListByCategory : ViewComponent
    {
        public ProductListByCategory(FinalContext context)
        {
            Context = context;
        }

        private FinalContext Context { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(int categoryId)
        {
            if (!Context.Categories.Any(c => c.ID == categoryId))
            {
                throw new Exception("fuck you");
            }

            IQueryable<Product> query = Context.Products
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
            IQueryable<Product> query = Context.Products
                        .Where(p => p.Available > 0)
                        .OrderBy(p => p.Name);

            query = query.Include(p => p.Seller);

            return View(await query.ToListAsync());
        }
    }

    public class ProductListFromList : ViewComponent
    {
        public ProductListFromList(FinalContext context)
        {
            Context = context;
        }

        private FinalContext Context { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<Product> products)
        {
            var productList = await Task.Run(() => products.ToList());
            return View(productList);
        }
    }
}
