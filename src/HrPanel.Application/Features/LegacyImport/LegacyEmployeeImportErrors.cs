using HrPanel.Application.Common.Results;

namespace HrPanel.Application.Features.LegacyImport;

public static class LegacyEmployeeImportErrors
{
    public static Error BatchNotFound(Guid batchId)
    {
        return Error.NotFound(
            "LegacyEmployeeImport.BatchNotFound",
            $"هیچ رکوردی برای BatchId '{batchId}' پیدا نشد.");
    }
}
