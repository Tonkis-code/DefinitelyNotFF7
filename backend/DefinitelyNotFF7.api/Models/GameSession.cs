namespace DefinitelyNotFF7.api.Models
{
    public class GameSession
    {
        public required Character Character {  get; set; }
        public bool IsActive { get; set; }
        public int EncounterNumber { get; set; }
        public int Potions { get; set; } = 3;
    }
}
