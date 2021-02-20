namespace _5204_LearnCodeApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class codeapp : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                //==================CODERS TABLE =====================
                "dbo.Coders",
                c => new
                    {
                        CoderID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        UserName = c.String(),
                        ProfileImage = c.String(),
                        CoderEmail = c.String(),
                        CoderURL = c.String(),
                        CoderBio = c.String(),
                    })
                .PrimaryKey(t => t.CoderID);


            //=============COMMENTS ================================
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        CommentID = c.Int(nullable: false, identity: true),
                        CommentContent = c.String(),
                        ResourceID = c.Int(nullable: false),
                        CoderID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CommentID)
                .ForeignKey("dbo.Coders", t => t.CoderID, cascadeDelete: true)
                .ForeignKey("dbo.Resources", t => t.ResourceID, cascadeDelete: true)
                .Index(t => t.ResourceID)
                .Index(t => t.CoderID);

            //================RESOURCES TABLE 
            
            CreateTable(
                "dbo.Resources",
                c => new
                    {
                        ResourceID = c.Int(nullable: false, identity: true),
                        ResourceTitle = c.String(),
                        MediaType = c.String(),
                        ResourceType = c.String(),
                        ResourceLanguage = c.String(),
                        ResourceDescription = c.String(),
                        ResourceImage = c.String(),
                    })
                .PrimaryKey(t => t.ResourceID);
            
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

            //=============Bridging table for many to many relationship=========
            
            CreateTable( 
                "dbo.ResourceCoders",
                c => new
                    {
                        Resource_ResourceID = c.Int(nullable: false),
                        Coder_CoderID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Resource_ResourceID, t.Coder_CoderID })
                .ForeignKey("dbo.Resources", t => t.Resource_ResourceID, cascadeDelete: true)
                .ForeignKey("dbo.Coders", t => t.Coder_CoderID, cascadeDelete: true)
                .Index(t => t.Resource_ResourceID)
                .Index(t => t.Coder_CoderID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Comments", "ResourceID", "dbo.Resources");
            DropForeignKey("dbo.ResourceCoders", "Coder_CoderID", "dbo.Coders");
            DropForeignKey("dbo.ResourceCoders", "Resource_ResourceID", "dbo.Resources");
            DropForeignKey("dbo.Comments", "CoderID", "dbo.Coders");
            DropIndex("dbo.ResourceCoders", new[] { "Coder_CoderID" });
            DropIndex("dbo.ResourceCoders", new[] { "Resource_ResourceID" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Comments", new[] { "CoderID" });
            DropIndex("dbo.Comments", new[] { "ResourceID" });
            DropTable("dbo.ResourceCoders");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Resources");
            DropTable("dbo.Comments");
            DropTable("dbo.Coders");
        }
    }
}
