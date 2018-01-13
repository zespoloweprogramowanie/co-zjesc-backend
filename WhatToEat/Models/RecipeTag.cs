using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WhatToEat.Models
{
    public class RecipeTag
    {
        public RecipeTag()
        {
            Recipes = new HashSet<Recipe>();
        }

        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        public string Name { get; set; }

        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}