namespace MessageEncryptionService.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Blogs",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        Content = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        Blog_Id = c.Guid(),
                        Tag_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Blogs", t => t.Blog_Id)
                .ForeignKey("dbo.Tags", t => t.Tag_Id)
                .Index(t => t.Blog_Id)
                .Index(t => t.Tag_Id);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Caption = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Posts", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.Posts", "Blog_Id", "dbo.Blogs");
            DropIndex("dbo.Posts", new[] { "Tag_Id" });
            DropIndex("dbo.Posts", new[] { "Blog_Id" });
            DropTable("dbo.Tags");
            DropTable("dbo.Posts");
            DropTable("dbo.Blogs");
        }
    }
}
