using Microsoft.AspNetCore.Mvc;
using DefinitelyNotFF7.api.Models;
using DefinitelyNotFF7.api.Services;

namespace DefinitelyNotFF7.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
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

        // Temp Endpoint for testing our battles with hardcoded characters/enemies
        [HttpGet("battle")]
        public IActionResult GetBattle()
        {
            var character = new Character
            {
                Name = "Definitely Not Cloud",
                MaxHealth = 100,
                CurrentHealth = 100,
                Attack = 20
            };

            var enemy = new Enemy
            {
                Name = "Definitely Not Guard Scorpion",
                MaxHealth = 150,
                CurrentHealth = 150,
                Attack = 15
            };
            var battle = new Battle
            {
                Character = character,
                Enemy = enemy
            };

            var battleService = new BattleService();

            int damage = battleService.Attack(battle);

            return Ok(battle);

        }
    }
}
