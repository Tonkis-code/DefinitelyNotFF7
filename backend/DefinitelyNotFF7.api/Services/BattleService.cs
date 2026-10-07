using DefinitelyNotFF7.api.Models;

namespace DefinitelyNotFF7.api.Services
{
    public class BattleService
    {
        // Attack checks damage based on defined values of Character and their Attack value and subtracts it from Enemy Health
        public int Attack(Battle battle)
        {
            int damage = battle.Character.Attack;
            battle.Enemy.CurrentHealth = Math.Max(0, battle.Enemy.CurrentHealth - damage);
            return damage;

        }

        // EnemyAttack checks damage based on defined values of Enemy and their Attack value and subtracts it from Character Health
        public int EnemyAttack(Battle battle)
        {
            int damage = battle.Enemy.Attack;
            battle.Character.CurrentHealth = Math.Max(0, battle.Character.CurrentHealth - damage);
            return damage;
        }

        public int Defend(Battle battle)
        {
            int damage = battle.Enemy.Attack / 2;
            battle.Character.CurrentHealth = Math.Max(0, battle.Character.CurrentHealth - damage);
            return damage;
        }
    }
}
