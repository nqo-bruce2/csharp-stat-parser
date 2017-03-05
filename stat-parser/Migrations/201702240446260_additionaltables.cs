namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class additionaltables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MatchPlayerStats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MatchId = c.String(unicode: false),
                        PlayerId = c.String(unicode: false),
                        TeamColor = c.String(unicode: false),
                        KillEfficiency = c.String(unicode: false),
                        WeaponEfficiency = c.String(unicode: false),
                        NumberOfQuads = c.Int(nullable: false),
                        QuadEfficiency = c.String(unicode: false),
                        NumQuadEnemyKills = c.Int(nullable: false),
                        NumQuadSelfKills = c.Int(nullable: false),
                        NumQuadTeamKills = c.Int(nullable: false),
                        NumOfFrags = c.Int(nullable: false),
                        NumOfEnemyKills = c.Int(nullable: false),
                        NumOfSelfKills = c.Int(nullable: false),
                        NumOfTeamKills = c.Int(nullable: false),
                        NumOfDeaths = c.Int(nullable: false),
                        BulletEfficiency = c.String(unicode: false),
                        NailsEfficiency = c.String(unicode: false),
                        RocketEfficiency = c.String(unicode: false),
                        LightningEfficiency = c.String(unicode: false),
                        TotalEfficiency = c.String(unicode: false),
                        DroppedPaks = c.Int(nullable: false),
                        SelfDamage = c.String(unicode: false),
                        TeamDamage = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IpAddress = c.String(maxLength: 20, storeType: "nvarchar"),
                        Name = c.String(unicode: false),
                        UserName = c.String(unicode: false),
                        Password = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.IpAddress, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Players", new[] { "IpAddress" });
            DropTable("dbo.Players");
            DropTable("dbo.MatchPlayerStats");
        }
    }
}
