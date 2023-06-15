using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using TemporalTableTest.Entities;

namespace TemporalTableTest.Repository;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitialiseAsync(bool shouldClearDatabaseOnRestart)
    {
        try
        {
            if (shouldClearDatabaseOnRestart)
                await _context.Database.EnsureDeletedAsync();

            if (_context.Database.IsSqlServer())
                await _context.Database.MigrateAsync();
            else
                await _context.Database.EnsureCreatedAsync();

            await SeedAsync();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            if (!_context.Recipients.Any())
            {
                await TrySeedAsync();
                await UpdateRecipients();
                await UpdateReferrals();
                
                if (!_context.StoredProcedureExists("sp_Obfuscate"))
                    CreateRecipientObfuscationStoredProcedure();

                if (!_context.StoredProcedureExists("sp_ObfuscateReferral"))
                    CreateReferralObfuscationStoredProcedure();

                //Use Stored Procedure
                _context.Database.ExecuteSql(FormattableStringFactory.Create("Exec sp_Obfuscate @Id = 1"));

                _context.Database.ExecuteSql(FormattableStringFactory.Create("Exec sp_ObfuscateReferral @Id = 1, @names = 'joe,Blogs'"));

            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        if (!_context.ReferralStatuses.Any())
        {
            IReadOnlyCollection<ReferralStatus> statuses = ReferralSeedData.SeedStatuses();

            _context.ReferralStatuses.AddRange(statuses);

            await _context.SaveChangesAsync();
        }

        if (!_context.Referrals.Any())
        {
            IReadOnlyCollection<Entities.Referral> referrals = ReferralSeedData.SeedReferral();

            foreach (Entities.Referral referral in referrals)
            {
                var status = _context.ReferralStatuses.SingleOrDefault(x => x.Name == referral.Status.Name);
                if (status != null)
                {
                    referral.Status = status;
                }

                var service = _context.ReferralServices.SingleOrDefault(x => x.Name == referral.ReferralService.Name);
                if ( service != null)
                {
                    referral.ReferralService = service;
                }

                _context.Referrals.Add(referral);

                await _context.SaveChangesAsync();
            }

            

            
        }
        //IReadOnlyCollection<Recipient> recipients = SeedRecipients();

        //_context.Recipients.AddRange(recipients);

        //await _context.SaveChangesAsync();  
    }

    private static IReadOnlyCollection<Recipient> SeedRecipients()
    {
        return new List<Recipient>()
        {
            new Recipient
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

            new Recipient
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
            }
        };

    }

    public async Task UpdateReferrals()
    {
        Referral? referral = _context.Referrals.SingleOrDefault(x => x.Recipient.Email == "joeblogs@email.com");
        if (referral != null)
        {
            referral.ReasonForSupport += " Some Extra Text";
            referral.EngageWithFamily += " Some Extra Text";
        }

        referral = _context.Referrals.SingleOrDefault(x => x.Recipient.Email == "fred.brown@email.com");
        if (referral != null)
        {
            referral.ReasonForSupport += " Some Extra Text";
            referral.EngageWithFamily += " Some Extra Text";
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateRecipients()
    {
        Recipient? recipient = _context.Recipients.FirstOrDefault(x => x.Email == "joeblogs@email.com");
        if (recipient != null) 
        {
            recipient.Telephone = "0121 111 3223";
            recipient.TextPhone = "0712345671";
            recipient.AddressLine1 = "Address Line 1A";
            recipient.AddressLine2 = "Address Line 2A";

            await _context.SaveChangesAsync();
        }

        recipient = _context.Recipients.FirstOrDefault(x => x.Email == "fred.brown@email.com");
        if (recipient != null)
        {
            recipient.Telephone = "0161 111 4444";
            recipient.TextPhone = "0712345672";
            recipient.AddressLine1 = "Address Line 1B";
            recipient.AddressLine2 = "Address Line 2B";

            await _context.SaveChangesAsync();
        }

        
    }

    public List<dynamic> GetHistory()
    {
        var history = _context.Recipients.TemporalAll().Where(x => x.Email == "joeblogs@email.com")
              .OrderByDescending(person => EF.Property<DateTime>(person, "PeriodStart"))
              .Select(person => new {
                  Recipient = person,
                  PeriodStart = EF.Property<DateTime>(person, "PeriodStart"),
                  PeriodEnd = EF.Property<DateTime>(person, "PeriodEnd")
              }).ToList<dynamic>();



        return history;
    }

    private void CreateReferralObfuscationStoredProcedure()
    {
        string storedProcedure = @"CREATE PROCEDURE [dbo].[sp_ObfuscateReferral]
	-- Add the parameters for the stored procedure here
	@Id BigInt,
	@names NVARCHAR(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; 
	BEGIN TRANSACTION;

	ALTER TABLE [dbo].[Referrals] SET (SYSTEM_VERSIONING = OFF);

	DECLARE @nameList TABLE (Name NVARCHAR(MAX))

    INSERT INTO @nameList (Name)
    SELECT value
    FROM STRING_SPLIT(@names, ',')

    UPDATE [dbo].[Referrals]
    SET [ReasonForSupport] = REPLACE(ReasonForSupport, nl.Name, 'XXXX'),
	[EngageWithFamily] = REPLACE(EngageWithFamily, nl.Name, 'XXXX')
    FROM [dbo].[Referrals]
    INNER JOIN @nameList nl ON [ReasonForSupport] LIKE '%' + nl.Name + '%'
	WHERE Id = @Id

	DECLARE @dynamicSQL NVARCHAR(MAX);

	SET @dynamicSQL = N'
		DECLARE @nameList TABLE (Name NVARCHAR(MAX));

		INSERT INTO @nameList (Name)
		SELECT value
		FROM STRING_SPLIT(''' + REPLACE(@names, '''', '''''') + ''', '','');

		UPDATE [dbo].[ReferralsHistory]
		SET [ReasonForSupport] = REPLACE(ReasonForSupport, nl.Name, ''XXXX''),
			[EngageWithFamily] = REPLACE(EngageWithFamily, nl.Name, ''XXXX'')
		FROM [dbo].[ReferralsHistory]
		INNER JOIN @nameList nl ON [ReasonForSupport] LIKE ''%'' + nl.Name + ''%''
		WHERE Id = ' + CAST(@Id AS NVARCHAR(MAX));

	EXEC sp_executesql @dynamicSQL;

	ALTER TABLE [dbo].[Referrals] SET (SYSTEM_VERSIONING = ON (  HISTORY_TABLE = [dbo].[ReferralsHistory], DATA_CONSISTENCY_CHECK = ON));

	COMMIT TRANSACTION;

END";

        var fs = FormattableStringFactory.Create(storedProcedure);
        _context.Database.ExecuteSql(fs);

    }
    private void CreateRecipientObfuscationStoredProcedure()
    {
        string storedProcedure = @"CREATE PROCEDURE sp_Obfuscate
	-- Add the parameters for the stored procedure here
	@Id BigInt
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; 
	BEGIN TRANSACTION;

	ALTER TABLE [dbo].[Recipients] SET (SYSTEM_VERSIONING = OFF);

	DECLARE @Name NVARCHAR(max) = 'Obfuscated ' + LEFT(REPLACE(REPLACE((SELECT CRYPT_GEN_RANDOM(16) FOR XML PATH(''), BINARY BASE64),'+',''),'/',''),16),
	@Email NVARCHAR(max) = 'Obfuscated ' + LEFT(REPLACE(REPLACE((SELECT CRYPT_GEN_RANDOM(16) FOR XML PATH(''), BINARY BASE64),'+',''),'/',''),16),
	@Telephone NVARCHAR(max) = 'Obfuscated ' + LEFT(REPLACE(REPLACE((SELECT CRYPT_GEN_RANDOM(16) FOR XML PATH(''), BINARY BASE64),'+',''),'/',''),16),
	@Textphone NVARCHAR(max) = 'Obfuscated ' + LEFT(REPLACE(REPLACE((SELECT CRYPT_GEN_RANDOM(16) FOR XML PATH(''), BINARY BASE64),'+',''),'/',''),16),
	@AddressLine1 NVARCHAR(max) = 'Obfuscated ' + LEFT(REPLACE(REPLACE((SELECT CRYPT_GEN_RANDOM(16) FOR XML PATH(''), BINARY BASE64),'+',''),'/',''),16),
	@AddressLine2 NVARCHAR(max) = 'Obfuscated ' + LEFT(REPLACE(REPLACE((SELECT CRYPT_GEN_RANDOM(16) FOR XML PATH(''), BINARY BASE64),'+',''),'/',''),16),
	@TownOrCity NVARCHAR(max) = 'Obfuscated ' + LEFT(REPLACE(REPLACE((SELECT CRYPT_GEN_RANDOM(16) FOR XML PATH(''), BINARY BASE64),'+',''),'/',''),16),
	@County NVARCHAR(max) = 'Obfuscated ' + LEFT(REPLACE(REPLACE((SELECT CRYPT_GEN_RANDOM(16) FOR XML PATH(''), BINARY BASE64),'+',''),'/',''),16),
	@PostCode NVARCHAR(max) = 'Obfuscated ' + LEFT(REPLACE(REPLACE((SELECT CRYPT_GEN_RANDOM(16) FOR XML PATH(''), BINARY BASE64),'+',''),'/',''),16)



	UPDATE [dbo].[Recipients]
   SET [Name] = @Name
      ,[Email] = @Email
      ,[Telephone] = @Telephone
      ,[TextPhone] = @Textphone
      ,[AddressLine1] = @AddressLine1
      ,[AddressLine2] = @AddressLine2
      ,[TownOrCity] = @TownOrCity
      ,[County] = @County
      ,[PostCode] = @PostCode
      --,[Created] = <Created, datetime2(7),>
      --,[CreatedBy] = <CreatedBy, nvarchar(max),>
      ,[LastModified] = GETUTCDATE()
      ,[LastModifiedBy] = 'System'
      WHERE Id = @Id

	  EXEC(N'
	  
	DECLARE @Name NVARCHAR(max) = ''Obfuscated '' + LEFT(REPLACE(REPLACE((SELECT CRYPT_GEN_RANDOM(16) FOR XML PATH(''''), BINARY BASE64),''+'',''''),''/'',''''),16)
	DECLARE @Email NVARCHAR(max) = ''Obfuscated '' + LEFT(REPLACE(REPLACE((SELECT CRYPT_GEN_RANDOM(16) FOR XML PATH(''''), BINARY BASE64),''+'',''''),''/'',''''),16)
	DECLARE @Telephone NVARCHAR(max) = ''Obfuscated '' + LEFT(REPLACE(REPLACE((SELECT CRYPT_GEN_RANDOM(16) FOR XML PATH(''''), BINARY BASE64),''+'',''''),''/'',''''),16)
	DECLARE @Textphone NVARCHAR(max) = ''Obfuscated '' + LEFT(REPLACE(REPLACE((SELECT CRYPT_GEN_RANDOM(16) FOR XML PATH(''''), BINARY BASE64),''+'',''''),''/'',''''),16)
	DECLARE @AddressLine1 NVARCHAR(max) = ''Obfuscated '' + LEFT(REPLACE(REPLACE((SELECT CRYPT_GEN_RANDOM(16) FOR XML PATH(''''), BINARY BASE64),''+'',''''),''/'',''''),16)
	DECLARE @AddressLine2 NVARCHAR(max) = ''Obfuscated '' + LEFT(REPLACE(REPLACE((SELECT CRYPT_GEN_RANDOM(16) FOR XML PATH(''''), BINARY BASE64),''+'',''''),''/'',''''),16)
	DECLARE @TownOrCity NVARCHAR(max) = ''Obfuscated '' + LEFT(REPLACE(REPLACE((SELECT CRYPT_GEN_RANDOM(16) FOR XML PATH(''''), BINARY BASE64),''+'',''''),''/'',''''),16)
	DECLARE @County NVARCHAR(max) = ''Obfuscated '' + LEFT(REPLACE(REPLACE((SELECT CRYPT_GEN_RANDOM(16) FOR XML PATH(''''), BINARY BASE64),''+'',''''),''/'',''''),16)
	DECLARE @PostCode NVARCHAR(max) = ''Obfuscated '' + LEFT(REPLACE(REPLACE((SELECT CRYPT_GEN_RANDOM(16) FOR XML PATH(''''), BINARY BASE64),''+'',''''),''/'',''''),16)
	  
	  UPDATE dbo.RecipientsHistory SET Name = @Name
      ,[Email] = @Email
      ,[Telephone] = @Telephone
      ,[TextPhone] = @Textphone
      ,[AddressLine1] = @AddressLine1
      ,[AddressLine2] = @AddressLine2
      ,[TownOrCity] = @TownOrCity
      ,[County] = @County
      ,[PostCode] = @PostCode
	  WHERE Id = ' + @Id + '');

	ALTER TABLE [dbo].[Recipients] SET (SYSTEM_VERSIONING = ON (  HISTORY_TABLE = [dbo].[RecipientsHistory], DATA_CONSISTENCY_CHECK = ON));

	COMMIT TRANSACTION;

END
";

        var fs = FormattableStringFactory.Create(storedProcedure);
        _context.Database.ExecuteSql(fs);
    }
}

