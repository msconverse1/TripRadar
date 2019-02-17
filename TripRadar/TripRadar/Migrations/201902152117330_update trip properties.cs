namespace TripRadar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetripproperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trips", "IsArchived", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Trips", "IsArchived");
        }
    }
}
