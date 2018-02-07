namespace WhatToEat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RecipeRates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RecipeId = c.Int(nullable: false),
                        RateId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Rates", t => t.RateId)
                .ForeignKey("dbo.Recipes", t => t.RecipeId)
                .Index(t => t.RecipeId)
                .Index(t => t.RateId);
            
            CreateTable(
                "dbo.Rates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Recipes", "AverageGrade", c => c.Int());
            AddColumn("dbo.Recipes", "AverageVote", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RecipeRates", "RecipeId", "dbo.Recipes");
            DropForeignKey("dbo.RecipeRates", "RateId", "dbo.Rates");
            DropIndex("dbo.RecipeRates", new[] { "RateId" });
            DropIndex("dbo.RecipeRates", new[] { "RecipeId" });
            DropColumn("dbo.Recipes", "AverageVote");
            DropColumn("dbo.Recipes", "AverageGrade");
            DropTable("dbo.Rates");
            DropTable("dbo.RecipeRates");
        }
    }
}
