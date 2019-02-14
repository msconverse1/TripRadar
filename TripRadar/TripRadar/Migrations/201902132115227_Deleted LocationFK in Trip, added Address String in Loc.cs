namespace TripRadar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeletedLocationFKinTripaddedAddressStringinLoc : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Trips", "LocationId", "dbo.Locations");
            DropIndex("dbo.Trips", new[] { "LocationId" });
            DropColumn("dbo.Trips", "LocationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Trips", "LocationId", c => c.Int(nullable: false));
            CreateIndex("dbo.Trips", "LocationId");
            AddForeignKey("dbo.Trips", "LocationId", "dbo.Locations", "ID", cascadeDelete: true);
        }
    }
}
