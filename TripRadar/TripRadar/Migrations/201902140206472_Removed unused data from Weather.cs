namespace TripRadar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedunuseddatafromWeather : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Weathers", "PrecipitationName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Weathers", "PrecipitationName", c => c.String());
        }
    }
}
