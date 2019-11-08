using IdentityAPI.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAPI.Infrastructure
{
    public class IndentityDBContext : DbContext
    {
        public IndentityDBContext(DbContextOptions<IndentityDBContext> options) : base(options)
        {

        }

        public DbSet<Users> Users {get;set;}
    }
}
