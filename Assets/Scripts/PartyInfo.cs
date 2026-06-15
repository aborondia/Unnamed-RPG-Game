using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace RPGGame
{
  public class PartyInfo
  {
    [Inject] private IObjectResolver resolver;
    [Inject] private GameEngine gameEngine;
    [Inject] private UIController uiController;
    private int _startingGold = 100;
    private List<PlayerCharacter> _partyMembers = new List<PlayerCharacter>();
    private int _gold;
    private Dictionary<Consumable, int> _usableItems = new Dictionary<Consumable, int>();
    private DateTime _gameStarted = DateTime.Now;
    private int _totalEnemiesKilled = 0;
    private int _totalExperienceAccrued = 0;
    private int _totalGoldAccrued = 100;
    private int _partyMembersSlain = 0;
    public List<PlayerCharacter> PartyMembers { get => _partyMembers; }
    public int Gold { get => _gold; set => _gold += value; }
    public Dictionary<Consumable, int> UsableItems { get => _usableItems; }

    public PartyInfo()
    {
      this._gold = this._startingGold;
    }

    public void ModifyGold(int amount)
    {
      _gold += amount;
    }

    public void EmptyGold()
    {
      _gold = 0;
    }

    public void AddItem(Consumable item, int amount)
    {
      if (_usableItems.ContainsKey(item))
      {
        _usableItems[item] += amount;
      }
      else
      {
        _usableItems[item] = amount;
      }
    }

    public async void ViewPartyInfo()
    {
      int keyIndex = 1;

      this.uiController.WriteLine($"Party Gold: {_gold}");
      this.uiController.WriteLine();
      this.uiController.WriteLine("For detailed character information press the corresponding key.");
      this.uiController.WriteLine();

      foreach (PlayerCharacter character in _partyMembers)
      {
        this.uiController.WriteColorText(ConsoleColor.Magenta, $"[{keyIndex++}]", true);

        if (character.CharacterStatus == CharacterStatus.Dead)
        {
          this.uiController.WriteColorText(ConsoleColor.Gray, $"{character.Name} - HP: {character.CurrentHealth}/{character.MaxHealth} MP: {character.CurrentMana}/{character.MaxMana}", false);
        }
        else
        {
          this.uiController.Write($"{character.Name} - HP: {character.CurrentHealth}/{character.MaxHealth} MP: {character.CurrentMana}/{character.MaxMana}");

        }
      }

      this.uiController.WriteLine("[Esc] Return to menu");

      if (_usableItems.Count > 0)
      {
        this.uiController.WriteLine();
        this.uiController.WriteLine("Inventory:");

        foreach (var item in _usableItems)
        {
          this.uiController.WriteLine($"{item.Key.Name}x{item.Value}");
        }
      }

      await this.gameEngine.WaitForPlayerKeyPress(() =>
           {
             switch (this.gameEngine.CurrentKeyPressed)
             {
               case "1":
                 ViewCharacterStatus(_partyMembers[0]);
                 return true;
               case "2":
                 ViewCharacterStatus(_partyMembers[1]);
                 return true;
               case "3":
                 ViewCharacterStatus(_partyMembers[2]);
                 return true;
               case "4":
                 ViewCharacterStatus(_partyMembers[3]);
                 return true;
               case "escape":
                 this.uiController.Clear();
                 this.resolver.Resolve<Menu>().StartMainMenu();
                 return true;
             }

             return false;
           });
    }

    private async void ViewCharacterStatus(PlayerCharacter character)
    {
      this.uiController.Clear();
      character.PrintStats();
      this.uiController.WriteLine();
      this.uiController.WriteLine("Press escape to return to party menu.");

      await this.gameEngine.WaitForPlayerKeyPress(() =>
        {
          if (this.gameEngine.CurrentKeyPressed == "escape")
          {
            return true;
          }

          return false;
        });

      this.uiController.Clear();
      ViewPartyInfo();
    }

    public async void ViewGameStats()
    {
      TimeSpan timePlayed = DateTime.Now - _gameStarted;
      string timePlayedText = $"H:{timePlayed.Hours} M:{timePlayed.Minutes} S:{timePlayed.Seconds}";
      this.uiController.WriteLine($"Total Enemies Slain: {_totalEnemiesKilled}");
      this.uiController.WriteLine($"Total Experience Accrued: {_totalExperienceAccrued}");
      this.uiController.WriteLine($"Total Gold Accrued: {_totalGoldAccrued}");
      this.uiController.WriteLine($"Total Times a Party Member Has Been Slain: {_partyMembersSlain}");
      this.uiController.WriteLine(timePlayedText);
      this.uiController.WriteLine();
      this.uiController.WriteLine("Press escape to return to menu.");

      await this.gameEngine.WaitForPlayerKeyPress(() =>
         {
           if (this.gameEngine.CurrentKeyPressed == "escape")
           {
             return true;
           }

           return false;
         });

      this.uiController.Clear();
      this.resolver.Resolve<Menu>().StartMainMenu();
    }

    public void UpdateDeathCount()
    {
      _partyMembersSlain++;
    }

    public void UpdateGameStats(int experience, int gold, bool enemyKilled = true)
    {
      _totalExperienceAccrued += experience;
      _totalGoldAccrued += gold;
      _totalEnemiesKilled += enemyKilled ? 1 : 0;
    }

    public async void Cheat()
    {
      string keyPressed = String.Empty;
      this.uiController.WriteLine("[1] Max Gold");
      this.uiController.WriteLine("[2] Max Character Levels");
      this.uiController.WriteLine("[3] Armed and Dangerous");
      this.uiController.WriteLine("[Esc] On second thought...");

      await this.gameEngine.WaitForPlayerKeyPress(() =>
        {
          switch (this.gameEngine.CurrentKeyPressed)
          {
            case "1":
            case "2":
            case "3":
            case "escape":
              keyPressed = this.gameEngine.CurrentKeyPressed;
              return true;
          }
          return false;
        });

      switch (keyPressed)
      {
        case "1":
          EmptyGold();
          ModifyGold(1000000);
          this.uiController.WriteLine("Happy spending.");
          this.gameEngine.PerformActionAfterPause(() =>
          {
            this.uiController.Clear();
            Cheat();
          });
          break;
        case "2":
          foreach (PlayerCharacter character in _partyMembers)
          {
            await character.LevelUpToMax();
          }

          this.uiController.WriteLine("You're so strong.");
          this.gameEngine.PerformActionAfterPause(() =>
          {
            this.uiController.Clear();
            Cheat();
          }, 3000);

          break;
        case "3":
          foreach (PlayerCharacter character in _partyMembers)
          {
            if (character.PlayerProfession is Warrior)
            {
              character.EquipItem(this.resolver.Resolve<GameDataBase>().Equipment[2]);
              character.EquipItem(this.resolver.Resolve<GameDataBase>().Equipment[5]);
              character.EquipItem(this.resolver.Resolve<GameDataBase>().Equipment[8]);
            }
            if (character.PlayerProfession is Rouge)
            {
              character.EquipItem(this.resolver.Resolve<GameDataBase>().Equipment[11]);
              character.EquipItem(this.resolver.Resolve<GameDataBase>().Equipment[14]);
              character.EquipItem(this.resolver.Resolve<GameDataBase>().Equipment[17]);
            }
            if (character.PlayerProfession is Wizard)
            {
              character.EquipItem(this.resolver.Resolve<GameDataBase>().Equipment[20]);
              character.EquipItem(this.resolver.Resolve<GameDataBase>().Equipment[23]);
              character.EquipItem(this.resolver.Resolve<GameDataBase>().Equipment[26]);
            }
            if (character.PlayerProfession is Cleric)
            {
              character.EquipItem(this.resolver.Resolve<GameDataBase>().Equipment[29]);
              character.EquipItem(this.resolver.Resolve<GameDataBase>().Equipment[32]);
              character.EquipItem(this.resolver.Resolve<GameDataBase>().Equipment[35]);
            }
          }
          this.uiController.WriteLine("Looking good in that fancy equipment.");
          this.gameEngine.PerformActionAfterPause(() =>
          {
            this.uiController.Clear();
            Cheat();
          });
          break;
        case "escape":
          this.uiController.Clear();
          this.resolver.Resolve<Menu>().StartMainMenu();
          break;
      }
    }

    public void ResetData()
    {
      _partyMembers.Clear();
      _usableItems.Clear();
      _gold = _startingGold;
    }
  }
}
