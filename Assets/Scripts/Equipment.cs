using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace RPGGame
{
  public enum EquipmentType
  {
    MainHand,
    OffHand,
    Armor
  }
  public class Equipment
  {
    private GameEngine gameEngine;
    private UIController uiController => gameEngine.UIController;
    private int _price;
    private EquipmentType _equipmentType;
    private string _name;
    private PlayerProfession _canBeUsedBy;
    private List<StatModifier> _statModifiers;
    private Element _element; // didn't have time to implement, sorry

    public string Name { get => this._name; }
    public EquipmentType EquipmentType { get => this._equipmentType; }
    public List<StatModifier> StatModifiers { get => this._statModifiers; }
    public PlayerProfession CanBeUsedBy { get => this._canBeUsedBy; }
    public int Price { get => this._price; }
    public int TradeInPrice { get => this._price / 2; }

    public Equipment(GameEngine gameEngine, int cost, EquipmentType equipmentType, string name, PlayerProfession canBeUsedBy, List<StatModifier> statModifiers, Element element = Element.None)
    {
      this.gameEngine = gameEngine;
      this._price = cost;
      this._equipmentType = equipmentType;
      this._name = name;
      this._canBeUsedBy = canBeUsedBy;
      this._statModifiers = statModifiers;
      this._element = element;
    }

    public void PrintEquipmentInfo(bool tradeInCost = true)
    {
      this.uiController.Write($"{this._equipmentType}: {this._name} -");

      foreach (StatModifier statModifier in this._statModifiers)
      {
        this.uiController.Write($" {statModifier.StatModifierType}+{statModifier.StatModifierValue} ");
      }

      if (tradeInCost)
      {
        this.uiController.Write($"Trade in price: {this.TradeInPrice}G");
      }
      else
      {
        this.uiController.Write($"Price: {this._price}G");
      }
    }
  }
}
