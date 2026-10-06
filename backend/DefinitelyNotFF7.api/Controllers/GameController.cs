using Microsoft.AspNetCore.Mvc;
using DefinitelyNotFF7.api.Models;

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
        
    }
}
