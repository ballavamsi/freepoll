using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

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
        public virtual DbSet<SurveyFeedback> SurveyFeedback { get; set; }
        public virtual DbSet<SurveyFeedbackQuestionOptions> SurveyFeedbackQuestionOptions { get; set; }
        public virtual DbSet<SurveyQuestionOptions> SurveyQuestionOptions { get; set; }
        public virtual DbSet<SurveyQuestions> SurveyQuestions { get; set; }
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
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.OptionText)
                    .HasColumnName("option_text")
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<Poll>(entity =>
            {
                entity.HasIndex(e => e.PollGuid)
                    .HasName("poll_guid")
                    .IsUnique();

                entity.Property(e => e.PollId)
                    .HasColumnName("poll_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.Duplicate)
                    .HasColumnName("duplicate")
                    .HasColumnType("int(10) unsigned zerofill")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Enddate)
                    .HasColumnName("enddate")
                    .HasColumnType("date")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(1000);

                entity.Property(e => e.PollGuid)
                    .HasColumnName("poll_guid")
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.StatusId)
                    .HasColumnName("status_id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("int(1) unsigned zerofill")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasDefaultValueSql("'current_timestamp()'")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<PollOptions>(entity =>
            {
                entity.HasKey(e => e.PollOptionId)
                    .HasName("PRIMARY");

                entity.ToTable("Poll_Options");

                entity.HasIndex(e => new { e.PollId, e.PollOptionId })
                    .HasName("poll_id")
                    .IsUnique();

                entity.Property(e => e.PollOptionId)
                    .HasColumnName("poll_option_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.OptionText)
                    .HasColumnName("option_text")
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.OrderId)
                    .HasColumnName("order_id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.PollId)
                    .HasColumnName("poll_id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.StatusId)
                    .HasColumnName("status_id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasDefaultValueSql("'current_timestamp()'")
                    .ValueGeneratedOnAddOrUpdate();
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
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.IpAddress)
                    .HasColumnName("ip_address")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.OptionId)
                    .HasColumnName("option_id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.PollId)
                    .HasColumnName("poll_id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.UserLocation)
                    .HasColumnName("user_location")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<QuestionType>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PRIMARY");

                entity.ToTable("Question_Type");

                entity.Property(e => e.TypeId)
                    .HasColumnName("type_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DisplayOrder)
                    .HasColumnName("display_order")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.IsActive)
                    .HasColumnName("is_active")
                    .HasColumnType("int(1) unsigned zerofill")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.TypeCode)
                    .HasColumnName("type_code")
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.TypeValue)
                    .HasColumnName("type_value")
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Statusid)
                    .HasColumnName("statusid")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Statusname)
                    .HasColumnName("statusname")
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<Survey>(entity =>
            {
                entity.HasIndex(e => e.SurveyGuid)
                    .HasName("survey_guid")
                    .IsUnique();

                entity.Property(e => e.Surveyid)
                    .HasColumnName("surveyid")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Allowduplicate)
                    .HasColumnName("allowduplicate")
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Askemail)
                    .HasColumnName("askemail")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.Emailidrequired)
                    .HasColumnName("emailidrequired")
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Enableprevious)
                    .HasColumnName("enableprevious")
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Enddate)
                    .HasColumnName("enddate")
                    .HasColumnType("date")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Endtitle)
                    .HasColumnName("endtitle")
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.StatusId)
                    .HasColumnName("status_id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.SurveyGuid)
                    .HasColumnName("survey_guid")
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasDefaultValueSql("'NULL'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Welcomedescription)
                    .HasColumnName("welcomedescription")
                    .HasMaxLength(1000)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Welcomeimage)
                    .HasColumnName("welcomeimage")
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Welcometitle)
                    .HasColumnName("welcometitle")
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<SurveyFeedback>(entity =>
            {
                entity.ToTable("Survey_Feedback");

                entity.Property(e => e.SurveyFeedbackId)
                    .HasColumnName("survey_feedback_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CompletedDatetime)
                    .HasColumnName("completed_datetime")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.InsertedDatetime)
                    .HasColumnName("inserted_datetime")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.ReviewComment)
                    .HasColumnName("review_comment")
                    .HasMaxLength(1000)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.ReviewCompleted)
                    .HasColumnName("review_completed")
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ReviewDatetime)
                    .HasColumnName("review_datetime")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.SurveyId)
                    .HasColumnName("survey_id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.SurveyUserEmail)
                    .HasColumnName("survey_user_email")
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.SurveyUserGuid)
                    .HasColumnName("survey_user_guid")
                    .HasMaxLength(1000)
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<SurveyFeedbackQuestionOptions>(entity =>
            {
                entity.ToTable("Survey_Feedback_Question_Options");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CustomAnswer)
                    .HasColumnName("custom_answer")
                    .HasColumnType("varchar(10000)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.InsertedDatetime)
                    .HasColumnName("inserted_datetime")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.SurveyFeedbackId)
                    .HasColumnName("survey_feedback_id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.SurveyQuestionId)
                    .HasColumnName("survey_question_id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.SurveyQuestionOptionId)
                    .HasColumnName("survey_question_option_id")
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<SurveyQuestionOptions>(entity =>
            {
                entity.HasKey(e => e.SurveyQuestionOptionId)
                    .HasName("PRIMARY");

                entity.ToTable("Survey_Question_Options");

                entity.Property(e => e.SurveyQuestionOptionId)
                    .HasColumnName("survey_question_option_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.DisplayOrder)
                    .HasColumnName("display_order")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.OptionKey)
                    .HasColumnName("option_key")
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.OptionValue)
                    .HasColumnName("option_value")
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.SurveyQuestionId)
                    .HasColumnName("survey_question_id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasDefaultValueSql("'NULL'")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<SurveyQuestions>(entity =>
            {
                entity.HasKey(e => e.SurveyQuestionId)
                    .HasName("PRIMARY");

                entity.ToTable("Survey_Questions");

                entity.Property(e => e.SurveyQuestionId)
                    .HasColumnName("survey_question_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.Isrequired)
                    .HasColumnName("isrequired")
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.QuestionDisplayOrder)
                    .HasColumnName("question_display_order")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.StatusId)
                    .HasColumnName("status_id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Subtitle)
                    .HasColumnName("subtitle")
                    .HasMaxLength(1000)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.SurveyId)
                    .HasColumnName("survey_id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(1000)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.TypeId)
                    .HasColumnName("type_id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasDefaultValueSql("'NULL'")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Userid)
                    .HasColumnName("userid")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("created_time")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(1000)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Facebook)
                    .HasColumnName("facebook")
                    .HasMaxLength(1000)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Github)
                    .HasColumnName("github")
                    .HasMaxLength(1000)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Google)
                    .HasColumnName("google")
                    .HasMaxLength(1000)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(1000)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(1000)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.PhotoUrl)
                    .HasColumnName("photo_url")
                    .HasMaxLength(1000)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.UserGuid)
                    .HasColumnName("user_guid")
                    .HasMaxLength(1000)
                    .HasDefaultValueSql("'NULL'");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        private partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}