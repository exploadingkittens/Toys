using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Toys.Models
{
    [NotMapped]
    public class LoginViewModel : User
    {
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
