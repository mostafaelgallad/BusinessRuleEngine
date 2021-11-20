using BusinessRuleEngine.Entities;
using FastMember;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessRuleEngine.Helpers
{
    public static class Test
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

        public static List<T> ConvertToListObject<T>(SqlDataReader rd) where T : class, new()
        {
            Type type = typeof(T);
            List<T> ts = new List<T>();
            var accessor = TypeAccessor.Create(type);
            var members = accessor.GetMembers();
            var t = new T();
            while (rd.Read())
            {
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
                ts.Add(t);
                t = new T();
            }
            return ts;
        }

        public static async Task<List<ContriesFromTestDB>> GetListOFContriesFromSql(DbContext context)
        {
            var timer = new Stopwatch();
            timer.Start();
      
            EnsureConnectionOpen(context);
            using (var command = CreateCommand(context, "GetListOFContries", CommandType.StoredProcedure))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {

                    List<ContriesFromTestDB> result = ConvertToListObject<ContriesFromTestDB>((SqlDataReader)dataReader);
                    //B: Run stuff you want timed
                    timer.Stop();

                    TimeSpan timeTaken = timer.Elapsed;
                    return result;
                }
            }
        }

        public static async Task<List<ContriesFromTestDB>> GetListOFContriesFromSql2(DbContext context)
        {
            var timer = new Stopwatch();
            timer.Start();

            EnsureConnectionOpen(context);
            using (var command = CreateCommand(context, "GetListOFContries", CommandType.StoredProcedure))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {

                    List<ContriesFromTestDB> result = new List<ContriesFromTestDB>();
                    while (dataReader.Read())
                    {
                        if (dataReader.HasRows)
                        {
                            var fff = Test.ConvertToObject<ContriesFromTestDB>((SqlDataReader)dataReader);
                            result.Add(fff);
                        }
                    }
                    //B: Run stuff you want timed
                    timer.Stop();

                    TimeSpan timeTaken = timer.Elapsed;
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

        private static DbCommand CreateCommand(DbContext context, string commandText, CommandType commandType, params SqlParameter[] parameters)
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
