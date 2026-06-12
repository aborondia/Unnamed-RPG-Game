using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace RPGGame
{
  static class Menu
  {
    public static GameDataBase GameData = GameEngine.Active.GameData;

    public static void StartMainMenu()
    {
      UsingMainMenu();
    }

    private static async void UsingMainMenu()
    {
      UIController.Active.WriteLine("Hello Adventurers. What would you like to do?");
      UIController.Active.WriteLine();
      UIController.Active.WriteLine("[1] Go to Adventure Town");
      UIController.Active.WriteLine("[2] Decide where to adventure");
      UIController.Active.WriteLine("[3] View your party");
      UIController.Active.WriteLine("[4] View game stats");
      UIController.Active.WriteLine("[5] Cheat");

      await GameEngine.Active.WaitForPlayerKeyPress(() =>
        {
          switch (GameEngine.Active.CurrentKeyPressed)
          {
            case "1":
              GameEngine.Active.SwitchGameState(GameState.Town);
              return true;
            case "2":
              GameEngine.Active.SwitchGameState(GameState.Map);
              return true;
            case "3":
              UIController.Active.Clear();
              PartyInfo.ViewPartyInfo();
              return true;
            case "4":
              UIController.Active.Clear();
              PartyInfo.ViewGameStats();
              return true;
            case "5":
              UIController.Active.Clear();
              PartyInfo.Cheat();
              return true;
          }

          return false;
        });
    }
  }
}
