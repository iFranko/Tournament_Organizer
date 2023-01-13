using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tournament_Organizer.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    First_Name = table.Column<string>(maxLength: 150, nullable: true),
                    Last_Name = table.Column<string>(maxLength: 150, nullable: true),
                    User_Role = table.Column<string>(maxLength: 150, nullable: false),
                    Age = table.Column<long>(nullable: false),
                    Image = table.Column<byte[]>(nullable: true),
                    Created_Date_Time = table.Column<DateTime>(nullable: true),
                    Ban = table.Column<bool>(nullable: true),
                    Fav_International_Team = table.Column<string>(maxLength: 150, nullable: true),
                    Fav_Club_Team = table.Column<string>(maxLength: 150, nullable: true),
                    Fav_Position = table.Column<string>(maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ms_Club",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Created_Date_Time = table.Column<DateTimeOffset>(nullable: false),
                    Club_Name = table.Column<string>(maxLength: 150, nullable: false),
                    Club_Logo = table.Column<byte[]>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ms_Club", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Ms_International_Team",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Created_Date_Time = table.Column<DateTimeOffset>(nullable: false),
                    International_Name = table.Column<string>(maxLength: 150, nullable: false),
                    International_Logo = table.Column<byte[]>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ms_International_Team", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Ms_Player_Position",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Created_Date_Time = table.Column<DateTimeOffset>(nullable: false),
                    Position_Name = table.Column<string>(maxLength: 150, nullable: false),
                    Position_Code = table.Column<string>(maxLength: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ms_Player_Position", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Tournament",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Created_Date_Time = table.Column<DateTimeOffset>(nullable: false),
                    Tournament_Name = table.Column<string>(maxLength: 250, nullable: false),
                    Start_Date = table.Column<DateTimeOffset>(nullable: false),
                    End_Date = table.Column<DateTimeOffset>(nullable: false),
                    Tournament_Type = table.Column<string>(maxLength: 150, nullable: false),
                    Image = table.Column<byte[]>(nullable: true),
                    Max_Teams_Tournament = table.Column<long>(nullable: false),
                    Max_Players_Per_Team = table.Column<long>(nullable: false),
                    Division_Id = table.Column<Guid>(nullable: false),
                    Review = table.Column<double>(nullable: false),
                    Ban = table.Column<bool>(nullable: true),
                    Country = table.Column<string>(maxLength: 150, nullable: false),
                    City = table.Column<string>(maxLength: 150, nullable: false),
                    Address = table.Column<string>(maxLength: 150, nullable: false),
                    Zip_Code = table.Column<string>(maxLength: 150, nullable: false),
                    Rule_One = table.Column<string>(nullable: false),
                    Rule_Two = table.Column<string>(nullable: false),
                    Rule_Three = table.Column<string>(nullable: false),
                    Rule_Four = table.Column<string>(nullable: false),
                    Stage = table.Column<long>(nullable: false),
                    NextStageMaxTeams = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournament", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Review_User_Tournament",
                columns: table => new
                {
                    Tournament_ID = table.Column<Guid>(nullable: false),
                    User_ID = table.Column<string>(nullable: false),
                    Review = table.Column<int>(nullable: false),
                    User_Role = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review_User_Tournament", x => new { x.Tournament_ID, x.User_ID });
                    table.ForeignKey(
                        name: "FK_Review_User_Tournament_Tournament_Tournament_ID",
                        column: x => x.Tournament_ID,
                        principalTable: "Tournament",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Review_User_Tournament_AspNetUsers_User_ID",
                        column: x => x.User_ID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Created_Date_Time = table.Column<DateTimeOffset>(nullable: false),
                    Team_Name = table.Column<string>(maxLength: 250, nullable: false),
                    JerseyColour = table.Column<string>(maxLength: 50, nullable: false),
                    Max_Player_Team = table.Column<long>(nullable: false),
                    Image = table.Column<byte[]>(nullable: true),
                    Ban = table.Column<bool>(nullable: true),
                    Country = table.Column<string>(maxLength: 150, nullable: false),
                    City = table.Column<string>(maxLength: 150, nullable: false),
                    Address = table.Column<string>(maxLength: 150, nullable: false),
                    Zip_Code = table.Column<string>(maxLength: 150, nullable: false),
                    Review = table.Column<double>(nullable: false),
                    Division_Id = table.Column<Guid>(nullable: false),
                    TournamentID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Team_Tournament_TournamentID",
                        column: x => x.TournamentID,
                        principalTable: "Tournament",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tournament_User",
                columns: table => new
                {
                    Tournament_ID = table.Column<Guid>(nullable: false),
                    User_ID = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournament_User", x => new { x.Tournament_ID, x.User_ID });
                    table.ForeignKey(
                        name: "FK_Tournament_User_Tournament_Tournament_ID",
                        column: x => x.Tournament_ID,
                        principalTable: "Tournament",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tournament_User_AspNetUsers_User_ID",
                        column: x => x.User_ID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Division",
                columns: table => new
                {
                    Division_Id = table.Column<Guid>(nullable: false),
                    DivisionName = table.Column<string>(maxLength: 50, nullable: true),
                    TeamID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Division", x => x.Division_Id);
                    table.ForeignKey(
                        name: "FK_Division_Team_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Team",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Review_User_Team",
                columns: table => new
                {
                    Team_ID = table.Column<Guid>(nullable: false),
                    User_ID = table.Column<string>(nullable: false),
                    Review = table.Column<int>(nullable: false),
                    User_Role = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review_User_Team", x => new { x.Team_ID, x.User_ID });
                    table.ForeignKey(
                        name: "FK_Review_User_Team_Team_Team_ID",
                        column: x => x.Team_ID,
                        principalTable: "Team",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Review_User_Team_AspNetUsers_User_ID",
                        column: x => x.User_ID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Team_User",
                columns: table => new
                {
                    Team_ID = table.Column<Guid>(nullable: false),
                    User_ID = table.Column<string>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    User_Role = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team_User", x => new { x.User_ID, x.Team_ID });
                    table.ForeignKey(
                        name: "FK_Team_User_Team_Team_ID",
                        column: x => x.Team_ID,
                        principalTable: "Team",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Team_User_AspNetUsers_User_ID",
                        column: x => x.User_ID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tournament_Leaderboard",
                columns: table => new
                {
                    Team_One_ID = table.Column<Guid>(nullable: false),
                    Team_Two_ID = table.Column<Guid>(nullable: false),
                    Tournament_ID = table.Column<Guid>(nullable: false),
                    Stage = table.Column<long>(nullable: false),
                    Team_One_Score = table.Column<int>(nullable: false),
                    Team_Two_Score = table.Column<int>(nullable: false),
                    Game_Date = table.Column<DateTimeOffset>(nullable: false),
                    GameNotes = table.Column<string>(maxLength: 1000, nullable: true),
                    Game_Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournament_Leaderboard", x => new { x.Tournament_ID, x.Team_One_ID, x.Team_Two_ID, x.Stage });
                    table.ForeignKey(
                        name: "FK_Tournament_Leaderboard_Team_Team_One_ID",
                        column: x => x.Team_One_ID,
                        principalTable: "Team",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tournament_Leaderboard_Team_Team_Two_ID",
                        column: x => x.Team_Two_ID,
                        principalTable: "Team",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tournament_Leaderboard_Tournament_Tournament_ID",
                        column: x => x.Tournament_ID,
                        principalTable: "Tournament",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tournament_Team",
                columns: table => new
                {
                    Team_ID = table.Column<Guid>(nullable: false),
                    Tournament_ID = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Active = table.Column<string>(nullable: true),
                    Group = table.Column<string>(nullable: true),
                    NumberOfMatches = table.Column<int>(nullable: true),
                    Points = table.Column<int>(nullable: true),
                    Scores = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournament_Team", x => new { x.Tournament_ID, x.Team_ID });
                    table.ForeignKey(
                        name: "FK_Tournament_Team_Team_Team_ID",
                        column: x => x.Team_ID,
                        principalTable: "Team",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tournament_Team_Tournament_Tournament_ID",
                        column: x => x.Tournament_ID,
                        principalTable: "Tournament",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Division_TeamID",
                table: "Division",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Review_User_Team_User_ID",
                table: "Review_User_Team",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Review_User_Tournament_User_ID",
                table: "Review_User_Tournament",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Team_TournamentID",
                table: "Team",
                column: "TournamentID");

            migrationBuilder.CreateIndex(
                name: "IX_Team_User_Team_ID",
                table: "Team_User",
                column: "Team_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Tournament_Leaderboard_Team_One_ID",
                table: "Tournament_Leaderboard",
                column: "Team_One_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Tournament_Leaderboard_Team_Two_ID",
                table: "Tournament_Leaderboard",
                column: "Team_Two_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Tournament_Team_Team_ID",
                table: "Tournament_Team",
                column: "Team_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Tournament_User_User_ID",
                table: "Tournament_User",
                column: "User_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Division");

            migrationBuilder.DropTable(
                name: "Ms_Club");

            migrationBuilder.DropTable(
                name: "Ms_International_Team");

            migrationBuilder.DropTable(
                name: "Ms_Player_Position");

            migrationBuilder.DropTable(
                name: "Review_User_Team");

            migrationBuilder.DropTable(
                name: "Review_User_Tournament");

            migrationBuilder.DropTable(
                name: "Team_User");

            migrationBuilder.DropTable(
                name: "Tournament_Leaderboard");

            migrationBuilder.DropTable(
                name: "Tournament_Team");

            migrationBuilder.DropTable(
                name: "Tournament_User");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Team");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Tournament");
        }
    }
}
