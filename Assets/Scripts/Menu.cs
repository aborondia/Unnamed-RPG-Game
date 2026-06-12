using UnityEngine;
using VContainer;

namespace RPGGame
{
  public class Menu 
  {
    [Inject] private GameEngine gameEngine;
    [Inject] private UIController uiController;
    [Inject] private PartyInfo partyInfo;
    [Inject] private GameDataBase GameData;

    public void StartMainMenu()
    {
      UsingMainMenu();
    }

    private async void UsingMainMenu()
    {
      this.uiController.WriteLine("Hello Adventurers. What would you like to do?");
      this.uiController.WriteLine();
      this.uiController.WriteLine("[1] Go to Adventure Town");
      this.uiController.WriteLine("[2] Decide where to adventure");
      this.uiController.WriteLine("[3] View your party");
      this.uiController.WriteLine("[4] View game stats");
      this.uiController.WriteLine("[5] Cheat");

      await this.gameEngine.WaitForPlayerKeyPress(() =>
        {
          switch (this.gameEngine.CurrentKeyPressed)
          {
            case "1":
              this.gameEngine.SwitchGameState(GameState.Town);
              return true;
            case "2":
              this.gameEngine.SwitchGameState(GameState.Map);
              return true;
            case "3":
              this.uiController.Clear();
              this.partyInfo.ViewPartyInfo();
              return true;
            case "4":
              this.uiController.Clear();
              this.partyInfo.ViewGameStats();
              return true;
            case "5":
              this.uiController.Clear();
              this.partyInfo.Cheat();
              return true;
          }

          return false;
        });
    }
  }
}
