namespace EScoringSystemWeb.Models
{
    public class Player
    {
        public static Player Hong = new(1, "Hong", 0, 0);

        public static Player Chong = new(2, "Chong", 0, 0);
        //Contructor for Player
        public Player(int id, string name, int penalties, int score)
        {
            Id = id;
            Name = name;
            Penalties = penalties;
            Score = score;

        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public int Penalties { get; set; }
        public int Score { get; set; }
        public int MatchPoint { get; set; }


    }
}
