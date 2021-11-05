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
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<Image> Images { get; set; }

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
            modelBuilder.Entity<Pet>()
            .HasOne<Location>(u => u.Location)
            .WithOne(p => p.Pet)
            .HasForeignKey<Location>(p => p.PetId);

            modelBuilder.Entity<Advertisement>()
                .HasMany(x => x.Images)
                .WithOne(x => x.Advertisement)
                .HasForeignKey(x => x.AdvertisementId);
            modelBuilder.Entity<Advertisement>()
                .HasOne(p => p.Pet)
                .WithOne(p => p.Advertisement)
                .HasForeignKey<Pet>(p => p.AdId);




            base.OnModelCreating(modelBuilder);
        }

    }
}

