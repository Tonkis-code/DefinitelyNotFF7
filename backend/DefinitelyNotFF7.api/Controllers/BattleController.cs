using Microsoft.AspNetCore.Mvc;
using DefinitelyNotFF7.api.Models;
using DefinitelyNotFF7.api.Services;
using DefinitelyNotFF7.api.Factories;

namespace DefinitelyNotFF7.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BattleController : ControllerBase
    {
        private readonly GameStateService _gameState;
        private readonly BattleOutcomeService _battleOutcome;

        public BattleController(GameStateService gameState, BattleOutcomeService battleOutcome)
        {
            _gameState = gameState;
            _battleOutcome = battleOutcome;

        }

        [HttpGet]
        public IActionResult GetBattle()
        {
            if (_gameState.CurrentBattle == null)
            {
                return NotFound();
            }

            return Ok(_gameState.CurrentBattle);
        }

        [HttpPost("start")]
        public IActionResult StartBattle(string enemyType)
        {
            // Makes sure players cant start a battle without starting a run
            if (_gameState.CurrentSession == null || _gameState.CurrentSession.IsActive == false)
            {
                return BadRequest("No active run. Start a new run first.");
            }

            // Prevent players from skipping an unfinished encounter.
            if (_gameState.CurrentBattle != null && _gameState.CurrentBattle.Winner == null)
            {
                return BadRequest("A battle is already in progress.");
            }

            var enemyFactory = new EnemyFactory();

            Enemy enemy;
            int floor = _gameState.CurrentSession.EncounterNumber + 1;

            try
            {
                enemy = enemyFactory.CreateEnemy(enemyType, floor);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            // Reuse the same character so progression persists between encounters.
            _gameState.CurrentBattle = new Battle
            {
                Character = _gameState.CurrentSession.Character,
                Enemy = enemy,
                CurrentFloor = floor
            };

            return Ok(_gameState.CurrentBattle);
        }

        [HttpPost("attack")]
        public IActionResult BattleAttack()
        {
            if (_gameState.CurrentBattle == null)
            {
                return NotFound();
            }

            if (_gameState.CurrentBattle.Winner != null)
            {
                return BadRequest("The battle is already over.");
            }

            var battleService = new BattleService();

            battleService.Attack(_gameState.CurrentBattle);

            if (_gameState.CurrentBattle.Enemy.CurrentHealth > 0)
            {
                battleService.EnemyAttack(_gameState.CurrentBattle);
            }

            battleService.CheckWinner(_gameState.CurrentBattle);
            _battleOutcome.HandleBattleOutcome();

            return Ok(_gameState.CurrentBattle);
        }

        [HttpPost("ability")]
        public IActionResult BattleAbility(string ability)
        {
            if (_gameState.CurrentBattle == null)
            {
                return NotFound();
            }

            if (_gameState.CurrentBattle.Winner != null)
            {
                return BadRequest("The battle is already over.");
            }

            if (_gameState.CurrentProgress == null ||
                !_gameState.CurrentProgress.UnlockedAbilities.Contains(ability))
            {
                return BadRequest("Ability is not unlocked.");
            }

            var battleService = new BattleService();

            try
            {
                switch (ability)
                {
                    case "Braver":
                        battleService.Braver(_gameState.CurrentBattle);
                        break;

                    case "FocusedThrust":
                        battleService.FocusedThrust(_gameState.CurrentBattle);
                        break;

                    default:
                        return BadRequest("Unknown ability.");
                }
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            if (_gameState.CurrentBattle.Enemy.CurrentHealth > 0)
            {
                battleService.EnemyAttack(_gameState.CurrentBattle);
            }

            battleService.CheckWinner(_gameState.CurrentBattle);
            _battleOutcome.HandleBattleOutcome();

            return Ok(_gameState.CurrentBattle);
        }


        [HttpPost("defend")]
        public IActionResult BattleDefend()
        {
            if (_gameState.CurrentBattle == null)
            {
                return NotFound();
            }

            if (_gameState.CurrentBattle.Winner != null)
            {
                return BadRequest("The battle is already over.");
            }

            var battleService = new BattleService();

            battleService.Defend(_gameState.CurrentBattle);

            battleService.CheckWinner(_gameState.CurrentBattle);
            _battleOutcome.HandleBattleOutcome();

            return Ok(_gameState.CurrentBattle);
        }

        [HttpPost("potion")]
        public IActionResult BattlePotion()
        {
            if (_gameState.CurrentSession == null || _gameState.CurrentSession.IsActive == false)
            {
                return BadRequest("No active run. Start a new run first.");
            }

            if (_gameState.CurrentBattle == null)
            {
                return NotFound();
            }

            if (_gameState.CurrentBattle.Winner != null)
            {
                return BadRequest("The battle is already over.");
            }

            if (_gameState.CurrentSession.Potions <= 0)
            {
                return BadRequest("No potions left in inventory.");
            }

            if (_gameState.CurrentBattle.Character.CurrentHealth >=
                _gameState.CurrentBattle.Character.MaxHealth)
            {
                return BadRequest($"{_gameState.CurrentBattle.Character.Name} has full health");
            }

            var healingService = new HealingService();
            var battleService = new BattleService();

            healingService.UsePotion(_gameState.CurrentBattle.Character);
            _gameState.CurrentSession.Potions--;

            if (_gameState.CurrentBattle.Enemy.CurrentHealth > 0)
            {
                battleService.EnemyAttack(_gameState.CurrentBattle);
            }

            battleService.CheckWinner(_gameState.CurrentBattle);
            _battleOutcome.HandleBattleOutcome();

            return Ok(new
            {
                Battle = _gameState.CurrentBattle,
                PotionsRemaining = _gameState.CurrentSession.Potions
            });
        }
    }
}
