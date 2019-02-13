namespace TripRadar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class intialsset : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Places",
                c => new
                    {
                        RecommendedPlace = c.String(nullable: false, maxLength: 128),
                        LocationOfPlace = c.String(),
                        Discription = c.String(),
                        DistanceFromUser = c.Double(nullable: false),
                        TripID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RecommendedPlace)
                .ForeignKey("dbo.Trips", t => t.TripID, cascadeDelete: true)
                .Index(t => t.TripID);
            
            CreateTable(
                "dbo.Trips",
                c => new
                    {
                        TripID = c.Int(nullable: false, identity: true),
                        StartLocation = c.String(),
                        EndLocation = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.TripID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CurrentLocation = c.String(),
                        VehicleId = c.Int(nullable: false),
                        TripID = c.Int(nullable: false),
                        ApplicationUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .ForeignKey("dbo.Trips", t => t.TripID, cascadeDelete: true)
                .ForeignKey("dbo.Vehicles", t => t.VehicleId, cascadeDelete: true)
                .Index(t => t.VehicleId)
                .Index(t => t.TripID)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Vehicles",
                c => new
                    {
                        VehicleId = c.Int(nullable: false, identity: true),
                        VehicleMake = c.String(),
                        VehicleModel = c.String(),
                        VehicleYear = c.Int(nullable: false),
                        VehicleKey = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.VehicleId);
            
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
            DropForeignKey("dbo.Users", "VehicleId", "dbo.Vehicles");
            DropForeignKey("dbo.Users", "TripID", "dbo.Trips");
            DropForeignKey("dbo.Users", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Places", "TripID", "dbo.Trips");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Users", new[] { "ApplicationUserId" });
            DropIndex("dbo.Users", new[] { "TripID" });
            DropIndex("dbo.Users", new[] { "VehicleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Places", new[] { "TripID" });
            DropTable("dbo.Weathers");
            DropTable("dbo.Vehicles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Users");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Trips");
            DropTable("dbo.Places");
        }
    }
}
