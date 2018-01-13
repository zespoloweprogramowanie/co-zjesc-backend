using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;
using System;

namespace WhatToEat.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class AppDb : IdentityDbContext<ApplicationUser>
    {
        public AppDb()
            : base("DefaultConnection", throwIfV1Schema: false)
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