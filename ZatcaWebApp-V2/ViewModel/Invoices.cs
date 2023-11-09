using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ZatcaWebApp_V2.ViewModel
{
    public class Invoices
    {
        public string VatNo { get; set; }
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public string FromDate { get; set; }
        //[DataType(DataType.Date)]
        public string ToDate { get; set; }
        public int? Irn { get; set; }
        public int? StoreNo { get; set; }
        public int? TermNo { get; set; }
        public int? TransNo { get; set; }
        public int? Status { get; set; }
        public int? Source { get; set; }
        public int? DocType { get; set; }
    }

    public class InvoicesViewModel
    {
        public DataTable InvoiceTable { get; set; }
        public DataTable InvoiceTableHead { get; set; }
    }
}
