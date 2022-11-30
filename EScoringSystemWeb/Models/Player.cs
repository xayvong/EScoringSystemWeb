namespace EScoringSystemWeb.Models
{
    public class Player
    {

        ////Contructor for Player
        //public Player(int id, string name, int penalties, int score)
        //{
        //    Id = id;
        //    Name = name;
        //    Penalties = penalties;
        //    Score = score;

        //}

        public int Id { get; set; }
        public string? Name { get; set; }
        public int Penalties { get; set; }
        public int Score { get; set; }
        public int MatchPoint { get; set; }


    }
}
