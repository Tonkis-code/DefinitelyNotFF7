namespace DefinitelyNotFF7.api.Models
{
    public class Battle
    {
        public required Character Character {  get; set; }
        public required Enemy Enemy { get; set; }
        public string? Winner { get; set; }
        public bool IsOutcomeProcessed { get; set; } = false;
        public int CurrentFloor { get; set; }
        public bool PotionDropped { get; set; } = false;
    }
}
