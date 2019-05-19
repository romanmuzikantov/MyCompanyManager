using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyCompanyManager.Models;

namespace MyCompanyManager.Data
{
    public class MyCompanyContext : IdentityDbContext
    {
        public MyCompanyContext(DbContextOptions<MyCompanyContext> options) : base(options)
        {
        }

        public DbSet<User> Members { get; set; }
        public DbSet<Conge> Conges { get; set; }
        public DbSet<CongeType> CongeTypes { get; set; }
        public DbSet<Equipe> Equipes { get; set; }
        public DbSet<Specialite> Specialites { get; set; }
        public DbSet<Statut> Statuts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Members");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<Conge>().ToTable("Conge");
            modelBuilder.Entity<CongeType>().ToTable("CongeType");
            modelBuilder.Entity<Equipe>().ToTable("Equipe");
            modelBuilder.Entity<Specialite>().ToTable("Specialite");
            modelBuilder.Entity<Statut>().ToTable("Statut");

            modelBuilder.Entity<Equipe>()
                .HasMany(e => e.Members)
                .WithOne(e => e.Equipe)
                .HasForeignKey(e => e.EquipeID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Specialite>()
                .HasMany(s => s.Collaborateurs)
                .WithOne(s => s.Specialite)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
