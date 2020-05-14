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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Status>(entity =>
            {
                entity.Property(e => e.Statusid)
                    .HasColumnName("statusid")
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
