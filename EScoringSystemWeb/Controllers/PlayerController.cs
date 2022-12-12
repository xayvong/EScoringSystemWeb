using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;
using WebApplication1;
using EScoringSystemWeb.Models;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Data.SqlClient;
using Dapper;
using Microsoft.VisualBasic;

namespace EScoringSystemWeb.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase
    {

        private readonly IConfiguration _config;

        //public static Player Hong = new(1, "Hong", 0, 0);
        //public static Player Chong = new(2, "Chong", 0, 0);

        public static DateTime MatchStart = DateTime.MinValue;
        public static int MatchDuration = 0;
        public static DateTime PauseTime = DateTime.MinValue;
        public static bool IsPaused = false;

        public PlayerController(IConfiguration config)
        {
            _config = config;

        }


        [HttpGet("GetPoints")]
        public async Task<ActionResult> GetPoints()
        {

            //var Json = "{\"Hong\" :\"" + Hong.Score + "\",\"Chong\" :\"" + Chong.Score + "\"}";
            //Response.Clear();
            //Response.WriteAsync(Json);

            //List<Player> players = new List<Player>();
            //players.Add(Hong);
            //players.Add(Chong);
            //return players.ToList();

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var players = await connection.QueryAsync<Player>("select * from Player");
            //int test = await connection.QuerySingleOrDefaultAsync<short>("select Score from Player where id = 1");
            return Ok(players);

        }


        [HttpGet("UpdateScoreBoard")]
        public async Task<string> GetScoreBoard()
        {
            string Json = "";
            var json = new JObject();
            var hong = new JObject();
            var chong = new JObject();

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var players = await connection.QueryAsync<Player>("select * from Player");
            var plist = players.ToList();
            hong.Add(new JProperty("Score", plist[0].Score));
            hong.Add(new JProperty("Penalties", plist[0].Penalties));
            hong.Add(new JProperty("MatchPoint", plist[0].MatchPoint));

            chong.Add(new JProperty("Score", plist[1].Score));
            chong.Add(new JProperty("Penalties", plist[1].Penalties));
            chong.Add(new JProperty("MatchPoint", plist[1].MatchPoint));

            json.Add("hong", hong);
            json.Add("chong", chong);

            json.Add("TimerPaused", IsPaused);
            json.Add("PauseTime", PauseTime.ToString());
            json.Add("TimeStart", MatchStart.ToString());
            json.Add("TimeDuration", MatchDuration.ToString());
            return json.ToString();

        }

        [HttpGet("GetScore")]
        public async Task<int> GetScore(int id)
        {


            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            int Score = await connection.QuerySingleOrDefaultAsync<short>("select Score from Player where id = @Id" , new { Id = id });
            return Score;

        }


        [HttpPost("AddPoints")]
        
        public async void UpdatePoints(int id, int points)
        {

            if (id == 1)
            {
                //Hong.Score += points;

                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.ExecuteAsync("update Player set Score = Score + @Points where id = @Id ", new { Id = id, Points = points });
            }
            else if (id == 2)
            {
                //Chong.Score += points;
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.ExecuteAsync("update Player set Score = Score + @Points where id = @Id ", new { Id = id, Points = points });
            }



        }

        [HttpPost("StartMatch")]

        public void StartMatch(DateTime matchStart, int minutes)
        {

            MatchStart = matchStart;
            MatchDuration = minutes;

        }

        [HttpPost("PauseMatch")]

        public void PauseMatch(DateTime pauseTime, bool isPaused)
        {

            PauseTime = pauseTime;
            IsPaused = isPaused;

        }

        [HttpPost("RemovePoints")]

        public async void RemovePoints(int id, int points)
        {

            if (id == 1)
            {
                //Hong.Score --;

                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.ExecuteAsync("update Player set Score = Score - @Points where id = @Id ", new { Id = id, Points = points });
                
            }
            else if (id == 2)
            {
                //Chong.Score --;
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.ExecuteAsync("update Player set Score = Score - @Points where id = @Id ", new { Id = id, Points = points });
                
            }



        }
        [HttpPost("AddPenalties")]

        public async void AddPenalties(int id, int penalties)
        {

            if (id == 1)
            {
                //Hong.Penalties += penalties;
                //Chong.Score++;

                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.ExecuteAsync("update Player set Penalties = Penalties + @Points where id = @Id ", new { Id = id, Points = penalties });
                UpdatePoints(2, penalties);
            }
            else if (id == 2)
            {
                //Chong.Penalties += penalties;
                //Hong.Score++;
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.ExecuteAsync("update Player set Penalties = Penalties + @Points where id = @Id ", new { Id = id, Points = penalties });
                UpdatePoints(1, penalties);
            }



        }
        [HttpPost("RemovePenalties")]

        public async void RemovePenalties(int id, int penalties)
        {

            if (id == 1)
            {
                //Hong.Penalties -= penalties;
                //Chong.Score--;

                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.ExecuteAsync("update Player set Penalties = Penalties - @Points where id = @Id ", new { Id = id, Points = penalties });
                RemovePoints(2, penalties);
            }
            else if (id == 2)
            {
                //Chong.Penalties -= penalties;
                //Hong.Score--;
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.ExecuteAsync("update Player set Penalties = Penalties - @Points where id = @Id ", new { Id = id, Points = penalties });
                RemovePoints(1, penalties);
            }



        }

        [HttpPost("ResetRound")]
        public async Task<Player> ResetRound()
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            //await conn.ExecuteAsync("Select Score from Player where id = @Id ", new { Id = 1 });
            Player Hong = new();
            Player Chong = new();
            int HongScore = await conn.QuerySingleOrDefaultAsync<short>("select Score from Player where id = @Id", new { Id = 1 });
            int ChongScore = await conn.QuerySingleOrDefaultAsync<short>("select Score from Player where id = @Id", new { Id = 2 });

            if (HongScore > ChongScore) 
            {
                Reset();

                //Hong.MatchPoint += 1;
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.ExecuteAsync("update Player set MatchPoint = MatchPoint + 1 where id = @Id ", new { Id = 1 });

            }
            else 
            {
                Reset();

                //Chong.MatchPoint += 1;
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.ExecuteAsync("update Player set MatchPoint = MatchPoint + 1 where id = @Id ", new { Id = 2 });
            }
                //Matchpoint
                int HongMatchPoint = await conn.QuerySingleOrDefaultAsync<short>("select MatchPoint from Player where id = @Id", new { Id = 1 });
                int ChongMatchPoint = await conn.QuerySingleOrDefaultAsync<short>("select MatchPoint from Player where id = @Id", new { Id = 2 });

            if (HongMatchPoint == 2)
            {
                Reset();
                //Hong.MatchPoint = 0;
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.ExecuteAsync("update Player set MatchPoint = 0 where id = 1");
                var player = await connection.QueryFirstOrDefaultAsync<Player>("select * from Player where id = 1");

                return player;
                //using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                //var players = await connection.QueryFirstAsync<Player>("select * from Player where id = 1");
                //return players ;

            }
            else if (ChongMatchPoint == 2)
            {
                Reset();
                //Chong.MatchPoint = 0;
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.ExecuteAsync("update Player set MatchPoint = 0 where id = 2");
                var player = await connection.QueryFirstOrDefaultAsync<Player>("select * from Player where id = 2");

                return player;
                //using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                //var players = await connection.QueryFirstAsync<Player>("select * from Player where id = 2");
                //return players;
            }
            else
            {
                return null;
            }


        }
        [HttpPost("Reset")]
        public async void Reset()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            //Hong.Score = 0;
            //Hong.Penalties = 0;
            MatchStart = DateTime.MinValue;
            MatchDuration = 0;
            PauseTime = DateTime.MinValue;
            IsPaused = false;

        await connection.ExecuteAsync("update Player set Penalties = @Clear, Score = @Clear where id = @Id ", new { Id = 1, Clear = 0 });

            //Chong.Score = 0;
            //Chong.Penalties = 0;
            await connection.ExecuteAsync("update Player set Penalties = @Clear, Score = @Clear where id = @Id ", new { Id = 2, Clear = 0 });
            

        }

        [HttpPost("AddMatchPoint")]
        public async void AddMatchPoint(string player)
        {
            var id = (player.ToLower() == "chong" ? 2:1);

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            await connection.ExecuteAsync("update Player set MatchPoint = @Point where id = @Id ", new { Id = id, Point = 1 });
        }



    }
}
