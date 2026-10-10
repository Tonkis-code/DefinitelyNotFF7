using DefinitelyNotFF7.api.Models;

namespace DefinitelyNotFF7.api.Services
{
    public class HealingService
    {
        public void HealCharacter(Character character)
        {
            double healing = character.MaxHealth * 0.25;
            int healingAmount = (int)Math.Round(healing, MidpointRounding.AwayFromZero);
            character.CurrentHealth = Math.Min(character.MaxHealth, character.CurrentHealth + healingAmount);
        }
    }
}
