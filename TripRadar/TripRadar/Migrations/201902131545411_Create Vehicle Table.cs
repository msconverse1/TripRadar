namespace TripRadar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateVehicleTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Trips",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartLocation = c.String(),
                        EndLocation = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.Vehicles", "VehicleMPG");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Vehicles", "VehicleMPG", c => c.Int(nullable: false));
            DropTable("dbo.Trips");
        }
    }
}
