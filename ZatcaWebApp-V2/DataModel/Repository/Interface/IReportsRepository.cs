using ZatcaWebApp_V2.ViewModel;

namespace ZatcaWebApp_V2.DataModel.Repository.Interface
{
    public interface IReportsRepository
    {
        Task<List<InvoicesVM>> GetInvoicesAsync(int PageNumber = 0, int PageSize = 5);
    }
}
