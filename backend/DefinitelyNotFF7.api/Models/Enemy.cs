namespace DefinitelyNotFF7.api.Models
{
    public class Enemy
    {
        public required string Name { get; set; }
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public int Attack {  get; set; }
        public int XpReward { get; set; }
    }
}
