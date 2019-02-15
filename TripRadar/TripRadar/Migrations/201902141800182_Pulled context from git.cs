namespace TripRadar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Pulledcontextfromgit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Weathers", "WindDeg", c => c.Single(nullable: false));
            AddColumn("dbo.Weathers", "TypeOfSkys", c => c.String());
            AddColumn("dbo.Weathers", "Humidity", c => c.Single(nullable: false));
            AddColumn("dbo.Weathers", "DateTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.Weathers", "WindName");
            DropColumn("dbo.Weathers", "CloudName");
            DropColumn("dbo.Weathers", "PrecipitationValue");
            DropColumn("dbo.Weathers", "PrecipitationName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Weathers", "PrecipitationName", c => c.String());
            AddColumn("dbo.Weathers", "PrecipitationValue", c => c.Single(nullable: false));
            AddColumn("dbo.Weathers", "CloudName", c => c.String());
            AddColumn("dbo.Weathers", "WindName", c => c.String());
            DropColumn("dbo.Weathers", "DateTime");
            DropColumn("dbo.Weathers", "Humidity");
            DropColumn("dbo.Weathers", "TypeOfSkys");
            DropColumn("dbo.Weathers", "WindDeg");
        }
    }
}
