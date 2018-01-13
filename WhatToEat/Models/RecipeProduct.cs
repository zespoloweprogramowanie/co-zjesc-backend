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
        public int ProductId { get; set; }

        public Product Product { get; set; }

        [ForeignKey("Recipe")]
        public int RecipeId { get; set; }

        public Recipe Recipe { get; set; }

        [ForeignKey("Unit")]
        public int UnitId { get; set; }

        public Unit Unit { get; set; }

        public double NumberOfUnit { get; set; }
    }
}