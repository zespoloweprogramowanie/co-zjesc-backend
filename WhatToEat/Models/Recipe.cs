using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WhatToEat.Models
{
    public class Recipe
    {
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

    public class RecipeDTO
    {
        public int id { get; set; }

        public List<int> products { get; set; }

        public List<string> images { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public int? difficulty { get; set; }

        public int? timeToPrepare { get; set; }

        public List<string> tags { get; set; }

        public int? estimatedCost { get; set; }

        public int? portionCount { get; set; }
    }
}