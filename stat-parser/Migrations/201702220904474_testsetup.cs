namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testsetup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Matches",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    MatchId = c.String(nullable: false, unicode: false),
                    MatchType = c.String(unicode: false),
                    MapName = c.String(unicode: false),
                    Date = c.DateTime(nullable: false, precision: 0),
                    MatchText = c.String(unicode: false),
                })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Matches");
        }
    }
}
