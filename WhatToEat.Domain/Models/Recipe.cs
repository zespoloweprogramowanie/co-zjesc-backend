using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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

        public DateTime CreatedDate { get; set; }

        public double AverageRate { get; set; } //średnia ocen

        //public int? AverageVote { get; set; }   //średnia głosów

        public virtual ICollection<RecipeRate> Rates { get; set; }

        public virtual ICollection<RecipeProduct> Products { get; set; }

        public virtual ICollection<RecipeImage> Images { get; set; }

        public virtual ICollection<RecipeTag> Tags { get; set; }

        public virtual ICollection<RecipeComment> Comments { get; set; }

        public virtual ICollection<UserFavouriteRecipe> FavouriteRecipes { get; set; }

        public bool CanUserVote(string userId) => Rates.All(x => x.UserId != userId);

        public bool IsUserFavourite(string userId) => FavouriteRecipes.Any(x => x.UserId == userId);
    }



    public class UploadRecipeImagesResult
    {
        public int Id { get; set; }
        public string RelativeUrl { get; set; }
        public string AbsoluteUrl { get; set; }
    }

    public class GetRecipeDto
    {
        public int Id { get; set; }

        public List<GetRecipeDtoProduct> Products { get; set; }

        public List<UploadRecipeImagesResult> Images { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? Difficulty { get; set; }

        public int? TimeToPrepare { get; set; }

        public List<GetRecipeDtoTag> Tags { get; set; }

        public double? EstimatedCost { get; set; }

        public int? PortionCount { get; set; }

        public CategoryDto Category { get; set; }

        public double AverageRate { get; set; }

        public bool? IsInFavorites { get; set; }

        public bool CanVote { get; set; }

        public class CategoryDto
        {
            public CategoryDto(RecipeCategory category)
            {
                if (category != null)
                {
                    Id = category.Id;
                    Name = category.Name;
                }
            }

            public int Id { get; set; }

            public String Name { get; set; }
        }
    }

    public class GetRecipeDtoProduct
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public GetRecipeDtoUnit Unit { get; set; }

        public double Amount { get; set; }
    }

    public class GetRecipeDtoUnit
    {
        public int Id { get; set; }

        public string Label { get; set; }
    }

    public class GetRecipeDtoTag
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}