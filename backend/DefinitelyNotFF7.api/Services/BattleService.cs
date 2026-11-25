using DefinitelyNotFF7.api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DefinitelyNotFF7.api.Services
{
    public class BattleService
    {
        // Attack checks damage based on defined values of Character and their Attack value and subtracts it from Enemy Health
        public int Attack(Battle battle)
        {
            int damage = battle.Character.Attack;
            battle.Enemy.CurrentHealth = Math.Max(0, battle.Enemy.CurrentHealth - damage);
            battle.Character.CurrentAtb = Math.Min(battle.Character.MaxAtb, battle.Character.CurrentAtb + 1);
            return damage;

        }

        // EnemyAttack checks damage based on defined values of Enemy and their Attack value and subtracts it from Character Health
        public int EnemyAttack(Battle battle)
        {
            int damage = battle.Character.IsDefending ? battle.Enemy.Attack /2 : battle.Enemy.Attack;
            battle.Character.IsDefending = false;
            battle.Character.CurrentHealth = Math.Max(0, battle.Character.CurrentHealth - damage);
            return damage;
        }

        public void Defend(Battle battle)
        {
            battle.Character.IsDefending = true;
            battle.Character.CurrentAtb = Math.Min(battle.Character.MaxAtb, battle.Character.CurrentAtb + 1);
        }

        public int Braver(Battle battle)
        {
            if (battle.Character.CurrentAtb < 1)
            {
                throw new InvalidOperationException("Not Enough ATB!");
            }
            battle.Character.CurrentAtb -= 1;
            int damage = battle.Character.Attack * 2;
            battle.Enemy.CurrentHealth = Math.Max(0, battle.Enemy.CurrentHealth - damage);
            return damage;
        }

        public int FocusedThrust(Battle battle)
        {
            if (battle.Character.CurrentAtb < 2)
            {
                throw new InvalidOperationException("Not Enough ATB!");
            }
            battle.Character.CurrentAtb -= 2;
            int damage = battle.Character.Attack * 4;
            battle.Enemy.CurrentHealth = Math.Max(0, battle.Enemy.CurrentHealth - damage);
            return damage;
        }

        public void CheckWinner(Battle battle)
        {
            if (battle.Enemy.CurrentHealth == 0)
            {
                battle.Winner = battle.Character.Name;
            }
            else if (battle.Character.CurrentHealth == 0)
            {
                battle.Winner = battle.Enemy.Name;
            }
        }
    }
}
