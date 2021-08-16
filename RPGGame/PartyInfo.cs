using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
  static class PartyInfo
  {
    public static GameDataBase GameData = GameEngine.GameData;
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

    public static void ViewPartyInfo()
    {
      int keyIndex = 1;

      Console.WriteLine($"Party Gold: {_gold}");
      Console.WriteLine();
      Console.WriteLine("For detailed character information press the corresponding key.");
      Console.WriteLine();

      foreach (PlayerCharacter character in _partyMembers)
      {
        if (character.CharacterStatus == CharacterStatus.Dead)
        {
          Console.ForegroundColor = ConsoleColor.Gray;
        }
        GameEngine.ColorText(ConsoleColor.Magenta, $"[{keyIndex++}]", false);
        Console.WriteLine($"{character.Name} - HP: {character.CurrentHealth}/{character.MaxHealth} MP: {character.CurrentMana}/{character.MaxMana}");
        Console.ForegroundColor = ConsoleColor.White;
      }
      GameEngine.ColorText(ConsoleColor.Magenta, "[Esc]", false);
      Console.WriteLine("Return to menu");

      if (_usableItems.Count > 0)
      {
        Console.WriteLine();
        Console.WriteLine("Inventory:");
        foreach (var item in _usableItems)
        {
          Console.WriteLine($"{item.Key.Name}x{item.Value}");
        }
      }

      bool deciding = true;

      while (deciding)
      {
        switch (Console.ReadKey(true).Key)
        {
          case ConsoleKey.D1:
            ViewCharacterStatus(_partyMembers[0]);
            deciding = false;
            break;
          case ConsoleKey.D2:
            deciding = false;
            ViewCharacterStatus(_partyMembers[1]);
            break;
          case ConsoleKey.D3:
            deciding = false;
            ViewCharacterStatus(_partyMembers[2]);
            break;
          case ConsoleKey.D4:
            deciding = false;
            ViewCharacterStatus(_partyMembers[3]);
            break;
          case ConsoleKey.Escape:
            deciding = false;
            Console.Clear();
            Menu.StartMainMenu();
            break;
        }
      }
    }

    private static void ViewCharacterStatus(PlayerCharacter character)
    {
      Console.Clear();
      character.PrintStats();
      Console.WriteLine();
      Console.WriteLine("Press escape to return to party menu.");

      ConsoleKey keyPressed = Console.ReadKey(true).Key;

      while (keyPressed != ConsoleKey.Escape)
      {
        keyPressed = Console.ReadKey(true).Key;
      }

      Console.Clear();
      PartyInfo.ViewPartyInfo();
    }

    public static void ViewGameStats()
    {
      TimeSpan timePlayed = DateTime.Now - _gameStarted;
      string timePlayedText = $"H:{timePlayed.Hours} M:{timePlayed.Minutes} S:{timePlayed.Seconds}";
      Console.WriteLine($"Total Enemies Slain: {_totalEnemiesKilled}");
      Console.WriteLine($"Total Experience Accrued: {_totalExperienceAccrued}");
      Console.WriteLine($"Total Gold Accrued: {_totalGoldAccrued}");
      Console.WriteLine($"Total Times a Party Member Has Been Slain: {_partyMembersSlain}");
      Console.WriteLine(timePlayedText);
      Console.WriteLine();
      Console.WriteLine("Press escape to return to menu.");

      ConsoleKey keyPressed = Console.ReadKey(true).Key;

      while (keyPressed != ConsoleKey.Escape)
      {
        keyPressed = Console.ReadKey(true).Key;
      }

      Console.Clear();
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

    public static void Cheat()
    {
      bool deciding = true;

      Console.WriteLine("[1] Max Gold");
      Console.WriteLine("[2] Max Character Levels");
      Console.WriteLine("[3] Armed and Dangerous");
      Console.WriteLine("[Esc] On second thought...");

      while (deciding)
      {
        switch (Console.ReadKey(true).Key)
        {
          case ConsoleKey.D1:
            deciding = false;
            EmptyGold();
            ModifyGold(1000000);
            Console.WriteLine("Happy spending.");
            GameEngine.Pause();
            Console.Clear();
            Cheat();
            break;
          case ConsoleKey.D2:
            deciding = false;
            foreach (PlayerCharacter character in _partyMembers)
            {
              while (character.Level < 8)
              {
                character.LevelUp(true);
              }
            }
            Console.WriteLine("You're so strong.");
            GameEngine.Pause();
            Console.Clear();
            Cheat();
            break;
          case ConsoleKey.D3:
            deciding = false;
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
            Console.WriteLine("Looking good in that fancy equipment.");
            GameEngine.Pause();
            Console.Clear();
            Cheat();
            break;
          case ConsoleKey.Escape:
            deciding = false;
            Console.Clear();
            Menu.StartMainMenu();
            break;
        }
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
