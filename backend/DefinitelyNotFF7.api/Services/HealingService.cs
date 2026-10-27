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
        public void UsePotion(Character character)
        {
            double healing = character.MaxHealth * 0.30;
            int potionAmount = (int)Math.Round(healing, MidpointRounding.AwayFromZero);
            character.CurrentHealth = Math.Min(character.MaxHealth, character.CurrentHealth + potionAmount);
        }
        
        public void RestoreMpCharacter(Character character)
        {
            double mpRestore = character.MaxMp * 0.25;
            int restoreAmount = (int)Math.Round(mpRestore, MidpointRounding.AwayFromZero);
            character.CurrentMp = Math.Min(character.MaxMp, character.CurrentMp + restoreAmount);
        }
    }
}
