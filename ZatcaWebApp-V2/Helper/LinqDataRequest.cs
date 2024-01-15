namespace ZatcaWebApp_V2.Helper
{
    public class LinqDataRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public IEnumerable<Sort> Sort { get; set; }
    }
}
