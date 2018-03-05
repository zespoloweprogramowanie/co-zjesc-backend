using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhatToEat.Domain.Models
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