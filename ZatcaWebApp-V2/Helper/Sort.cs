namespace ZatcaWebApp_V2.Helper
{
    public class Sort
    {
        public string Field { get; set; }
        public string Direction { get; set; }
        public string ToExpression()
        {
            return Field+ " "+Direction;
        }
    }
}
