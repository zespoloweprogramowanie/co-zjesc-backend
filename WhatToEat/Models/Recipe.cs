using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WhatToEat.Models
{
    public class Recipe
    {
        //komentarz 
        public Recipe()
        {
            Products = new HashSet<Product>();
            Images = new HashSet<RecipeImage>();
        }

        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        public String Name { get; set; }

        public String Description { get; set; }

        public int? Difficulty { get; set; }

        public int? TimeToPrepare { get; set; }

        public int? EstimatedCost { get; set; }

        public int? PortionCount { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public virtual ICollection<RecipeImage> Images { get; set; }

        public virtual ICollection<RecipeTag> Tags { get; set; }
    }
}