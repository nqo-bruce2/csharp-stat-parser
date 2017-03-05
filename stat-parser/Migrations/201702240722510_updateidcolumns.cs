namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateidcolumns : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MatchPlayerStats", "MatchId", c => c.Int(nullable: false));
            AlterColumn("dbo.MatchPlayerStats", "PlayerId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MatchPlayerStats", "PlayerId", c => c.String(unicode: false));
            AlterColumn("dbo.MatchPlayerStats", "MatchId", c => c.String(unicode: false));
        }
    }
}
