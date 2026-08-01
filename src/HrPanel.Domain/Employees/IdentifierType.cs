namespace HrPanel.Domain.Employees
{
    // شماره کارمندی و کد ملی اینجا نیستند، EmployeeNumber متعلق به Employee و NationalCode متعلق به EmployeePersonalDetails است
    public enum IdentifierType : short
    {
        AccessCard = 1,
        ArchiveNumber = 2,
        FoodCode = 3,
        StaffNumber = 4,
        InsuranceNumber = 5,
        AttendanceCode = 6
    }
}
