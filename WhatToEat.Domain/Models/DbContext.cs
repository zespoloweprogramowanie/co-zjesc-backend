﻿using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WhatToEat.Domain.Models
{
    public interface IContext
    {
        IDbSet<Recipe> Recipes { get; set; }
        IDbSet<Product> Products { get; set; }
        IDbSet<Unit> Unit { get; set; }
        IDbSet<RecipeProduct> RecipeProducts { get; set; }
        IDbSet<RecipeTag> RecipeTags { get; set; }
        IDbSet<RecipeCategory> RecipeCategories { get; set; }
        IDbSet<Log> Logs { get; set; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        int SaveChanges();
        Task<int> SaveChangesAsync();
        void Dispose();
    }

    public class AppDb : IdentityDbContext<User>, IContext
    {
        public AppDb()
            : base("DefaultConnection")
        {
        }

        //public virtual DbSet<Recipe> Recipes { get; set; }
        //public virtual DbSet<Product> Products { get; set; }
        //public virtual DbSet<Unit> Unit { get; set; }
        //public virtual DbSet<RecipeProduct> RecipeProducts { get; set; }
        //public virtual DbSet<RecipeTag> RecipeTags { get; set; }
        //public System.Data.Entity.DbSet<RecipeCategory> RecipeCategories { get; set; }

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

        public IDbSet<Recipe> Recipes { get; set; }
        public IDbSet<Product> Products { get; set; }
        public IDbSet<Unit> Unit { get; set; }
        public IDbSet<RecipeProduct> RecipeProducts { get; set; }
        public IDbSet<RecipeTag> RecipeTags { get; set; }
        public IDbSet<RecipeCategory> RecipeCategories { get; set; }
        public IDbSet<Log> Logs { get; set; }
    }
}