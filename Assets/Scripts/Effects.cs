using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace RPGGame
{
  public enum Effect
  {
    HealHP,
    HealMP,
    DamageHp,
    Buff,
  }
  public class Effects
  {
    [Inject] private GameEngine gameEngine;
    [Inject] private UIController uiController;
    [Inject] private PartyInfo partyInfo;

    public async UniTask HealHP(List<Character> targets, int healAmount)
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

        this.uiController.WriteColorText(ConsoleColor.Green, $"{targets[i].Name} recovered {amountText}.");
        await this.gameEngine.Pause();
      }
    }

    public async UniTask HealMP(PlayerCharacter target, int healAmount)
    {
      if (target.MaxMana <= target.CurrentMana + healAmount)
      {
        target.CurrentMana = target.MaxMana;
        this.uiController.WriteLine($"{target.Name} recovered full MP.");
        await this.gameEngine.Pause();
      }
      else
      {
        target.CurrentMana += healAmount;
        this.uiController.WriteLine($"{target.Name} regained {healAmount} MP.");
        await this.gameEngine.Pause();
      }
    }

    public async UniTask DamageHP(List<Character> targets, List<int> damage)
    {
      for (int i = 0; i < targets.Count; i++)
      {

        if (targets[i].CharacterStatus == CharacterStatus.Dead)
        {
          continue;
        }

        if (damage[i] <= 0)
        {
          this.uiController.WriteLine($"{targets[i].Name} dodged the attack!");
          await this.gameEngine.Pause();
          continue;
        }

        targets[i].CurrentHealth -= damage[i];
        this.uiController.WriteColorText(ConsoleColor.Red, $"{targets[i].Name} received {damage[i]} damage!");
        await this.gameEngine.Pause();

        if (targets[i].CurrentHealth <= 0)
        {
          targets[i].CurrentHealth = 0;
          targets[i].CharacterStatus = CharacterStatus.Dead;
          targets[i].RemoveAllBuffs();

          if (targets[i] is PlayerCharacter)
          {
            this.partyInfo.UpdateDeathCount();
          }

          this.uiController.WriteColorText(ConsoleColor.DarkRed, $"{targets[i].Name} has been slain!");
          await this.gameEngine.Pause();
        }
      }
    }

    public void ModifyStats(List<Character> targets, List<TemporaryBuff> temporaryBuffs)
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
