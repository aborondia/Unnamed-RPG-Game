using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RPGGame
{
  enum GameState
  {
    Menu,
    Town,
    Map,
    Battle,
  }
  static class GameEngine
  {
    public static GameDataBase GameData;
    public static Difficulty Difficulty;
    private static int _defaultPauseDuration = 1000;
    private static GameState _currentGameState;

    public static GameState CurrentGameState { get => _currentGameState; }
    public static int PauseDuration { get => _defaultPauseDuration; }

    public static void StartGame()
    {
      if (GameData == null)
      {
        GameData = new GameDataBase();
      }

      bool deciding = true;

      Console.WriteLine("Welcome to the world of Unnamed RPG Project!");
      Pause(1600);
      Console.Clear();

      Console.WriteLine("Do you want to create your own party or choose your own?");
      Console.WriteLine("[1] Use pre-made party.");
      Console.WriteLine("[2] Create my own.");

      while (deciding)
      {
        switch (Console.ReadKey(true).Key)
        {
          case ConsoleKey.D1:
            deciding = false;
            GameData.InitializeData(true);
            break;
          case ConsoleKey.D2:
            deciding = false;
            GameData.InitializeData(false);
            break;
          default: continue;
        }
      }

      SwitchGameState(GameState.Menu);
    }

    public static void SwitchGameState(GameState newState)
    {
      _currentGameState = newState;

      Console.Clear();

      switch (_currentGameState)
      {
        case GameState.Menu:
          Menu.StartMainMenu();
          return;
        case GameState.Map:
          Map.StartMap();
          return;
        case GameState.Town:
          Town.StartTown();
          return;
        case GameState.Battle:
          Battle.StartBattle();
          return;
      }
    }

    public static void Pause(int duration = 0)
    {
      if (duration == 0)
      {
        Thread.Sleep(_defaultPauseDuration);
      }
      else
      {
        Thread.Sleep(duration);
      }
    }

    public static int ConsoleKeyToInt(ConsoleKey keyBind)
    {
      switch (keyBind)
      {
        case ConsoleKey.D1: return 1;
        case ConsoleKey.D2: return 2;
        case ConsoleKey.D3: return 3;
        case ConsoleKey.D4: return 4;
        case ConsoleKey.D5: return 5;
        case ConsoleKey.D6: return 6;
        case ConsoleKey.D7: return 7;
        case ConsoleKey.D8: return 8;
        case ConsoleKey.D9: return 9;
      }

      return -1;
    }

    public static ConsoleKey IntToConsoleKey(int index)
    {
      switch (index)
      {
        case 1: return ConsoleKey.D1;
        case 2: return ConsoleKey.D2;
        case 3: return ConsoleKey.D3;
        case 4: return ConsoleKey.D4;
        case 5: return ConsoleKey.D5;
        case 6: return ConsoleKey.D6;
        case 7: return ConsoleKey.D7;
        case 8: return ConsoleKey.D8;
        case 9: return ConsoleKey.D9;
      }

      return ConsoleKey.Escape;
    }

    public static void ColorText(ConsoleColor color, string str, bool lineBreak = true)
    {
      Console.ForegroundColor = color;
      if (lineBreak)
      {
        Console.WriteLine(str);
      }
      else
      {
        Console.Write(str);
      }
      Console.ForegroundColor = ConsoleColor.White;
    }
  }
}
