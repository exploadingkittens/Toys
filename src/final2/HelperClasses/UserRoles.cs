using toysRus.Models;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace toysRus.HelperClasses
{
    public static class UserRoles
    {
        public const string NormalUser = "User";
        public const string Admin = "Admin";

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.IsSignedIn() &&
                   user.Claims.Any() && 
                   user.Claims.Last().Value == Admin;
        }

        internal static async Task<User> GetUser(ClaimsPrincipal user, UserManager<User> userManager)
        {
            return await userManager.FindByIdAsync(user.GetUserId());
        }
    }
}
