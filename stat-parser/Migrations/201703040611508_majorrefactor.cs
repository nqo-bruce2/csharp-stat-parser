namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class majorrefactor : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MatchTeamStats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MatchId = c.Int(nullable: false),
                        TeamColor = c.String(unicode: false),
                        TeamVerdict = c.String(unicode: false),
                        TeamTotalFrags = c.Int(nullable: false),
                        TeamTotalQuads = c.Int(nullable: false),
                        TeamTotalPents = c.Int(nullable: false),
                        TeamTotalEyes = c.Int(nullable: false),
                        TeamTotalRL = c.Int(nullable: false),
                        TeamTotalLG = c.Int(nullable: false),
                        TeamTotalGL = c.Int(nullable: false),
                        TeamTotalSNG = c.Int(nullable: false),
                        TeamTotalNG = c.Int(nullable: false),
                        TeamTotalMH = c.Int(nullable: false),
                        TeamTotalRA = c.Int(nullable: false),
                        TeamTotalYA = c.Int(nullable: false),
                        TeamTotalGA = c.Int(nullable: false),
                        TeamPlusRLPack = c.String(unicode: false),
                        TeamMinusRLPack = c.String(unicode: false),
                        TeamControlPercentage = c.Decimal(nullable: true, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            AlterColumn("dbo.MatchPlayerStats", "KillEfficiency", c => c.Decimal(nullable: true, precision: 18, scale: 2));
            AlterColumn("dbo.MatchPlayerStats", "WeaponEfficiency", c => c.Decimal(nullable: true, precision: 18, scale: 2));
            AlterColumn("dbo.MatchPlayerStats", "QuadEfficiency", c => c.Decimal(nullable: true, precision: 18, scale: 2));
            AlterColumn("dbo.MatchPlayerStats", "BulletEfficiency", c => c.Decimal(nullable: true, precision: 18, scale: 2));
            AlterColumn("dbo.MatchPlayerStats", "NailsEfficiency", c => c.Decimal(nullable: true, precision: 18, scale: 2));
            AlterColumn("dbo.MatchPlayerStats", "RocketEfficiency", c => c.Decimal(nullable: true, precision: 18, scale: 2));
            AlterColumn("dbo.MatchPlayerStats", "LightningEfficiency", c => c.Decimal(nullable: true, precision: 18, scale: 2));
            AlterColumn("dbo.MatchPlayerStats", "TotalEfficiency", c => c.Decimal(nullable: true, precision: 18, scale: 2));
            AlterColumn("dbo.MatchPlayerStats", "SelfDamage", c => c.Decimal(nullable: true, precision: 18, scale: 2));
            AlterColumn("dbo.MatchPlayerStats", "TeamDamage", c => c.Decimal(nullable: true, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MatchPlayerStats", "TeamDamage", c => c.String(unicode: false));
            AlterColumn("dbo.MatchPlayerStats", "SelfDamage", c => c.String(unicode: false));
            AlterColumn("dbo.MatchPlayerStats", "TotalEfficiency", c => c.String(unicode: false));
            AlterColumn("dbo.MatchPlayerStats", "LightningEfficiency", c => c.String(unicode: false));
            AlterColumn("dbo.MatchPlayerStats", "RocketEfficiency", c => c.String(unicode: false));
            AlterColumn("dbo.MatchPlayerStats", "NailsEfficiency", c => c.String(unicode: false));
            AlterColumn("dbo.MatchPlayerStats", "BulletEfficiency", c => c.String(unicode: false));
            AlterColumn("dbo.MatchPlayerStats", "QuadEfficiency", c => c.String(unicode: false));
            AlterColumn("dbo.MatchPlayerStats", "WeaponEfficiency", c => c.String(unicode: false));
            AlterColumn("dbo.MatchPlayerStats", "KillEfficiency", c => c.String(unicode: false));
            DropTable("dbo.MatchTeamStats");
        }
    }
}
