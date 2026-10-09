namespace DefinitelyNotFF7.api.Models
{
    public class GameSession
    {
        public required Character Character {  get; set; }
        public bool IsActive { get; set; }
    }
}
