using BAMS.Databse.Models;
using Microsoft.EntityFrameworkCore;

namespace BAMS.Databse
{
    public class UserDbContext: DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }
    }
}
