using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SIMS;

namespace SIMS.Data
{
    public class SIMSContext : DbContext
    {
        public SIMSContext (DbContextOptions<SIMSContext> options)
            : base(options)
        {
        }

        public DbSet<SIMS.Users> Users { get; set; } = default!;
    }
}
