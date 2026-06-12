using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
  public enum StatModifierType
  {
    Health,
    Mana,
    Attack,
    Defense,
    Magic,
    Will,
    MagiDefense,
    Agility,
    Dexterity

  }
  public class StatModifier
  {
    public static GameDataBase GameData = GameEngine.Active.GameData;
    private StatModifierType _statModifierType;
    private int _statModifierValue;

    public StatModifierType StatModifierType { get => this._statModifierType; }
    public int StatModifierValue { get => this._statModifierValue; }

    public StatModifier(StatModifierType statModifierType, int statModifierValue)
    {
      this._statModifierType = statModifierType;
      this._statModifierValue = statModifierValue;
    }
  }
}
