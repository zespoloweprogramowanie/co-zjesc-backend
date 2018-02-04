using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace WhatToEat.Models
{
    public class AppDb : IdentityDbContext<User>
    {
        public AppDb()
            : base("DefaultConnection")
        {
        }

        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<RecipeProduct> RecipeProducts { get; set; }
        public virtual DbSet<RecipeTag> RecipeTags { get; set; }

        public static AppDb Create()
        {
            return new AppDb();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            base.OnModelCreating(modelBuilder);

        }
    }
}