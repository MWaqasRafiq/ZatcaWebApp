using Microsoft.EntityFrameworkCore;

namespace ZatcaWebApp_V2.Common.Interfaces
{
    public interface IApplicationDBContext
    {
        //DbSet<AppSetting> AppSettings { get; set; }
        Task<int> SaveChangesAsync();
    }
}
