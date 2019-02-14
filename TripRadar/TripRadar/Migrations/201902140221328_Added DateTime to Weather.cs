namespace TripRadar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDateTimetoWeather : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Weathers", "DateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Weathers", "DateTime");
        }
    }
}
