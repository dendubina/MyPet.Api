using Microsoft.EntityFrameworkCore;
using MyPet.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.DAL.EF
{
    public class AppDbContext : DbContext
    {
        public DbSet<Advertisement> Advertisements;
        public DbSet<Image> Images;
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Advertisement>()
                .HasMany(x => x.Images)
                .WithOne(x => x.Advertisement)
                .HasForeignKey(x => x.AdvertisementId);


            base.OnModelCreating(modelBuilder);
        }

    }
}

