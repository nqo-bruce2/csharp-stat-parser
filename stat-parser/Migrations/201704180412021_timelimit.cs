namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timelimit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "MatchTimeLimit", c => c.Int(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "MatchTimeLimit");
        }
    }
}
