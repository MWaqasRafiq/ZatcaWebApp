namespace ZatcaWebApp_V2.Helper
{
    public class LinqDataResult<T>
    {
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
