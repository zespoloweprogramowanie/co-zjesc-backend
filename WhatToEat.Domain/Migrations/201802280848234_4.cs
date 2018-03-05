using System.Data.Entity.Migrations;

namespace WhatToEat.Domain.Migrations
{
    public partial class _4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Recipes", "Name", c => c.String(nullable: false, maxLength: 500));
            AlterColumn("dbo.Recipes", "Description", c => c.String(nullable: false));
            AlterColumn("dbo.Recipes", "Difficulty", c => c.Int(nullable: false));
            AlterColumn("dbo.Recipes", "TimeToPrepare", c => c.Int(nullable: false));
            AlterColumn("dbo.Recipes", "EstimatedCost", c => c.Int(nullable: false));
            AlterColumn("dbo.Recipes", "PortionCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Recipes", "PortionCount", c => c.Int());
            AlterColumn("dbo.Recipes", "EstimatedCost", c => c.Int());
            AlterColumn("dbo.Recipes", "TimeToPrepare", c => c.Int());
            AlterColumn("dbo.Recipes", "Difficulty", c => c.Int());
            AlterColumn("dbo.Recipes", "Description", c => c.String());
            AlterColumn("dbo.Recipes", "Name", c => c.String(maxLength: 500));
        }
    }
}
