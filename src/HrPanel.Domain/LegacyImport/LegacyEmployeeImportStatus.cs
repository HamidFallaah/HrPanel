namespace HrPanel.Domain.LegacyImport
{
    public enum LegacyEmployeeImportStatus : short
    {
        Pending = 1, // Waiting to be processed
        Processing = 2, // Currently being imported
        Imported = 3, // Successfully converted to normalized records
        Failed = 4, // Import failed
        Skipped = 5 // Deliberately ignored
    }
}
