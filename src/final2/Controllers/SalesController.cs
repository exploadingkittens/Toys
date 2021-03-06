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
            public int ToyId { get; set; }

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
        [HttpPost]
        public async Task<IActionResult> RemoveToy([FromBody]int ToyId)
        {
            await RemoveToyInternal(int.Parse(Request.Form["ToyId"].ToArray()[0]));
            
            return RedirectToAction("Index", "Home");
        }

        private async Task RemoveToyInternal(int toyId)
        {
            var toy = await _context.Toys.FirstOrDefaultAsync(p => p.ID == toyId);

            var currUser = await UserRoles.GetUser(User, _userManager);

            if (toy == null)
            {
                AddError($"No toy with id {toyId} was found");
                return;
            }
            if (toy.Seller != (currUser) && !UserRoles.IsAdmin(User))
            {
                AddError("You cannot remove a toy that is not yours!");
                return;
            }
            
            //if (toy.Seller != R)

            AddInfo("Toy - " + toy.Name + " - was deleted."); 
            _context.Toys.Remove(toy);
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

            var toy = await _context.Toys.FirstOrDefaultAsync(p => p.ID == prms.ToyId);

            if (toy == null)
            {
                AddError($"No toy with id {prms.ToyId} was found");
                return;
            }

            if (toy.Available < prms.Amount)
            {
                AddError($"Requested to purchase {prms.Amount} but there only {toy.Available} available");
                return;
            }

            toy.Available -= prms.Amount;

            _context.Sales.Add(new Sale
            {
                Amount = prms.Amount,
                Toy = toy,
                User = await UserRoles.GetUser(User, _userManager),
                SaleTime = DateTime.Now,
                TotalPrice = toy.Price * prms.Amount
            });

            await _context.SaveChangesAsync();
        }
    }
}