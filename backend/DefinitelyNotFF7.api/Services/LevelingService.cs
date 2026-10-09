using DefinitelyNotFF7.api.Models;

namespace DefinitelyNotFF7.api.Services
{
    public class LevelingService
    {
        public void GainXp(Character character, int amount)
        {
            character.CurrentXp += amount;

            while (character.CurrentXp >= character.XpToNextLevel)
            {
                character.Level++;
                character.CurrentXp -= character.XpToNextLevel;
                character.XpToNextLevel += 50;
                character.MaxHealth += 10;
                character.CurrentHealth += 10;
                character.Attack += 3;
            }            
        }
    }
}