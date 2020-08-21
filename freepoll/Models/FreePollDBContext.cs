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

        public virtual DbSet<DataStarOptions> DataStarOptions { get; set; }
        public virtual DbSet<Poll> Poll { get; set; }
        public virtual DbSet<PollOptions> PollOptions { get; set; }
        public virtual DbSet<PollVotes> PollVotes { get; set; }
        public virtual DbSet<QuestionType> QuestionType { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<Survey> Survey { get; set; }
        public virtual DbSet<SurveyQuestionOptions> SurveyQuestionOptions { get; set; }
        public virtual DbSet<SurveyQuestions> SurveyQuestions { get; set; }
        public virtual DbSet<SurveyUser> SurveyUser { get; set; }
        public virtual DbSet<SurveyUserQuestionOptions> SurveyUserQuestionOptions { get; set; }
        public virtual DbSet<User> User { get; set; }

      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataStarOptions>(entity =>
            {
                entity.HasKey(e => e.StarOptionId)
                    .HasName("PRIMARY");

                entity.ToTable("Data_StarOptions");

                entity.Property(e => e.StarOptionId)
                    .HasColumnName("star_option_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DisplayOrder)
                    .HasColumnName("display_order")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.IsActive)
                    .HasColumnName("is_active")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.OptionDisplayText)
                    .HasColumnName("option_display_text")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OptionText)
                    .HasColumnName("option_text")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

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
                    .HasMaxLength(100)
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

            modelBuilder.Entity<PollVotes>(entity =>
            {
                entity.HasKey(e => e.PollVoteId)
                    .HasName("PRIMARY");

                entity.ToTable("Poll_Votes");

                entity.Property(e => e.PollVoteId)
                    .HasColumnName("poll_vote_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IpAddress)
                    .HasColumnName("ip_address")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OptionId)
                    .HasColumnName("option_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PollId)
                    .HasColumnName("poll_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserLocation)
                    .HasColumnName("user_location")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<QuestionType>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PRIMARY");

                entity.ToTable("Question_Type");

                entity.Property(e => e.TypeId)
                    .HasColumnName("type_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.DisplayOrder)
                    .HasColumnName("display_order")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.IsActive)
                    .HasColumnName("is_active")
                    .HasColumnType("int(1) unsigned zerofill")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.TypeCode)
                    .HasColumnName("type_code")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TypeValue)
                    .HasColumnName("type_value")
                    .HasMaxLength(100)
                    .IsUnicode(false);
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

            modelBuilder.Entity<Survey>(entity =>
            {
                entity.Property(e => e.Surveyid)
                    .HasColumnName("surveyid")
                    .HasColumnType("int(10)");

                entity.Property(e => e.Allowduplicate)
                    .HasColumnName("allowduplicate")
                    .HasColumnType("int(1) unsigned zerofill");

                entity.Property(e => e.Askemail)
                    .HasColumnName("askemail")
                    .HasColumnType("int(1) unsigned zerofill");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasColumnName("created_date");

                entity.Property(e => e.Emailidrequired)
                    .HasColumnName("emailidrequired")
                    .HasColumnType("int(1) unsigned zerofill");

                entity.Property(e => e.Enableprevious)
                    .HasColumnName("enableprevious")
                    .HasColumnType("int(1) unsigned zerofill");

                entity.Property(e => e.Enddate)
                    .HasColumnName("enddate")
                    .HasColumnType("date");

                entity.Property(e => e.Endtitle)
                    .IsRequired()
                    .HasColumnName("endtitle")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.StatusId)
                    .HasColumnName("status_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SurveyGuid)
                    .IsRequired()
                    .HasColumnName("survey_guid")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");

                entity.Property(e => e.Welcomedescription)
                    .HasColumnName("welcomedescription")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Welcomeimage)
                    .HasColumnName("welcomeimage")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Welcometitle)
                    .IsRequired()
                    .HasColumnName("welcometitle")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SurveyQuestionOptions>(entity =>
            {
                entity.HasKey(e => e.SurveyQuestionOptionId)
                    .HasName("PRIMARY");

                entity.ToTable("Survey_Question_Options");

                entity.Property(e => e.SurveyQuestionOptionId)
                    .HasColumnName("survey_question_option_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasColumnName("created_date");

                entity.Property(e => e.DisplayOrder)
                    .HasColumnName("display_order")
                    .HasColumnType("int(11)");

                entity.Property(e => e.OptionKey)
                    .HasColumnName("option_key")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OptionValue)
                    .HasColumnName("option_value")
                    .HasMaxLength(8000)
                    .IsUnicode(false);

                entity.Property(e => e.SurveyQuestionId)
                    .HasColumnName("survey_question_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
            });

            modelBuilder.Entity<SurveyQuestions>(entity =>
            {
                entity.HasKey(e => e.SurveyQuestionId)
                    .HasName("PRIMARY");

                entity.ToTable("Survey_Questions");

                entity.Property(e => e.SurveyQuestionId)
                    .HasColumnName("survey_question_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasColumnName("created_date");

                entity.Property(e => e.Isrequired)
                    .HasColumnName("isrequired")
                    .HasColumnType("int(1) unsigned zerofill");

                entity.Property(e => e.QuestionDisplayOrder)
                    .HasColumnName("question_display_order")
                    .HasColumnType("int(10)");

                entity.Property(e => e.StatusId)
                    .HasColumnName("status_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Subtitle)
                    .HasColumnName("subtitle")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SurveyId)
                    .HasColumnName("survey_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TypeId)
                    .HasColumnName("type_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
            });

            modelBuilder.Entity<SurveyUser>(entity =>
            {
                entity.ToTable("Survey_User");

                entity.Property(e => e.SurveyUserId)
                    .HasColumnName("survey_user_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.CompletedDatetime).HasColumnName("completed_datetime");

                entity.Property(e => e.InsertedDatetime)
                    .HasColumnName("inserted_datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.SurveyId)
                    .HasColumnName("survey_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SurveyUserEmail)
                    .IsRequired()
                    .HasColumnName("survey_user_email")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SurveyUserGuid)
                    .HasColumnName("survey_user_guid")
                    .HasMaxLength(1000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SurveyUserQuestionOptions>(entity =>
            {
                entity.ToTable("Survey_User_Question_Options");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.CustomAnswer)
                    .HasColumnName("custom_answer")
                    .HasMaxLength(10000)
                    .IsUnicode(false);

                entity.Property(e => e.InsertedDatetime)
                    .HasColumnName("inserted_datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.SurveyQuestionId)
                    .HasColumnName("survey_question_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.SurveyQuestionOptionId)
                    .HasColumnName("survey_question_option_id")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SurveyUserId)
                    .HasColumnName("survey_user_id")
                    .HasColumnType("int(10)");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Userid)
                    .HasColumnName("userid")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("created_time")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Facebook)
                    .HasColumnName("facebook")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Github)
                    .HasColumnName("github")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Google)
                    .HasColumnName("google")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PhotoUrl)
                    .HasColumnName("photo_url")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.UserGuid)
                    .IsRequired()
                    .HasColumnName("user_guid")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
