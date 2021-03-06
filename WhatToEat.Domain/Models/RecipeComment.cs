﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhatToEat.Domain.Models
{
    public class RecipeComment
    {
        [Key]
        public int Id { get; set; }

        public int RecipeId { get; set; }

        [ForeignKey("RecipeId")]
        public Recipe Recipe {get;set;}

        public string AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public User Author { get; set; }

        [MaxLength(500)]
        public String Content { get; set; }

        public DateTime Created { get; set; }

        public bool Accepted { get; set; }
    }
}