namespace ZatcaWebApp_V2.ViewModel
{
    public class InvoicesVM
    {
        public string IRN { get; set; }
        public string UUID { get; set; }
        public string XMLFileName { get; set; }
        //public decimal TaxTotal { get; set; }
        public string DocType { get; set; }
        public string Action { get; set; }
        public string ActionStatus { get; set; }
        public DateTime TransactionDate { get; set; }
        public string StoreNo { get; set; }
        public string TerminalNo { get; set; }
        public string TrxNo { get; set; }
        public string TimeleftToReport { get; set; }
        public string ReportedIn { get; set; }
        public string Source { get; set; }
        public int ActionCount { get; set; }
        public int ErrorCount { get; set; }
        public int WarningCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int TotalRows { get; set; }
    }

    public class InvoiceVMs
    {
        public List<InvoicesVM> invoices { get; set; }
        public InvoiceVMs()
        {
            invoices = new List<InvoicesVM>();
        }
    }
}
