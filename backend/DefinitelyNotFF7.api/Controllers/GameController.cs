using Microsoft.AspNetCore.Mvc;
using DefinitelyNotFF7.api.Models;
using DefinitelyNotFF7.api.Services;

namespace DefinitelyNotFF7.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private static Battle? currentBattle;

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


        [HttpPost("battle/start")]
        public IActionResult StartBattle()
        {
            var character = new Character
            {
                Name = "Definitely Not Cloud",
                MaxHealth = 100,
                CurrentHealth = 100,
                CurrentAtb = 0,
                MaxAtb = 2,
                Attack = 20
            };

            var enemy = new Enemy
            {
                Name = "Definitely Not Guard Scorpion",
                MaxHealth = 150,
                CurrentHealth = 150,
                Attack = 15
            };

            currentBattle = new Battle
            {
                Character = character,
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

            if (currentBattle.Enemy.CurrentHealth == 0)
            {
                currentBattle.Winner = currentBattle.Character.Name;
            }
            else if (currentBattle.Character.CurrentHealth == 0)
            {
                currentBattle.Winner = currentBattle.Enemy.Name;
            }

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

            if (currentBattle.Enemy.CurrentHealth == 0)
            {
                currentBattle.Winner = currentBattle.Character.Name;
            }
            else if (currentBattle.Character.CurrentHealth == 0)
            {
                currentBattle.Winner = currentBattle.Enemy.Name;
            }

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

            if (currentBattle.Enemy.CurrentHealth == 0)
            {
                currentBattle.Winner = currentBattle.Character.Name;
            }
            else if (currentBattle.Character.CurrentHealth == 0)
            {
                currentBattle.Winner = currentBattle.Enemy.Name;
            }

            return Ok(currentBattle);
        }
    }
}
