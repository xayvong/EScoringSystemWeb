using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;
using WebApplication1;
using EScoringSystemWeb.Models;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Data.SqlClient;
using Dapper;

namespace EScoringSystemWeb.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase
    {

        private readonly IConfiguration _config;
        public Player Hong;
        public Player Chong;


        public PlayerController(IConfiguration config) 
        { 
            _config = config; 
        
        }

        [HttpGet("GetPoints")]
        public async Task<ActionResult<List<Player>>> GetPoints()
        {

            //var Json = "{\"Hong\" :\"" + StaticPlayer.Hong.Score + "\",\"Chong\" :\"" + StaticPlayer.Chong.Score + "\"}";
            //Response.Clear();
            //Response.WriteAsync(Json);

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var players = await connection.QueryAsync<Player>("select * from Player");
            return Ok(players); 
                
        }



        [HttpPost("AddPoints")]
        
        public void UpdatePoints(int id, int points)
        {

            if (id == 1)
            {

                Hong.Score += points;
            }
            else if (id == 2)
            {
                Chong.Score += points;
            }



        }

        [HttpPost("ResetRound")]
        public int ResetRound ()
        {
            if (Hong.Score > Chong.Score) 
            {
                Hong.MatchPoint += 1;
                    
            }
            else
            {
                Chong.MatchPoint += 1;
            }
            if (Hong.MatchPoint == 2)
            {
                Reset();
                return Hong.Id;
            }
            else if (Chong.MatchPoint == 2)
            {
                Reset();
                return Chong.Id;
            }   
            else
            {
                return 0;
            }

        }
        [HttpPost("Reset")]
        public void Reset()
        {
            Hong.Score = 0;
            Hong.Penalties = 0;

            Chong.Score = 0;
            Chong.Penalties = 0;

        }



    }
}
