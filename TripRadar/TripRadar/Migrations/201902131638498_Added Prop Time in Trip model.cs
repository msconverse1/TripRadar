namespace TripRadar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPropTimeinTripmodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trips", "TripTime", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Trips", "TripTime");
        }
    }
}
