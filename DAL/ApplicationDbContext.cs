using System;
using System.Linq;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class ApplicationDbContext: DbContext
    {
        
        // replace <@UNI_ID_HERE@> with your UNI-ID.
        private static string ConnectionString =
            "Server=barrel.itcollege.ee;User Id=student;Password=Student.Pass.1;Database=student_ilvasi_battleshipdemo;MultipleActiveResultSets=true";
        
        
        public DbSet<GameSave> GameSaves { get; set; } = default!;
        public DbSet<GameConfig> GameConfigs { get; set; } = default!;
        public DbSet<GameShipConfig> GameShipConfig { get; set; } = default!;
        public DbSet<GameShip> GameShip { get; set; } = default!;

        // not recommended - do not hardcode DB conf!
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder){
            base.OnModelCreating(modelBuilder);

            foreach (var relationship in modelBuilder.Model
                .GetEntityTypes()
                .Where(e => !e.IsOwned())
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

    }
}
    