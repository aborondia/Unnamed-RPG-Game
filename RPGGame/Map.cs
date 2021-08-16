using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
  enum Difficulty
  {
    Easy,
    Average,
    Hard,
    VeryHard
  }
  static class Map
  {
    public static GameDataBase GameData = GameEngine.GameData;


    public static void StartMap()
    {
      Console.WriteLine("Where would you like to look for monsters to slay?");
      Console.WriteLine();
      Console.WriteLine("[1] Forest of Illusion - Easy");
      Console.WriteLine("[2] Caves of Despair - Average");
      Console.WriteLine("[3] Lair of Vile Beasts - Hard");
      Console.WriteLine("[4] The Underworld - Very Hard");
      Console.WriteLine("[Esc] Return to menu");

      while (GameEngine.CurrentGameState == GameState.Map)
      {
        switch (Console.ReadKey(true).Key)
        {
          case ConsoleKey.D1:
            GameEngine.Difficulty = Difficulty.Easy;
            Console.WriteLine("You explore The Forest of Illusion...");
            GameEngine.Pause(1600);
            GameEngine.SwitchGameState(GameState.Battle);
            break;
          case ConsoleKey.D2:
            Console.WriteLine("You explore The Caves of Despair...");
            GameEngine.Pause(1600);
            GameEngine.Difficulty = Difficulty.Average;
            GameEngine.SwitchGameState(GameState.Battle);
            break;
          case ConsoleKey.D3:
            GameEngine.Difficulty = Difficulty.Hard;
            Console.WriteLine("You explore The Lair of Vile Beasts...");
            GameEngine.Pause(1600);
            GameEngine.SwitchGameState(GameState.Battle);
            break;
          case ConsoleKey.D4:
            Console.WriteLine("You explore The Underworld...");
            GameEngine.Difficulty = Difficulty.VeryHard;
            GameEngine.Pause(1600);
            GameEngine.SwitchGameState(GameState.Battle);
            break;
          case ConsoleKey.Escape:
            GameEngine.SwitchGameState(GameState.Menu);
            break;
        }
      }
    }
  }
}
