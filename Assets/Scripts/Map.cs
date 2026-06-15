using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace RPGGame
{
  public enum Difficulty
  {
    Easy = 0,
    Average = 1,
    Hard = 2,
    VeryHard = 3,
  }
  public enum Shops
  {
    Inn = 0,
    ArmsDealer = 1,
    Apothecary = 2,
    Doctor = 3,
  }
  public class Map
  {
    [Inject] private GameEngine gameEngine;
    [Inject] private UIController uiController;
    [Inject] private GameDataBase GameData;

    public async void StartMap()
    {
      string keyPressed = String.Empty;
      this.uiController.WriteLine("Where would you like to look for monsters to slay?");
      this.uiController.WriteLine();
      this.uiController.WriteLine("[1] Forest of Illusion - Easy", "1");
      this.uiController.WriteLine("[2] Caves of Despair - Average", "2");
      this.uiController.WriteLine("[3] Lair of Vile Beasts - Hard", "3");
      this.uiController.WriteLine("[4] The Underworld - Very Hard", "4");
      this.uiController.WriteLine("[Esc] Return to menu", "escape");

      await this.gameEngine.WaitForPlayerKeyPress(() =>
      {
        switch (this.gameEngine.CurrentKeyPressed)
        {
          case "1":
          case "2":
          case "3":
          case "4":
          case "escape":
            keyPressed = this.gameEngine.CurrentKeyPressed;
            return true;
        }

        return false;
      });

      this.uiController.WriteLine();

      switch (keyPressed)
      {
        case "1":
          this.gameEngine.Difficulty = Difficulty.Easy;
          this.uiController.WriteLine("You explore The Forest of Illusion...");
          await this.gameEngine.Pause(1600);
          this.gameEngine.SwitchGameState(GameState.Battle);
          break;
        case "2":
          this.uiController.WriteLine("You explore The Caves of Despair...");
          await this.gameEngine.Pause(1600);
          this.gameEngine.Difficulty = Difficulty.Average;
          this.gameEngine.SwitchGameState(GameState.Battle);
          break;
        case "3":
          this.gameEngine.Difficulty = Difficulty.Hard;
          this.uiController.WriteLine("You explore The Lair of Vile Beasts...");
          await this.gameEngine.Pause(1600);
          this.gameEngine.SwitchGameState(GameState.Battle);
          break;
        case "4":
          this.uiController.WriteLine("You explore The Underworld...");
          this.gameEngine.Difficulty = Difficulty.VeryHard;
          await this.gameEngine.Pause(1600);
          this.gameEngine.SwitchGameState(GameState.Battle);
          break;
        case "escape":
          this.gameEngine.SwitchGameState(GameState.Menu);
          break;
      }
    }
  }
}
