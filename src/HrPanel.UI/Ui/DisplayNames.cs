using HrPanel.Domain.Assets;
using HrPanel.Domain.Employment;
using HrPanel.Domain.Organization;
using HrPanel.Domain.Scheduling;

namespace HrPanel.UI.Ui;
public static class DisplayNames
{
    public static string AssetStatus(AssetStatus status) => status switch
    {
        HrPanel.Domain.Assets.AssetStatus.Available => "آماده واگذاری",
        HrPanel.Domain.Assets.AssetStatus.Assigned => "واگذارشده",
        HrPanel.Domain.Assets.AssetStatus.UnderMaintenance => "در تعمیر",
        HrPanel.Domain.Assets.AssetStatus.Retired => "از رده خارج",
        HrPanel.Domain.Assets.AssetStatus.Lost => "مفقود",
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
        HrPanel.Domain.Employment.AssignmentContext.Hr => "منابع انسانی",
        HrPanel.Domain.Employment.AssignmentContext.Cr => "عملیاتی",
        _ => context.ToString()
    };

    public static string RelationshipType(RelationshipType type) => type switch
    {
        HrPanel.Domain.Employment.RelationshipType.Manager => "مدیر",
        HrPanel.Domain.Employment.RelationshipType.Supervisor => "سرپرست",
        HrPanel.Domain.Employment.RelationshipType.QualityAssurance => "تضمین کیفیت",
        HrPanel.Domain.Employment.RelationshipType.SeniorManager => "مدیر ارشد",
        HrPanel.Domain.Employment.RelationshipType.ManagerLevel2 => "مدیر سطح دو",
        HrPanel.Domain.Employment.RelationshipType.ManagerLevel3 => "مدیر سطح سه",
        HrPanel.Domain.Employment.RelationshipType.ManagerLevel4 => "مدیر سطح چهار",
        _ => type.ToString()
    };

    public static string RelationshipContext(RelationshipContext context) => context switch
    {
        HrPanel.Domain.Employment.RelationshipContext.General => "عمومی",
        HrPanel.Domain.Employment.RelationshipContext.Hr => "منابع انسانی",
        HrPanel.Domain.Employment.RelationshipContext.Cr => "عملیاتی",
        _ => context.ToString()
    };
}
