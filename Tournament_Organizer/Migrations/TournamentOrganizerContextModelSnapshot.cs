// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tournament_Organizer.Data;

namespace Tournament_Organizer.Migrations
{
    [DbContext(typeof(TournamentOrganizerContext))]
    partial class TournamentOrganizerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.30")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Tournament_Organizer.Models.Division", b =>
                {
                    b.Property<Guid>("Division_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DivisionName")
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<Guid?>("TeamID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Division_Id");

                    b.HasIndex("TeamID");

                    b.ToTable("Division");
                });

            modelBuilder.Entity("Tournament_Organizer.Models.Ms_Club", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte[]>("Club_Logo")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Club_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.Property<DateTimeOffset>("Created_Date_Time")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("ID");

                    b.ToTable("Ms_Club");
                });

            modelBuilder.Entity("Tournament_Organizer.Models.Ms_International_Team", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("Created_Date_Time")
                        .HasColumnType("datetimeoffset");

                    b.Property<byte[]>("International_Logo")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("International_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.HasKey("ID");

                    b.ToTable("Ms_International_Team");
                });

            modelBuilder.Entity("Tournament_Organizer.Models.Ms_Player_Position", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("Created_Date_Time")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Position_Code")
                        .HasColumnType("nvarchar(2)")
                        .HasMaxLength(2);

                    b.Property<string>("Position_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.HasKey("ID");

                    b.ToTable("Ms_Player_Position");
                });

            modelBuilder.Entity("Tournament_Organizer.Models.Review_User_Team", b =>
                {
                    b.Property<Guid>("Team_ID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("User_ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Review")
                        .HasColumnType("int");

                    b.Property<string>("User_Role")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Team_ID", "User_ID");

                    b.HasIndex("User_ID");

                    b.ToTable("Review_User_Team");
                });

            modelBuilder.Entity("Tournament_Organizer.Models.Review_User_Tournament", b =>
                {
                    b.Property<Guid>("Tournament_ID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("User_ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Review")
                        .HasColumnType("int");

                    b.Property<string>("User_Role")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Tournament_ID", "User_ID");

                    b.HasIndex("User_ID");

                    b.ToTable("Review_User_Tournament");
                });

            modelBuilder.Entity("Tournament_Organizer.Models.Team", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.Property<bool?>("Ban")
                        .HasColumnType("bit");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.Property<DateTimeOffset?>("Created_Date_Time")
                        .IsRequired()
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid>("Division_Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte[]>("Image")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("JerseyColour")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<long>("Max_Player_Team")
                        .HasColumnType("bigint");

                    b.Property<double>("Review")
                        .HasColumnType("float");

                    b.Property<string>("Team_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<Guid?>("TournamentID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Zip_Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.HasKey("ID");

                    b.HasIndex("TournamentID");

                    b.ToTable("Team");
                });

            modelBuilder.Entity("Tournament_Organizer.Models.Team_User", b =>
                {
                    b.Property<string>("User_ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("Team_ID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("User_Role")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("User_ID", "Team_ID");

                    b.HasIndex("Team_ID");

                    b.ToTable("Team_User");
                });

            modelBuilder.Entity("Tournament_Organizer.Models.Tournament", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.Property<bool?>("Ban")
                        .HasColumnType("bit");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.Property<DateTimeOffset?>("Created_Date_Time")
                        .IsRequired()
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid>("Division_Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("End_Date")
                        .HasColumnType("datetimeoffset");

                    b.Property<byte[]>("Image")
                        .HasColumnType("varbinary(max)");

                    b.Property<long>("Max_Players_Per_Team")
                        .HasColumnType("bigint");

                    b.Property<long>("Max_Teams_Tournament")
                        .HasColumnType("bigint");

                    b.Property<long>("NextStageMaxTeams")
                        .HasColumnType("bigint");

                    b.Property<double>("Review")
                        .HasColumnType("float");

                    b.Property<string>("Rule_Four")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Rule_One")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Rule_Three")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Rule_Two")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("Stage")
                        .HasColumnType("bigint");

                    b.Property<DateTimeOffset>("Start_Date")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Tournament_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("Tournament_Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.Property<string>("Zip_Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.HasKey("ID");

                    b.ToTable("Tournament");
                });

            modelBuilder.Entity("Tournament_Organizer.Models.Tournament_Leaderboard", b =>
                {
                    b.Property<Guid>("Tournament_ID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("Team_One_ID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("Team_Two_ID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("Stage")
                        .HasColumnType("bigint");

                    b.Property<string>("GameNotes")
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.Property<DateTimeOffset>("Game_Date")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Game_Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Team_One_Score")
                        .HasColumnType("int");

                    b.Property<int>("Team_Two_Score")
                        .HasColumnType("int");

                    b.HasKey("Tournament_ID", "Team_One_ID", "Team_Two_ID", "Stage");

                    b.HasIndex("Team_One_ID");

                    b.HasIndex("Team_Two_ID");

                    b.ToTable("Tournament_Leaderboard");
                });

            modelBuilder.Entity("Tournament_Organizer.Models.Tournament_Team", b =>
                {
                    b.Property<Guid>("Tournament_ID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("Team_ID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Active")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Group")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("NumberOfMatches")
                        .HasColumnType("int");

                    b.Property<int?>("Points")
                        .HasColumnType("int");

                    b.Property<int?>("Scores")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Tournament_ID", "Team_ID");

                    b.HasIndex("Team_ID");

                    b.ToTable("Tournament_Team");
                });

            modelBuilder.Entity("Tournament_Organizer.Models.Tournament_User", b =>
                {
                    b.Property<Guid>("Tournament_ID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("User_ID")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Tournament_ID", "User_ID");

                    b.HasIndex("User_ID");

                    b.ToTable("Tournament_User");
                });

            modelBuilder.Entity("Tournament_Organizer.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<long>("Age")
                        .HasColumnType("bigint");

                    b.Property<bool?>("Ban")
                        .HasColumnType("bit");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Created_Date_Time")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("Fav_Club_Team")
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.Property<string>("Fav_International_Team")
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.Property<string>("Fav_Position")
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.Property<string>("First_Name")
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.Property<byte[]>("Image")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Last_Name")
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("User_Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Tournament_Organizer.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Tournament_Organizer.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tournament_Organizer.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Tournament_Organizer.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Tournament_Organizer.Models.Division", b =>
                {
                    b.HasOne("Tournament_Organizer.Models.Team", null)
                        .WithMany("Division")
                        .HasForeignKey("TeamID");
                });

            modelBuilder.Entity("Tournament_Organizer.Models.Review_User_Team", b =>
                {
                    b.HasOne("Tournament_Organizer.Models.Team", "Team")
                        .WithMany("Review_User_Team")
                        .HasForeignKey("Team_ID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Tournament_Organizer.Models.User", "User")
                        .WithMany("Review_User_Team")
                        .HasForeignKey("User_ID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Tournament_Organizer.Models.Review_User_Tournament", b =>
                {
                    b.HasOne("Tournament_Organizer.Models.Tournament", "Tournament")
                        .WithMany("Review_User_Tournament")
                        .HasForeignKey("Tournament_ID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Tournament_Organizer.Models.User", "User")
                        .WithMany("Review_User_Tournament")
                        .HasForeignKey("User_ID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Tournament_Organizer.Models.Team", b =>
                {
                    b.HasOne("Tournament_Organizer.Models.Tournament", null)
                        .WithMany("Teams")
                        .HasForeignKey("TournamentID");
                });

            modelBuilder.Entity("Tournament_Organizer.Models.Team_User", b =>
                {
                    b.HasOne("Tournament_Organizer.Models.Team", "Team")
                        .WithMany("Team_User")
                        .HasForeignKey("Team_ID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Tournament_Organizer.Models.User", "User")
                        .WithMany("Team_User")
                        .HasForeignKey("User_ID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Tournament_Organizer.Models.Tournament_Leaderboard", b =>
                {
                    b.HasOne("Tournament_Organizer.Models.Team", "Team_One")
                        .WithMany("Tournament_Team_One")
                        .HasForeignKey("Team_One_ID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Tournament_Organizer.Models.Team", "Team_Two")
                        .WithMany("Tournament_Team_Two")
                        .HasForeignKey("Team_Two_ID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Tournament_Organizer.Models.Tournament", "Tournament")
                        .WithMany("Tournament_Leaderboard")
                        .HasForeignKey("Tournament_ID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Tournament_Organizer.Models.Tournament_Team", b =>
                {
                    b.HasOne("Tournament_Organizer.Models.Team", "Team")
                        .WithMany("Tournament_Team")
                        .HasForeignKey("Team_ID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Tournament_Organizer.Models.Tournament", "Tournament")
                        .WithMany("Tournament_Team")
                        .HasForeignKey("Tournament_ID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Tournament_Organizer.Models.Tournament_User", b =>
                {
                    b.HasOne("Tournament_Organizer.Models.Tournament", "Tournament")
                        .WithMany("Tournament_User")
                        .HasForeignKey("Tournament_ID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Tournament_Organizer.Models.User", "User")
                        .WithMany("Tournament_User")
                        .HasForeignKey("User_ID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
