using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class PsychologicalCouselingContext : DbContext
    {
        public PsychologicalCouselingContext()
        {
        }

        public PsychologicalCouselingContext(DbContextOptions<PsychologicalCouselingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AggregatedCounter> AggregatedCounters { get; set; }
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<ArticleZodiac> ArticleZodiacs { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Consultant> Consultants { get; set; }
        public virtual DbSet<Counter> Counters { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<DailyHoroscope> DailyHoroscopes { get; set; }
        public virtual DbSet<Deposit> Deposits { get; set; }
        public virtual DbSet<DeviceToken> DeviceTokens { get; set; }
        public virtual DbSet<Hash> Hashes { get; set; }
        public virtual DbSet<House> Houses { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<JobParameter> JobParameters { get; set; }
        public virtual DbSet<JobQueue> JobQueues { get; set; }
        public virtual DbSet<List> Lists { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<OptionQuestion> OptionQuestions { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Planet> Planets { get; set; }
        public virtual DbSet<PlanetHouse> PlanetHouses { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductType> ProductTypes { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<QuestionSurvey> QuestionSurveys { get; set; }
        public virtual DbSet<ReceiveAccount> ReceiveAccounts { get; set; }
        public virtual DbSet<ResponseResult> ResponseResults { get; set; }
        public virtual DbSet<ResultSurvey> ResultSurveys { get; set; }
        public virtual DbSet<RoomVideoCall> RoomVideoCalls { get; set; }
        public virtual DbSet<Schema> Schemas { get; set; }
        public virtual DbSet<Server> Servers { get; set; }
        public virtual DbSet<Set> Sets { get; set; }
        public virtual DbSet<Shop> Shops { get; set; }
        public virtual DbSet<SlotBooking> SlotBookings { get; set; }
        public virtual DbSet<Specialization> Specializations { get; set; }
        public virtual DbSet<SpecializationType> SpecializationTypes { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<Survey> Surveys { get; set; }
        public virtual DbSet<SurveyType> SurveyTypes { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Wallet> Wallets { get; set; }
        public virtual DbSet<Withdrawal> Withdrawals { get; set; }
        public virtual DbSet<Zodiac> Zodiacs { get; set; }
        public virtual DbSet<ZodiacHouse> ZodiacHouses { get; set; }
        public virtual DbSet<ZodiacPlanet> ZodiacPlanets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=tcp:psycteam.database.windows.net,1433;Initial Catalog=PsychologicalCouseling;Persist Security Info=False;User ID=psycteam;Password=Admin@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<AggregatedCounter>(entity =>
            {
                entity.HasKey(e => e.Key)
                    .HasName("PK_HangFire_CounterAggregated");

                entity.ToTable("AggregatedCounter", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_AggregatedCounter_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Article>(entity =>
            {
                entity.ToTable("Article");

                entity.Property(e => e.CreateDay).HasColumnType("date");

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Title).HasMaxLength(250);

                entity.Property(e => e.UrlBanner).IsUnicode(false);
            });

            modelBuilder.Entity<ArticleZodiac>(entity =>
            {
                entity.ToTable("ArticleZodiac");

                entity.Property(e => e.CreateDay).HasColumnType("date");

                entity.Property(e => e.Title).HasMaxLength(250);

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.ArticleZodiacs)
                    .HasForeignKey(d => d.Articleid)
                    .HasConstraintName("FK_ArticleZodiac_Article");

                entity.HasOne(d => d.Zodiac)
                    .WithMany(p => p.ArticleZodiacs)
                    .HasForeignKey(d => d.Zodiacid)
                    .HasConstraintName("FK_ArticleZodiac_Zodiac");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Booking");

                entity.HasIndex(e => e.PaymentId, "IX_Booking")
                    .IsUnique();

                entity.Property(e => e.DateBooking).HasColumnType("datetime");

                entity.Property(e => e.Duration)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Feedback).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Booking_Customers");

                entity.HasOne(d => d.Payment)
                    .WithOne(p => p.Booking)
                    .HasForeignKey<Booking>(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Booking_Payment");
            });

            modelBuilder.Entity<Consultant>(entity =>
            {
                entity.ToTable("Consultant");

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.AvartarUrl).IsUnicode(false);

                entity.Property(e => e.Dob).HasColumnType("date");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FullName).HasMaxLength(50);

                entity.Property(e => e.Gender).HasMaxLength(10);

                entity.Property(e => e.ImageUrl).IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Counter>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Id })
                    .HasName("PK_HangFire_Counter");

                entity.ToTable("Counter", "HangFire");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.Birthchart).IsUnicode(false);

                entity.Property(e => e.Dob).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fullname).HasMaxLength(50);

                entity.Property(e => e.Gender).HasMaxLength(10);

                entity.Property(e => e.HourBirth)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ImageUrl).IsUnicode(false);

                entity.Property(e => e.MinuteBirth)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.SecondBirth)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DailyHoroscope>(entity =>
            {
                entity.ToTable("DailyHoroscope");

                entity.Property(e => e.Color).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.GoodTime)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ImageUrl).IsUnicode(false);

                entity.Property(e => e.ShouldNotThing).HasMaxLength(255);

                entity.Property(e => e.ShouldThing).HasMaxLength(255);

                entity.HasOne(d => d.Zodiac)
                    .WithMany(p => p.DailyHoroscopes)
                    .HasForeignKey(d => d.ZodiacId)
                    .HasConstraintName("FK_DailyHoroscope_Zodiac");
            });

            modelBuilder.Entity<Deposit>(entity =>
            {
                entity.ToTable("Deposit");

                entity.Property(e => e.Code)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.ReceiveAccount)
                    .WithMany(p => p.Deposits)
                    .HasForeignKey(d => d.ReceiveAccountid)
                    .HasConstraintName("FK_Deposit_ReceiveAccount");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.Deposits)
                    .HasForeignKey(d => d.WalletId)
                    .HasConstraintName("FK_Deposit_Wallet");
            });

            modelBuilder.Entity<DeviceToken>(entity =>
            {
                entity.ToTable("DeviceToken");

                entity.Property(e => e.Datechange).HasColumnType("datetime");
            });

            modelBuilder.Entity<Hash>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Field })
                    .HasName("PK_HangFire_Hash");

                entity.ToTable("Hash", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Hash_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Field).HasMaxLength(100);
            });

            modelBuilder.Entity<House>(entity =>
            {
                entity.ToTable("House");

                entity.Property(e => e.Element).HasMaxLength(25);

                entity.Property(e => e.ImageUrl).IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(20);

                entity.Property(e => e.Tag).HasMaxLength(50);
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("Job", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Job_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => e.StateName, "IX_HangFire_Job_StateName")
                    .HasFilter("([StateName] IS NOT NULL)");

                entity.Property(e => e.Arguments).IsRequired();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.InvocationData).IsRequired();

                entity.Property(e => e.StateName).HasMaxLength(20);
            });

            modelBuilder.Entity<JobParameter>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Name })
                    .HasName("PK_HangFire_JobParameter");

                entity.ToTable("JobParameter", "HangFire");

                entity.Property(e => e.Name).HasMaxLength(40);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobParameters)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_JobParameter_Job");
            });

            modelBuilder.Entity<JobQueue>(entity =>
            {
                entity.HasKey(e => new { e.Queue, e.Id })
                    .HasName("PK_HangFire_JobQueue");

                entity.ToTable("JobQueue", "HangFire");

                entity.Property(e => e.Queue).HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FetchedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<List>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Id })
                    .HasName("PK_HangFire_List");

                entity.ToTable("List", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_List_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.Type).HasMaxLength(50);

                entity.HasOne(d => d.Consultant)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.ConsultantId)
                    .HasConstraintName("FK_Notification_Consultant");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Notification_Customer");
            });

            modelBuilder.Entity<OptionQuestion>(entity =>
            {
                entity.ToTable("OptionQuestion");

                entity.Property(e => e.Type).HasMaxLength(255);

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.OptionQuestions)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("FK_OptionQuestion_Question");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.CreateDay).HasColumnType("date");

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Order_Customers");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetail");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK_OrderDetail_Item");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderDetail_Order");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Planet>(entity =>
            {
                entity.ToTable("Planet");

                entity.Property(e => e.Element).HasMaxLength(25);

                entity.Property(e => e.ImageUrl).IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Tag).HasMaxLength(50);
            });

            modelBuilder.Entity<PlanetHouse>(entity =>
            {
                entity.ToTable("PlanetHouse");

                entity.HasOne(d => d.House)
                    .WithMany(p => p.PlanetHouses)
                    .HasForeignKey(d => d.HouseId)
                    .HasConstraintName("FK_PlanetHouse_House");

                entity.HasOne(d => d.Planet)
                    .WithMany(p => p.PlanetHouses)
                    .HasForeignKey(d => d.PlanetId)
                    .HasConstraintName("FK_PlanetHouse_Planet");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.CreateDay).HasColumnType("date");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.ImageUrl).IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ProductType>(entity =>
            {
                entity.ToTable("ProductType");

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.ProductTypes)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK_ItemType_Item");

                entity.HasOne(d => d.Zodiac)
                    .WithMany(p => p.ProductTypes)
                    .HasForeignKey(d => d.ZodiacId)
                    .HasConstraintName("FK_ItemType_Zodiac");
            });

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.ToTable("Profile");

                entity.Property(e => e.BirthPlace).HasMaxLength(255);

                entity.Property(e => e.Dob).HasColumnType("datetime");

                entity.Property(e => e.Gender).HasMaxLength(50);

                entity.Property(e => e.ImageUrl).IsUnicode(false);

                entity.Property(e => e.Latitude)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Longitude)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Profiles)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Profile_Customers");

                entity.HasOne(d => d.House)
                    .WithMany(p => p.Profiles)
                    .HasForeignKey(d => d.HouseId)
                    .HasConstraintName("FK_Profile_House");

                entity.HasOne(d => d.Planet)
                    .WithMany(p => p.Profiles)
                    .HasForeignKey(d => d.PlanetId)
                    .HasConstraintName("FK_Profile_Planet");

                entity.HasOne(d => d.Zodiac)
                    .WithMany(p => p.Profiles)
                    .HasForeignKey(d => d.ZodiacId)
                    .HasConstraintName("FK_Profile_Zodiac");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.ToTable("Question");

                entity.HasOne(d => d.Survey)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.SurveyId)
                    .HasConstraintName("FK_Question_Survey");
            });

            modelBuilder.Entity<QuestionSurvey>(entity =>
            {
                entity.ToTable("QuestionSurvey");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.QuestionSurveys)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("FK_QuestionSurvey_Question");

                entity.HasOne(d => d.Survey)
                    .WithMany(p => p.QuestionSurveys)
                    .HasForeignKey(d => d.SurveyId)
                    .HasConstraintName("FK_QuestionSurvey_Survey");
            });

            modelBuilder.Entity<ReceiveAccount>(entity =>
            {
                entity.ToTable("ReceiveAccount");

                entity.Property(e => e.BankName).HasMaxLength(50);

                entity.Property(e => e.BankNumber)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DateCreate).HasColumnType("date");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.QrCode).IsUnicode(false);

                entity.Property(e => e.Status).HasMaxLength(50);
            });

            modelBuilder.Entity<ResponseResult>(entity =>
            {
                entity.ToTable("ResponseResult");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.ResponseResults)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_ResponseResult_Customer");

                entity.HasOne(d => d.Survey)
                    .WithMany(p => p.ResponseResults)
                    .HasForeignKey(d => d.SurveyId)
                    .HasConstraintName("FK_ResponseResult_Survey");
            });

            modelBuilder.Entity<ResultSurvey>(entity =>
            {
                entity.ToTable("ResultSurvey");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.ResultSurveys)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_ResultSurvey_Customer");

                entity.HasOne(d => d.OptionQuestion)
                    .WithMany(p => p.ResultSurveys)
                    .HasForeignKey(d => d.OptionQuestionId)
                    .HasConstraintName("FK_ResultSurvey_OptionQuestion");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.ResultSurveys)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("FK_ResultSurvey_Question");
            });

            modelBuilder.Entity<RoomVideoCall>(entity =>
            {
                entity.ToTable("RoomVideoCall");

                entity.HasIndex(e => e.SlotId, "IX_RoomVideoCall")
                    .IsUnique();

                entity.Property(e => e.ChanelName).HasMaxLength(100);

                entity.Property(e => e.Token).HasMaxLength(255);

                entity.HasOne(d => d.Slot)
                    .WithOne(p => p.RoomVideoCall)
                    .HasForeignKey<RoomVideoCall>(d => d.SlotId)
                    .HasConstraintName("FK_RoomVideoCall_SlotBooking");
            });

            modelBuilder.Entity<Schema>(entity =>
            {
                entity.HasKey(e => e.Version)
                    .HasName("PK_HangFire_Schema");

                entity.ToTable("Schema", "HangFire");

                entity.Property(e => e.Version).ValueGeneratedNever();
            });

            modelBuilder.Entity<Server>(entity =>
            {
                entity.ToTable("Server", "HangFire");

                entity.HasIndex(e => e.LastHeartbeat, "IX_HangFire_Server_LastHeartbeat");

                entity.Property(e => e.Id).HasMaxLength(200);

                entity.Property(e => e.LastHeartbeat).HasColumnType("datetime");
            });

            modelBuilder.Entity<Set>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Value })
                    .HasName("PK_HangFire_Set");

                entity.ToTable("Set", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Set_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => new { e.Key, e.Score }, "IX_HangFire_Set_Score");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Value).HasMaxLength(256);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Shop>(entity =>
            {
                entity.ToTable("Shop");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<SlotBooking>(entity =>
            {
                entity.HasKey(e => e.SlotId);

                entity.ToTable("SlotBooking");

                entity.Property(e => e.DateSlot).HasColumnType("date");

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.TimeEnd)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TimeStart)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.SlotBookings)
                    .HasForeignKey(d => d.BookingId)
                    .HasConstraintName("FK_SlotBooking_Booking");

                entity.HasOne(d => d.Consultant)
                    .WithMany(p => p.SlotBookings)
                    .HasForeignKey(d => d.ConsultantId)
                    .HasConstraintName("FK_SlotBooking_Consultant1");
            });

            modelBuilder.Entity<Specialization>(entity =>
            {
                entity.ToTable("Specialization");

                entity.HasOne(d => d.Consultant)
                    .WithMany(p => p.Specializations)
                    .HasForeignKey(d => d.ConsultantId)
                    .HasConstraintName("FK_Specialization_Consultant");

                entity.HasOne(d => d.SpecializationType)
                    .WithMany(p => p.Specializations)
                    .HasForeignKey(d => d.SpecializationTypeId)
                    .HasConstraintName("FK_Specialization_SpecializationType");
            });

            modelBuilder.Entity<SpecializationType>(entity =>
            {
                entity.ToTable("SpecializationType");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Id })
                    .HasName("PK_HangFire_State");

                entity.ToTable("State", "HangFire");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Reason).HasMaxLength(100);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.States)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_State_Job");
            });

            modelBuilder.Entity<Survey>(entity =>
            {
                entity.ToTable("Survey");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.SurveyType)
                    .WithMany(p => p.Surveys)
                    .HasForeignKey(d => d.SurveyTypeId)
                    .HasConstraintName("FK_Survey_SurveyType");
            });

            modelBuilder.Entity<SurveyType>(entity =>
            {
                entity.ToTable("SurveyType");

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("Transaction");

                entity.HasIndex(e => e.PaymentId, "IX_Transaction")
                    .IsUnique();

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.HasOne(d => d.Payment)
                    .WithOne(p => p.Transaction)
                    .HasForeignKey<Transaction>(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_Payment");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.WalletId)
                    .HasConstraintName("FK_Transaction_Wallet");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Code)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FcmToken)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Firebaseid)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsAdmin)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.PassWord)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.ToTable("Wallet");

                entity.Property(e => e.HistoryTrans).HasColumnType("datetime");

                entity.Property(e => e.IsAdmin).HasMaxLength(50);

                entity.Property(e => e.PassWord)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.HasOne(d => d.Consultant)
                    .WithMany(p => p.Wallets)
                    .HasForeignKey(d => d.ConsultantId)
                    .HasConstraintName("FK_Wallet_Consultant1");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Wallets)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Wallet_Customers1");
            });

            modelBuilder.Entity<Withdrawal>(entity =>
            {
                entity.ToTable("Withdrawal");

                entity.Property(e => e.BankName).HasMaxLength(255);

                entity.Property(e => e.BankNumber).HasMaxLength(50);

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.FullName).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.Withdrawals)
                    .HasForeignKey(d => d.WalletId)
                    .HasConstraintName("FK_Withdrawal_Wallet");
            });

            modelBuilder.Entity<Zodiac>(entity =>
            {
                entity.ToTable("Zodiac");

                entity.Property(e => e.DateEnd).HasColumnType("date");

                entity.Property(e => e.DateStart).HasColumnType("date");

                entity.Property(e => e.DescriptionShort).HasMaxLength(1500);

                entity.Property(e => e.ImageUrl).IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<ZodiacHouse>(entity =>
            {
                entity.ToTable("ZodiacHouse");

                entity.HasOne(d => d.House)
                    .WithMany(p => p.ZodiacHouses)
                    .HasForeignKey(d => d.HouseId)
                    .HasConstraintName("FK_ZodiacHouse_House");

                entity.HasOne(d => d.Zodiac)
                    .WithMany(p => p.ZodiacHouses)
                    .HasForeignKey(d => d.ZodiacId)
                    .HasConstraintName("FK_ZodiacHouse_Zodiac");
            });

            modelBuilder.Entity<ZodiacPlanet>(entity =>
            {
                entity.ToTable("ZodiacPlanet");

                entity.HasOne(d => d.Planet)
                    .WithMany(p => p.ZodiacPlanets)
                    .HasForeignKey(d => d.PlanetId)
                    .HasConstraintName("FK_ZodiacPlanet_Planet");

                entity.HasOne(d => d.Zodiac)
                    .WithMany(p => p.ZodiacPlanets)
                    .HasForeignKey(d => d.ZodiacId)
                    .HasConstraintName("FK_ZodiacPlanet_Zodiac");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
