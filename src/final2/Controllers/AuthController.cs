using Toys.HelperClasses;
using Toys.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;

namespace Toys.Controllers
{
    public class AuthController : BaseController
    {
        private UserManager<User> m_UserManager;
        private SignInManager<User> m_SigninManager;

        public AuthController(UserManager<User> userManager, SignInManager<User> signinManager)
        { 
            m_UserManager = userManager;
            m_SigninManager = signinManager;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Register(LoginViewModel model)
        {
            bool success = await RegisterInternal(model);

            if (!success)
            {
                return View(model);
            }

            AddInfo("User created");

            return RedirectToAction("Login");
        }

        private async Task<bool> RegisterInternal(LoginViewModel model)
        {
            var user = await m_UserManager.FindByNameAsync(model.UserName);

            if (user != null)
            {
                AddError("User already exists!");
                return false;
            }

            var result = await m_UserManager.CreateAsync(new User
            {
                UserName = model.UserName,
                Password = model.Password
            });

            if (!result.Succeeded)
            {
                AddError("Failed creating user, try again later!");
                return false;
            }

            user = await m_UserManager.FindByNameAsync(model.UserName);

            await m_UserManager.AddToRoleAsync(user, UserRoles.NormalUser);

            return true;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                AddError("Incorrect data!");
                return View(model);
            }

            var user = await m_UserManager.FindByNameAsync(model.UserName);

            if (user == null || user.Password != model.Password)
            {
                AddError("Incorrect username or password!");
                return View(model);
            }

            await m_SigninManager.SignInAsync(user, isPersistent: model.RememberMe);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Logout()
        {
            await m_SigninManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_UserManager != null)
                {
                    m_UserManager.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
