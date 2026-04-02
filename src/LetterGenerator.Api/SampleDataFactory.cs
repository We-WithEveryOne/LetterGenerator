using LetterGenerator.Core.Models;

namespace LetterGenerator.Api;

/// <summary>
/// Creates realistic sample data for testing letter generation.
/// Use the /sample endpoints to generate letters without needing to POST data.
/// </summary>
public static class SampleDataFactory
{
    public static DenialLetterModel CreateSampleDenialLetter() => new()
    {
        LetterDate = DateTime.Today,
        EffectiveDate = DateTime.Today,

        Organization = new OrganizationInfo
        {
            Name = "Test Health Solutions",
            AddressLine1 = "9900 Bren Road East",
            City = "Minnetonka",
            State = "MN",
            ZipCode = "55343",
            Phone = "(800) 555-0100",
            Fax = "(800) 555-0101"
        },

        Plan = new PlanInfo
        {
            PlanName = "Test Choice Plus",
            PlanId = "Test-CP-2024",
            GroupNumber = "GRP-887421",
            LineOfBusiness = "Commercial"
        },

        Member = new MemberInfo
        {
            FirstName = "Jane",
            LastName = "Morrison",
            MemberId = "U1234567890",
            DateOfBirth = new DateTime(1985, 3, 15),
            AddressLine1 = "1234 Elm Street",
            AddressLine2 = "Apt 5B",
            City = "Minneapolis",
            State = "MN",
            ZipCode = "55401"
        },

        RequestingProvider = new ProviderInfo
        {
            Name = "Dr. Robert Chen, MD",
            Npi = "1234567890",
            Specialty = "Orthopedic Surgery",
            Phone = "(612) 555-0200",
            Fax = "(612) 555-0201",
            AddressLine1 = "500 Medical Center Drive",
            City = "Minneapolis",
            State = "MN",
            ZipCode = "55415"
        },

        Authorization = new AuthorizationInfo
        {
            AuthorizationNumber = "AUTH-2024-0098765",
            RequestDate = DateTime.Today.AddDays(-5),
            DecisionDate = DateTime.Today,
            ServiceStartDate = DateTime.Today.AddDays(14),
            ServiceEndDate = DateTime.Today.AddDays(14),
            ServiceType = "Outpatient Surgery",
            PlaceOfService = "Ambulatory Surgical Center",
            ServiceLines =
            [
                new ServiceLineItem
                {
                    ProcedureCode = "27447",
                    Description = "Total Knee Arthroplasty (Right)",
                    UnitsRequested = 1,
                    UnitsApproved = 0,
                    Status = "Denied",
                    DenialReasonCode = "MN-001",
                    DenialReasonDescription = "Does not meet medical necessity criteria"
                },
                new ServiceLineItem
                {
                    ProcedureCode = "99213",
                    Description = "Post-operative follow-up visit",
                    UnitsRequested = 3,
                    UnitsApproved = 0,
                    Status = "Denied",
                    DenialReasonCode = "MN-001",
                    DenialReasonDescription = "Dependent on primary procedure"
                }
            ]
        },

        ContactPhone = "(800) 555-0100",
        ContactFax = "(800) 555-0101",
        ContactHours = "Monday – Friday, 8:00 AM – 6:00 PM CT",
        Website = "www.test.com",

        // Denial-specific
        DenialType = "Medical Necessity",
        DenialCategory = "Initial",
        UrgencyLevel = "Standard",
        ClinicalRationale = "Based on the clinical information submitted, the requested total knee " +
            "arthroplasty does not meet medical necessity criteria at this time. The documentation " +
            "provided does not demonstrate that conservative treatment options — including physical " +
            "therapy (minimum 6 weeks), corticosteroid injections, and anti-inflammatory medications " +
            "— have been attempted or have failed. Per InterQual criteria, surgical intervention " +
            "requires documented failure of conservative management.",
        GuidelineReference = "InterQual 2024 – Musculoskeletal: Joint Replacement Criteria, Section 4.2",
        ReviewerName = "Dr. Sarah Williams",
        ReviewerCredentials = "MD, Board Certified Orthopedic Surgery",
        ReviewDate = DateTime.Today,

        AppealRights = new AppealRightsInfo
        {
            AppealDeadlineDays = 180,
            AppealDeadlineDate = DateTime.Today.AddDays(180),
            AppealSubmissionAddress = "Test Appeals Department, P.O. Box 30512, Salt Lake City, UT 84130",
            AppealPhone = "(800) 555-0150",
            AppealFax = "(800) 555-0151",
            HasExpeditedAppealRight = true,
            ExpeditedAppealDeadlineDays = 72,
            ExternalReviewOrganization = "MAXIMUS Federal Services",
            ExternalReviewPhone = "(800) 555-0175",
            StateInsuranceDepartmentInfo = "Minnesota Department of Commerce: (651) 539-1500"
        },

        AdditionalNotes =
        [
            "You may request a copy of the clinical criteria used to make this determination at no cost.",
            "If you have additional clinical information that was not previously submitted, please include it with your appeal."
        ]
    };

    public static ApprovalLetterModel CreateSampleApprovalLetter() => new()
    {
        LetterDate = DateTime.Today,
        EffectiveDate = DateTime.Today,

        Organization = new OrganizationInfo
        {
            Name = "Test Health Solutions",
            AddressLine1 = "9900 Bren Road East",
            City = "Minnetonka",
            State = "MN",
            ZipCode = "55343",
            Phone = "(800) 555-0100"
        },

        Plan = new PlanInfo
        {
            PlanName = "Test Choice Plus",
            PlanId = "Test-CP-2024",
            GroupNumber = "GRP-887421",
            LineOfBusiness = "Commercial"
        },

        Member = new MemberInfo
        {
            FirstName = "Michael",
            LastName = "Thompson",
            MemberId = "U9876543210",
            DateOfBirth = new DateTime(1972, 7, 22),
            AddressLine1 = "789 Oak Avenue",
            City = "St. Paul",
            State = "MN",
            ZipCode = "55102"
        },

        RequestingProvider = new ProviderInfo
        {
            Name = "Dr. Lisa Patel, MD",
            Npi = "9876543210",
            Specialty = "Cardiology",
            Phone = "(651) 555-0300"
        },

        Authorization = new AuthorizationInfo
        {
            AuthorizationNumber = "AUTH-2024-0054321",
            RequestDate = DateTime.Today.AddDays(-3),
            DecisionDate = DateTime.Today,
            ServiceStartDate = DateTime.Today.AddDays(7),
            ServiceEndDate = DateTime.Today.AddDays(7),
            ServiceType = "Outpatient Diagnostic",
            PlaceOfService = "Hospital Outpatient",
            ServiceLines =
            [
                new ServiceLineItem
                {
                    ProcedureCode = "93306",
                    Description = "Transthoracic Echocardiogram with Doppler",
                    UnitsRequested = 1,
                    UnitsApproved = 1,
                    Status = "Approved"
                },
                new ServiceLineItem
                {
                    ProcedureCode = "93017",
                    Description = "Cardiovascular Stress Test",
                    UnitsRequested = 1,
                    UnitsApproved = 1,
                    Status = "Approved"
                }
            ]
        },

        ContactPhone = "(800) 555-0100",
        ContactHours = "Monday – Friday, 8:00 AM – 6:00 PM CT",
        Website = "www.test.com",

        ApprovalType = "Full",
        ApprovalExpirationDate = DateTime.Today.AddDays(90),
        Conditions =
        [
            "Services must be performed at an in-network facility.",
            "Prior authorization is valid for 90 days from the date of this letter."
        ],
        ReviewerName = "Clinical Review Team"
    };
}
