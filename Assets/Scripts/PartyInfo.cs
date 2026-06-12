using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RPGGame
{
  public static class PartyInfo
  {
    public static GameDataBase GameData = GameEngine.Active.GameData;
    private static int _startingGold = 100;
    private static List<PlayerCharacter> _partyMembers = new List<PlayerCharacter>();
    private static int _gold = _startingGold;
    private static Dictionary<Consumable, int> _usableItems = new Dictionary<Consumable, int>();
    private static DateTime _gameStarted = DateTime.Now;
    private static int _totalEnemiesKilled = 0;
    private static int _totalExperienceAccrued = 0;
    private static int _totalGoldAccrued = 100;
    private static int _partyMembersSlain = 0;

    public static List<PlayerCharacter> PartyMembers { get => _partyMembers; }
    public static int Gold { get => _gold; set => _gold += value; }
    public static Dictionary<Consumable, int> UsableItems { get => _usableItems; }

    public static void ModifyGold(int amount)
    {
      _gold += amount;
    }

    public static void EmptyGold()
    {
      _gold = 0;
    }

    public static void AddItem(Consumable item, int amount)
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

    public static async void ViewPartyInfo()
    {
      int keyIndex = 1;

      UIController.Active.WriteLine($"Party Gold: {_gold}");
      UIController.Active.WriteLine();
      UIController.Active.WriteLine("For detailed character information press the corresponding key.");
      UIController.Active.WriteLine();

      foreach (PlayerCharacter character in _partyMembers)
      {
        if (character.CharacterStatus == CharacterStatus.Dead)
        {
          Console.ForegroundColor = ConsoleColor.Gray;
        }

        UIController.Active.WriteColorText(ConsoleColor.Magenta, $"[{keyIndex++}]", true);
        UIController.Active.Write($"{character.Name} - HP: {character.CurrentHealth}/{character.MaxHealth} MP: {character.CurrentMana}/{character.MaxMana}");
        Console.ForegroundColor = ConsoleColor.White;
      }
      UIController.Active.WriteLine("[Esc]");
      UIController.Active.Write("Return to menu");

      if (_usableItems.Count > 0)
      {
        UIController.Active.WriteLine();
        UIController.Active.WriteLine("Inventory:");
        foreach (var item in _usableItems)
        {
          UIController.Active.WriteLine($"{item.Key.Name}x{item.Value}");
        }
      }

      await GameEngine.Active.WaitForPlayerKeyPress(() =>
           {
             switch (GameEngine.Active.CurrentKeyPressed)
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
                 UIController.Active.Clear();
                 Menu.StartMainMenu();
                 return true;
             }

             return false;
           });
    }

    private static async void ViewCharacterStatus(PlayerCharacter character)
    {
      UIController.Active.Clear();
      character.PrintStats();
      UIController.Active.WriteLine();
      UIController.Active.WriteLine("Press escape to return to party menu.");

      ConsoleKey keyPressed = Console.ReadKey(true).Key;

      await GameEngine.Active.WaitForPlayerKeyPress(() =>
        {
          if (GameEngine.Active.CurrentKeyPressed == "escape")
          {
            return true;
          }

          return false;
        });

      UIController.Active.Clear();
      PartyInfo.ViewPartyInfo();
    }

    public static async void ViewGameStats()
    {
      TimeSpan timePlayed = DateTime.Now - _gameStarted;
      string timePlayedText = $"H:{timePlayed.Hours} M:{timePlayed.Minutes} S:{timePlayed.Seconds}";
      UIController.Active.WriteLine($"Total Enemies Slain: {_totalEnemiesKilled}");
      UIController.Active.WriteLine($"Total Experience Accrued: {_totalExperienceAccrued}");
      UIController.Active.WriteLine($"Total Gold Accrued: {_totalGoldAccrued}");
      UIController.Active.WriteLine($"Total Times a Party Member Has Been Slain: {_partyMembersSlain}");
      UIController.Active.WriteLine(timePlayedText);
      UIController.Active.WriteLine();
      UIController.Active.WriteLine("Press escape to return to menu.");

      await GameEngine.Active.WaitForPlayerKeyPress(() =>
         {
           if (GameEngine.Active.CurrentKeyPressed == "escape")
           {
             return true;
           }

           return false;
         });

      UIController.Active.Clear();
      Menu.StartMainMenu();
    }

    public static void UpdateDeathCount()
    {
      _partyMembersSlain++;
    }

    public static void UpdateGameStats(int experience, int gold, bool enemyKilled = true)
    {
      _totalExperienceAccrued += experience;
      _totalGoldAccrued += gold;
      _totalEnemiesKilled += enemyKilled ? 1 : 0;
    }

    public static async void Cheat()
    {
      string keyPressed = String.Empty;
      UIController.Active.WriteLine("[1] Max Gold");
      UIController.Active.WriteLine("[2] Max Character Levels");
      UIController.Active.WriteLine("[3] Armed and Dangerous");
      UIController.Active.WriteLine("[Esc] On second thought...");

      await GameEngine.Active.WaitForPlayerKeyPress(() =>
        {
          switch (GameEngine.Active.CurrentKeyPressed)
          {
            case "1":
            case "2":
            case "3":
            case "escape":
              keyPressed = GameEngine.Active.CurrentKeyPressed;
              return true;
          }
          return false;
        });

      switch (keyPressed)
      {
        case "1":
          EmptyGold();
          ModifyGold(1000000);
          UIController.Active.WriteLine("Happy spending.");
          GameEngine.Active.PerformActionAfterPause(() =>
          {
            UIController.Active.Clear();
            Cheat();
          });
          break;
        case "2":
          foreach (PlayerCharacter character in _partyMembers)
          {
            await LevelUpToMax(character);
          }
          UIController.Active.WriteLine("You're so strong.");
          GameEngine.Active.PerformActionAfterPause(() =>
          {
            UIController.Active.Clear();
            Cheat();
          }, 3000);

          break;
        case "3":
          foreach (PlayerCharacter character in _partyMembers)
          {
            if (character.PlayerProfession is Warrior)
            {
              character.EquipItem(GameData.Equipment[2]);
              character.EquipItem(GameData.Equipment[5]);
              character.EquipItem(GameData.Equipment[8]);
            }
            if (character.PlayerProfession is Rouge)
            {
              character.EquipItem(GameData.Equipment[11]);
              character.EquipItem(GameData.Equipment[14]);
              character.EquipItem(GameData.Equipment[17]);
            }
            if (character.PlayerProfession is Wizard)
            {
              character.EquipItem(GameData.Equipment[20]);
              character.EquipItem(GameData.Equipment[23]);
              character.EquipItem(GameData.Equipment[26]);
            }
            if (character.PlayerProfession is Cleric)
            {
              character.EquipItem(GameData.Equipment[29]);
              character.EquipItem(GameData.Equipment[32]);
              character.EquipItem(GameData.Equipment[35]);
            }
          }
          UIController.Active.WriteLine("Looking good in that fancy equipment.");
          GameEngine.Active.PerformActionAfterPause(() =>
          {
            UIController.Active.Clear();
            Cheat();
          });
          break;
        case "escape":
          UIController.Active.Clear();
          Menu.StartMainMenu();
          break;
      }
    }

    private async static UniTask LevelUpToMax(PlayerCharacter character)
    {
      if (character.Level < 8)
      {
        await character.LevelUp(true);
        await LevelUpToMax(character);
      }
    }

    public static void ResetData()
    {
      _partyMembers.Clear();
      _usableItems.Clear();
      _gold = _startingGold;
    }
  }
}
