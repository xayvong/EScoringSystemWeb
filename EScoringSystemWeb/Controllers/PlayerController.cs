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
            Hong = await conn.QuerySingleAsync<Player>("select * from Player where id = @Id", new { Id = 1 });
            Chong = await conn.QuerySingleAsync<Player>("select * from Player where id = @Id", new { Id = 2 });

            if (Hong.Score > Chong.Score) 
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
            if (Hong.MatchPoint == 2)
            {
                Reset();
                //Hong.MatchPoint = 0;
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var players = await connection.ExecuteAsync("update Player set MatchPoint = 0 where id = 1");

                return Hong;
                //using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                //var players = await connection.QueryFirstAsync<Player>("select * from Player where id = 1");
                //return players ;

            }
            else if (Chong.MatchPoint == 2)
            {
                Reset();
                //Chong.MatchPoint = 0;
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var players = await connection.ExecuteAsync("update Player set MatchPoint = 0 where id = 2");

                return Chong;
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


            await connection.ExecuteAsync("update Player set Penalties = @Clear, Score = @Clear where id = @Id ", new { Id = 1, Clear = 0 });

            //Chong.Score = 0;
            //Chong.Penalties = 0;
            await connection.ExecuteAsync("update Player set Penalties = @Clear, Score = @Clear where id = @Id ", new { Id = 2, Clear = 0 });
            

        }



    }
}
