namespace TripRadar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WeatherModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Weathers",
                c => new
                    {
                        WeatherId = c.Int(nullable: false, identity: true),
                        MainTemp = c.Single(nullable: false),
                        Speedvalue = c.Single(nullable: false),
                        WindName = c.String(),
                        CloudValue = c.Single(nullable: false),
                        CloudName = c.String(),
                        PrecipitationValue = c.Single(nullable: false),
                        PrecipitationName = c.String(),
                    })
                .PrimaryKey(t => t.WeatherId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Weathers");
        }
    }
}
