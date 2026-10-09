namespace DefinitelyNotFF7.api.Models
{
    public class Character
    {
        public required string Name { get; set; }
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public int Attack {  get; set; }
        public int CurrentAtb { get; set; }
        public int MaxAtb { get; set; }
        public int Level { get; set; }
        public int CurrentXp { get; set; }
        public int XpToNextLevel { get; set; }

    }
}
