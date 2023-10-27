using Microsoft.Data.SqlClient;
using System.Data;
using ZatcaWebApp_V2.Pages;

namespace ZatcaWebApp_V2.DataModel
{
    public class MasterPage
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        private static string ConStr = string.Empty;
        public MasterPage(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
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
    }
}
