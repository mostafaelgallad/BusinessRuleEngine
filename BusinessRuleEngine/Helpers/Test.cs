using BusinessRuleEngine.Entities;
using FastMember;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessRuleEngine.Helpers
{
    public  static class Test
    {
        public static T ConvertToObject<T>(this SqlDataReader rd) where T : class, new()
        {
            Type type = typeof(T);
            var accessor = TypeAccessor.Create(type);
            var members = accessor.GetMembers();
            var t = new T();

            for (int i = 0; i < rd.FieldCount; i++)
            {
                if (!rd.IsDBNull(i))
                {
                    string fieldName = rd.GetName(i);

                    if (members.Any(m => string.Equals(m.Name, fieldName, StringComparison.OrdinalIgnoreCase)))
                    {
                        accessor[t, fieldName] = rd.GetValue(i);
                    }
                }
            }
            return t;
        }

        public static async Task<List<ContriesFromTestDB>> GetListOFContriesFromSql(DbContext context)
        {
            EnsureConnectionOpen(context);
            using (var command = CreateCommand(context, "GetListOFContries", CommandType.StoredProcedure))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {

                    List<ContriesFromTestDB> result = new List<ContriesFromTestDB>();
                    DataTable schemaTable = dataReader.GetSchemaTable();
                    while (dataReader.Read())
                    {
                        if (dataReader.HasRows)
                        {
                            var fff = Test.ConvertToObject<ContriesFromTestDB>((SqlDataReader)dataReader);
                            result.Add(fff);
                        }
                    }
                    return result;
                }
            }
        }

        private static void EnsureConnectionOpen(DbContext context)
        {
            var connection = context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        private static DbCommand CreateCommand(DbContext context,string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = commandText;
            command.CommandType = commandType;
            //command.Transaction = GetActiveTransaction();
            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command;
        }
    }
}
