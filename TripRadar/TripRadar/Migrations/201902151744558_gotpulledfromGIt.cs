namespace TripRadar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gotpulledfromGIt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trips", "TripDistance", c => c.String());
            AlterColumn("dbo.Trips", "TripTime", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Trips", "TripTime", c => c.Single(nullable: false));
            DropColumn("dbo.Trips", "TripDistance");
        }
    }
}
