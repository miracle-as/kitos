namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUniqueItInterfaceIdToNamePerOrg : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("ItInterface", "FK_ItInterface_Organization_OrganizationId");
            DropIndex("ItInterface", "IX_NamePerOrg");
            AlterColumn("ItInterface", "ItInterfaceId", c => c.String(maxLength: 100, storeType: "nvarchar"));
            CreateIndex("ItInterface", new[] { "OrganizationId", "ItInterfaceId", "Name" }, unique: true, name: "IX_NamePerOrg");
            AddForeignKey("ItInterface", "OrganizationId", "Organization");
        }
        
        public override void Down()
        {
            DropForeignKey("ItInterface", "FK_ItInterface_Organization_OrganizationId");
            DropIndex("ItInterface", "IX_NamePerOrg");
            AlterColumn("ItInterface", "ItInterfaceId", c => c.String(unicode: false));
            CreateIndex("ItInterface", new[] { "OrganizationId", "Name" }, unique: true, name: "IX_NamePerOrg");
            AddForeignKey("ItInterface", "OrganizationId", "Organization");
        }
    }
}
