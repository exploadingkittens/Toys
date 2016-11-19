﻿using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace toysRus.Models
{
    public class Category
    {
        [HiddenInput]
        [Required]
        public int ID { get; set; }

        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
