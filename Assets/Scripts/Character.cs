using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
  enum CharacterStatus
  {
    Alive,
    Dead,
  }
  class Character
  {
    public static GameDataBase GameData = GameEngine.GameData;
    public bool IsDefending = false;
    protected CharacterStatus _characterStatus = CharacterStatus.Alive;
    protected Dictionary<StatModifierType, int> _statModifiers = new Dictionary<StatModifierType, int>
    {
      [StatModifierType.Health] = 0,
      [StatModifierType.Mana] = 0,
      [StatModifierType.Attack] = 0,
      [StatModifierType.Magic] = 0,
      [StatModifierType.Defense] = 0,
      [StatModifierType.MagiDefense] = 0,
      [StatModifierType.Will] = 0,
      [StatModifierType.Agility] = 0,
      [StatModifierType.Dexterity] = 0,
    };

    protected List<TemporaryBuff> _temporaryBuffs = new List<TemporaryBuff>();
    protected HashSet<Element> _weaknesses;
    protected HashSet<Element> _resistances;
    protected string _name;
    protected int _maxHealth;
    protected int _currentHealth;
    protected int _strength;
    protected int _vitality;
    protected int _magic;
    protected int _willPower;
    protected int _agility;
    protected int _dexterity;

    public string Name { get => this._name; }
    public CharacterStatus CharacterStatus { get => this._characterStatus; set => _characterStatus = value; }
    public Dictionary<StatModifierType, int> StatModifiers { get => this._statModifiers; }
    public List<TemporaryBuff> TemporaryBuffs { get => this._temporaryBuffs; }
    public HashSet<Element> Weaknesses { get => this._weaknesses; }
    public HashSet<Element> Resistances { get => this._resistances; }
    public int MaxHealth { get => this is PlayerCharacter ? this._maxHealth + (this as PlayerCharacter).StatModifiers[StatModifierType.Health] : this._maxHealth; }
    public int CurrentHealth { get => this._currentHealth; set => _currentHealth = value; }
    public int AttackPower { get => this._strength + this._statModifiers[StatModifierType.Attack]; }
    public int DefensePower { get => this._vitality + this._statModifiers[StatModifierType.Defense]; }
    public int MagicPower { get => this._magic + this._statModifiers[StatModifierType.Magic]; }
    public int MagicDefense { get => this._willPower + this._statModifiers[StatModifierType.MagiDefense]; }
    public int Agility { get => this._agility + this._statModifiers[StatModifierType.Agility]; }
    public int Dexterity { get => this._dexterity + this._statModifiers[StatModifierType.Dexterity]; }

    public Character(string name, int maxHealth, int strength, int vitality, int magic, int will, int agility, int dexterity, HashSet<Element> resistances, HashSet<Element> weaknesses)
    {
      this._name = name;
      this._maxHealth = maxHealth;
      this._currentHealth = maxHealth;
      this._strength = strength;
      this._vitality = vitality;
      this._magic = magic;
      this._willPower = will;
      this._agility = agility;
      this._dexterity = dexterity;
      this._weaknesses = weaknesses == null ? new HashSet<Element>() : weaknesses;
      this._resistances = resistances == null ? new HashSet<Element>() : resistances;
    }

    public void UpdateTemporaryBuffs(List<TemporaryBuff> temporaryBuffs = null)
    {
      if (temporaryBuffs == null)
      {
        for (int i = 0; i < this._temporaryBuffs.Count; i++)
        {
          TemporaryBuff buff = this._temporaryBuffs[i];

          buff.Duration--;

          if (buff.Duration < 0)
          {
            UpdateStatModifiers(buff.StatModifier, true);
            this._temporaryBuffs.Remove(buff);
            i--;
          }
        }
      }
      else
      {
        foreach (var buff in temporaryBuffs)
        {
          this._temporaryBuffs.Add(new TemporaryBuff(buff.Duration, buff.StatModifier));
          UpdateStatModifiers(buff.StatModifier, false);
        }
      }
    }

    public void RemoveAllBuffs()
    {
      foreach (var buff in this._temporaryBuffs)
      {
        UpdateStatModifiers(buff.StatModifier, true);
      }

      this._temporaryBuffs.Clear();
    }

    private void UpdateStatModifiers(StatModifier statModifier, bool removing)
    {
      int modifier = removing ? -1 : 1;

      switch (statModifier.StatModifierType)
      {
        case StatModifierType.Health:
          this._statModifiers[StatModifierType.Health] += (statModifier.StatModifierValue * modifier);
          break;
        case StatModifierType.Mana:
          this._statModifiers[StatModifierType.Mana] += (statModifier.StatModifierValue * modifier);
          break;
        case StatModifierType.Attack:
          this._statModifiers[StatModifierType.Attack] += (statModifier.StatModifierValue * modifier);
          break;
        case StatModifierType.Defense:
          this._statModifiers[StatModifierType.Defense] += (statModifier.StatModifierValue * modifier);
          break;
        case StatModifierType.Magic:
          this._statModifiers[StatModifierType.Magic] += (statModifier.StatModifierValue * modifier);
          break;
        case StatModifierType.MagiDefense:
          this._statModifiers[StatModifierType.MagiDefense] += (statModifier.StatModifierValue * modifier);
          break;
        case StatModifierType.Agility:
          this._statModifiers[StatModifierType.Agility] += (statModifier.StatModifierValue * modifier);
          break;
        case StatModifierType.Dexterity:
          this._statModifiers[StatModifierType.Dexterity] += (statModifier.StatModifierValue * modifier);
          break;
      }
    }

  }

  class PlayerCharacter : Character
  {
    private int _maxMana;
    private int _currentMana;
    private int _level = 1;
    private int _experience = 0;
    private PlayerProfession _playerProfession;
    private Dictionary<ConsoleKey, PlayerAbility> _specialAbilities;
    private Equipment _mainHand;
    private Equipment _offHand;
    private Equipment _armor;
    private Dictionary<int, int> _levelChart = new Dictionary<int, int>
    {
      [1] = 0,
      [2] = 50,
      [3] = 100,
      [4] = 200,
      [5] = 500,
      [6] = 800,
      [7] = 1200,
      [8] = 2000
    };

    public Dictionary<ConsoleKey, PlayerAbility> SpecialAbilities { get => this._specialAbilities; }
    public PlayerProfession PlayerProfession { get => this._playerProfession; }

    public int Level { get => this._level; }
    public Dictionary<int, int> LevelChart { get => this._levelChart; }
    public int Experience { get => this._experience; }
    public int MaxMana { get => this._maxMana; }
    public int CurrentMana { get => this._currentMana; set => this._currentMana = value; }
    public Equipment MainHand { get => this._mainHand; }
    public Equipment OffHand { get => this._offHand; }
    public Equipment Armor { get => this._armor; }

    public PlayerCharacter
      (string name, PlayerProfession playerProfession, HashSet<Element> resistances = null, HashSet<Element> weaknesses = null) : base
      (name, playerProfession.BaseHealth, playerProfession.BaseStrength, playerProfession.BaseVitality, playerProfession.BaseMagic, playerProfession.BaseWill, playerProfession.BaseAgility, playerProfession.BaseDexterity, resistances, weaknesses)
    {
      this._maxMana = playerProfession.BaseMana;
      this._currentMana = this._maxMana;
      this._playerProfession = playerProfession;
      this._specialAbilities = new Dictionary<ConsoleKey, PlayerAbility>();

      this._specialAbilities.Add(playerProfession.GetNewAbility(1).KeyBind, playerProfession.GetNewAbility(1));
      EquipItem(GameData.Equipment[playerProfession.StartingMainHand]);
      EquipItem(GameData.Equipment[playerProfession.StartingOffHand]);
      EquipItem(GameData.Equipment[playerProfession.StartingArmor]);
    }

    public void EquipItem(Equipment equipmentToEquip)
    {
      switch (equipmentToEquip.EquipmentType)
      {
        case EquipmentType.MainHand:
          UnequipItem(this._mainHand);
          UpdateStatModifiers(equipmentToEquip, false);
          this._mainHand = equipmentToEquip;
          break;
        case EquipmentType.OffHand:
          UnequipItem(this._offHand);
          UpdateStatModifiers(equipmentToEquip, false);
          this._offHand = equipmentToEquip;
          break;
        case EquipmentType.Armor:
          UnequipItem(this._armor);
          UpdateStatModifiers(equipmentToEquip, false);
          this._armor = equipmentToEquip;
          break;
      }
    }

    private void UnequipItem(Equipment equipmentToRemove)
    {
      if (equipmentToRemove == null)
      {
        return;
      }

      UpdateStatModifiers(equipmentToRemove, true);
    }

    private void UpdateStatModifiers(Equipment equipment, bool removing)
    {
      int modifier = removing ? -1 : 1;

      foreach (StatModifier statModifier in equipment.StatModifiers)
      {
        switch (statModifier.StatModifierType)
        {
          case StatModifierType.Health:
            this._statModifiers[StatModifierType.Health] += (statModifier.StatModifierValue * modifier);
            continue;
          case StatModifierType.Mana:
            this._statModifiers[StatModifierType.Mana] += (statModifier.StatModifierValue * modifier);
            continue;
          case StatModifierType.Attack:
            this._statModifiers[StatModifierType.Attack] += (statModifier.StatModifierValue * modifier);
            continue;
          case StatModifierType.Defense:
            this._statModifiers[StatModifierType.Defense] += (statModifier.StatModifierValue * modifier);
            continue;
          case StatModifierType.Magic:
            this._statModifiers[StatModifierType.Magic] += (statModifier.StatModifierValue * modifier);
            continue;
          case StatModifierType.MagiDefense:
            this._statModifiers[StatModifierType.MagiDefense] += (statModifier.StatModifierValue * modifier);
            continue;
          case StatModifierType.Agility:
            this._statModifiers[StatModifierType.Agility] += (statModifier.StatModifierValue * modifier);
            continue;
          case StatModifierType.Dexterity:
            this._statModifiers[StatModifierType.Dexterity] += (statModifier.StatModifierValue * modifier);
            continue;
        }
      }
    }

    public void UpdateExperience(int amount)
    {
      this._experience += amount;

      if (this._level < 8 && this._experience >= this._levelChart[this._level + 1])
      {
        LevelUp();
      }
    }

    public void LevelUp(bool fastForward = false)
    {
      int pauseDuration = fastForward ? 1 : GameEngine.PauseDuration;
      Console.WriteLine(pauseDuration);
      Console.WriteLine($"{this._name} has leveled up!");
      GameEngine.Pause(pauseDuration);

      this._level++;
      PlayerAbility newAbility = this._playerProfession.GetNewAbility(this._level);
      this._specialAbilities.Add(newAbility.KeyBind, newAbility);

      Console.WriteLine($"They have learned {newAbility.Name}");
      GameEngine.Pause(pauseDuration);

      Console.WriteLine("They gained the following stats:");

      foreach (var statUp in this._playerProfession.LevelUpStats)
      {
        Console.WriteLine($"{statUp.Key}+{statUp.Value}");
        GameEngine.Pause(pauseDuration);
        switch (statUp.Key)
        {
          case StatModifierType.Attack:
            this._strength += statUp.Value;
            continue;
          case StatModifierType.Defense:
            this._vitality += statUp.Value;
            continue;
          case StatModifierType.Magic:
            this._magic += statUp.Value;
            continue;
          case StatModifierType.MagiDefense:
            this._willPower += statUp.Value;
            continue;
          case StatModifierType.Agility:
            this._agility += statUp.Value;
            continue;
          case StatModifierType.Dexterity:
            this._dexterity += statUp.Value;
            continue;
          case StatModifierType.Health:
            this._maxHealth += statUp.Value;
            continue;
          case StatModifierType.Mana:
            this._maxMana += statUp.Value;
            continue;
        }
      }
    }

    public void PrintStats()
    {
      string nextLevelText = this._level < 8 ? this._levelChart[this._level + 1].ToString() : $"{this._name} has reached max level.";

      Console.WriteLine(this._name);
      Console.WriteLine($"Level: {this._level}");
      Console.Write($"Current Exp: {this._experience} - Next Level: ");
      Console.WriteLine(nextLevelText);

      this._mainHand.PrintEquipmentInfo();
      Console.WriteLine();

      this._offHand.PrintEquipmentInfo();
      Console.WriteLine();

      this._armor.PrintEquipmentInfo();
      Console.WriteLine();

      Console.WriteLine($"Strength: {this._strength}");
      Console.WriteLine($"Magic: {this._magic}");
      Console.WriteLine($"Vitality: {this._vitality}");
      Console.WriteLine($"Will Power: {this._willPower}");
      Console.WriteLine($"Agility: {this._agility}");
      Console.WriteLine($"Dexterity: {this._dexterity}");
    }

    public void PrintBattleInfo()
    {
      HashSet<string> buffDisplays = new HashSet<string>();

      Console.Write($"{this._name} HP: {this._currentHealth}/{this._maxHealth} MP: {this._currentMana}/{this._maxMana}");

      foreach (var buff in this._temporaryBuffs)
      {
        buffDisplays.Add(buff.BuffDisplay);
      }

      Console.ForegroundColor = ConsoleColor.Cyan;

      foreach (string buffDisplay in buffDisplays)
      {
        Console.Write($" {buffDisplay}");
      }

      Console.WriteLine();
      Console.ForegroundColor = ConsoleColor.White;
    }
  }

  class EnemyCharacter : Character
  {
    private int _modelIndex;
    private int _experienceReward;
    private int _goldReward;
    private Dictionary<EnemyAbility, int> _enemyBehaviour;


    public int ExperienceReward { get => this._experienceReward; }
    public int GoldReward { get => this._goldReward; }
    public int ModelIndex { get => this._modelIndex; }
    public Dictionary<EnemyAbility, int> EnemyBehaviour { get => this._enemyBehaviour; }

    public EnemyCharacter
      (string name, int maxHealth, int strength, int vitality, int magic, int will, int agility, int dexterity, int experienceReward, int goldReward, HashSet<Element> resistances, HashSet<Element> weaknesses, Dictionary<EnemyAbility, int> enemyBehaviour, int model) : base
      (name, maxHealth, strength, vitality, magic, will, agility, dexterity, resistances, weaknesses)
    {
      this._experienceReward = experienceReward;
      this._goldReward = goldReward;
      this._enemyBehaviour = enemyBehaviour;
      this._modelIndex = model;
    }

    public void PrintModel()
    {
      Console.WriteLine(GameData.EnemyModels[this.ModelIndex]);
    }

    public void PrintBuffs()
    {
      HashSet<string> buffDisplays = new HashSet<string>();

      foreach (var buff in this._temporaryBuffs)
      {
        buffDisplays.Add(buff.BuffDisplay);
      }

      Console.ForegroundColor = ConsoleColor.Cyan;

      foreach (string buffDisplay in buffDisplays)
      {
        Console.Write($" {buffDisplay}");
      }

      Console.WriteLine();
      Console.ForegroundColor = ConsoleColor.White;
    }
  }
}

