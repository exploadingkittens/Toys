using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Toys.DAL;
using Toys.Models;
using Microsoft.AspNet.Authorization;
using Toys.HelperClasses;
using Microsoft.AspNet.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Toys.Controllers
{
    public class SalesController : BaseController
    {
        public class SaleParameter
        {
            [Display(Name = "Toy")]
            public int ProductId { get; set; }

            [Range(1, int.MaxValue)]
            public int Amount { get; set; }
        }

        private UserManager<User> _userManager;
        private FinalContext _context;

        public SalesController(FinalContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult SalesList()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> RemoveProduct([FromBody]int ProductId)
        {
            await RemoveProductInternal(ProductId);

            return RedirectToAction("Index", "Home");
        }

        private async Task RemoveProductInternal(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ID == productId);

            if (product == null)
            {
                AddError($"No product with id {productId} was found");
                return;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        [ValidateAntiForgeryToken]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> MakeSale(SaleParameter prms)
        {
            if (!ModelState.IsValid)
            {
                AddError($"Invalid model");
            }
            else
            {
                await MakeSaleInternal(prms);
            }

            return RedirectToAction("Index", "Home");
        }

        private async Task MakeSaleInternal(SaleParameter prms)
        {
            if (prms.Amount <= 0)
            {
                AddError("Amount must be a positive number");
                return;
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ID == prms.ProductId);

            if (product == null)
            {
                AddError($"No product with id {prms.ProductId} was found");
                return;
            }

            if (product.Available < prms.Amount)
            {
                AddError($"Requested to purchase {prms.Amount} but there only {product.Available} available");
                return;
            }

            product.Available -= prms.Amount;

            _context.Sales.Add(new Sale
            {
                Amount = prms.Amount,
                Product = product,
                User = await UserRoles.GetUser(User, _userManager),
                SaleTime = DateTime.Now,
                TotalPrice = product.Price * prms.Amount
            });

            await _context.SaveChangesAsync();
        }
    }
}