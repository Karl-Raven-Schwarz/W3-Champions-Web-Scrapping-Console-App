namespace WebScrapping.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link
        {
            get
            {
                return $"{Name}%{Id}";
            }
        }

        public Player(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}