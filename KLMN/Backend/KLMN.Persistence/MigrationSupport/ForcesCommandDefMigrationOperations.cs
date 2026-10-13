using Microsoft.EntityFrameworkCore.Migrations;

namespace KLMN.Persistence.MigrationSupport;

/// <summary>Mevcut nullable ForcesCommandDef kolonunu yeni kuvvet tablosuna bağlar.</summary>
public static class ForcesCommandDefMigrationOperations
{
    /// <summary>Yerel Add-Migration çıktısının Up metodundan çağrılabilir. Mevcut referanslar silinmez.</summary>
    public static void Up(MigrationBuilder migration, string? schema = null)
    {
        migration.CreateTable(
            name: "ForcesCommandDefs", schema: schema,
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_ForcesCommandDefs", x => x.Id));
        migration.CreateIndex("IX_ForcesCommandDefs_Code", "ForcesCommandDefs", "Code", schema,
            unique: true, filter: "\"IsDeleted\" = false");
        migration.CreateIndex("IX_ForcesCommandDefs_Name", "ForcesCommandDefs", "Name", schema);
        // Daha önce serbest GUID girilmişse gerçek kuvvet kaydı aynı kimlikle aktarılmadan
        // FK kurulamaz. Bu durumda PostgreSQL migration'ı geri alır; veri silinmez/null yapılmaz.
        migration.AddForeignKey("FK_MilitaryUnitDefs_ForcesCommandDefs_ForcesCommandDef",
            "MilitaryUnitDefs", "ForcesCommandDef", "ForcesCommandDefs", schema: schema,
            principalSchema: schema, principalColumn: "Id", onDelete: ReferentialAction.Restrict);
    }

    public static void Down(MigrationBuilder migration, string? schema = null)
    {
        migration.DropForeignKey("FK_MilitaryUnitDefs_ForcesCommandDefs_ForcesCommandDef", "MilitaryUnitDefs", schema);
        migration.DropTable("ForcesCommandDefs", schema);
    }
}
