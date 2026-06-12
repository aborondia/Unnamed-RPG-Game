using System;
using UnityEngine;

namespace RPGGame
{
  public class Consumable
  {
    private GameEngine gameEngine;
    private UIController uiController => gameEngine.UIController;
    private string _name;
    private int _price;
    private string _description;
    private int _effectValue;
    private ConsoleKey _keyBind;
    private TargetType _targetType;
    private Effect _specialEffect;

    public string Name { get => this._name; }
    public int Price { get => this._price; }
    public string Description { get => this._description; }
    public int EffectValue { get => this._effectValue; }
    public ConsoleKey KeyBind { get => this._keyBind; }
    public TargetType TargetType { get => this._targetType; }
    public Effect SpecialEffect { get => this._specialEffect; }

    public Consumable(GameEngine gameEngine, string name, int price, string description, int effectValue, TargetType targetType, ConsoleKey keyBind, Effect specialEffect)
    {
      this.gameEngine = gameEngine;
      this._name = name;
      this._price = price;
      this._description = description;
      this._effectValue = effectValue;
      this._targetType = targetType;
      this._keyBind = keyBind;
      this._specialEffect = specialEffect;
    }

    public void PrintItemInfo(bool withCost = false)
    {
      this.uiController.Write($"{this._name} - {this._description}");

      if (withCost)
      {
        this.uiController.Write($" - {this._price}G");
      }

      this.uiController.Write("\n");
    }
  }
}
