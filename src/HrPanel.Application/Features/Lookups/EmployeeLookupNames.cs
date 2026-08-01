using HrPanel.Domain.Employees;

namespace HrPanel.Application.Features.Lookups;

internal static class EmployeeLookupNames
{
    public static string GetDisplayName(ContactType value)
    {
        return value switch
        {
            ContactType.Mobile => "موبایل",
            ContactType.Telephone => "تلفن ثابت",
            ContactType.Email => "ایمیل",
            ContactType.AlternateEmail => "ایمیل جایگزین",
            ContactType.EmergencyPhone => "تلفن اضطراری",
            _ => "نامشخص"
        };
    }

    public static string GetDisplayName(IdentifierType value)
    {
        return value switch
        {
            IdentifierType.AccessCard => "کارت تردد",
            IdentifierType.ArchiveNumber => "شماره بایگانی",
            IdentifierType.FoodCode => "کد غذا",
            IdentifierType.StaffNumber => "کد پرسنلی سیستم",
            IdentifierType.InsuranceNumber => "شماره بیمه",
            IdentifierType.AttendanceCode => "کد حضور و غیاب",
            _ => "نامشخص"
        };
    }

    public static string GetDisplayName(Gender value)
    {
        return value switch
        {
            Gender.Male => "مرد",
            Gender.Female => "زن",
            _ => "نامشخص"
        };
    }

    public static string GetDisplayName(MaritalStatus value)
    {
        return value switch
        {
            MaritalStatus.Single => "مجرد",
            MaritalStatus.Married => "متأهل",
            MaritalStatus.Divorced => "جداشده",
            MaritalStatus.Widowed => "همسر فوت‌شده",
            _ => "نامشخص"
        };
    }

    public static string GetDisplayName(DependentRelationshipType value)
    {
        return value switch
        {
            DependentRelationshipType.Spouse => "همسر",
            DependentRelationshipType.Child => "فرزند",
            DependentRelationshipType.Father => "پدر",
            DependentRelationshipType.Mother => "مادر",
            DependentRelationshipType.Other => "سایر",
            _ => "نامشخص"
        };
    }
}
