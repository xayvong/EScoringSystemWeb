using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;
using WebApplication1;
using EScoringSystemWeb.Models;


namespace EScoringSystemWeb.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PlayerController : Controller
    {
    
        [HttpGet("GetPoints")]
        public void GetPoints()
        {
            
            var Json = "{\"Hong\" :\"" + Player.Hong.Score + "\",\"Chong\" :\"" + Player.Chong.Score + "\"}";
            Response.Clear();
            Response.WriteAsync(Json);
            

        }



        [HttpPost("AddPoints")]
        public void UpdatePoints(int id, int points)
        {
            if (id == 1)
            {            
                
                Player.Hong.Score += points;
            }
            else if (id == 2)
            {
                Player.Chong.Score += points;
            }
            
        }

        [HttpPost("ResetRound")]
        public int ResetRound ()
        {
            if (Player.Hong.Score > Player.Chong.Score) 
            {
                Player.Hong.MatchPoint += 1;
                    
            }
            else
            {
                Player.Chong.MatchPoint += 1;
            }
            if (Player.Hong.MatchPoint == 2)
            {
                Reset();
                return Player.Hong.Id;
            }
            else if (Player.Chong.MatchPoint == 2)
            {
                Reset();
                return Player.Chong.Id;
            }   
            else
            {
                return 0;
            }

        }
        [HttpPost("Reset")]
        public void Reset()
        {
            Player.Hong = new(1, "Hong", 0, 0);
            Player.Chong = new(2, "Chong", 0, 0);

        }



    }
}
