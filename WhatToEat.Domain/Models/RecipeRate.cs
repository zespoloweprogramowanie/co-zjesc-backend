using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhatToEat.Domain.Models
{
    public class RecipeRate
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Recipe")]
        public int RecipeId { get; set; }

        public Recipe Recipe { get; set; }

        [ForeignKey("Rate")]
        public int RateId { get; set; }

        public Rate Rate { get; set; }
    }
}