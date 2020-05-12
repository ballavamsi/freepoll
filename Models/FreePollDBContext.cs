using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace freepoll.Models
{
    public partial class FreePollDBContext : DbContext
    {
        public FreePollDBContext()
        {
        }

        public FreePollDBContext(DbContextOptions<FreePollDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Status> Status { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySQL("server=remotemysql.com;port=3306;user=3wAtUqE7dU;password=esHlHotlf9;database=3wAtUqE7dU");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Status>(entity =>
            {
                entity.HasKey(e => e.Statusisd)
                    .HasName("PRIMARY");

                entity.Property(e => e.Statusisd)
                    .HasColumnName("statusisd")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Statusname)
                    .IsRequired()
                    .HasColumnName("statusname")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
