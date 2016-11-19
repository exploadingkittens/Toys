using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Toys.Models
{
    public class Product
    {
        public Product()
        {}

        public Product(Product other)
        {
            this.ID = other.ID;
            this.Name = other.Name;
            this.Description = other.Description;
            this.Price = other.Price;
            this.ImageUrl = other.ImageUrl;
            this.Available = other.Available;
            this.Category = other.Category;
            this.Seller = other.Seller;
        }

        [HiddenInput]
        [Required]
        public int ID { get; set; }

        [Required]
        [MinLength(3)]
        [Display(Name="Product Name")]
        public string Name { get; set; }

        [Required]
        [MinLength(10)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Range(0.1, double.MaxValue)]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Product Image")]
        public string ImageUrl { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Quantity")]
        public int Available { get; set; }

        public virtual Category Category { get; set; }

        public virtual User Seller { get; set; }
    }
}
