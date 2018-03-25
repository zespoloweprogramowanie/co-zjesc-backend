﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhatToEat.Domain.Models
{
    public class Recipe
    {
        public Recipe()
        {
            Products = new HashSet<RecipeProduct>();
            Images = new HashSet<RecipeImage>();
            Tags = new HashSet<RecipeTag>();
            Comments = new HashSet<RecipeComment>();
            FavouriteRecipes = new HashSet<UserFavouriteRecipe>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public String Name { get; set; }

        [Required]
        public String Description { get; set; }

        [Required]
        public int? Difficulty { get; set; }

        [Required]
        public int? TimeToPrepare { get; set; }

        [Required]
        public int? EstimatedCost { get; set; }

        [Required]
        public int? PortionCount { get; set; }

        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public RecipeCategory Category { get; set; }

        public string AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public User Author { get; set; }

        public int? AverageGrade { get; set; }  //średnia ocen

        public int? AverageVote { get; set; }   //średnia głosów

        public virtual ICollection<RecipeRate> Rate { get; set; }

        public virtual ICollection<RecipeProduct> Products { get; set; }

        public virtual ICollection<RecipeImage> Images { get; set; }

        public virtual ICollection<RecipeTag> Tags { get; set; }

        public virtual ICollection<RecipeComment> Comments { get; set; }

        public virtual ICollection<UserFavouriteRecipe> FavouriteRecipes { get; set; }
    }



    public class UploadRecipeImagesResult
    {
        public string relativeUrl { get; set; }
        public string absoluteUrl { get; set; }
    }

    public class GetRecipeDTO
    {
        public int id { get; set; }

        public List<GetRecipeDTOProduct> products { get; set; }

        public List<UploadRecipeImagesResult> images { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public int? difficulty { get; set; }

        public int? timeToPrepare { get; set; }

        public List<GetRecipeDTOTag> tags { get; set; }

        public double? estimatedCost { get; set; }

        public int? portionCount { get; set; }
    }

    public class GetRecipeDTOProduct
    {
        public int id { get; set; }

        public string name { get; set; }

        public GetRecipeDTOUnit unit { get; set; }

        public double amount { get; set; }
    }

    public class GetRecipeDTOUnit
    {
        public int id { get; set; }

        public string label { get; set; }
    }

    public class GetRecipeDTOTag
    {
        public int id { get; set; }

        public string name { get; set; }
    }
}