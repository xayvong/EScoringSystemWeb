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
        List<Player> players = new();

        Player Hong = new(1,"Hong",0,0);       

        Player Chong = new(2, "Chong", 0, 0);

        [HttpGet(Name = "Player")]
        public IEnumerable<Player> Get()
        {

            players.Add(Hong);
            players.Add(Chong);

            return players.ToArray();

        }

        [HttpGet("HongPunch")]
        public Player HongPunchPoint()
        {
            Hong.Score += 1;

            return Hong;
        }

        [HttpGet("ChongPunch")]
        public Player ChongPunchPoint()
        {
            Chong.Score += 1;

            return Chong;
        }

        [HttpGet("HongBodyKick")]
        public Player HongKickPoint()
        {
            Hong.Score += 2;

            return Hong;
        }

        [HttpGet("ChongBodyKick")]
        public Player ChongKickPoint()
        {
            Chong.Score += 2;

            return Chong;
        }


    }
}
