using HrPanel.Domain.Assets;
using HrPanel.Domain.Employment;
using HrPanel.Domain.Organization;
using HrPanel.Domain.Scheduling;

namespace HrPanel.Application.Features.Lookups;

internal static class ModuleLookupNames
{
    public static string GetDisplayName(AssignmentContext value) => value switch
    {
        AssignmentContext.Hr => "منابع انسانی",
        AssignmentContext.Cr => "ارتباط با مشتری",
        _ => "نامشخص"
    };

    public static string GetDisplayName(RelationshipType value) => value switch
    {
        RelationshipType.Manager => "مدیر",
        RelationshipType.Supervisor => "سرپرست",
        RelationshipType.QualityAssurance => "کنترل کیفیت",
        RelationshipType.SeniorManager => "مدیر ارشد",
        RelationshipType.ManagerLevel2 => "مدیر سطح دوم",
        RelationshipType.ManagerLevel3 => "مدیر سطح سوم",
        RelationshipType.ManagerLevel4 => "مدیر سطح چهارم",
        _ => "نامشخص"
    };

    public static string GetDisplayName(RelationshipContext value) => value switch
    {
        RelationshipContext.General => "عمومی",
        RelationshipContext.Hr => "منابع انسانی",
        RelationshipContext.Cr => "ارتباط با مشتری",
        _ => "نامشخص"
    };

    public static string GetDisplayName(OperationalGroupType value) => value switch
    {
        OperationalGroupType.ContactCenterAgentGroup => "گروه کارشناسان مرکز تماس",
        _ => "نامشخص"
    };

    public static string GetDisplayName(WorkSchedulePatternType value) => value switch
    {
        WorkSchedulePatternType.Weekly => "هفتگی",
        WorkSchedulePatternType.Rotating => "چرخشی",
        WorkSchedulePatternType.Flexible => "شناور",
        _ => "نامشخص"
    };

    public static string GetDisplayName(AssetStatus value) => value switch
    {
        AssetStatus.Available => "آماده واگذاری",
        AssetStatus.Assigned => "واگذارشده",
        AssetStatus.UnderMaintenance => "در تعمیر",
        AssetStatus.Retired => "از رده خارج",
        AssetStatus.Lost => "مفقود",
        _ => "نامشخص"
    };
}
