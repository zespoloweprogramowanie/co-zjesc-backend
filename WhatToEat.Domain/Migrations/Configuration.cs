using System.Data.Entity.Migrations;

namespace WhatToEat.Domain.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<WhatToEat.Domain.Models.AppDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WhatToEat.Domain.Models.AppDb context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
