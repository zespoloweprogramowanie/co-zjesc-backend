namespace WhatToEat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RecipeProducts", "Recipe_Id", "dbo.Recipes");
            DropForeignKey("dbo.RecipeProducts", "Product_Id", "dbo.Products");
            DropIndex("dbo.RecipeProducts", new[] { "Recipe_Id" });
            DropIndex("dbo.RecipeProducts", new[] { "Product_Id" });
            CreateTable(
                "dbo.RecipeProducts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        RecipeId = c.Int(nullable: false),
                        UnitId = c.Int(nullable: false),
                        NumberOfUnit = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .ForeignKey("dbo.Recipes", t => t.RecipeId)
                .ForeignKey("dbo.Units", t => t.UnitId)
                .Index(t => t.ProductId)
                .Index(t => t.RecipeId)
                .Index(t => t.UnitId);
            
            CreateTable(
                "dbo.Units",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
            //DropTable("dbo.RecipeProducts");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.RecipeProducts",
                c => new
                    {
                        Recipe_Id = c.Int(nullable: false),
                        Product_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Recipe_Id, t.Product_Id });
            
            DropForeignKey("dbo.RecipeProducts", "UnitId", "dbo.Units");
            DropForeignKey("dbo.RecipeProducts", "RecipeId", "dbo.Recipes");
            DropForeignKey("dbo.RecipeProducts", "ProductId", "dbo.Products");
            DropIndex("dbo.RecipeProducts", new[] { "UnitId" });
            DropIndex("dbo.RecipeProducts", new[] { "RecipeId" });
            DropIndex("dbo.RecipeProducts", new[] { "ProductId" });
            DropTable("dbo.Units");
            DropTable("dbo.RecipeProducts");
            CreateIndex("dbo.RecipeProducts", "Product_Id");
            CreateIndex("dbo.RecipeProducts", "Recipe_Id");
            AddForeignKey("dbo.RecipeProducts", "Product_Id", "dbo.Products", "Id", cascadeDelete: true);
            AddForeignKey("dbo.RecipeProducts", "Recipe_Id", "dbo.Recipes", "Id", cascadeDelete: true);
        }
    }
}
