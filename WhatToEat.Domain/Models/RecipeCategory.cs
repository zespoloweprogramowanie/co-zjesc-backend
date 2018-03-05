using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhatToEat.Domain.Models
{
    public class RecipeCategory
    {
        public RecipeCategory()
        {
            Recipes = new HashSet<Recipe>();
        }

        [Key]
        public int Id { get; set; }

        [DisplayName("Nazwa")]
        public String Name { get; set; }

        [DisplayName("Kategoria nadrzędna")]
        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public RecipeCategory ParentCategory { get; set; }

        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}