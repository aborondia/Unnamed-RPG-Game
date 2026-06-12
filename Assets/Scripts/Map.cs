using System;
using Cysharp.Threading.Tasks;

namespace RPGGame
{
  public enum Difficulty
  {
    Easy,
    Average,
    Hard,
    VeryHard
  }
  public static class Map
  {
    public static GameDataBase GameData = GameEngine.Active.GameData;

    public static async void StartMap()
    {
      string keyPressed = String.Empty;
      UIController.Active.WriteLine("Where would you like to look for monsters to slay?");
      UIController.Active.WriteLine();
      UIController.Active.WriteLine("[1] Forest of Illusion - Easy");
      UIController.Active.WriteLine("[2] Caves of Despair - Average");
      UIController.Active.WriteLine("[3] Lair of Vile Beasts - Hard");
      UIController.Active.WriteLine("[4] The Underworld - Very Hard");
      UIController.Active.WriteLine("[Esc] Return to menu");

      await GameEngine.Active.WaitForPlayerKeyPress(() =>
      {
        switch (GameEngine.Active.CurrentKeyPressed)
        {
          case "1":
          case "2":
          case "3":
          case "4":
          case "escape":
            keyPressed = GameEngine.Active.CurrentKeyPressed;
            return true;
        }

        return false;
      });

      switch (keyPressed)
      {
        case "1":
          GameEngine.Active.Difficulty = Difficulty.Easy;
          UIController.Active.WriteLine("You explore The Forest of Illusion...");
          await GameEngine.Active.Pause(1600);
          GameEngine.Active.SwitchGameState(GameState.Battle);
          break;
        case "2":
          UIController.Active.WriteLine("You explore The Caves of Despair...");
          await GameEngine.Active.Pause(1600);
          GameEngine.Active.Difficulty = Difficulty.Average;
          GameEngine.Active.SwitchGameState(GameState.Battle);
          break;
        case "3":
          GameEngine.Active.Difficulty = Difficulty.Hard;
          UIController.Active.WriteLine("You explore The Lair of Vile Beasts...");
          await GameEngine.Active.Pause(1600);
          GameEngine.Active.SwitchGameState(GameState.Battle);
          break;
        case "4":
          UIController.Active.WriteLine("You explore The Underworld...");
          GameEngine.Active.Difficulty = Difficulty.VeryHard;
          await GameEngine.Active.Pause(1600);
          GameEngine.Active.SwitchGameState(GameState.Battle);
          break;
        case "escape":
          GameEngine.Active.SwitchGameState(GameState.Menu);
          break;
      }
    }
  }
}
