using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace RPGGame
{
  public enum Difficulty
  {
    Easy,
    Average,
    Hard,
    VeryHard
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
      this.uiController.WriteLine("[1] Forest of Illusion - Easy");
      this.uiController.WriteLine("[2] Caves of Despair - Average");
      this.uiController.WriteLine("[3] Lair of Vile Beasts - Hard");
      this.uiController.WriteLine("[4] The Underworld - Very Hard");
      this.uiController.WriteLine("[Esc] Return to menu");

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
