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
        private static Battle? currentBattle;
        private static GameSession? currentSession;

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                game = "Definitely Not FF7",
                status = "Definitely in development"
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
                CurrentAtb = 0,
                MaxAtb = 2,
                Attack = 20,
                Level = 1,
                CurrentXp = 0,
                XpToNextLevel = 100
            };

            currentSession = new GameSession
            {
                Character = character,
                IsActive = true
            };

            currentBattle = null;

            return Ok(currentSession);
        }

        [HttpPost("battle/start")]
        public IActionResult StartBattle(string enemyType)
        {
            // Makes sure players cant start a battle without starting a run
            if (currentSession == null || currentSession.IsActive == false)
            {
                return BadRequest("No active run. Start a new run first.");
            }

            // Prevent players from skipping an unfinished encounter.
            if (currentBattle != null && currentBattle.Winner == null)
            {
                return BadRequest("A battle is already in progress.");
            }

            var enemyFactory = new EnemyFactory();

            Enemy enemy;

            try
            {
                enemy = enemyFactory.CreateEnemy(enemyType);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            // Reuse the same character so progression persists between encounters.
            currentBattle = new Battle
            {
                Character = currentSession.Character,
                Enemy = enemy
            };


            return Ok(currentBattle);
        }

        [HttpPost("battle/attack")]
        public IActionResult BattleAttack()
        {
            if (currentBattle == null)
            {
                return NotFound();
            }

            if (currentBattle.Winner != null)
            {
                return BadRequest("The battle is already over.");
            }

            var battleService = new BattleService();

            battleService.Attack(currentBattle);

            if (currentBattle.Enemy.CurrentHealth > 0)
            {
                battleService.EnemyAttack(currentBattle);
            }

            battleService.CheckWinner(currentBattle);
            AwardBattleXp();
            CheckRunEnd();

            return Ok(currentBattle);
        }

        [HttpPost("battle/ability")]
        public IActionResult BattleAbility(string ability)
        {
            if (currentBattle == null)
            {
                return NotFound();
            }

            if (currentBattle.Winner != null)
            {
                return BadRequest("The battle is already over.");
            }

            var battleService = new BattleService();

            try
            {
                switch (ability)
                {
                    case "Braver":
                        battleService.Braver(currentBattle);
                        break;

                    case "FocusedThrust":
                        battleService.FocusedThrust(currentBattle);
                        break;

                    default:
                        return BadRequest("Unknown ability.");

                }
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            if (currentBattle.Enemy.CurrentHealth > 0)
            {
                battleService.EnemyAttack(currentBattle);
            }

            battleService.CheckWinner(currentBattle);
            AwardBattleXp();
            CheckRunEnd();

            return Ok(currentBattle);
        }

        [HttpGet("battle")]
        public IActionResult GetBattle()
        {
            if (currentBattle == null)
            {
                return NotFound();
            }
            
            return Ok(currentBattle);
            
        }

        [HttpPost("battle/defend")]
        public IActionResult BattleDefend()
        {
            if (currentBattle == null)
            {
                return NotFound();
            }

            if (currentBattle.Winner != null)
            {
                return BadRequest("The battle is already over.");
            }

            var battleService = new BattleService();

            battleService.Defend(currentBattle);

            battleService.CheckWinner(currentBattle);

            CheckRunEnd();

            return Ok(currentBattle);
        }

        private void AwardBattleXp()
        {
            if (currentBattle == null)
            {
                return;
            }
            var levelingService = new LevelingService();

            if (currentBattle.Winner == currentBattle.Character.Name)
            {
                levelingService.GainXp(currentBattle.Character, currentBattle.Enemy.XpReward);
            }
        }

        private void CheckRunEnd()
        {
            if (currentBattle != null && currentSession != null && currentBattle.Winner == currentBattle.Enemy.Name)
            {
                currentSession.IsActive = false;
            }
        }
    }
}
