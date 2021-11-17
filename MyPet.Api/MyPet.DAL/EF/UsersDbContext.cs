using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.DAL.EF
{
    public class UsersDbContext : IdentityDbContext
    {
        
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base (options)
        {
            Database.EnsureCreated();
        }        
    }
}
