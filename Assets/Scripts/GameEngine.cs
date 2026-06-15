using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

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
    [Inject] private IObjectResolver resolver;
    [Inject] private UIController uiController;
    public UIController UIController => uiController;
    public GameDataBase GameData => resolver.Resolve<GameDataBase>();
    public delegate bool WaitForPlayerActionDelegate();
    public Difficulty Difficulty;
    private int _defaultPauseDuration = 1000;
    private GameState _currentGameState;
    public GameState CurrentGameState { get => _currentGameState; }
    public int PauseDuration { get => _defaultPauseDuration; }
    private string currentKeyPressed;
    public string CurrentKeyPressed => currentKeyPressed;

    private void Start()
    {
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
        SetCurrentKey("escape");
      }
      else if (Keyboard.current.spaceKey.isPressed)
      {
        SetCurrentKey("space");
      }
      else if (Keyboard.current.enterKey.isPressed)
      {
        SetCurrentKey("enter");
      }
      else
      {
        SetCurrentKey(value.ToString());
      }
    }

    public void SetCurrentKey(string value)
    {
      if (String.IsNullOrWhiteSpace(value))
      {
        return;
      }

      this.currentKeyPressed = value.ToLower();
    }

    public async void StartGame()
    {
      string keyPress = String.Empty;

      this.uiController.WriteLine("Welcome to the world of Unnamed RPG Project!");
      await Pause(3000);
      this.uiController.Clear();

      this.uiController.WriteLine("Do you want to create your own party or use the pre-made party?");
      this.uiController.WriteLine("[1] Use pre-made party.", "1");
      this.uiController.WriteLine("[2] Create my own.", "2");

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
          await this.GameData.InitializeData(true);
          break;
        case "2":
          await this.GameData.InitializeData(false);
          break;
      }

      SwitchGameState(GameState.Menu);
    }

    public void SwitchGameState(GameState newState)
    {
      _currentGameState = newState;

      this.uiController.Clear();

      switch (_currentGameState)
      {
        case GameState.Menu:
          this.resolver.Resolve<Menu>().StartMainMenu();
          return;
        case GameState.Map:
          this.resolver.Resolve<Map>().StartMap();
          return;
        case GameState.Town:
          this.resolver.Resolve<Town>().StartTown();
          return;
        case GameState.Battle:
          this.resolver.Resolve<Battle>().StartBattle();
          return;
      }
    }

    public async UniTask Pause(int duration = 0)
    {
      // public static UniTask Delay(int millisecondsDelay, bool ignoreTimeScale = false, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false)

      if (duration == 0)
      {
        await UniTask.Delay(_defaultPauseDuration, false, PlayerLoopTiming.Update, this.destroyCancellationToken);
      }
      else
      {
        await UniTask.Delay(duration, false, PlayerLoopTiming.Update, this.destroyCancellationToken);
      }
    }

    public async UniTask WaitForPlayerKeyPress(WaitForPlayerActionDelegate action)
    {
      this.currentKeyPressed = String.Empty;

      await Pause(250);

      while (!action.Invoke())
      {
        this.uiController.ScrollToEnd();
        await UniTask.Yield(this.destroyCancellationToken);
      }

      this.currentKeyPressed = String.Empty;
    }

    public async UniTask<string> WaitForPlayerInput()
    {
      await Pause(250);
      this.uiController.ShowUserInputField();

      while (String.IsNullOrWhiteSpace(this.uiController.UserInputField.value) || this.currentKeyPressed != "enter")
      {
        this.currentKeyPressed = String.Empty;
        this.uiController.ScrollToEnd();
        await UniTask.Yield(this.destroyCancellationToken);
      }

      this.uiController.HideUserInputField();
      return this.uiController.UserInputField.value;
    }

    public async UniTask PerformActionWhenTrue(WaitForPlayerActionDelegate waitDelegate, Action action)
    {
      while (!waitDelegate.Invoke())
      {
        await UniTask.Yield(this.destroyCancellationToken);
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
