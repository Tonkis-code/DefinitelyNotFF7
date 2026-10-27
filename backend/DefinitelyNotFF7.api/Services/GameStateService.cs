using DefinitelyNotFF7.api.Models;

namespace DefinitelyNotFF7.api.Services
{
    public class GameStateService
    {
        public Battle? CurrentBattle { get; set; }
        public GameSession? CurrentSession { get; set; }
        public PlayerProgress? CurrentProgress { get; set; }
    }
}
