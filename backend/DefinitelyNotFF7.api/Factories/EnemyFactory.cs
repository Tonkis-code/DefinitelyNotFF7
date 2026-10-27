using DefinitelyNotFF7.api.Models;

namespace DefinitelyNotFF7.api.Factories
{
    public class EnemyFactory
    {
        public Enemy CreateEnemy(string enemyType, int floor)
        {
            double healthMultiplier = 1 + (floor - 1) * 0.1;
            double attackMultiplier = 1 + (floor - 1) * 0.05;

            switch (enemyType)
            {
                case "Grunt":
                    return new Enemy
                    {
                        Name = "Definitely Not Shinra Grunt",
                        MaxHealth = (int)Math.Round(60 * healthMultiplier),
                        CurrentHealth = (int)Math.Round(60 * healthMultiplier),
                        Attack = (int)Math.Round(10 * attackMultiplier),
                        XpReward = 40
                    };

                case "Scorpion":
                    return new Enemy
                    {
                        Name = "Definitely Not Scorpion",
                        MaxHealth = (int)Math.Round(125 * healthMultiplier),
                        CurrentHealth = (int)Math.Round(125 * healthMultiplier),
                        Attack = (int)Math.Round(15 * attackMultiplier),
                        XpReward = 100
                    };

                default:
                    throw new InvalidOperationException("Not A Valid Enemy.");
            }
        }
    }
}
