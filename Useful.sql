select * from [dbo].[Recipients]

select * from [dbo].[RecipientsHistory]

select substring('ABCDEFGHIJKLMNOPQRSTUVWXYZ',
                 (abs(checksum(newid())) % 26)+1, 1)

SELECT 'Obfuscated ' + LEFT(REPLACE(REPLACE((SELECT CRYPT_GEN_RANDOM(16) FOR XML PATH(''), BINARY BASE64),'+',''),'/',''),16);

select top 1 * from sys.procedures where [type_desc] = 'sp_Obfuscate'

Exec sp_Obfuscate @Id = 1

DECLARE
    @columnNames    NVARCHAR(MAX)
,   @dSql           NVARCHAR(MAX) = ''

SET @columnNames = 'John,Smith'

select value FROM STRING_SPLIT(@columnNames, ',')