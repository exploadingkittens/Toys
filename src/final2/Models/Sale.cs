using Toys.Models;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Toys.Models
{
    public class Sale
    {
        [HiddenInput]
        [Required]
        public int ID { get; set; }

        public DateTime SaleTime { get; set; }

        public int Amount { get; set; }

        public double TotalPrice { get; set; }

        public virtual Toy Product { get; set; }

        public virtual User User { get; set; }
    }
}
