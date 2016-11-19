using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Toys.Models
{
    public class User: IdentityUser
    {
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public virtual ICollection<Toy> Pruducts { get; set; }
    }
}
