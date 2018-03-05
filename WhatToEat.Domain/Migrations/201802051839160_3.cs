using System.Data.Entity.Migrations;

namespace WhatToEat.Domain.Migrations
{
    public partial class _3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Rates", "Amount", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Rates", "Amount", c => c.Int(nullable: false));
        }
    }
}
