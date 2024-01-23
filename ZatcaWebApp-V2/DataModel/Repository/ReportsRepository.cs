using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ZatcaWebApp_V2.Common;
using ZatcaWebApp_V2.DataModel.Repository.Interface;
using ZatcaWebApp_V2.Pages;
using ZatcaWebApp_V2.ViewModel;
using Dapper;
using System.Data.SqlClient;

namespace ZatcaWebApp_V2.DataModel.Repository
{
    public class ReportsRepository:IReportsRepository
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext _context; 
        private readonly MasterPage _masterPage;
        public ReportsRepository(ILogger<IndexModel> logger, IConfiguration configuration, ApplicationDBContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _masterPage = new MasterPage(_logger, _configuration, _context);
        }
        public async Task<List<InvoicesVM>> GetInvoicesAsync(List<KeyValuePair<string, string>> keyValues, int PageNumber = 0, int PageSize = 5)
        {
            List<InvoicesVM> invoices = new List<InvoicesVM>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConStr")))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@PageNumber", PageNumber);
                    parameters.Add("@PageSize", PageSize);
                    if(!string.IsNullOrEmpty(keyValues.FirstOrDefault(x => x.Key == "irn").Value))
                        parameters.Add("@IRN", keyValues.FirstOrDefault(x => x.Key == "irn").Value);
                    
                    if(!string.IsNullOrEmpty(keyValues.FirstOrDefault(x => x.Key == "docType").Value))
                        parameters.Add("@DocType", keyValues.FirstOrDefault(x => x.Key == "docType").Value);
                    
                    if(!string.IsNullOrEmpty(keyValues.FirstOrDefault(x => x.Key == "actionStatus").Value))
                        parameters.Add("@ActionStatus", keyValues.FirstOrDefault(x => x.Key == "actionStatus").Value);

                    if (!string.IsNullOrEmpty(keyValues.FirstOrDefault(x => x.Key == "transactionDate").Value))
                        parameters.Add("@TransactionDate", keyValues.FirstOrDefault(x => x.Key == "transactionDate").Value);

                    if (!string.IsNullOrEmpty(keyValues.FirstOrDefault(x => x.Key == "storeNo").Value))
                        parameters.Add("@StoreNo", keyValues.FirstOrDefault(x => x.Key == "storeNo").Value);

                    if (!string.IsNullOrEmpty(keyValues.FirstOrDefault(x => x.Key == "terminalNo").Value))
                        parameters.Add("@TerminalNo", keyValues.FirstOrDefault(x => x.Key == "terminalNo").Value);

                    if (!string.IsNullOrEmpty(keyValues.FirstOrDefault(x => x.Key == "trxNo").Value))
                        parameters.Add("@TrxNo", keyValues.FirstOrDefault(x => x.Key == "trxNo").Value);

                    if (!string.IsNullOrEmpty(keyValues.FirstOrDefault(x => x.Key == "source").Value))
                        parameters.Add("@Source", keyValues.FirstOrDefault(x => x.Key == "source").Value);

                    //parameters.Add(new SqlParameter() { ParameterName = "", Value = ""});
                    //var parameters = new { 
                    //    PageNumber = PageNumber, 
                    //    PageSize = PageSize,
                    //    IRN = string.IsNullOrEmpty(keyValues.FirstOrDefault(x=>x.Key == "irn").Value) ? ",
                    //    DocType = keyValues.FirstOrDefault(x=>x.Key == "docType").Value,
                    //    ActionStatus = keyValues.FirstOrDefault(x=>x.Key == "actionStatus").Value
                    //};

                    invoices = (List<InvoicesVM>)connection.Query<InvoicesVM>
                        ("SP_RPT_INVOICE", parameters, commandType: System.Data.CommandType.StoredProcedure);
                }
                //var param = new SqlParameter[] 
                //{
                //    new SqlParameter(){ParameterName = "PageNumber", Value = PageNumber},
                //    new SqlParameter(){ParameterName = "PageSize", Value = PageSize}
                //};
                //invoices = await _masterPage.ExecuteStoredProcedure<InvoicesVM>("SP_RPT_INVOICE", param.ToList(), true);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetInvoicesAsync: "+ ex.Message);
            }
            return invoices;
        }
    }
}
