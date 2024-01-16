using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ZatcaWebApp_V2.Common;
using ZatcaWebApp_V2.DataModel.Repository.Interface;
using ZatcaWebApp_V2.Pages;
using ZatcaWebApp_V2.ViewModel;

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
        public async Task<List<InvoicesVM>> GetInvoicesAsync(int PageNumber = 0, int PageSize = 5)
        {
            List<InvoicesVM> invoices = new List<InvoicesVM>();
            try
            {
                var param = new SqlParameter[] 
                {
                    new SqlParameter(){ParameterName = "PageNumber", Value = PageNumber},
                    new SqlParameter(){ParameterName = "PageSize", Value = PageSize}
                };
                //_context.Database.ExecuteSqlRaw("EXEC StoredProcedureName");
                invoices = await _masterPage.ExecuteStoredProcedure<InvoicesVM>
                                        ("SP_RPT_INVOICE", param.ToList(), true);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetInvoicesAsync: "+ ex.Message);
            }
            return invoices;
        }
    }
}
