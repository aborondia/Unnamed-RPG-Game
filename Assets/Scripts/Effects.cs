using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
  public enum Effect
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

        UIController.Active.WriteColorText(ConsoleColor.Green, $"{targets[i].Name} recovered {amountText}.");
        GameEngine.Active.Pause();
      }
    }

    public static void HealMP(PlayerCharacter target, int healAmount)
    {
      if (target.MaxMana <= target.CurrentMana + healAmount)
      {
        target.CurrentMana = target.MaxMana;
        UIController.Active.WriteLine($"{target.Name} recovered full MP.");
        GameEngine.Active.Pause();
      }
      else
      {
        target.CurrentMana += healAmount;
        UIController.Active.WriteLine($"{target.Name} regained {healAmount} MP.");
        GameEngine.Active.Pause();
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
          UIController.Active.WriteLine($"{targets[i].Name} dodged the attack!");
          GameEngine.Active.Pause();
          continue;
        }

        targets[i].CurrentHealth -= damage[i];
        UIController.Active.WriteColorText(ConsoleColor.Red, $"{targets[i].Name} received {damage[i]} damage!");
        GameEngine.Active.Pause();

        if (targets[i].CurrentHealth <= 0)
        {
          targets[i].CurrentHealth = 0;
          targets[i].CharacterStatus = CharacterStatus.Dead;
          targets[i].RemoveAllBuffs();

          if (targets[i] is PlayerCharacter)
          {
            PartyInfo.UpdateDeathCount();
          }

          UIController.Active.WriteColorText(ConsoleColor.DarkRed, $"{targets[i].Name} has been slain!");
          GameEngine.Active.Pause();
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
