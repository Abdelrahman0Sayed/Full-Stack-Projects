using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions): base(dbContextOptions){   
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Card>().HasKey(k => k.CardNumber);
            
            modelBuilder.Entity<Card>()
            .HasOne(x => x.AppUser)
            .WithMany(c => c.Cards)
            .HasForeignKey(fk => fk.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Products>()
                .HasOne(x => x.AppUser)
                .WithMany(c => c.ProductsList)
                .HasForeignKey(fk => fk.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Products>().HasKey(x => x.Id);
        }


        
        public DbSet<Card> Card { get; set; }
        public DbSet<Products> Products { get; set; }
    }
}