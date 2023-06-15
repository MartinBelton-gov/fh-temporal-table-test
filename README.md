# TemporalTableTest
Console Application to Test Temporal Tables with Entity Framework.

## Requirements

* DotNet Core 7.0 and any supported IDE for DEV running.


## Local running

In the appsetting.json file you set the Database connection string


The startup project is: TemporalTableTest


## Migrations

To Add Migration

<br />
 dotnet ef migrations add CreateIntialSchema 
<br />

To Apply Latest Schema Manually

<br />
 dotnet ef database update 
<br />

