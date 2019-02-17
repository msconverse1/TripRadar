namespace TripRadar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changetonewuser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "TripID", "dbo.Trips");
            DropForeignKey("dbo.Users", "VehicleId", "dbo.Vehicles");
            DropIndex("dbo.Users", new[] { "VehicleId" });
            DropIndex("dbo.Users", new[] { "TripID" });
            AlterColumn("dbo.Users", "VehicleId", c => c.Int());
            AlterColumn("dbo.Users", "TripID", c => c.Int());
            CreateIndex("dbo.Users", "VehicleId");
            CreateIndex("dbo.Users", "TripID");
            AddForeignKey("dbo.Users", "TripID", "dbo.Trips", "TripID");
            AddForeignKey("dbo.Users", "VehicleId", "dbo.Vehicles", "VehicleId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "VehicleId", "dbo.Vehicles");
            DropForeignKey("dbo.Users", "TripID", "dbo.Trips");
            DropIndex("dbo.Users", new[] { "TripID" });
            DropIndex("dbo.Users", new[] { "VehicleId" });
            AlterColumn("dbo.Users", "TripID", c => c.Int(nullable: false));
            AlterColumn("dbo.Users", "VehicleId", c => c.Int(nullable: false));
            CreateIndex("dbo.Users", "TripID");
            CreateIndex("dbo.Users", "VehicleId");
            AddForeignKey("dbo.Users", "VehicleId", "dbo.Vehicles", "VehicleId", cascadeDelete: true);
            AddForeignKey("dbo.Users", "TripID", "dbo.Trips", "TripID", cascadeDelete: true);
        }
    }
}
