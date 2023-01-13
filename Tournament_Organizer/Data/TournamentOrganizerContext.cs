
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Tournament_Organizer.Models;
namespace Tournament_Organizer.Data
{
    public class TournamentOrganizerContext : IdentityDbContext<User>
    {
        public TournamentOrganizerContext()
        {
        }

        public TournamentOrganizerContext(DbContextOptions<TournamentOrganizerContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);



            builder.Entity<Team_User>().HasKey(teamUser => new
            {
    
                teamUser.User_ID,
                teamUser.Team_ID,
            });

            builder.Entity<Team_User>().HasOne(teamUser => teamUser.Team).WithMany(tu => tu.Team_User).HasForeignKey(teamUser =>
            teamUser.Team_ID)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Team_User>().HasOne(teamUser => teamUser.User).WithMany(tu => tu.Team_User).HasForeignKey(teamUser =>
            teamUser.User_ID)
           .OnDelete(DeleteBehavior.Restrict);


            // add tournament and team relationship
            builder.Entity<Tournament_Team>().HasKey(x => new
            {
            
                x.Tournament_ID,
                x.Team_ID,
            });

            builder.Entity<Tournament_Team>().HasOne(x => x.Team).WithMany(u => u.Tournament_Team).HasForeignKey(x =>
            x.Team_ID)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Tournament_Team>().HasOne(x => x.Tournament).WithMany(u => u.Tournament_Team).HasForeignKey(x =>
            x.Tournament_ID)
           .OnDelete(DeleteBehavior.Restrict);


            // add tournament and user relationship
            builder.Entity<Tournament_User>().HasKey(x => new
            {
           
                x.Tournament_ID,
                x.User_ID,
            });

            builder.Entity<Tournament_User>().HasOne(x => x.Tournament).WithMany(u => u.Tournament_User).HasForeignKey(x =>
            x.Tournament_ID)
           .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Tournament_User>().HasOne(x => x.User).WithMany(u => u.Tournament_User).HasForeignKey(x =>
            x.User_ID)
           .OnDelete(DeleteBehavior.Restrict);




            // add tournament and user relationship
            builder.Entity<Tournament_Leaderboard>().HasKey(x => new
            {
                
                x.Tournament_ID,
                x.Team_One_ID,
                x.Team_Two_ID,
                x.Stage,

            });

            builder.Entity<Tournament_Leaderboard>().HasOne(x => x.Tournament).WithMany(u => u.Tournament_Leaderboard).HasForeignKey(x =>
            x.Tournament_ID)
           .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Tournament_Leaderboard>().HasOne(x => x.Team_One).WithMany(u => u.Tournament_Team_One).HasForeignKey(x =>
            x.Team_One_ID)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Tournament_Leaderboard>().HasOne(x => x.Team_Two).WithMany(u => u.Tournament_Team_Two).HasForeignKey(x =>
            x.Team_Two_ID)
            .OnDelete(DeleteBehavior.Restrict);






            // add tournament and user relationship for Reviews
            builder.Entity<Review_User_Tournament>().HasKey(x => new
            {
                x.Tournament_ID,
                x.User_ID,
            });

            builder.Entity<Review_User_Tournament>().HasOne(x => x.Tournament).WithMany(u => u.Review_User_Tournament).HasForeignKey(x =>
            x.Tournament_ID)
           .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review_User_Tournament>().HasOne(x => x.User).WithMany(u => u.Review_User_Tournament).HasForeignKey(x =>
            x.User_ID)
            .OnDelete(DeleteBehavior.Restrict);


            // add team and user relationship for Reviews
            builder.Entity<Review_User_Team>().HasKey(x => new
            {
                x.Team_ID,
                x.User_ID,
            });

            builder.Entity<Review_User_Team>().HasOne(x => x.Team).WithMany(u => u.Review_User_Team).HasForeignKey(x =>
            x.Team_ID)
           .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review_User_Team>().HasOne(x => x.User).WithMany(u => u.Review_User_Team).HasForeignKey(x =>
            x.User_ID)
            .OnDelete(DeleteBehavior.Restrict);




        }



        // Add Dbset to our Dbcontext
        public DbSet<Ms_Club> Ms_Club { get; set; }
        // Add Dbset to our Dbcontext
        public DbSet<Ms_International_Team> Ms_International_Team { get; set; }
        // Add Dbset to our Dbcontext
        public DbSet<Ms_Player_Position> Ms_Player_Position { get; set; }
        // Add Dbset to our Dbcontext
        // Add Dbset to our Dbcontext
        public DbSet<Team> Team { get; set; }
        // Add Dbset to our Dbcontext
        public DbSet<Tournament> Tournament { get; set; }
        // Add Dbset to our Dbcontext
        public DbSet<Team_User> Team_User { get; set; }
        // Add Dbset to our Dbcontext
        public DbSet<Tournament_Team> Tournament_Team { get; set; }
        // Add Dbset to our Dbcontext
        public DbSet<Tournament_User> Tournament_User { get; set; }
        // Add Dbset to our Dbcontext
        public DbSet<Tournament_Leaderboard> Tournament_Leaderboard { get; set; }
        // Add Dbset to our Dbcontext
        public DbSet<Review_User_Team> Review_User_Team { get; set; }
        // Add Dbset to our Dbcontext
        public DbSet<Review_User_Tournament> Review_User_Tournament { get; set; }
        // Add Dbset to our Dbcontext
        public DbSet<Division> Division { get; set; }


    }
}
