using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
  class PlayerProfession
  {
    public static GameDataBase GameData = GameEngine.GameData;
    protected string _name;
    protected Dictionary<int, PlayerAbility> _learnableAbilities;
    protected Dictionary<StatModifierType, int> _levelUpStats = new Dictionary<StatModifierType, int>();
    protected int _startingMainHand;
    protected int _startingOffHand;
    protected int _startingArmor;
    protected int _baseHealth;
    protected int _baseMana;
    protected int _baseStrength;
    protected int _baseVitality;
    protected int _baseMagic;
    protected int _baseWill;
    protected int _baseAgility;
    protected int _baseDexterity;

    public string Name { get => this._name; }
    public Dictionary<StatModifierType, int> LevelUpStats { get => this._levelUpStats; }
    public int StartingMainHand { get => this._startingMainHand; }
    public int StartingOffHand { get => this._startingOffHand; }
    public int StartingArmor { get => this._startingArmor; }
    public int BaseHealth { get => this._baseHealth; }
    public int BaseMana { get => this._baseMana; }
    public int BaseStrength { get => this._baseStrength; }
    public int BaseVitality { get => this._baseVitality; }
    public int BaseMagic { get => this._baseMagic; }
    public int BaseWill { get => this._baseWill; }
    public int BaseAgility { get => this._baseAgility; }
    public int BaseDexterity { get => this._baseDexterity; }

    public PlayerProfession()
    {
      this._learnableAbilities = new Dictionary<int, PlayerAbility>();
    }

    public PlayerAbility GetNewAbility(int id)
    {
      return this._learnableAbilities[id];
    }

    protected void PopulateLevelUpStats()
    {
      this._levelUpStats.Add(StatModifierType.Health, (int)(_baseHealth * .25));
      this._levelUpStats.Add(StatModifierType.Mana, (int)(_baseMana * .25));
      this._levelUpStats.Add(StatModifierType.Attack, (int)(_baseStrength * .25));
      this._levelUpStats.Add(StatModifierType.Defense, (int)(_baseVitality * .25));
      this._levelUpStats.Add(StatModifierType.Magic, (int)(_baseMagic * .25));
      this._levelUpStats.Add(StatModifierType.MagiDefense, (int)(_baseWill * .25));
      this._levelUpStats.Add(StatModifierType.Agility, (int)(_baseAgility * .25));
      this._levelUpStats.Add(StatModifierType.Dexterity, (int)(_baseDexterity * .25));
    }
  }

  class Warrior : PlayerProfession
  {

    public Dictionary<int, PlayerAbility> LearnableAbilities { get => this._learnableAbilities; }
    public Dictionary<StatModifierType, int> LevelUpStats { get => this._levelUpStats; }
    public Warrior()
    {
      this._name = "Warrior";
      this._startingMainHand = 0;
      this._startingOffHand = 3;
      this._startingArmor = 6;
      this._baseHealth = 50;
      this._baseMana = 10;
      this._baseStrength = 20;
      this._baseVitality = 15;
      this._baseMagic = 0;
      this._baseWill = 5;
      this._baseAgility = 8;
      this._baseDexterity = 8;
      this._learnableAbilities.Add(1, GameData.PlayerAbilities[1]);
      this._learnableAbilities.Add(2, GameData.PlayerAbilities[2]);
      this._learnableAbilities.Add(3, GameData.PlayerAbilities[3]);
      this._learnableAbilities.Add(4, GameData.PlayerAbilities[4]);
      this._learnableAbilities.Add(5, GameData.PlayerAbilities[5]);
      this._learnableAbilities.Add(6, GameData.PlayerAbilities[6]);
      this._learnableAbilities.Add(7, GameData.PlayerAbilities[7]);
      this._learnableAbilities.Add(8, GameData.PlayerAbilities[8]);
      PopulateLevelUpStats();
    }
  }

  class Rouge : PlayerProfession
  {
    public Dictionary<int, PlayerAbility> LearnableAbilities { get => this._learnableAbilities; }
    public Rouge()
    {
      this._name = "Rouge";
      this._startingMainHand = 9;
      this._startingOffHand = 12;
      this._startingArmor = 15;
      this._baseHealth = 40;
      this._baseMana = 10;
      this._baseStrength = 15;
      this._baseVitality = 10;
      this._baseMagic = 0;
      this._baseWill = 8;
      this._baseAgility = 20;
      this._baseDexterity = 20;
      this._learnableAbilities.Add(1, GameData.PlayerAbilities[9]);
      this._learnableAbilities.Add(2, GameData.PlayerAbilities[10]);
      this._learnableAbilities.Add(3, GameData.PlayerAbilities[11]);
      this._learnableAbilities.Add(4, GameData.PlayerAbilities[12]);
      this._learnableAbilities.Add(5, GameData.PlayerAbilities[13]);
      this._learnableAbilities.Add(6, GameData.PlayerAbilities[14]);
      this._learnableAbilities.Add(7, GameData.PlayerAbilities[15]);
      this._learnableAbilities.Add(8, GameData.PlayerAbilities[16]);
      PopulateLevelUpStats();
    }
  }

  class Wizard : PlayerProfession
  {
    public Dictionary<int, PlayerAbility> LearnableAbilities { get => this._learnableAbilities; }
    public Wizard()
    {
      this._name = "Wizard";
      this._startingMainHand = 18;
      this._startingOffHand = 21;
      this._startingArmor = 24;
      this._baseHealth = 30;
      this._baseMana = 30;
      this._baseStrength = 5;
      this._baseVitality = 8;
      this._baseMagic = 25;
      this._baseWill = 15;
      this._baseAgility = 10;
      this._baseDexterity = 12;
      this._learnableAbilities.Add(1, GameData.PlayerAbilities[17]);
      this._learnableAbilities.Add(2, GameData.PlayerAbilities[18]);
      this._learnableAbilities.Add(3, GameData.PlayerAbilities[19]);
      this._learnableAbilities.Add(4, GameData.PlayerAbilities[20]);
      this._learnableAbilities.Add(5, GameData.PlayerAbilities[21]);
      this._learnableAbilities.Add(6, GameData.PlayerAbilities[22]);
      this._learnableAbilities.Add(7, GameData.PlayerAbilities[23]);
      this._learnableAbilities.Add(8, GameData.PlayerAbilities[24]);
      PopulateLevelUpStats();
    }
  }

  class Cleric : PlayerProfession
  {
    public Dictionary<int, PlayerAbility> LearnableAbilities { get => this._learnableAbilities; }
    public Cleric()
    {
      this._name = "Cleric";
      this._startingMainHand = 27;
      this._startingOffHand = 30;
      this._startingArmor = 33;
      this._baseHealth = 40;
      this._baseMana = 25;
      this._baseStrength = 12;
      this._baseVitality = 12;
      this._baseMagic = 15;
      this._baseWill = 25;
      this._baseAgility = 10;
      this._baseDexterity = 10;
      this._learnableAbilities.Add(1, GameData.PlayerAbilities[25]);
      this._learnableAbilities.Add(2, GameData.PlayerAbilities[26]);
      this._learnableAbilities.Add(3, GameData.PlayerAbilities[27]);
      this._learnableAbilities.Add(4, GameData.PlayerAbilities[28]);
      this._learnableAbilities.Add(5, GameData.PlayerAbilities[29]);
      this._learnableAbilities.Add(6, GameData.PlayerAbilities[30]);
      this._learnableAbilities.Add(7, GameData.PlayerAbilities[31]);
      this._learnableAbilities.Add(8, GameData.PlayerAbilities[32]);
      PopulateLevelUpStats();
    }
  }
}
