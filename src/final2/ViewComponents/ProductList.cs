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

            IQueryable<Toy> query = Context.Products
                        .Where(p => p.Available > 0 &&
                                    p.Category.ID == categoryId)
                        .OrderBy(p => p.Name);

            query = query.Include(p => p.Seller);


            return View(await query.ToListAsync());
        }
    }

    public class AllProductsList : ViewComponent
    {
        public AllProductsList(FinalContext context)
        {
            Context = context;
        }

        private FinalContext Context { get; set; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IQueryable<Toy> query = Context.Products
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

        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<Toy> products)
        {
            var productList = await Task.Run(() => products.ToList());
            return View(productList);
        }
    }
}
