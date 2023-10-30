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
        public string VatChartJson { get; set; }
        public string VatProcChartJson { get; set; }
        public string VatB2BChartJson { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var res = LoadVATChartData().GetAwaiter().GetResult();
            return Page();
        }

        public async Task<bool> LoadVATChartData()
        {
            List<DashboardChartData> data = new List<DashboardChartData>();
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
                data = await _masterPage.ExecuteStoredProcedure<DashboardChartData>(qq,new List<SqlParameter>(),false);
                var chartData = @"{
                type: 'pie',
                responsive: true
                ,data:
                {
                    labels: ['Failed', 'Submitted'],
                    datasets: [{
                        label: 'VAT',
                        data: [],
                        backgroundColor: [
                        'rgba(255, 118, 118, 1)',
                        'rgba(121, 106, 238, 1)',
                            ],
                        borderColor: [
                        'rgba(255, 118, 118, 0.7)',
                        'rgba(121, 106, 238, 0.7)',
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

                Chart = JsonConvert.DeserializeObject<ChartJs>(chartData) ?? new ChartJs();

                Chart.data.datasets[0].data = new int[2];
                Chart.data.datasets[0].data[1] = data[0].TotalVatSubmitted > 0 ? Convert.ToInt32(data[0].TotalVatSubmitted) : 0;
                Chart.data.datasets[0].data[0] = data[0].TotalVatFailed > 0 ? Convert.ToInt32(data[0].TotalVatFailed) : 0;

                VatChartJson = JsonConvert.SerializeObject(Chart, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });

                chartData = @"{
                type: 'pie',
                responsive: true
                ,data:
                {
                    labels: ['Pending', 'Processed', 'Count'],
                    datasets: [{
                        label: 'VAT',
                        data: [],
                        backgroundColor: [
                        'rgba(255, 118, 118, 1)',
                        'rgba(121, 106, 238, 1)',
                        'rgba(255, 195, 109, 1)',
                            ],
                        borderColor: [
                        'rgba(255, 118, 118, 0.7)',
                        'rgba(121, 106, 238, 0.7)',
                        'rgba(255, 195, 109, 0.7)',
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

                Chart = JsonConvert.DeserializeObject<ChartJs>(chartData) ?? new ChartJs();

                Chart.data.datasets[0].data = new int[3];
                
                Chart.data.datasets[0].data[0] = data[0].VATProcessed > 0 ? Convert.ToInt32(data[0].VATProcessed) : 0;
                Chart.data.datasets[0].data[1] = data[0].VATPending > 0 ? Convert.ToInt32(data[0].VATPending) : 0;
                Chart.data.datasets[0].data[2] = data[0].CountProcessed > 0 ? Convert.ToInt32(data[0].CountProcessed) : 0;

                VatProcChartJson = JsonConvert.SerializeObject(Chart, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });


                chartData = @"{
                type: 'pie',
                responsive: true
                ,data:
                {
                    labels: ['B2B', 'B2C'],
                    datasets: [{
                        label: 'VAT',
                        data: [],
                        backgroundColor: [
                        'rgba(255, 195, 109, 1)',
                        'rgba(121, 106, 238, 1)',
                            ],
                        borderColor: [
                        'rgba(255, 195, 109, 0.7)',
                        'rgba(121, 106, 238, 0.7)',
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

                Chart = JsonConvert.DeserializeObject<ChartJs>(chartData) ?? new ChartJs();

                Chart.data.datasets[0].data = new int[2];

                Chart.data.datasets[0].data[0] = data[0].VatB2B > 0 ? Convert.ToInt32(data[0].VatB2B) : 0;
                Chart.data.datasets[0].data[1] = data[0].VatB2C > 0 ? Convert.ToInt32(data[0].VatB2C) : 0;

                VatB2BChartJson = JsonConvert.SerializeObject(Chart, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });
            }
            catch (Exception ex)
            {
                new Basepage().logWrite(ex.ToString(), LOGTYPE.LG);
                return false;
            }
            return true;
        }
    }
}