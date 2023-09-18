using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testWpfProcedure
{
  public  class DataTestContext  : DbContext
    {
        public DataTestContext() : base("DefaultConnection") { }


        public DbSet<User> Users { get; set; }


    }
}
