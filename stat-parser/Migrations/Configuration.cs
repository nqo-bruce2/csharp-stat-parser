namespace Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Integration.StatsDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Integration.StatsDbContext context)
        {
            //context.Match.Add(new Models.Match() { MatchId = "seed_match_id", MatchType = "seed_type_1v1", MapName = "seed_map_dm3", MatchText = "seed_data", Date = DateTime.Now });
        }
    }
}
