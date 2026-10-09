using DefinitelyNotFF7.api.Models;

namespace DefinitelyNotFF7.api.Factories
{
    public class EnemyFactory
    {
        public Enemy CreateEnemy(string enemyType)
        {
            switch (enemyType)
            {
                case "Grunt":
                    return new Enemy
                    {
                        Name = "Definitely Not Shinra Grunt",
                        MaxHealth = 60,
                        CurrentHealth = 60,
                        Attack = 10,
                        XpReward = 40
                    };

                case "Scorpion":
                    return new Enemy
                    {
                        Name = "Definitely Not Scorpion",
                        MaxHealth = 125,
                        CurrentHealth = 125,
                        Attack = 15,
                        XpReward = 100
                    };

                default:
                    throw new InvalidOperationException("Not A Valid Enemy.");
            }
        }
    }
}
