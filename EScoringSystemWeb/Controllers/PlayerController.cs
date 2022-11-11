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
        List<Player> players= new List<Player>();

        [HttpGet(Name = "Player")]
        public IEnumerable<Player> Get()
        {
            var Hong = new Player();
            Hong.Name = "Hong";
            Hong.Score = 0;
            Hong.Id = 0;
            Hong.Penalties= 0;

            var Chong = new Player();
            Chong.Name = "Chong";
            Chong.Score = 0;
            Chong.Id = 0;
            Chong.Penalties = 0;

            players.Add(Hong);
            players.Add(Chong);

            return players.ToArray();
            
        }
    }
}
