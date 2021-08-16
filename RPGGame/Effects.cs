using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
  enum Effect
  {
    HealHP,
    HealMP,
    DamageHp,
    Buff,
  }
  static class Effects
  {
    public static void HealHP(List<Character> targets, int healAmount)
    {
      for (int i = 0; i < targets.Count; i++)
      {
        if (targets[i].CharacterStatus == CharacterStatus.Dead)
        {
          continue;
        }

        string amountText = $"{healAmount} HP";

        if (targets[i].MaxHealth <= targets[i].CurrentHealth + healAmount)
        {
          targets[i].CurrentHealth = targets[i].MaxHealth;
          amountText = "full health";
        }
        else
        {
          targets[i].CurrentHealth = targets[i].CurrentHealth + healAmount;
        }

        GameEngine.ColorText(ConsoleColor.Green, $"{targets[i].Name} recovered {amountText}.");
        GameEngine.Pause();
      }
    }

    public static void HealMP(PlayerCharacter target, int healAmount)
    {
      if (target.MaxMana <= target.CurrentMana + healAmount)
      {
        target.CurrentMana = target.MaxMana;
        Console.WriteLine($"{target.Name} recovered full MP.");
        GameEngine.Pause();
      }
      else
      {
        target.CurrentMana += healAmount;
        Console.WriteLine($"{target.Name} regained {healAmount} MP.");
        GameEngine.Pause();
      }
    }

    public static void DamageHP(List<Character> targets, List<int> damage)
    {
      for (int i = 0; i < targets.Count; i++)
      {
        if (targets[i].CharacterStatus == CharacterStatus.Dead)
        {
          continue;
        }

        if (damage[i] <= 0)
        {
          Console.WriteLine($"{targets[i].Name} dodged the attack!");
          GameEngine.Pause();
          continue;
        }

        targets[i].CurrentHealth -= damage[i];
        GameEngine.ColorText(ConsoleColor.Red, $"{targets[i].Name} received {damage[i]} damage!");
        GameEngine.Pause();

        if (targets[i].CurrentHealth <= 0)
        {
          targets[i].CurrentHealth = 0;
          targets[i].CharacterStatus = CharacterStatus.Dead;
          targets[i].RemoveAllBuffs();

          if (targets[i] is PlayerCharacter)
          {
            PartyInfo.UpdateDeathCount();
          }

          GameEngine.ColorText(ConsoleColor.DarkRed, $"{targets[i].Name} has been slain!");
          GameEngine.Pause();
        }
      }
    }

    public static void ModifyStats(List<Character> targets, List<TemporaryBuff> temporaryBuffs)
    {
      foreach (Character target in targets)
      {
        if (target.CharacterStatus != CharacterStatus.Dead)
        {
          target.UpdateTemporaryBuffs(temporaryBuffs);
        }
      }
    }
  }
}
