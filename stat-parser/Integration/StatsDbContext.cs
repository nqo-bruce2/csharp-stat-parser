using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Integration
{
    /// <summary>
    /// The person db context.
    /// </summary>
    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class StatsDbContext : DbContext
    {
        public DbSet<Match> Match { get; set; }
        public DbSet<Player> Player { get; set; }
        public DbSet<MatchPlayerStats> MatchPlayerStats { get; set; }
    }
}
