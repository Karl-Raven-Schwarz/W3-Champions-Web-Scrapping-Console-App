namespace WebScrapping.Models
{
    public class Match
    {
        public string Id { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public bool Winner { get; set; }
        public string GameMode { get; set; } = string.Empty;
        public string Map { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }

        public Match(string id, string player1Name, string player2Name, bool winner, string gameMode, string map, DateTime date, TimeSpan duration)
        {
            Id = id;
            Player1Name = player1Name;
            Player2Name = player2Name;
            Winner = winner;
            GameMode = gameMode;
            Map = map;
            Date = date;
            Duration = duration;
        }
    }
}