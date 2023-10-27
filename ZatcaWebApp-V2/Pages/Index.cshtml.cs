using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using ZatcaWebApp;
using ZatcaWebApp_V2.Common;
using ZatcaWebApp_V2.DataModel;
using ZatcaWebApp_V2.ViewModel;

namespace ZatcaWebApp_V2.Pages
{
    public class IndexModel : PageModel 
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly MasterPage _masterPage;
        private readonly ApplicationDBContext _context;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration, ApplicationDBContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _masterPage = new MasterPage(_logger, _configuration, _context);
        }
        public ChartJs Chart { get; set; }
        public string ChartJson { get; set; }
        public List<int> GetJson()
        {
            //get the data you want from database...
            int[] data = { 6,2,3 };
            return data.ToList();
        }
        public async void OnGet()
        {
            var data = await LoadChartData();
            var chartData = @"{
                type: 'pie',
                responsive: true
                ,data:
                {
                    labels: ['Red', 'Blue'],
                    datasets: [{
                        label: 'VAT',
                        data: [],
                        backgroundColor: [
                        'rgba(255, 99, 132, 0.2)',
                        'rgba(54, 162, 235, 0.2)',
                            ],
                        borderColor: [
                        'rgba(255, 99, 132, 1)',
                        'rgba(54, 162, 235, 1)',
                            ],
                        borderWidth: 1
                    }]
                },
                options:
                {
                    scales:
                    {
                        yAxes: [{
                            ticks:
                            {
                                beginAtZero: true
                            }
                        }]
                    }
                }
            }";

            Chart = JsonConvert.DeserializeObject<ChartJs>(chartData);

            var res = GetJson();  //get the data

            //must remember to initialize the array....
            Chart.data.datasets[0].data = new int[res.Count()];
            //for (int i = 0; i < res.Count(); i++)
            //{
            //    Chart.data.datasets[0].data[i] = res[i];
            //}
            if(data[0].TotalVatSubmitted > 0)
                    Chart.data.datasets[0].data[0] = Convert.ToInt32(data[0].TotalVatSubmitted);
            else
                    Chart.data.datasets[0].data[0] = 0;
            if(data[0].TotalVatFailed > 0)
                Chart.data.datasets[0].data[1] = Convert.ToInt32(data[0].TotalVatFailed);
            else
                Chart.data.datasets[0].data[1] = 0;
            ChartJson = JsonConvert.SerializeObject(Chart, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            });
        }

        public async Task<List<DashboardChartData>> LoadChartData()
        {
            List<DashboardChartData> chartData = new List<DashboardChartData>();
            try
            {
                string qq = @"select  
                            (select sum(taxtotal) from invoices where LTRIM(RTRIM(ActionStatus))  in('Cleared','Reported')) as TotalVatSubmitted,
                            (select sum(taxtotal) from invoices where LTRIM(RTRIM(ActionStatus)) not  in('Cleared','Reported')) as TotalVatFailed,
                            CAST('0' as int) as VATPending,
                            (select count(id) from invoices) as CountProcessed,
                            (select sum(taxtotal) from invoices) as VATProcessed,
                            (select sum(taxtotal) from invoices where DocType='B2B') as VatB2B,
                            (select sum(taxtotal) from invoices where DocType='B2C') as VatB2C";
                chartData = await _masterPage.ExecuteStoredProcedure<DashboardChartData>(qq,new List<SqlParameter>(),false);
                //DataTable dt = await masterPage.getDataDBQuery(qq);
            }
            catch (Exception ex)
            {
                new Basepage().logWrite(ex.ToString(), LOGTYPE.LG);
            }
            return chartData;
        }
    }
}