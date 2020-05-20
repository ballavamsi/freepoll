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

        public virtual DbSet<Poll> Poll { get; set; }
        public virtual DbSet<PollOptions> PollOptions { get; set; }
        public virtual DbSet<Status> Status { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Poll>(entity =>
            {
                entity.HasComment("polling details");

                entity.HasIndex(e => e.StatusId)
                    .HasName("status_id");

                entity.Property(e => e.PollId)
                    .HasColumnName("poll_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Duplicate)
                    .HasColumnName("duplicate")
                    .HasColumnType("int(1) unsigned zerofill");

                entity.Property(e => e.Enddate)
                    .HasColumnName("enddate")
                    .HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.PollGuid)
                    .IsRequired()
                    .HasColumnName("poll_guid")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.StatusId)
                    .HasColumnName("status_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("int(1) unsigned zerofill");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<PollOptions>(entity =>
            {
                entity.HasKey(e => e.PollOptionId)
                    .HasName("PRIMARY");

                entity.ToTable("Poll_Options");

                entity.HasIndex(e => e.PollId)
                    .HasName("poll_id");

                entity.HasIndex(e => e.StatusId)
                    .HasName("status_id");

                entity.Property(e => e.PollOptionId)
                    .HasColumnName("poll_option_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasColumnName("created_date");

                entity.Property(e => e.OptionText)
                    .IsRequired()
                    .HasColumnName("option_text")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.OrderId)
                    .HasColumnName("order_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PollId)
                    .HasColumnName("poll_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.StatusId)
                    .HasColumnName("status_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.Property(e => e.Statusid)
                    .HasColumnName("statusid")
                    .HasColumnType("int(11)");

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
