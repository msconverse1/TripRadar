namespace TripRadar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateLocation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StreetName = c.String(),
                        City = c.String(),
                        State = c.String(),
                        ZipCode = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Trips", "WeatherID", c => c.Int(nullable: false));
            AddColumn("dbo.Trips", "LocationId", c => c.Int(nullable: false));
            CreateIndex("dbo.Trips", "WeatherID");
            CreateIndex("dbo.Trips", "LocationId");
            AddForeignKey("dbo.Trips", "LocationId", "dbo.Locations", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Trips", "WeatherID", "dbo.Weathers", "WeatherId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Trips", "WeatherID", "dbo.Weathers");
            DropForeignKey("dbo.Trips", "LocationId", "dbo.Locations");
            DropIndex("dbo.Trips", new[] { "LocationId" });
            DropIndex("dbo.Trips", new[] { "WeatherID" });
            DropColumn("dbo.Trips", "LocationId");
            DropColumn("dbo.Trips", "WeatherID");
            DropTable("dbo.Locations");
        }
    }
}
