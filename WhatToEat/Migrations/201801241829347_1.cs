namespace WhatToEat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Users", newName: "AspNetUsers");
            DropForeignKey("dbo.Recipes", "AuthorId", "dbo.Users");
            DropForeignKey("dbo.RecipeComments", "AuthorId", "dbo.Users");
            DropForeignKey("dbo.UserFavouriteRecipes", "UserId", "dbo.Users");
            DropIndex("dbo.Recipes", new[] { "AuthorId" });
            DropIndex("dbo.RecipeComments", new[] { "AuthorId" });
            DropIndex("dbo.UserFavouriteRecipes", new[] { "UserId" });
            //DropPrimaryKey("dbo.AspNetUsers");
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            AddColumn("dbo.AspNetUsers", "Email", c => c.String(maxLength: 256));
            AddColumn("dbo.AspNetUsers", "EmailConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "PasswordHash", c => c.String());
            AddColumn("dbo.AspNetUsers", "SecurityStamp", c => c.String());
            AddColumn("dbo.AspNetUsers", "PhoneNumber", c => c.String());
            AddColumn("dbo.AspNetUsers", "PhoneNumberConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "TwoFactorEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "LockoutEndDateUtc", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "LockoutEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "AccessFailedCount", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "UserName", c => c.String(nullable: false, maxLength: 256));
            AlterColumn("dbo.Recipes", "AuthorId", c => c.String(maxLength: 128));
            AlterColumn("dbo.AspNetUsers", "Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.RecipeComments", "AuthorId", c => c.String(maxLength: 128));
            AlterColumn("dbo.UserFavouriteRecipes", "UserId", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.AspNetUsers", "Id");
            CreateIndex("dbo.Recipes", "AuthorId");
            CreateIndex("dbo.AspNetUsers", "UserName", unique: true, name: "UserNameIndex");
            CreateIndex("dbo.RecipeComments", "AuthorId");
            CreateIndex("dbo.UserFavouriteRecipes", "UserId");
            AddForeignKey("dbo.Recipes", "AuthorId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.RecipeComments", "AuthorId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.UserFavouriteRecipes", "UserId", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.AspNetUsers", "Login");
            DropColumn("dbo.AspNetUsers", "Password");
            DropColumn("dbo.AspNetUsers", "Admin");
            DropColumn("dbo.AspNetUsers", "AccountType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "AccountType", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Admin", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "Password", c => c.String());
            AddColumn("dbo.AspNetUsers", "Login", c => c.String());
            DropForeignKey("dbo.UserFavouriteRecipes", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.RecipeComments", "AuthorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Recipes", "AuthorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.UserFavouriteRecipes", new[] { "UserId" });
            DropIndex("dbo.RecipeComments", new[] { "AuthorId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Recipes", new[] { "AuthorId" });
            DropPrimaryKey("dbo.AspNetUsers");
            AlterColumn("dbo.UserFavouriteRecipes", "UserId", c => c.Int(nullable: false));
            AlterColumn("dbo.RecipeComments", "AuthorId", c => c.Int(nullable: false));
            AlterColumn("dbo.AspNetUsers", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Recipes", "AuthorId", c => c.Int(nullable: false));
            DropColumn("dbo.AspNetUsers", "UserName");
            DropColumn("dbo.AspNetUsers", "AccessFailedCount");
            DropColumn("dbo.AspNetUsers", "LockoutEnabled");
            DropColumn("dbo.AspNetUsers", "LockoutEndDateUtc");
            DropColumn("dbo.AspNetUsers", "TwoFactorEnabled");
            DropColumn("dbo.AspNetUsers", "PhoneNumberConfirmed");
            DropColumn("dbo.AspNetUsers", "PhoneNumber");
            DropColumn("dbo.AspNetUsers", "SecurityStamp");
            DropColumn("dbo.AspNetUsers", "PasswordHash");
            DropColumn("dbo.AspNetUsers", "EmailConfirmed");
            DropColumn("dbo.AspNetUsers", "Email");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            AddPrimaryKey("dbo.AspNetUsers", "Id");
            CreateIndex("dbo.UserFavouriteRecipes", "UserId");
            CreateIndex("dbo.RecipeComments", "AuthorId");
            CreateIndex("dbo.Recipes", "AuthorId");
            AddForeignKey("dbo.UserFavouriteRecipes", "UserId", "dbo.Users", "Id");
            AddForeignKey("dbo.RecipeComments", "AuthorId", "dbo.Users", "Id");
            AddForeignKey("dbo.Recipes", "AuthorId", "dbo.Users", "Id");
            RenameTable(name: "dbo.AspNetUsers", newName: "Users");
        }
    }
}
