using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
  class TemporaryBuff
  {
    public int Duration;
    private StatModifier _statModifier;
    private string _buffDisplay;
    public StatModifier StatModifier { get => this._statModifier; }
    public string BuffDisplay { get => this._buffDisplay; }

    public TemporaryBuff(int duration, StatModifier statModifier)
    {
      this.Duration = duration;
      this._statModifier = statModifier;
      char buffOrDebuff = this._statModifier.StatModifierValue < 0 ? '-' : '+';

      switch (statModifier.StatModifierType)
      {
        case StatModifierType.Attack:
          this._buffDisplay = $"Att{buffOrDebuff}";
          break;
        case StatModifierType.Defense:
          this._buffDisplay = $"Def{buffOrDebuff}";
          break;
        case StatModifierType.Magic:
          this._buffDisplay = $"Mag{buffOrDebuff}";
          break;
        case StatModifierType.MagiDefense:
          this._buffDisplay = $"Mdf{buffOrDebuff}";
          break;
        case StatModifierType.Agility:
          this._buffDisplay = $"Agi{buffOrDebuff}";
          break;
        case StatModifierType.Dexterity:
          this._buffDisplay = $"Dex{buffOrDebuff}";
          break;
      }
    }
  }
}
