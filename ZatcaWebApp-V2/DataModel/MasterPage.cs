using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using System.Reflection;
using ZatcaWebApp_V2.Common;
using ZatcaWebApp_V2.Pages;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ZatcaWebApp_V2.DataModel
{
    public class MasterPage
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext _context;
        private static string ConStr = string.Empty;
        public MasterPage(ILogger<IndexModel> logger, IConfiguration configuration, ApplicationDBContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            ConStr = _configuration.GetConnectionString("ConStr") ?? "";
        }
        public async Task<DataTable> getDataDBQuery(string query)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            try
            {
                await using (SqlConnection conn = new SqlConnection(ConStr))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    //da.SelectCommand.CommandTimeout = 50;
                    da.Fill(dt);
                    da.Dispose();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return dt;
        }

        public async Task<List<T>> ExecuteStoredProcedure<T>(string storedProcedure, List<SqlParameter> parameters, bool IsProcedure = true) where T : new()
        {
            List<T> entity = new List<T>();
            try
            {
                await using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = storedProcedure;
                    if (IsProcedure)
                        command.CommandType = CommandType.StoredProcedure;
                    else
                        command.CommandType = CommandType.Text;

                    // set some parameters of the stored procedure
                    foreach (var parameter in parameters)
                    {
                        parameter.Value = parameter.Value ?? DBNull.Value;
                        command.Parameters.Add(parameter);
                    }

                    if (command.Connection.State != ConnectionState.Open)
                        command.Connection.Open();

                    using (var dataReader = await command.ExecuteReaderAsync())
                    {
                        entity = DataReaderMapToList<T>(dataReader);
                        command.Connection.Close();
                        //return entity;
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
            return entity;
        }
        public async Task<T> ExecuteSingleStoredProcedure<T>(string storedProcedure, List<SqlParameter> parameters, string ConnectionString, bool IsProcedure = true) where T : new()
        {
            T entity = new T();
            try
            {
                if (string.IsNullOrEmpty(ConnectionString))
                {
                    await using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = storedProcedure;
                        if (IsProcedure)
                            command.CommandType = CommandType.StoredProcedure;
                        else
                            command.CommandType = CommandType.Text;

                        // set some parameters of the stored procedure
                        foreach (var parameter in parameters)
                        {
                            parameter.Value = parameter.Value ?? DBNull.Value;
                            command.Parameters.Add(parameter);
                        }

                        if (command.Connection.State != ConnectionState.Open)
                            command.Connection.Open();

                        using (var dataReader = await command.ExecuteReaderAsync())
                        {
                            entity = DataReaderMapToModel<T>(dataReader);
                            command.Connection.Close();
                            return entity;
                        }
                    }
                }
                else
                {
                    await using (var connection = new SqlConnection(ConnectionString))
                    {
                        using (var command = new SqlCommand(storedProcedure, connection))
                        {
                            if (IsProcedure)
                                command.CommandType = CommandType.StoredProcedure;
                            else
                                command.CommandType = CommandType.Text;

                            foreach (var parameter in parameters)
                            {
                                parameter.Value = parameter.Value ?? DBNull.Value;
                                command.Parameters.Add(parameter);
                            }
                            if (command.Connection.State != ConnectionState.Open)
                                command.Connection.Open();

                            using (var dataReader = await command.ExecuteReaderAsync())
                            {
                                entity = DataReaderMapToModel<T>(dataReader);
                                command.Connection.Close();
                                return entity;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return entity;
        }

        private List<T> DataReaderMapToList<T>(DbDataReader dr)
        {
            List<T> list = new List<T>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    var obj = Activator.CreateInstance<T>();
                    foreach (PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        if (HasColumn(dr, prop.Name) && !Equals(dr[prop.Name], DBNull.Value))
                        {
                            prop.SetValue(obj, dr[prop.Name], null);
                        }
                    }
                    list.Add(obj);
                }
                return list;
            }
            return new List<T>();
        }
        private T DataReaderMapToModel<T>(DbDataReader dr) where T : new()
        {
            T list = new T();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    var obj = Activator.CreateInstance<T>();
                    foreach (PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        if (HasColumn(dr, prop.Name) && !Equals(dr[prop.Name], DBNull.Value))
                        {
                            prop.SetValue(obj, dr[prop.Name], null);
                        }
                    }
                    list = obj;
                }
                return list;
            }
            return list;
        }

        public bool HasColumn(IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
