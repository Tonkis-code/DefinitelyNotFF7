using DefinitelyNotFF7.api.Models;

namespace DefinitelyNotFF7.api.Services
{
    public class BattleService
    {
        public int Attack(Battle battle)
        {
            int damage = battle.Character.Attack;
            battle.Enemy.CurrentHealth = Math.Max(0, battle.Enemy.CurrentHealth - damage);
            return damage;

        }
    }
}
