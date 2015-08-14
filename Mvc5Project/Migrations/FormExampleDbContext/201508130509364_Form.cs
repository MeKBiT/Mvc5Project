namespace Mvc5Project.Migrations.FormExampleDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Form : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Country",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Course",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Checked = c.Boolean(nullable: false),
                        UserCourse_UserID = c.String(maxLength: 128),
                        UserCourse_CourseID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.UserCourse", t => new { t.UserCourse_UserID, t.UserCourse_CourseID })
                .Index(t => new { t.UserCourse_UserID, t.UserCourse_CourseID });
            
            CreateTable(
                "dbo.UserCountry",
                c => new
                    {
                        UserID = c.String(nullable: false, maxLength: 128),
                        CountryID = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserID, t.CountryID })
                .ForeignKey("dbo.Country", t => t.CountryID, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.CountryID);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Gender = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.UserCourse",
                c => new
                    {
                        UserID = c.String(nullable: false, maxLength: 128),
                        CourseID = c.String(nullable: false, maxLength: 128),
                        Checked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserID, t.CourseID })
                .ForeignKey("dbo.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.UserDescription",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        UserID = c.String(maxLength: 128),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.UserID)
                .Index(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserDescription", "UserID", "dbo.User");
            DropForeignKey("dbo.UserCourse", "UserID", "dbo.User");
            DropForeignKey("dbo.Course", new[] { "UserCourse_UserID", "UserCourse_CourseID" }, "dbo.UserCourse");
            DropForeignKey("dbo.UserCountry", "UserID", "dbo.User");
            DropForeignKey("dbo.UserCountry", "CountryID", "dbo.Country");
            DropIndex("dbo.UserDescription", new[] { "UserID" });
            DropIndex("dbo.UserCourse", new[] { "UserID" });
            DropIndex("dbo.UserCountry", new[] { "CountryID" });
            DropIndex("dbo.UserCountry", new[] { "UserID" });
            DropIndex("dbo.Course", new[] { "UserCourse_UserID", "UserCourse_CourseID" });
            DropTable("dbo.UserDescription");
            DropTable("dbo.UserCourse");
            DropTable("dbo.User");
            DropTable("dbo.UserCountry");
            DropTable("dbo.Course");
            DropTable("dbo.Country");
        }
    }
}
