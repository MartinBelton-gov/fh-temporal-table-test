using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace TemporalTableTest.Repository;

internal static class DbContextExtensions
{
    public static bool StoredProcedureExists(this DbContext context,
        string procedureName)
    {
        string query = $"select top 1 * from sys.procedures where [type_desc] = '{procedureName}'";

        var fs = FormattableStringFactory.Create(query);

        return context.Database.SqlQuery<string>(fs).Any();
    }
}
