
using Microsoft.AspNetCore.Mvc;
using DefinitelyNotFF7.api.Models;
using DefinitelyNotFF7.api.Services;
using DefinitelyNotFF7.api.Factories;

namespace DefinitelyNotFF7.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly GameStateService _gameState;
        private readonly BattleOutcomeService _battleOutcome;

        public GameController(GameStateService gameState, BattleOutcomeService battleOutcome)
        {
            _gameState = gameState;
            _battleOutcome = battleOutcome;
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                game = "Definitely Not FF7",
                status = "Definitely in development",
                runActive = _gameState.CurrentSession?.IsActive ?? false,
                encounterNumber = _gameState.CurrentSession?.EncounterNumber ?? 0
            });
        }

        [HttpGet("character")]
        public IActionResult GetCharacter()
        {
            var character = new Character
            {
                Name = "Definitely Not Cloud",
                MaxHealth = 100,
                CurrentHealth = 100,
                Attack = 20
            };

            return Ok(character);
        }

        [HttpGet("enemy")]
        public IActionResult GetEnemy()
        {
            var enemy = new Enemy
            {
                Name = "Definitely Not Guard Scorpion",
                MaxHealth = 100,
                CurrentHealth = 100,
                Attack = 15
            };

            return Ok(enemy);
        }

        [HttpPost("run/start")]
        public IActionResult StartRun()
        {
            var character = new Character
            {
                Name = "Definitely Not Cloud",
                MaxHealth = 100,
                CurrentHealth = 100,
                MaxMp = 35,
                CurrentMp = 35,
                CurrentAtb = 0,
                MaxAtb = 2,
                Attack = 20,
                Level = 1,
                CurrentXp = 0,
                XpToNextLevel = 100
            };

            _gameState.CurrentSession = new GameSession
            {
                Character = character,
                IsActive = true
            };

            _gameState.CurrentBattle = null;

            if (_gameState.CurrentProgress == null)
            {
                _gameState.CurrentProgress = _battleOutcome.CreatePlayerProgress();
            }

            return Ok(_gameState.CurrentSession);
        }
    }
}
