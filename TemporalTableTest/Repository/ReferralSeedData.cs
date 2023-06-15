using TemporalTableTest.Entities;

namespace TemporalTableTest.Repository;

public static class ReferralSeedData
{
#if SeedJasonService
    const string JsonService = "{\"Id\":\"ba1cca90-b02a-4a0b-afa0-d8aed1083c0d\",\"Name\":\"Test County Council\",\"Description\":\"Test County Council\",\"Logo\":null,\"Uri\":\"https://www.test.gov.uk/\",\"Url\":\"https://www.test.gov.uk/\",\"Services\":[{\"Id\":\"c1b5dd80-7506-4424-9711-fe175fa13eb8\",\"Name\":\"Test Organisation for Children with Tracheostomies\",\"Description\":\"Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.\",\"Accreditations\":null,\"Assured_date\":null,\"Attending_access\":null,\"Attending_type\":null,\"Deliverable_type\":null,\"Status\":\"active\",\"Url\":\"www.testservice.com\",\"Email\":\"support@testservice.com\",\"Fees\":null,\"ServiceDelivery\":[{\"Id\":\"14db2aef-9292-4afc-be09-5f6f43765938\",\"ServiceDelivery\":2}],\"Eligibilities\":[{\"Id\":\"Test9109Children\",\"Eligibility\":\"\",\"Maximum_age\":0,\"Minimum_age\":13}],\"Contacts\":[{\"Id\":\"5eac5cb6-cc7e-444d-a29b-76ccb85be866\",\"Title\":\"Service\",\"Name\":\"\",\"Phones\":[{\"Id\":\"1568\",\"Number\":\"01827 65779\"}]}],\"Cost_options\":[],\"Languages\":[{\"Id\":\"442a06cd-aa14-4ea3-9f11-b45c1bc4861f\",\"Language\":\"English\"}],\"Service_areas\":[{\"Id\":\"68af19cd-bc81-4585-99a2-85a2b0d62a1d\",\"Service_area\":\"National\",\"Extent\":null,\"Uri\":\"http://statistics.data.gov.uk/id/statistical-geography/K02000001\"}],\"Service_at_locations\":[{\"Id\":\"Test1749\",\"Location\":{\"Id\":\"a878aadc-6097-4a0f-b3e1-77fd4511175d\",\"Name\":\"\",\"Description\":\"\",\"Latitude\":52.6312,\"Longitude\":-1.66526,\"Physical_addresses\":[{\"Id\":\"1076aaa8-f99d-4395-8e4f-c0dde8095085\",\"Address_1\":\"75 Sheepcote Lane\",\"City\":\", Stathe, Tamworth, Staffordshire, \",\"Postal_code\":\"B77 3JN\",\"Country\":\"England\",\"State_province\":null}]}}],\"Service_taxonomys\":[{\"Id\":\"Test9107\",\"Taxonomy\":{\"Id\":\"Test bccsource:Organisation\",\"Name\":\"Organisation\",\"Vocabulary\":\"Test BCC Data Sources\",\"Parent\":null}},{\"Id\":\"Test9108\",\"Taxonomy\":{\"Id\":\"Test bccprimaryservicetype:38\",\"Name\":\"Support\",\"Vocabulary\":\"Test BCC Primary Services\",\"Parent\":null}},{\"Id\":\"Test9109\",\"Taxonomy\":{\"Id\":\"Test bccagegroup:37\",\"Name\":\"Children\",\"Vocabulary\":\"Test BCC Age Groups\",\"Parent\":null}},{\"Id\":\"Test9110\",\"Taxonomy\":{\"Id\":\"Testbccusergroup:56\",\"Name\":\"Long Term Health Conditions\",\"Vocabulary\":\"Test BCC User Groups\",\"Parent\":null}}]}]}";
#endif

    public static IReadOnlyCollection<ReferralStatus> SeedStatuses()
    {
        return new HashSet<ReferralStatus>()
        {
            new ReferralStatus()
            {
                Name = "New",
                SortOrder = 0
            },
            new ReferralStatus()
            {
                Name = "Opened",
                SortOrder = 1
            },
            new ReferralStatus()
            {
                Name = "Accepted",
                SortOrder = 2
            },
            new ReferralStatus()
            {
                Name = "Declined",
                SortOrder = 3
            },
        };
    }

    public static IReadOnlyCollection<Entities.Referral> SeedReferral()
    {
        List<Entities.Referral> listReferrals = new()
        {
            new Entities.Referral
            {
                ReasonForSupport = "Joe has previously experienced domestic abuse. He is now safe and happy in a new relationship, however he continues to suffer from anxiety attacks as a result. He would like some counselling to help him deal with his past trauma.",
                EngageWithFamily = "Joe has only shared her email address. Please email him and he will respond.  He’s not comfortable being contacted any other way.",
                Recipient = new Recipient
                {
                    Name = "Joe Blogs",
                    Email = "joeblogs@email.com",
                    Telephone = "0121 111 2222",
                    TextPhone = "0712345678",
                    AddressLine1 = "Address Line 1",
                    AddressLine2 = "Address Line 2",
                    TownOrCity = "Town or City",
                    County = "County",
                    PostCode = "B37 2RX"
                },
                Referrer = new Referrer
                {
                    EmailAddress = "Joe.Professional@email.com",
                    Name = "Joe Professional",
                    PhoneNumber = "011 222 3333",
                    Role = "Social Worker",
                    Team = "Social Work team North"
                },
                Status = new ReferralStatus
                {
                    Id = 1,
                    Name = "New",
                    SortOrder = 1,
                },
                ReferralService = new Entities.ReferralService
                {
                    Id = 1,
                    Name = "Test Service",
                    Description = "Test Service Description",
                    ReferralOrganisation = new ReferralOrganisation
                    {
                        Id = 1,
                        ReferralServiceId = 1,
                        Name = "Test Organisation",
                        Description = "Test Organisation Description",
                    }
                }
            },

            new Entities.Referral
            {
                ReasonForSupport = "Fred has previously experienced domestic abuse. He is now safe and happy in a new relationship, however he continues to suffer from anxiety attacks as a result. He would like some counselling to help him deal with his past trauma.",
                EngageWithFamily = "Fred has only shared her email address. Please email him and he will respond.  He’s not comfortable being contacted any other way.",
                Recipient = new Recipient
                {
                    Name = "Fred Brown",
                    Email = "fred.brown@email.com",
                    Telephone = "0121 111 2223",
                    TextPhone = "0712345679",
                    AddressLine1 = "Address Line 1",
                    AddressLine2 = "Address Line 2",
                    TownOrCity = "Town or City",
                    County = "County",
                    PostCode = "B36 3RY"
                },
                Referrer = new Referrer
                {
                    EmailAddress = "Joe.Professional@email.com",
                    Name = "Joe Professional",
                    PhoneNumber = "011 222 3333",
                    Role = "Social Worker",
                    Team = "zSocial Work team North"
                },
                Status = new ReferralStatus
                {
                    Id = 1,
                    Name = "Opened",
                    SortOrder = 1,
                },
                ReferralService = new Entities.ReferralService
                {
                    Id = 1,
                    Name = "Test Service",
                    Description = "Test Service Description",
                    ReferralOrganisation = new ReferralOrganisation
                    {
                        Id = 1,
                        ReferralServiceId = 1,
                        Name = "Test Organisation",
                        Description = "Test Organisation Description",
                    }
                }
            }
        };

        return listReferrals;

    }
}
