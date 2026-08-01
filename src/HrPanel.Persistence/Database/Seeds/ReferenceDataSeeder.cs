using HrPanel.Domain.Assets;
using HrPanel.Domain.Employment;
using HrPanel.Domain.Organization;
using HrPanel.Persistence.Identity;
using Microsoft.EntityFrameworkCore;
using HrPanel.Application.Common.Authorization;

namespace HrPanel.Persistence.Database.Seeds;

public static class ReferenceDataSeeder
{
    // These IDs must remain unchanged after the first migration.
    private static readonly Guid AdministratorRoleId = Guid.Parse("E8137CF2-5B8B-4C4E-9EAA-899A0430476A");

    private static readonly Guid HrStaffRoleId = Guid.Parse("0A40CD90-4BB6-4F98-8CE3-082ECFCB1BCB");

    public static void Seed(ModelBuilder modelBuilder)
    {
        SeedEmploymentTypes(modelBuilder);
        SeedEmploymentStatuses(modelBuilder);
        SeedWorkTimeTypes(modelBuilder);
        SeedOrganizationUnitTypes(modelBuilder);
        SeedJobLevels(modelBuilder);
        SeedAssetTypes(modelBuilder);
        SeedRoles(modelBuilder);
    }
    private static void SeedEmploymentTypes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmploymentType>().HasData(
            new
            {
                Id = (short)1,
                Code = "PERMANENT",
                NameFa = "رسمی",
                NameEn = "Permanent",
                IsActive = true
            },
            new
            {
                Id = (short)2,
                Code = "CONTRACT",
                NameFa = "قراردادی",
                NameEn = "Contract",
                IsActive = true
            },
            new
            {
                Id = (short)3,
                Code = "VENDOR",
                NameFa = "پیمانکاری",
                NameEn = "Vendor",
                IsActive = true
            },
            new
            {
                Id = (short)4,
                Code = "PROJECT",
                NameFa = "پروژه‌ای",
                NameEn = "Project",
                IsActive = true
            },
            new
            {
                Id = (short)5,
                Code = "LOCAL",
                NameFa = "نیروی داخلی",
                NameEn = "Local",
                IsActive = true
            });
    }

    private static void SeedWorkTimeTypes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkTimeType>().HasData(
            new
            {
                Id = (short)1,
                Code = "FULL_TIME",
                NameFa = "تمام‌وقت",
                NameEn = "Full Time",
                IsActive = true
            },
            new
            {
                Id = (short)2,
                Code = "PART_TIME",
                NameFa = "پاره‌وقت",
                NameEn = "Part Time",
                IsActive = true
            });
    }

    private static void SeedEmploymentStatuses(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmploymentStatus>().HasData(
            new
            {
                Id = (short)1,
                Code = "ACTIVE",
                NameFa = "فعال",
                NameEn = "Active",
                IsActive = true
            },
            new
            {
                Id = (short)2,
                Code = "INACTIVE",
                NameFa = "غیرفعال",
                NameEn = "Inactive",
                IsActive = true
            },
            new
            {
                Id = (short)3,
                Code = "TERMINATED",
                NameFa = "خاتمه همکاری",
                NameEn = "Terminated",
                IsActive = true
            },
            new
            {
                Id = (short)4,
                Code = "RESIGNED",
                NameFa = "استعفا",
                NameEn = "Resigned",
                IsActive = true
            },
            new
            {
                Id = (short)5,
                Code = "MATERNITY_LEAVE",
                NameFa = "مرخصی زایمان",
                NameEn = "Maternity Leave",
                IsActive = true
            },
            new
            {
                Id = (short)6,
                Code = "TRANSFERRED",
                NameFa = "انتقال‌یافته",
                NameEn = "Transferred",
                IsActive = true
            });
    }
    private static void SeedOrganizationUnitTypes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrganizationUnitType>().HasData(
            new
            {
                Id = (short)1,
                Code = "COMPANY",
                NameFa = "شرکت",
                NameEn = "Company",
                HierarchyOrder = (short)1,
                IsActive = true
            },
            new
            {
                Id = (short)2,
                Code = "DIVISION",
                NameFa = "معاونت",
                NameEn = "Division",
                HierarchyOrder = (short)2,
                IsActive = true
            },
            new
            {
                Id = (short)3,
                Code = "SUBDIVISION",
                NameFa = "زیرمعاونت",
                NameEn = "Subdivision",
                HierarchyOrder = (short)3,
                IsActive = true
            },
            new
            {
                Id = (short)4,
                Code = "DEPARTMENT",
                NameFa = "اداره",
                NameEn = "Department",
                HierarchyOrder = (short)4,
                IsActive = true
            },
            new
            {
                Id = (short)5,
                Code = "SECTION",
                NameFa = "بخش",
                NameEn = "Section",
                HierarchyOrder = (short)5,
                IsActive = true
            },
            new
            {
                Id = (short)6,
                Code = "UNIT",
                NameFa = "واحد",
                NameEn = "Unit",
                HierarchyOrder = (short)6,
                IsActive = true
            });
    }

    private static void SeedJobLevels(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobLevel>().HasData(
        new
        {
            Id = (short)1,
            Code = "1",
            TitleFa = "سطح ۱",
            TitleEn = "Level 1",
            Rank = (short)1,
            IsActive = true
        },
        new
        {
            Id = (short)2,
            Code = "1H",
            TitleFa = "سطح ۱H",
            TitleEn = "Level 1H",
            Rank = (short)2,
            IsActive = true
        },
        new
        {
            Id = (short)3,
            Code = "2",
            TitleFa = "سطح ۲",
            TitleEn = "Level 2",
            Rank = (short)3,
            IsActive = true
        },
        new
        {
            Id = (short)4,
            Code = "2H",
            TitleFa = "سطح ۲H",
            TitleEn = "Level 2H",
            Rank = (short)4,
            IsActive = true
        },
        new
        {
            Id = (short)5,
            Code = "3",
            TitleFa = "سطح ۳",
            TitleEn = "Level 3",
            Rank = (short)5,
            IsActive = true
        },
        new
        {
            Id = (short)6,
            Code = "3H",
            TitleFa = "سطح ۳H",
            TitleEn = "Level 3H",
            Rank = (short)6,
            IsActive = true
        },
        new
        {
            Id = (short)7,
            Code = "4",
            TitleFa = "سطح ۴",
            TitleEn = "Level 4",
            Rank = (short)7,
            IsActive = true
        });
    }

    private static void SeedAssetTypes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssetType>().HasData(
            new
            {
                Id = (short)1,
                Code = "TDLTE_MODEM",
                NameFa = "مودم TD-LTE",
                NameEn = "TD-LTE Modem",
                IsActive = true
            },
            new
            {
                Id = (short)2,
                Code = "SIM_CARD",
                NameFa = "سیم‌کارت",
                NameEn = "SIM Card",
                IsActive = true
            },
            new
            {
                Id = (short)3,
                Code = "LAPTOP",
                NameFa = "لپ‌تاپ",
                NameEn = "Laptop",
                IsActive = true
            },
            new
            {
                Id = (short)4,
                Code = "ACCESS_CARD",
                NameFa = "کارت تردد",
                NameEn = "Access Card",
                IsActive = true
            },
            new
            {
                Id = (short)5,
                Code = "MOBILE_PHONE",
                NameFa = "تلفن همراه",
                NameEn = "Mobile Phone",
                IsActive = true
            },
            new
            {
                Id = (short)6,
                Code = "OTHER",
                NameFa = "سایر",
                NameEn = "Other",
                IsActive = true
            });
    }

    private static void SeedRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationRole>().HasData(
            new
            {
                Id = AdministratorRoleId,
                Name = RoleNames.Administrator,
                NormalizedName = "ADMINISTRATOR",
                ConcurrencyStamp = "7A418004-C400-4B97-AAB4-9A9D7789C114"
            },
            new
            {
                Id = HrStaffRoleId,
                Name = RoleNames.HrStaff,
                NormalizedName = "HRSTAFF",
                ConcurrencyStamp = "1AA90549-3B27-4015-9CC8-CBF6B931671A"
            });
    }
}