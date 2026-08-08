using HrPanel.Domain.Assets;
using HrPanel.Domain.Employment;
using HrPanel.Domain.Organization;
using HrPanel.Domain.Scheduling;

namespace HrPanel.UI.Common.Localization;
public static class DisplayNames
{
    public static string AssetStatuses(AssetStatus status) => status switch
    {
        AssetStatus.Available => "آماده واگذاری",
        AssetStatus.Assigned => "واگذارشده",
        AssetStatus.UnderMaintenance => "در تعمیر",
        AssetStatus.Retired => "از رده خارج",
        AssetStatus.Lost => "مفقود",
        _ => status.ToString()
    };

    public static string SchedulePattern(WorkSchedulePatternType type) => type switch
    {
        WorkSchedulePatternType.Weekly => "هفتگی",
        WorkSchedulePatternType.Rotating => "چرخشی",
        WorkSchedulePatternType.Flexible => "شناور",
        _ => type.ToString()
    };

    public static string GroupType(OperationalGroupType type) => type switch
    {
        OperationalGroupType.ContactCenterAgentGroup => "گروه کارشناسان مرکز تماس",
        _ => type.ToString()
    };

    public static string AssignmentContext(AssignmentContext context) => context switch
    {
        Domain.Employment.AssignmentContext.Hr => "منابع انسانی",
        Domain.Employment.AssignmentContext.Cr => "عملیاتی",
        _ => context.ToString()
    };

    public static string RelationshipTypes(RelationshipType type) => type switch
    {
        RelationshipType.Manager => "مدیر",
        RelationshipType.Supervisor => "سرپرست",
        RelationshipType.QualityAssurance => "تضمین کیفیت",
        RelationshipType.SeniorManager => "مدیر ارشد",
        RelationshipType.ManagerLevel2 => "مدیر سطح دو",
        RelationshipType.ManagerLevel3 => "مدیر سطح سه",
        RelationshipType.ManagerLevel4 => "مدیر سطح چهار",
        _ => type.ToString()
    };

    public static string RelationshipContexts(RelationshipContext context) => context switch
    {
        RelationshipContext.General => "عمومی",
        RelationshipContext.Hr => "منابع انسانی",
        RelationshipContext.Cr => "عملیاتی",
        _ => context.ToString()
    };
}
