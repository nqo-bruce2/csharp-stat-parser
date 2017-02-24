namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class proptest : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Matches", "MatchId", c => c.String(maxLength: 200, storeType: "nvarchar"));
            CreateIndex("dbo.Matches", "MatchId", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Matches", new[] { "MatchId" });
            AlterColumn("dbo.Matches", "MatchId", c => c.String(unicode: false));
        }
    }
}
