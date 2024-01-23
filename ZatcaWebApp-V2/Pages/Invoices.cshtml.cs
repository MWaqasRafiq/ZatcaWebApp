using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using ZatcaWebApp_V2.Common;
using ZatcaWebApp_V2.DataModel;
using ZatcaWebApp_V2.ViewModel;
using System.Linq.Dynamic.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ZatcaWebApp_V2.DataModel.Repository;
using ZatcaWebApp_V2.DataModel.Repository.Interface;
using Microsoft.Extensions.Primitives;

namespace ZatcaWebApp_V2.Pages
{
    public class InvoicesModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly IReportsRepository _repository;
        private readonly ApplicationDBContext _context;
        public InvoicesModel(ILogger<IndexModel> logger, IConfiguration configuration,
            ApplicationDBContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _repository = new ReportsRepository(_logger, _configuration, _context);
        }
        public Invoices invoicesVM { get; set; }
        public List<SelectListItem> VatNo { get; set; }
        public List<DataTable> invoicesViewModel { get; set; }
        public List<InvoicesViewModel> InvoicesVM { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            LoadVATDDL(false).GetAwaiter().GetResult();
            // GetDynamicTable(new Invoices()).ConfigureAwait(false).GetAwaiter().GetResult();
            DataSet ds = new DataSet("Invoices");
            using (SqlConnection conn = new SqlConnection(_context.Database.GetConnectionString()))
            {
                SqlCommand sqlComm = new SqlCommand("SP_RPT_INVOICE", conn);
                //sqlComm.Parameters.AddRange(param);

                sqlComm.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;

                da.Fill(ds);
            }
            invoicesViewModel = new List<DataTable>();
            invoicesViewModel.Add(ds.Tables[0]);

            return Page();
        }

        public async Task<IActionResult> OnPostInvoiceData()
        {
            try
            {
                int recordsTotal = 0;
                int recordsFiltered = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "5");
                int pageNum = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                if (pageNum > 0)
                    pageNum = (pageNum / pageSize) + 1;
                else
                    pageNum++;

                List<KeyValuePair<string, string>> valuePairs = new List<KeyValuePair<string, string>>();

                for(int i = 0; i < 17; i++)
                {
                    string val = Request.Form["columns[" + i + "]" + "[search][value]"].FirstOrDefault() ?? "";
                    if (!string.IsNullOrEmpty(val))
                    {
                        string colmn = Request.Form["columns[" + i + "]" + "[name]"].FirstOrDefault() ?? "";
                        valuePairs.Add(new KeyValuePair<string, string>(colmn,val));
                    }
                }

                var data = await _repository.GetInvoicesAsync(valuePairs, pageNum, pageSize);

                //var searchText = DataTablesRequest.Search.Value?.ToUpper();
                //if (!string.IsNullOrWhiteSpace(searchText))
                //{
                //    query = query.Where(s =>
                //        s.Name.ToUpper().Contains(searchText) ||
                //        s.PhoneNumber.ToUpper().Contains(searchText) ||
                //        s.Address.ToUpper().Contains(searchText) ||
                //        s.PostalCode.ToUpper().Contains(searchText)
                //    );
                //}

                if (data.Count > 0)
                {
                    recordsTotal = data[0].TotalRows;
                    recordsFiltered = recordsTotal;
                }

                return new JsonResult(new
                {
                    Draw = draw,//DataTablesRequest.Draw,
                    RecordsTotal = recordsTotal,
                    RecordsFiltered = recordsFiltered,
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Draw = 1,
                    RecordsTotal = 0,
                    RecordsFiltered = 0,
                    Data = new List<InvoicesVM>()
                });
            }
        }
        private async Task LoadVATDDL(bool enableAll)
        {
            try
            {
                enableAll = true;
                VatNo = new List<SelectListItem>();
                if (enableAll)
                    VatNo.Add(new SelectListItem("ALL", "ALL"));

                //var data = await _masterPage.ExecuteStoredProcedure<VatIdsVM>("select * from VATIDs where status=1", new List<SqlParameter>(), false);

                //var itemList = (from p in data
                //                select new SelectListItem { Value = p.ConnectionString, Text = p.VATNo.ToString() }).ToList<SelectListItem>();
                //VatNo.AddRange(itemList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        public async Task<List<DataTable>> GetDynamicTable(Invoices invoices)
        {
            invoicesViewModel = new List<DataTable>();
            var param = new SqlParameter[] {
                                new SqlParameter("@IRN", invoices.VatNo.ToString()),
                                //new SqlParameter("@DocType",docType_ddl.SelectedValue.Trim()),
                                //new SqlParameter("@ActionStatus",status_ddl.SelectedValue.Trim()),
                                new SqlParameter("@TransactionDate", invoices.ToDate),
                                //new SqlParameter("@StoreNo",valuesSelected),
                                //new SqlParameter("@TerminalNo", txtTermNo.Text.Trim()),
                                //new SqlParameter("@TrxNo", txtTrxNo.Text.Trim()),
                                //new SqlParameter("@Source",  ddlSource.SelectedValue.Trim()),
                                new SqlParameter("@FullResponse", invoices.FromDate),
                                //new SqlParameter("@CustIdentifier",Basepage.CustIdentifier),
                                };
            DataSet ds = new DataSet("Invoices");
            using (SqlConnection conn = new SqlConnection(_context.Database.GetConnectionString()))
            {
                SqlCommand sqlComm = new SqlCommand("SP_RPT_INVOICE", conn);
                sqlComm.Parameters.AddRange(param);

                sqlComm.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;

                da.Fill(ds);
            }

            foreach (var i in ds.Tables)
            {
                invoicesViewModel.Add((DataTable)i);
            }

            return invoicesViewModel;
            //invoicesViewModel = await _masterPage.ExecuteStoredProcedure<InvoicesViewModel>
            //                    ("SP_RPT_INVOICE", param.ToList(), true);
            //return invoicesViewModel;

        }

        public async Task<IActionResult> OnPostInvoicePartial()//[FromBody]Invoices invoices
        {
            InvoicesViewModel model = new InvoicesViewModel();
            //var param = new SqlParameter[] {
            //                    new SqlParameter("@IRN", invoices.VatNo.ToString()),
            //                    //new SqlParameter("@DocType",docType_ddl.SelectedValue.Trim()),
            //                    //new SqlParameter("@ActionStatus",status_ddl.SelectedValue.Trim()),
            //                    new SqlParameter("@TransactionDate", invoices.ToDate),
            //                    //new SqlParameter("@StoreNo",valuesSelected),
            //                    //new SqlParameter("@TerminalNo", txtTermNo.Text.Trim()),
            //                    //new SqlParameter("@TrxNo", txtTrxNo.Text.Trim()),
            //                    //new SqlParameter("@Source",  ddlSource.SelectedValue.Trim()),
            //                    new SqlParameter("@FullResponse", invoices.FromDate),
            //                    //new SqlParameter("@CustIdentifier",Basepage.CustIdentifier),
            //                    };
            DataSet ds = new DataSet("Invoices");
            using (SqlConnection conn = new SqlConnection(_context.Database.GetConnectionString()))
            {
                SqlCommand sqlComm = new SqlCommand("SP_RPT_INVOICE", conn);
                //sqlComm.Parameters.AddRange(param);

                sqlComm.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;

                da.Fill(ds);
            }

            model.InvoiceTable = ds.Tables[0];
            model.InvoiceTableHead = ds.Tables[1];

            return Partial("~/Pages/Shared/_InvoiceTable.cshtml", model);
        }

        public async Task<IActionResult> JsonInvoicePartial()//[FromBody]Invoices invoices
        {
            InvoicesViewModel model = new InvoicesViewModel();
            //var param = new SqlParameter[] {
            //                    new SqlParameter("@IRN", invoices.VatNo.ToString()),
            //                    //new SqlParameter("@DocType",docType_ddl.SelectedValue.Trim()),
            //                    //new SqlParameter("@ActionStatus",status_ddl.SelectedValue.Trim()),
            //                    new SqlParameter("@TransactionDate", invoices.ToDate),
            //                    //new SqlParameter("@StoreNo",valuesSelected),
            //                    //new SqlParameter("@TerminalNo", txtTermNo.Text.Trim()),
            //                    //new SqlParameter("@TrxNo", txtTrxNo.Text.Trim()),
            //                    //new SqlParameter("@Source",  ddlSource.SelectedValue.Trim()),
            //                    new SqlParameter("@FullResponse", invoices.FromDate),
            //                    //new SqlParameter("@CustIdentifier",Basepage.CustIdentifier),
            //                    };
            DataSet ds = new DataSet("Invoices");
            using (SqlConnection conn = new SqlConnection(_context.Database.GetConnectionString()))
            {
                SqlCommand sqlComm = new SqlCommand("SP_RPT_INVOICE", conn);
                //sqlComm.Parameters.AddRange(param);

                sqlComm.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;

                da.Fill(ds);
            }

            model.InvoiceTable = ds.Tables[0];
            model.InvoiceTableHead = ds.Tables[1];

            return new JsonResult(model);
        }
      

    }
}
