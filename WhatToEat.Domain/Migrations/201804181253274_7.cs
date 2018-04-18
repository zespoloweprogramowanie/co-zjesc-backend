namespace WhatToEat.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _7 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RecipeRates", "RateId", "dbo.Rates");
            DropIndex("dbo.RecipeRates", new[] { "RateId" });
            AddColumn("dbo.RecipeRates", "UserId", c => c.String(maxLength: 128));
            AddColumn("dbo.RecipeRates", "Rate", c => c.Int(nullable: false));
            CreateIndex("dbo.RecipeRates", "UserId");
            AddForeignKey("dbo.RecipeRates", "UserId", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.RecipeRates", "RateId");
            DropTable("dbo.Rates");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Rates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.RecipeRates", "RateId", c => c.Int(nullable: false));
            DropForeignKey("dbo.RecipeRates", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.RecipeRates", new[] { "UserId" });
            DropColumn("dbo.RecipeRates", "Rate");
            DropColumn("dbo.RecipeRates", "UserId");
            CreateIndex("dbo.RecipeRates", "RateId");
            AddForeignKey("dbo.RecipeRates", "RateId", "dbo.Rates", "Id");
        }
    }
}
