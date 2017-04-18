namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pingandaddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MatchPlayerStats", "Ping", c => c.Int(nullable: true));
            AddColumn("dbo.MatchPlayerStats", "IpAddress", c => c.String(unicode: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MatchPlayerStats", "IpAddress");
            DropColumn("dbo.MatchPlayerStats", "Ping");
        }
    }
}
