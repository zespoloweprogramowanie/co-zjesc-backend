namespace WhatToEat.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "AverageRate", c => c.Double(nullable: false));
            DropColumn("dbo.Recipes", "AverageGrade");
            DropColumn("dbo.Recipes", "AverageVote");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Recipes", "AverageVote", c => c.Int());
            AddColumn("dbo.Recipes", "AverageGrade", c => c.Int());
            DropColumn("dbo.Recipes", "AverageRate");
        }
    }
}
