using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
  enum PoolUsed
  {
    HP,
    MP
  }
  enum StatUsed
  {
    Strength,
    Magic,
    Will,
    Agility,
    Dexterity
  }
  class SpecialAbility
  {
    protected string _name;
    protected string _actionText;
    protected string _description;
    protected double _statMultiplier;
    protected StatUsed _statUsed;
    protected Element _element;
    protected AttackType _attacktype;
    protected TargetType _targetType;
    protected Effect _effect;
    protected Dictionary<StatModifierType, int> _statModifiers;
    protected int _duration;
    // add custom text
    public string Name { get => this._name; }
    public string ActionText { get => this._actionText; }
    public string Description { get => this._description; }
    public AttackType AttackType { get => this._attacktype; }
    public Element Element { get => this._element; }
    public TargetType TargetType { get => this._targetType; }
    public Effect Effect { get => this._effect; }
    public Dictionary<StatModifierType, int> StatModifiers { get => this._statModifiers; }
    public int Duration { get => this._duration; }
    public SpecialAbility(string name, string actionText, string description, double statMultiplier, StatUsed statUsed, Element element, AttackType attacktype, TargetType targetType, Effect effect)
    {
      this._name = name;
      this._actionText = actionText;
      this._description = description;
      this._statMultiplier = statMultiplier;
      this._statUsed = statUsed;
      this._element = element;
      this._attacktype = attacktype;
      this._targetType = targetType;
      this._effect = effect;
    }
    public SpecialAbility(string name, string actionText, string description, double statMultiplier, StatUsed statUsed, Element element, AttackType attacktype, TargetType targetType, Effect effect, Dictionary<StatModifierType, int> statModifiers, int duration)
    {
      this._name = name;
      this._actionText = actionText;
      this._description = description;
      this._statMultiplier = statMultiplier;
      this._statUsed = statUsed;
      this._element = element;
      this._attacktype = attacktype;
      this._targetType = targetType;
      this._effect = effect;
      this._duration = duration;
      this._statModifiers = new Dictionary<StatModifierType, int>();
      foreach (var statModifier in statModifiers)
      {
        this._statModifiers.Add(statModifier.Key, statModifier.Value);
      }
    }

    public int GetAbilityPower(Character character)
    {
      double statToUse = 0;

      switch (this._statUsed)
      {
        case StatUsed.Strength:
          statToUse = character.AttackPower;
          break;
        case StatUsed.Magic:
          statToUse = character.MagicPower;
          break;
        case StatUsed.Agility:
          statToUse = character.Agility;
          break;
        case StatUsed.Dexterity:
          statToUse = character.Dexterity;
          break;
        case StatUsed.Will:
          statToUse = character.MagicDefense;
          break;
      }

      return (int)Math.Ceiling(statToUse += (statToUse * this._statMultiplier));
    }
  }

  class PlayerAbility : SpecialAbility
  {
    private ConsoleKey _keyBind;
    private PoolUsed _poolUsed;
    private double _poolCost;

    public ConsoleKey KeyBind { get => this._keyBind; }
    public PoolUsed PoolUsed { get => this._poolUsed; }
    public double Cost { get => this._poolCost; }

    public PlayerAbility(string name, string actionText, string description, double statMultiplier, StatUsed statUsed, Element element, AttackType attacktype, TargetType targetType, Effect effect, PoolUsed poolUsed, double poolCost, ConsoleKey keyBind) :
base(name, actionText, description, statMultiplier, statUsed, element, attacktype, targetType, effect)
    {
      this._poolUsed = poolUsed;
      this._poolCost = poolCost;
      this._keyBind = keyBind;
    }
    public PlayerAbility(string name, string actionText, string description, double statMultiplier, StatUsed statUsed, Element element, AttackType attacktype, TargetType targetType, Effect effect, PoolUsed poolUsed, double poolCost, ConsoleKey keyBind, Dictionary<StatModifierType, int> statModifiers, int duration) :
base(name, actionText, description, statMultiplier, statUsed, element, attacktype, targetType, effect, statModifiers, duration)
    {
      this._poolUsed = poolUsed;
      this._poolCost = poolCost;
      this._keyBind = keyBind;
    }


    public void PrintAbilityInfo(PlayerCharacter character)
    {
      int cost = GetAbilityCost(character);

      if (!AbilityCanBeUsed(cost, character))
      {
        Console.ForegroundColor = ConsoleColor.DarkGray;
      }

      Console.Write($"{this._name} - {cost}{this._poolUsed} ");
      Console.Write("- Element: ");
      switch (this._element)
      {
        case Element.None:
          Console.Write("None ");
          break;
        case Element.Fire:
          GameEngine.ColorText(ConsoleColor.DarkRed, "Fire ", false);
          break;
        case Element.Water:
          GameEngine.ColorText(ConsoleColor.DarkBlue, "Water ", false);
          break;
        case Element.Wind:
          GameEngine.ColorText(ConsoleColor.DarkGreen, "Wind ", false);
          break;
        case Element.Earth:
          GameEngine.ColorText(ConsoleColor.DarkYellow, "Earth ", false);
          break;
        case Element.Dark:
          GameEngine.ColorText(ConsoleColor.DarkGray, "Dark ", false);
          break;
        case Element.Light:
          GameEngine.ColorText(ConsoleColor.Yellow, "Light ", false);
          break;
      }
      Console.Write($"- Stat Used: {this._statUsed} ");


      Console.Write($"- {this._description}");
      Console.ForegroundColor = ConsoleColor.White;
    }

    public int GetAbilityCost(PlayerCharacter character)
    {
      double cost = this._poolUsed == PoolUsed.HP ? Math.Ceiling(this._poolCost * character.MaxHealth) : this._poolCost;

      return (int)cost;
    }

    public bool AbilityCanBeUsed(int cost, PlayerCharacter character)
    {
      if (this._poolUsed == PoolUsed.HP && character.CurrentHealth <= cost)
      {
        return false;
      }

      if (this._poolUsed == PoolUsed.MP && character.CurrentMana < cost)
      {
        return false;
      }

      return true;
    }
  }

  class EnemyAbility : SpecialAbility
  {
    public EnemyAbility(string name, string actionText, string description, double statMultiplier, StatUsed statUsed, Element element, AttackType attacktype, TargetType targetType, Effect effect) :
  base(name, actionText, description, statMultiplier, statUsed, element, attacktype, targetType, effect)
    {
    }

    public EnemyAbility(string name, string actionText, string description, double statMultiplier, StatUsed statUsed, Element element, AttackType attacktype, TargetType targetType, Effect effect, Dictionary<StatModifierType, int> statModifiers, int duration) :
      base(name, actionText, description, statMultiplier, statUsed, element, attacktype, targetType, effect, statModifiers, duration)
    {
    }
  }
}
