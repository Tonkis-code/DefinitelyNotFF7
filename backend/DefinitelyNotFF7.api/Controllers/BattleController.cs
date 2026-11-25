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


        [HttpPost("turn")]
        public IActionResult TakeTurn([FromBody] BattleActionRequest request)
        {
            var healingService = new HealingService();
            var battleService = new BattleService();

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



            switch (request.Type)
            {
                case BattleActionType.Attack:
                    battleService.Attack(_gameState.CurrentBattle);
                    break;

                case BattleActionType.Defend:
                    battleService.Defend(_gameState.CurrentBattle);
                    break;

                case BattleActionType.Ability:

                    if (string.IsNullOrWhiteSpace(request.Ability))
                    {
                        return BadRequest("No ability chosen.");
                    }

                    if (_gameState.CurrentProgress == null ||
                        !_gameState.CurrentProgress.UnlockedAbilities.Contains(request.Ability))
                    {
                        return BadRequest("Ability is not unlocked.");
                    }
                    try
                    {
                        switch (request.Ability)
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
                    break;

                case BattleActionType.Potion:
                    if (_gameState.CurrentSession.Potions <= 0)
                    {
                        return BadRequest("No potions left in inventory.");
                    }

                    if (_gameState.CurrentBattle.Character.CurrentHealth >=
                        _gameState.CurrentBattle.Character.MaxHealth)
                    {
                        return BadRequest($"{_gameState.CurrentBattle.Character.Name} has full health");
                    }
                    healingService.UsePotion(_gameState.CurrentBattle.Character);
                    _gameState.CurrentSession.Potions--;
                    break;

                default:
                    return BadRequest("Unknown action.");

            }

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
