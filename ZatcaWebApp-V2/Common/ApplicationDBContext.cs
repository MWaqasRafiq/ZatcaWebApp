using Microsoft.EntityFrameworkCore;
using ZatcaWebApp_V2.Common.Interfaces;


namespace ZatcaWebApp_V2.Common
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

    }
}
