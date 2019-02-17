namespace TripRadar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class weatherbugfix : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trips", "HasBigChangeWeather", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Trips", "HasBigChangeWeather");
        }
    }
}
