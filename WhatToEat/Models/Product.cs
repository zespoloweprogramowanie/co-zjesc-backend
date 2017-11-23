using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WhatToEat.Models
{
    public class Product
    {
        public Product()
        {

        }

        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        public String Name { get; set; }

        [MaxLength(500)]
        public String Image { get; set; }
    }
}