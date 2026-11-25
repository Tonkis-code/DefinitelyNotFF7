namespace DefinitelyNotFF7.api.Models
{
    public class BattleActionRequest
    {
        public string? Ability { get; set; }
        public required BattleActionType Type { get; set; }
    }
}
