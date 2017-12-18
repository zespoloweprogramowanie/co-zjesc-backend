using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WhatToEat.Models
{
    public class RecipeProduct
    {
        public RecipeProduct()
        {
            
        }

        [Key]
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int Id_product { get; set; }

        [ForeignKey("Recipe")]
        public int Id_recipe { get; set; }

        public double Number_of_unit { get; set; }
    }
}