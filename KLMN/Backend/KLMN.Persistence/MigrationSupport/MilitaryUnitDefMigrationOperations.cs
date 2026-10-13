using Microsoft.EntityFrameworkCore.Migrations;

namespace KLMN.Persistence.MigrationSupport;

/// <summary>
/// Yerel migration geçmişindeki OrganizationUnits tablosunu veri kaybetmeden dönüştürür.
/// Add-Migration ile üretilen migration'ın Up/Down metotlarından çağrılır;
/// yeni veritabanında InitialCreate zaten güncel modeli oluşturur.
/// </summary>
public static class MilitaryUnitDefMigrationOperations
{
    public static void Up(MigrationBuilder migration, string? schema = null)
    {
        migration.DropForeignKey("FK_Users_OrganizationUnits_OrganizationUnitId", "Users", schema);
        migration.DropForeignKey("FK_OrganizationUnits_OrganizationUnits_ParentOrganizationUnitId", "OrganizationUnits", schema);
        migration.DropPrimaryKey("PK_OrganizationUnits", "OrganizationUnits", schema);
        migration.RenameTable("OrganizationUnits", schema, "MilitaryUnitDefs", schema);
        migration.RenameColumn("OrganizationUnitId", "Users", "MilitaryUnitDefId", schema);
        migration.RenameColumn("ParentOrganizationUnitId", "MilitaryUnitDefs", "ConnectedTopUnity", schema);
        migration.RenameIndex("IX_Users_OrganizationUnitId", "IX_Users_MilitaryUnitDefId", "Users", schema);
        migration.RenameIndex("IX_OrganizationUnits_Code", "IX_MilitaryUnitDefs_Code", "MilitaryUnitDefs", schema);
        migration.RenameIndex("IX_OrganizationUnits_Name", "IX_MilitaryUnitDefs_Name", "MilitaryUnitDefs", schema);
        migration.RenameIndex("IX_OrganizationUnits_ParentOrganizationUnitId", "IX_MilitaryUnitDefs_ConnectedTopUnity", "MilitaryUnitDefs", schema);
        migration.AddPrimaryKey("PK_MilitaryUnitDefs", "MilitaryUnitDefs", "Id", schema);
        migration.AddColumn<string>("ExternalCode", "MilitaryUnitDefs", type: "character varying(100)", schema: schema, maxLength: 100, nullable: true);
        migration.AddColumn<Guid>("ForcesCommandDef", "MilitaryUnitDefs", type: "uuid", schema: schema, nullable: true);
        migration.AddColumn<DateTime>("ValidFrom", "MilitaryUnitDefs", type: "timestamp with time zone", schema: schema, nullable: true);
        migration.AddColumn<DateTime>("ValidTo", "MilitaryUnitDefs", type: "timestamp with time zone", schema: schema, nullable: true);
        migration.CreateIndex("IX_MilitaryUnitDefs_ForcesCommandDef", "MilitaryUnitDefs", "ForcesCommandDef", schema);
        migration.AddForeignKey("FK_Users_MilitaryUnitDefs_MilitaryUnitDefId", "Users", "MilitaryUnitDefId", "MilitaryUnitDefs", schema: schema, principalSchema: schema, principalColumn: "Id", onDelete: ReferentialAction.Restrict);
        migration.AddForeignKey("FK_MilitaryUnitDefs_MilitaryUnitDefs_ConnectedTopUnity", "MilitaryUnitDefs", "ConnectedTopUnity", "MilitaryUnitDefs", schema: schema, principalSchema: schema, principalColumn: "Id", onDelete: ReferentialAction.Restrict);
    }

    /// <summary>Geri dönüşte yalnızca yeni alanlardaki veriler kaldırılır; kimlikler ve kullanıcı bağlantıları korunur.</summary>
    public static void Down(MigrationBuilder migration, string? schema = null)
    {
        migration.DropForeignKey("FK_Users_MilitaryUnitDefs_MilitaryUnitDefId", "Users", schema);
        migration.DropForeignKey("FK_MilitaryUnitDefs_MilitaryUnitDefs_ConnectedTopUnity", "MilitaryUnitDefs", schema);
        migration.DropPrimaryKey("PK_MilitaryUnitDefs", "MilitaryUnitDefs", schema);
        migration.DropIndex("IX_MilitaryUnitDefs_ForcesCommandDef", "MilitaryUnitDefs", schema);
        foreach (var column in new[] { "ExternalCode", "ForcesCommandDef", "ValidFrom", "ValidTo" })
            migration.DropColumn(column, "MilitaryUnitDefs", schema);
        migration.RenameTable("MilitaryUnitDefs", schema, "OrganizationUnits", schema);
        migration.RenameColumn("MilitaryUnitDefId", "Users", "OrganizationUnitId", schema);
        migration.RenameColumn("ConnectedTopUnity", "OrganizationUnits", "ParentOrganizationUnitId", schema);
        migration.RenameIndex("IX_Users_MilitaryUnitDefId", "IX_Users_OrganizationUnitId", "Users", schema);
        migration.RenameIndex("IX_MilitaryUnitDefs_Code", "IX_OrganizationUnits_Code", "OrganizationUnits", schema);
        migration.RenameIndex("IX_MilitaryUnitDefs_Name", "IX_OrganizationUnits_Name", "OrganizationUnits", schema);
        migration.RenameIndex("IX_MilitaryUnitDefs_ConnectedTopUnity", "IX_OrganizationUnits_ParentOrganizationUnitId", "OrganizationUnits", schema);
        migration.AddPrimaryKey("PK_OrganizationUnits", "OrganizationUnits", "Id", schema);
        migration.AddForeignKey("FK_Users_OrganizationUnits_OrganizationUnitId", "Users", "OrganizationUnitId", "OrganizationUnits", schema: schema, principalSchema: schema, principalColumn: "Id", onDelete: ReferentialAction.Restrict);
        migration.AddForeignKey("FK_OrganizationUnits_OrganizationUnits_ParentOrganizationUnitId", "OrganizationUnits", "ParentOrganizationUnitId", "OrganizationUnits", schema: schema, principalSchema: schema, principalColumn: "Id", onDelete: ReferentialAction.Restrict);
    }
}
