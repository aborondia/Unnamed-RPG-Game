using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace RPGGame
{
  static class Menu
  {
    public static GameDataBase GameData = GameEngine.GameData;

    public static void StartMainMenu()
    {
      UsingMainMenu();
    }

    private static void UsingMainMenu()
    {
      Console.WriteLine("Hello Adventurers. What would you like to do?");
      Console.WriteLine();
      Console.WriteLine("[1] Go to Adventure Town");
      Console.WriteLine("[2] Decide where to adventure");
      Console.WriteLine("[3] View your party");
      Console.WriteLine("[4] View game stats");
      Console.WriteLine("[5] Cheat");

      while (GameEngine.CurrentGameState == GameState.Menu)
      {
        switch (Console.ReadKey(true).Key)
        {
          case ConsoleKey.D1:
            GameEngine.SwitchGameState(GameState.Town);
            return;
          case ConsoleKey.D2:
            GameEngine.SwitchGameState(GameState.Map);
            continue;
          case ConsoleKey.D3:
            Console.Clear();
            PartyInfo.ViewPartyInfo();
            continue;
          case ConsoleKey.D4:
            Console.Clear();
            PartyInfo.ViewGameStats();
            continue;
          case ConsoleKey.D5:
            Console.Clear();
            PartyInfo.Cheat();
            continue;
        }
      }
    }
  }
}
