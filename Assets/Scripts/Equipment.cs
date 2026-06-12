using System;
using System.Collections.Generic;
using System.Text;

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
    public static GameDataBase GameData = GameEngine.Active.GameData;
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

    public Equipment(int cost, EquipmentType equipmentType, string name, PlayerProfession canBeUsedBy, List<StatModifier> statModifiers, Element element = Element.None)
    {
      this._price = cost;
      this._equipmentType = equipmentType;
      this._name = name;
      this._canBeUsedBy = canBeUsedBy;
      this._statModifiers = statModifiers;
      this._element = element;
    }

    public void PrintEquipmentInfo(bool tradeInCost = true)
    {
      UIController.Active.Write($"{this._equipmentType}: {this._name} -");

      foreach (StatModifier statModifier in this._statModifiers)
      {
        UIController.Active.Write($" {statModifier.StatModifierType}+{statModifier.StatModifierValue} ");
      }

      if (tradeInCost)
      {
        UIController.Active.Write($"Trade in price: {this.TradeInPrice}G");
      }
      else
      {
        UIController.Active.Write($"Price: {this._price}G");
      }
    }
  }
}
