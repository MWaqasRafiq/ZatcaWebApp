using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
//using ZatcaWebApp;
using ZatcaWebApp_V2.Common;
using ZatcaWebApp_V2.DataModel;
using ZatcaWebApp_V2.ViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public string ConnectionString { get; set; }
        public DashboardChartData dashboardChartData { get; set; }
        public List<SelectListItem> VatNo { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            LoadVATDDL(false).GetAwaiter().GetResult();
            LoadVATChartData("").GetAwaiter().GetResult();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            LoadVATDDL(false).GetAwaiter().GetResult();
            LoadVATChartData(ConnectionString).GetAwaiter().GetResult();
            return Page();
        }
        public async Task LoadVATDDL(bool enableAll)
        {
            //enableAll = true;
            VatNo = new List<SelectListItem>();
            if (enableAll)
                VatNo.Add(new SelectListItem("ALL", "ALL"));

            var data = await _masterPage.ExecuteStoredProcedure<VatIdsVM>("select * from VATIDs where status=1", new List<SqlParameter>(), false);

            var itemList = (from p in data
                               select new SelectListItem { Value = p.ConnectionString, Text = p.VATNo.ToString() }).ToList<SelectListItem>();
            VatNo.AddRange(itemList);
        }

        public async Task<bool> LoadVATChartData(string ConnectionString)
        {
            DashboardChartData data = new DashboardChartData();
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
                data = await _masterPage.ExecuteSingleStoredProcedure<DashboardChartData>(qq,new List<SqlParameter>(), ConnectionString, false);

                dashboardChartData = data;
                var chartData = @"{
                responsive: true,
                type: 'doughnut',
                options: 
                {
                    cutoutPercentage: 70,
                }
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
                        'rgba(255, 255, 255, 1)',
                        'rgba(255, 255, 255, 1)',
                            ],
                        borderWidth: 2
                    }]
                }
            }";

                Chart = JsonConvert.DeserializeObject<ChartJs>(chartData) ?? new ChartJs();

                Chart.data.datasets[0].data = new int[2];
                Chart.data.datasets[0].data[1] = data.TotalVatSubmitted > 0 ? Convert.ToInt32(data.TotalVatSubmitted) : 0;
                Chart.data.datasets[0].data[0] = data.TotalVatFailed > 0 ? Convert.ToInt32(data.TotalVatFailed) : 0;

                VatChartJson = JsonConvert.SerializeObject(Chart, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });

            //    chartData = @"{
            //    type: 'pie',
            //    responsive: true
            //    ,data:
            //    {
            //        labels: ['Pending', 'Processed', 'Count'],
            //        datasets: [{
            //            label: 'VAT',
            //            data: [],
            //            backgroundColor: [
            //            'rgba(255, 118, 118, 1)',
            //            'rgba(121, 106, 238, 1)',
            //            'rgba(255, 195, 109, 1)',
            //                ],
            //            borderColor: [
            //            'rgba(255, 255, 255, 1)',
            //            'rgba(255, 255, 255, 1)',
            //            'rgba(255, 255, 255, 1)',
            //                ],
            //            borderWidth: 2,
            //        }]
            //    }
            //}";

                chartData = @"{
                responsive: true,
                type: 'doughnut',
                options: 
                {
                    cutoutPercentage: 70,
                }
                ,data:
                {
                    labels: ['Pending', 'Processed', 'Count'],
                    datasets: [{
                        label: 'VAT',
                        data: [],
                        backgroundColor: [
                        'rgba(255, 118, 118, 1)',
                        'rgba(84, 230, 157, 1)',
                        'rgba(121, 106, 238, 1)',
                            ],
                        borderColor: [
                        'rgba(255, 255, 255, 1)',
                        'rgba(255, 255, 255, 1)',
                        'rgba(255, 255, 255, 1)',
                            ],
                        borderWidth: 2,
                    }]
                }
            }";

                Chart = JsonConvert.DeserializeObject<ChartJs>(chartData) ?? new ChartJs();

                Chart.data.datasets[0].data = new int[3];
                
                Chart.data.datasets[0].data[0] = data.VATPending > 0 ? Convert.ToInt32(data.VATPending) : 0;
                Chart.data.datasets[0].data[1] = data.VATProcessed > 0 ? Convert.ToInt32(data.VATProcessed) : 0;
                Chart.data.datasets[0].data[2] = data.CountProcessed > 0 ? Convert.ToInt32(data.CountProcessed) : 0;

                VatProcChartJson = JsonConvert.SerializeObject(Chart, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });


                chartData = @"{
                responsive: true,
                type: 'doughnut',
                options: 
                {
                    cutoutPercentage: 70,
                }
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
                        'rgba(255, 255, 255, 1)',
                        'rgba(255, 255, 255, 1)',
                            ],
                        borderWidth: 2
                    }]
                }
            }";

                Chart = JsonConvert.DeserializeObject<ChartJs>(chartData) ?? new ChartJs();

                Chart.data.datasets[0].data = new int[2];

                Chart.data.datasets[0].data[0] = data.VatB2B > 0 ? Convert.ToInt32(data.VatB2B) : 0;
                Chart.data.datasets[0].data[1] = data.VatB2C > 0 ? Convert.ToInt32(data.VatB2C) : 0;

                VatB2BChartJson = JsonConvert.SerializeObject(Chart, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });
            }
            catch (Exception ex)
            {
                //new Basepage().logWrite(ex.ToString(), LOGTYPE.LG);
                return false;
            }
            return true;
        }


        public IActionResult OnPostddlVAT_Changed([FromBody] string ConnectionString)
        {
            try
            {
                LoadVATChartData(ConnectionString).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
            }
                return new JsonResult("Hello " + ConnectionString);
        }
        public IActionResult OnPostGetAjax(string name)
        {
            return new JsonResult("Hello " + name);
        }
    }
}