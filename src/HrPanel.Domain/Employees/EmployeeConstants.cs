namespace HrPanel.Domain.Employees
{
    // اگر نیاز بود بعدا این مقادیر میتونیم با Configuaration یا FluentValidation عوض کنیم
    public static class EmployeeConstants
    {
        public const int EmployeeNumberMaxLength = 50;
        public const int LegacyUserIdMaxLength = 128;

        public const int NameMaxLength = 50;
        public const int NationalCodeLength = 10;
        public const int BirthPlaceMaxLength = 150;

        public const int ContactValueMaxLength = 320;
        public const int IdentifierValueMaxLength = 100;

        public const int EducationTitleMaxLength = 150;
        public const int InstitutionNameMaxLength = 200;
        public const int DependentFullNameMaxLength = 150;
        public const int EmergencyPhoneMaxLength = 30;
    }
}
