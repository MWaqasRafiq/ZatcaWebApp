using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data;
using ZatcaWebApp;
using ZatcaWebApp_V2.DataModel;

namespace ZatcaWebApp_V2.Pages
{
    public class IndexModel : PageModel 
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly MasterPage masterPage;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            masterPage = new MasterPage(logger,configuration);
        }

        public async void OnGet()
        {
            await LoadChartData();
        }

        public async Task LoadChartData()
        {
            try
            {
                string qq = @"select  
                            (select sum(taxtotal) from invoices where LTRIM(RTRIM(ActionStatus))  in('Cleared','Reported')) as TotalVatSubmitted,
                            (select sum(taxtotal) from invoices where LTRIM(RTRIM(ActionStatus)) not  in('Cleared','Reported')) as TotalVatFailed,
                            '0' as VATPending,
                            (select count(id) from invoices) as CountProcessed,
                            (select sum(taxtotal) from invoices) as VATProcessed,
                            (select sum(taxtotal) from invoices where DocType='B2B') as VatB2B,
                            (select sum(taxtotal) from invoices where DocType='B2C') as VatB2C";

                DataTable dt = await masterPage.getDataDBQuery(qq);
            }
            catch (Exception ex)
            {
                new Basepage().logWrite(ex.ToString(), LOGTYPE.LG);
            }
        }
    }
}