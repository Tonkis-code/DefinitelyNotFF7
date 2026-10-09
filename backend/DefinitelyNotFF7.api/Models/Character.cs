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


    }
}
