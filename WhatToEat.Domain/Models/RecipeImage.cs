using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhatToEat.Domain.Models
{
    public class RecipeImage
    {
        [Key]
        public int Id { get; set; }

        public string Path { get; set; }

        [ForeignKey("Recipe")]
        public int RecipeId { get; set; }

        public Recipe Recipe { get; set; }
    }
}