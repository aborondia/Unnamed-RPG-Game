using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPGGame
{
  public enum GameState
  {
    Menu,
    Town,
    Map,
    Battle,
  }
  public class GameEngine : MonoBehaviour
  {
    public static GameEngine Active;
    public delegate bool WaitForPlayerActionDelegate();
    public GameDataBase GameData;
    public Difficulty Difficulty;
    private int _defaultPauseDuration = 1000;
    private GameState _currentGameState;
    public GameState CurrentGameState { get => _currentGameState; }
    public int PauseDuration { get => _defaultPauseDuration; }
    private string currentKeyPressed;
    public string CurrentKeyPressed => currentKeyPressed;

    private void Awake()
    {
      if (Active != null)
      {
        Destroy(Active);
      }

      Active = this;

      StartGame();
    }

    void OnEnable()
    {
      Keyboard.current.onTextInput += OnKeyPressed;
    }

    void OnDisable()
    {
      Keyboard.current.onTextInput -= OnKeyPressed;
    }

    private void OnKeyPressed(char value)
    {
      if (Keyboard.current.escapeKey.isPressed)
      {
        this.currentKeyPressed = "escape";
      }
      else if (Keyboard.current.spaceKey.isPressed)
      {
        this.currentKeyPressed = "space";
      }
      else if (Keyboard.current.enterKey.isPressed)
      {
        this.currentKeyPressed = "enter";
      }
      else
      {
        this.currentKeyPressed = value.ToString();
      }
    }

    public async void StartGame()
    {
      string keyPress = String.Empty;

      if (GameData == null)
      {
        GameData = new GameDataBase();
      }

      UIController.Active.WriteLine("Welcome to the world of Unnamed RPG Project!");
      await Pause(3000);
      UIController.Active.Clear();

      UIController.Active.WriteLine("Do you want to create your own party or use the pre-made party?");
      UIController.Active.WriteLine("[1] Use pre-made party.");
      UIController.Active.WriteLine("[2] Create my own.");

      await WaitForPlayerKeyPress(() =>
      {
        switch (this.currentKeyPressed)
        {
          case "1":
            keyPress = "1";
            return true;
          case "2":
            keyPress = "2";
            return true;
        }

        return false;
      });

      switch (keyPress)
      {
        case "1":
          await GameData.InitializeData(true);
          break;
        case "2":
          await GameData.InitializeData(false);
          break;
      }

      SwitchGameState(GameState.Menu);
    }

    public void SwitchGameState(GameState newState)
    {
      _currentGameState = newState;

      UIController.Active.Clear();

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

    public async UniTask Pause(int duration = 0)
    {
      if (duration == 0)
      {
        await UniTask.Delay(_defaultPauseDuration);
      }
      else
      {
        await UniTask.Delay(duration);
      }
    }

    public int ConsoleKeyToInt(ConsoleKey keyBind)
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

    public async UniTask WaitForPlayerKeyPress(WaitForPlayerActionDelegate action)
    {
      this.currentKeyPressed = String.Empty;

      await Pause(250);

      while (!action.Invoke())
      {
        UIController.Active.ScrollToEnd();
        await UniTask.Yield();
      }

      this.currentKeyPressed = String.Empty;
    }

    public async UniTask<string> WaitForPlayerInput()
    {
      await Pause(250);
      UIController.Active.ShowUserInputField();

      while (String.IsNullOrWhiteSpace(UIController.Active.UserInputField.value) || this.currentKeyPressed != "enter")
      {
        this.currentKeyPressed = String.Empty;
        UIController.Active.ScrollToEnd();
        await UniTask.Yield();
      }

      UIController.Active.HideUserInputField();
      return UIController.Active.UserInputField.value;
    }

    public async UniTask PerformActionWhenTrue(WaitForPlayerActionDelegate waitDelegate, Action action)
    {
      while (!waitDelegate.Invoke())
      {
        await UniTask.Yield();
      }

      action.Invoke();
    }

    public async void PerformActionAfterPause(Action action, int duration = 0)
    {
      await Pause(duration);

      action.Invoke();
    }
  }
}
