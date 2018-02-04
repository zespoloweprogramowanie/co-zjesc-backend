using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WhatToEat.Models
{
    public class RecipeCategory
    {
        public RecipeCategory()
        {
            Recipes = new HashSet<Recipe>();
        }

        [Key]
        public int Id { get; set; }

        public String Name { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public RecipeCategory ParentCategory { get; set; }

        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}