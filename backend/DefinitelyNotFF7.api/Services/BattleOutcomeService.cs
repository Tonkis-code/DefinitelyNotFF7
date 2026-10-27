using DefinitelyNotFF7.api.Models;

namespace DefinitelyNotFF7.api.Services
{
    public class BattleOutcomeService
    {
        private readonly GameStateService _gameState;

        public BattleOutcomeService(GameStateService gameState)
        {
            _gameState = gameState;
        }

        public PlayerProgress CreatePlayerProgress()
        {
            var progress = new PlayerProgress
            {
                UnlockedAbilities = new List<string> { "Braver" }
            };

            return progress;
        }

        public void CheckAbilityUnlocks()
        {
            if (_gameState.CurrentSession != null && _gameState.CurrentProgress != null)
            {
                if (_gameState.CurrentSession.Character.Level >= 3 &&
                    !_gameState.CurrentProgress.UnlockedAbilities.Contains("FocusedThrust"))
                {
                    _gameState.CurrentProgress.UnlockedAbilities.Add("FocusedThrust");
                }
            }
        }

        public void AwardBattleXp()
        {
            if (_gameState.CurrentBattle == null)
            {
                return;
            }

            var levelingService = new LevelingService();

            if (_gameState.CurrentBattle.Winner == _gameState.CurrentBattle.Character.Name)
            {
                levelingService.GainXp(
                    _gameState.CurrentBattle.Character,
                    _gameState.CurrentBattle.Enemy.XpReward
                );

                CheckAbilityUnlocks();
            }
        }

        public void CheckRunEnd()
        {
            if (_gameState.CurrentBattle != null &&
                _gameState.CurrentSession != null &&
                _gameState.CurrentBattle.Winner == _gameState.CurrentBattle.Enemy.Name)
            {
                _gameState.CurrentSession.IsActive = false;
            }
        }

        public void HandleBattleOutcome()
        {
            if (_gameState.CurrentBattle == null || _gameState.CurrentBattle.Winner == null)
            {
                return;
            }

            if (_gameState.CurrentBattle.IsOutcomeProcessed)
            {
                return;
            }

            _gameState.CurrentBattle.IsOutcomeProcessed = true;

            if (_gameState.CurrentBattle.Winner == _gameState.CurrentBattle.Character.Name &&
                _gameState.CurrentSession != null)
            {
                AwardBattleXp();

                var healingService = new HealingService();
                healingService.HealCharacter(_gameState.CurrentBattle.Character);
                healingService.RestoreMpCharacter(_gameState.CurrentBattle.Character);

                _gameState.CurrentSession.EncounterNumber++;

                int roll = Random.Shared.Next(1, 101);

                if (roll <= 30)
                {
                    _gameState.CurrentSession.Potions++;
                    _gameState.CurrentBattle.PotionDropped = true;
                }
            }
            else if (_gameState.CurrentBattle.Winner == _gameState.CurrentBattle.Enemy.Name)
            {
                CheckRunEnd();
            }
        }
    }
}
