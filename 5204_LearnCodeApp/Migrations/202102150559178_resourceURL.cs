namespace _5204_LearnCodeApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class resourceURL : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Resources", "ResourceURL", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Resources", "ResourceURL");
        }
    }
}
