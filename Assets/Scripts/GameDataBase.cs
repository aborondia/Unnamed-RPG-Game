using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace RPGGame
{
  public enum Element
  {
    None,
    Fire,
    Water,
    Wind,
    Earth,
    Light,
    Dark
  }
  public enum AttackType
  {
    Physical,
    Magical,
    Almighty,
  }
  public enum TargetType
  {
    Enemy,
    Ally,
    Self,
    Party
  }
  public class GameDataBase
  {
    [Inject] private IObjectResolver resolver;
    [Inject] private UIController uiController;
    [Inject] private PartyInfo partyInfo;
    private GameEngine gameEngine;
    private Dictionary<int, Consumable> _consumables;
    private List<PlayerProfession> _playerProfessions;
    private List<Equipment> _equipment;
    private List<PlayerCharacter> _playerCharacters;
    private List<PlayerAbility> _playerAbilities;
    private Dictionary<Difficulty, List<EnemyCharacter>> _enemyCharacters;
    private List<EnemyAbility> _enemyAbilities;
    public List<PlayerCharacter> PlayerCharacters { get => this._playerCharacters; }
    public List<PlayerProfession> PlayerProfessions { get => this._playerProfessions; }
    public List<PlayerAbility> PlayerAbilities { get => this._playerAbilities; }
    public Dictionary<Difficulty, List<EnemyCharacter>> EnemyCharacters { get => this._enemyCharacters; }
    public List<Equipment> Equipment { get => this._equipment; }
    public Dictionary<int, Consumable> Consumables { get => this._consumables; }
    private bool initialized = false;
    public bool Initialized => initialized;

    public GameDataBase()
    {
      this._playerProfessions = new List<PlayerProfession>();
      this._playerAbilities = new List<PlayerAbility>();
      this._consumables = new Dictionary<int, Consumable>();
      this._equipment = new List<Equipment>();
      this._playerCharacters = new List<PlayerCharacter>();
      this._enemyCharacters = new Dictionary<Difficulty, List<EnemyCharacter>>
      {
        [Difficulty.Easy] = new List<EnemyCharacter>(),
        [Difficulty.Average] = new List<EnemyCharacter>(),
        [Difficulty.Hard] = new List<EnemyCharacter>(),
        [Difficulty.VeryHard] = new List<EnemyCharacter>()
      };
      this._enemyAbilities = new List<EnemyAbility>();
    }

    public async UniTask InitializeData(bool standardParty)
    {
      this.gameEngine = this.resolver.Resolve<GameEngine>();
      this.partyInfo.ResetData();
      this.Consumables.Clear();
      this.PlayerAbilities.Clear();
      this.PlayerProfessions.Clear();
      this.Equipment.Clear();
      this.PlayerCharacters.Clear();
      this._enemyAbilities.Clear();

      InitializeConsumables();
      InitializePlayerAbilities();
      InitializePlayerProfessions();
      InitializeEquipment();
      await InitializePlayerCharacters(standardParty);
      InitializeEnemyAbilities();
      InitializeEnemyCharacters();
    }

    private async UniTask InitializePlayerCharacters(bool standardParty)
    {
      PlayerCharacter player1;
      PlayerCharacter player2;
      PlayerCharacter player3;
      PlayerCharacter player4;

      if (standardParty)
      {
        player1 = new PlayerCharacter(this.gameEngine, "Terra", _playerProfessions[2]);
        player2 = new PlayerCharacter(this.gameEngine, "Cyan", _playerProfessions[0]);
        player3 = new PlayerCharacter(this.gameEngine, "Locke", _playerProfessions[1]);
        player4 = new PlayerCharacter(this.gameEngine, "Celes", _playerProfessions[3]);
      }
      else
      {
        player1 = await CreateCharacter(1);
        player2 = await CreateCharacter(2);
        player3 = await CreateCharacter(3);
        player4 = await CreateCharacter(4);
      }

      this._playerCharacters.Add(player1);
      this.partyInfo.PartyMembers.Add(player1);
      this._playerCharacters.Add(player2);
      this.partyInfo.PartyMembers.Add(player2);
      this._playerCharacters.Add(player3);
      this.partyInfo.PartyMembers.Add(player3);
      this._playerCharacters.Add(player4);
      this.partyInfo.PartyMembers.Add(player4);
    }

    private void InitializePlayerAbilities()
    {
      Dictionary<StatModifierType, int> statModifiers = new Dictionary<StatModifierType, int>();

      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Attack", "attacked!", "", 0, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Enemy, Effect.DamageHp, PoolUsed.HP, 0, 0));

      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Tackle", "charged forward!", "Deal light physical damage", .2, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Enemy, Effect.DamageHp, PoolUsed.HP, .05, 1));
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Fire Slash", "enveloped their blade in flames and slashed!", "Deal light fire damage", .4, StatUsed.Strength, Element.Fire, AttackType.Magical, TargetType.Enemy, Effect.DamageHp, PoolUsed.MP, 3, 2));
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Ice Slash", "enveloped their blade in ice and slashed!", "Deal light ice damage", .4, StatUsed.Strength, Element.Water, AttackType.Magical, TargetType.Enemy, Effect.DamageHp, PoolUsed.MP, 3, 3));
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Take Down", "slammed the enemy into the ground!", "Deal medium physical damage", .8, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Enemy, Effect.DamageHp, PoolUsed.HP, .15, 4));

      statModifiers.Add(StatModifierType.Attack, 1);
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Power Charge", "focused their strength.", "Massively increase your own attack power for a single turn", 2.5, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Self, Effect.Buff, PoolUsed.MP, 6, 5, statModifiers, 1));
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "War Cry", "let loose a primal scream!", "Increase the attack power of all allies for 3 turns", .6, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Party, Effect.Buff, PoolUsed.MP, 10, 6, statModifiers, 3));

      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Ground Splitter", "swung down and split the earth beneath the enemy!", "Deal heavy earth damage", 1, StatUsed.Strength, Element.Earth, AttackType.Physical, TargetType.Enemy, Effect.DamageHp, PoolUsed.HP, .25, 7));
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Dimension Slash", "slashed with such force it split the fabric of time!", "Deal massive damage - Ignores defense", 1.5, StatUsed.Strength, Element.None, AttackType.Almighty, TargetType.Enemy, Effect.DamageHp, PoolUsed.HP, .30, 8));

      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Back Stab", "quietly snuck up behind the enemy and struck!", "Deal light physical damage", .5, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Enemy, Effect.DamageHp, PoolUsed.HP, .15, 1));

      statModifiers.Clear();
      statModifiers.Add(StatModifierType.Attack, 1);
      statModifiers.Add(StatModifierType.Magic, 1);
      statModifiers.Add(StatModifierType.Defense, -1);
      statModifiers.Add(StatModifierType.MagiDefense, -1);
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Taunt", "hurled several colourful insults towards the enemy", "greatly lower enemy defense, and increase their attack for 2 turns", .6, StatUsed.Dexterity, Element.None, AttackType.Magical, TargetType.Enemy, Effect.Buff, PoolUsed.MP, .2, 2, statModifiers, 2));

      statModifiers.Clear();
      statModifiers.Add(StatModifierType.Agility, 1);
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Fleet Foot", "danced around the battlefield", "Increase own agility for 3 turns", .5, StatUsed.Dexterity, Element.None, AttackType.Magical, TargetType.Self, Effect.Buff, PoolUsed.MP, 3, 3, statModifiers, 3));

      statModifiers.Clear();
      statModifiers.Add(StatModifierType.Dexterity, 1);
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Pinpoint Accuracy", "took a deep breath...", "Increase dexterity for 3 turns", .5, StatUsed.Dexterity, Element.None, AttackType.Magical, TargetType.Self, Effect.Buff, PoolUsed.MP, 3, 4, statModifiers, 3));

      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Sword Dance", "rapidly swuncg their blade at the enemy", "Deal moderate physical damage", 1.25, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Enemy, Effect.DamageHp, PoolUsed.HP, .20, 5));

      statModifiers.Clear();
      statModifiers.Add(StatModifierType.Dexterity, 1);
      statModifiers.Add(StatModifierType.Agility, 1);
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Encourage", "gave the party a pep talk!", "Raise the entire party's agility and dexterity for 3 turns", .5, StatUsed.Dexterity, Element.None, AttackType.Magical, TargetType.Party, Effect.Buff, PoolUsed.MP, 6, 6, statModifiers, 3));

      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Swallow Cut", "slashed the enemy faster than the eye can see!", "Deal massive wind damage", 2.5, StatUsed.Dexterity, Element.Wind, AttackType.Physical, TargetType.Enemy, Effect.DamageHp, PoolUsed.HP, .30, 7));

      statModifiers.Clear();
      statModifiers.Add(StatModifierType.Agility, 1);
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "One With the Shadows", "disappeared into the shadows...", "Become almost impossible to hit for 2 turns", 1000, StatUsed.Agility, Element.Dark, AttackType.Magical, TargetType.Self, Effect.Buff, PoolUsed.MP, 10, 8, statModifiers, 2));

      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Magic Missiles", "fired magical missiles at the enemy!", "Deal light magical damage", .4, StatUsed.Magic, Element.None, AttackType.Magical, TargetType.Enemy, Effect.DamageHp, PoolUsed.MP, 4, 1));
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Firebolt", "sent forth a fiery bolt of magic!", "Deal light fire damage", .5, StatUsed.Magic, Element.Fire, AttackType.Magical, TargetType.Enemy, Effect.DamageHp, PoolUsed.MP, 6, 2));
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Icebolt", "sent forth an icy bolt of magic!", "deal light water damage", .5, StatUsed.Magic, Element.Water, AttackType.Magical, TargetType.Enemy, Effect.DamageHp, PoolUsed.MP, 6, 3));
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Wind Shear", "sent forth slicing blades of wind! ", "deal light wind damage", .5, StatUsed.Magic, Element.Wind, AttackType.Magical, TargetType.Enemy, Effect.DamageHp, PoolUsed.MP, 6, 4));
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Stalagmite", "called forth sharp blades of earth from beneath!", "deal light earth damage", .5, StatUsed.Magic, Element.Earth, AttackType.Magical, TargetType.Enemy, Effect.DamageHp, PoolUsed.MP, 6, 5));
      statModifiers.Clear();
      statModifiers.Add(StatModifierType.Magic, 1);
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Mind Charge", "focused their mind...", "Massively increase magic for one turn", 3.5, StatUsed.Magic, Element.None, AttackType.Magical, TargetType.Self, Effect.Buff, PoolUsed.MP, 10, 6, statModifiers, 1));
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Dark Wave", "called forth a wave of chilling darkness!", "deal high dark damage", 1, StatUsed.Magic, Element.Dark, AttackType.Magical, TargetType.Enemy, Effect.DamageHp, PoolUsed.MP, 10, 7));
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Cataclysm", "summoned forth all the destructive elements of the earth!", "Deal insane magic damage", 5, StatUsed.Magic, Element.None, AttackType.Almighty, TargetType.Enemy, Effect.DamageHp, PoolUsed.MP, 30, 8));

      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Heal", "sent forth a healing light.", "Light healing on one ally", .5, StatUsed.Will, Element.Light, AttackType.Magical, TargetType.Ally, Effect.HealHP, PoolUsed.MP, 4, 1));
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Holy Smite", "focused their faith into their mace and struck!", "Deal average light damage", 1, StatUsed.Strength, Element.Light, AttackType.Physical, TargetType.Enemy, Effect.DamageHp, PoolUsed.HP, .15, 2));

      statModifiers.Clear();
      statModifiers.Add(StatModifierType.Defense, 1);
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Wall", "erected a magical wall around the party!", "Raise all allies defense for 3 turns", .5, StatUsed.Will, Element.Light, AttackType.Magical, TargetType.Party, Effect.Buff, PoolUsed.MP, 10, 3, statModifiers, 3));

      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "High Heal", "sent forth a brilliant healing light!", "Moderate healing on one ally", 1.5, StatUsed.Will, Element.Light, AttackType.Magical, TargetType.Ally, Effect.HealHP, PoolUsed.MP, 8, 4));

      statModifiers.Clear();
      statModifiers.Add(StatModifierType.MagiDefense, 1);
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Barrier", "erected a magical barrier around the party!", "Raise all allies magic defense for 3 turns", .5, StatUsed.Will, Element.Light, AttackType.Magical, TargetType.Party, Effect.Buff, PoolUsed.MP, 10, 5, statModifiers, 3));

      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Healing Wind", "enveloped the party in a soothing wind...", "Moderate healing on all allies", 1.5, StatUsed.Will, Element.Light, AttackType.Magical, TargetType.Party, Effect.HealHP, PoolUsed.MP, 15, 6));

      statModifiers.Clear();
      statModifiers.Add(StatModifierType.MagiDefense, 1);
      statModifiers.Add(StatModifierType.Defense, 1);
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Aegis", "erected a powerful barrier around the party", "Raise all allies defense and magic defense for 3 turns", .5, StatUsed.Will, Element.Light, AttackType.Magical, TargetType.Party, Effect.Buff, PoolUsed.MP, 20, 7, statModifiers, 3));

      statModifiers.Clear();
      statModifiers.Add(StatModifierType.Attack, -1);
      statModifiers.Add(StatModifierType.Defense, -1);
      statModifiers.Add(StatModifierType.Magic, -1);
      statModifiers.Add(StatModifierType.MagiDefense, -1);
      statModifiers.Add(StatModifierType.Agility, -1);
      statModifiers.Add(StatModifierType.Dexterity, -1);
      this._playerAbilities.Add(new PlayerAbility(this.gameEngine, "Debilitate", "sent debilitating waves toward the enemy!", "Reduce all enemy stats for 2 turns", .6, StatUsed.Will, Element.Light, AttackType.Magical, TargetType.Enemy, Effect.Buff, PoolUsed.MP, 25, 8, statModifiers, 2));
    }

    private void InitializePlayerProfessions()
    {
      this._playerProfessions.Add(new Warrior(this));
      this._playerProfessions.Add(new Rouge(this));
      this._playerProfessions.Add(new Wizard(this));
      this._playerProfessions.Add(new Cleric(this));

    }

    private void InitializeEnemyAbilities()
    {
      this._enemyAbilities.Add(new EnemyAbility(this.gameEngine, "Punch", "lashed out with its fist!", "", 0, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Enemy, Effect.DamageHp));
      this._enemyAbilities.Add(new EnemyAbility(this.gameEngine, "Goblin Punch", "pummeled furiously!", "", .2, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Enemy, Effect.DamageHp));

      this._enemyAbilities.Add(new EnemyAbility(this.gameEngine, "Stomp", "stomped down!", "", 0, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Enemy, Effect.DamageHp));
      this._enemyAbilities.Add(new EnemyAbility(this.gameEngine, "Rush", "rushed forward!", "", .5, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Enemy, Effect.DamageHp));
      this._enemyAbilities.Add(new EnemyAbility(this.gameEngine, "Tremor", "split the earth beneath the party!", "", .5, StatUsed.Strength, Element.Earth, AttackType.Physical, TargetType.Party, Effect.DamageHp));

      this._enemyAbilities.Add(new EnemyAbility(this.gameEngine, "Claw", "lashed out with its claw!", "", 0, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Enemy, Effect.DamageHp));
      this._enemyAbilities.Add(new EnemyAbility(this.gameEngine, "Rend", "gnashed its teeth!", "", 0, StatUsed.Strength, Element.None, AttackType.Almighty, TargetType.Enemy, Effect.DamageHp));
      this._enemyAbilities.Add(new EnemyAbility(this.gameEngine, "Fire Breath", "spewed a torrent of flames at the party!", "", 1, StatUsed.Magic, Element.Fire, AttackType.Magical, TargetType.Party, Effect.DamageHp));

      this._enemyAbilities.Add(new EnemyAbility(this.gameEngine, "Evil Beam", "sent a beam of pure darkness forth!", "", .4, StatUsed.Magic, Element.Dark, AttackType.Magical, TargetType.Enemy, Effect.DamageHp));
      this._enemyAbilities.Add(new EnemyAbility(this.gameEngine, "Lightning Fall", "called forth lightning from the skies!", "", 0, StatUsed.Strength, Element.Light, AttackType.Almighty, TargetType.Party, Effect.DamageHp));
      this._enemyAbilities.Add(new EnemyAbility(this.gameEngine, "Dark Charge", "is focusing its magical energy...", "", 1.5, StatUsed.Magic, Element.Dark, AttackType.Magical, TargetType.Self, Effect.Buff, new Dictionary<StatModifierType, int> { [StatModifierType.Attack] = 1, [StatModifierType.Magic] = 1 }, 1));
      this._enemyAbilities.Add(new EnemyAbility(this.gameEngine, "Darkness Falls", "enveloped the world in pure darkness... horrors sprang forth from the void!", "", 1.5, StatUsed.Magic, Element.Dark, AttackType.Magical, TargetType.Party, Effect.DamageHp));
    }

    private void InitializeEnemyCharacters()
    {
      Dictionary<EnemyAbility, int> enemyBehaviour = new Dictionary<EnemyAbility, int>();

      enemyBehaviour.Add(this._enemyAbilities[0], 60);
      enemyBehaviour.Add(this._enemyAbilities[1], 40);
      this._enemyCharacters[Difficulty.Easy].Add(new EnemyCharacter(Difficulty.Easy, this.gameEngine, "Goblin", 200, 30, 10, 2, 1, 10, 10, 80, 50, null, new HashSet<Element> { Element.Water }, enemyBehaviour, 0));

      enemyBehaviour = new Dictionary<EnemyAbility, int>();
      enemyBehaviour.Add(this._enemyAbilities[2], 50);
      enemyBehaviour.Add(this._enemyAbilities[3], 30);
      enemyBehaviour.Add(this._enemyAbilities[4], 20);
      this._enemyCharacters[Difficulty.Average].Add(new EnemyCharacter(Difficulty.Average, this.gameEngine, "Minotaur", 500, 50, 40, 10, 25, 15, 25, 150, 200, new HashSet<Element> { Element.Fire, Element.Earth }, new HashSet<Element> { Element.Wind, Element.Dark }, enemyBehaviour, 1));

      enemyBehaviour = new Dictionary<EnemyAbility, int>();
      enemyBehaviour.Add(this._enemyAbilities[5], 40);
      enemyBehaviour.Add(this._enemyAbilities[6], 30);
      enemyBehaviour.Add(this._enemyAbilities[7], 30);
      this._enemyCharacters[Difficulty.Hard].Add(new EnemyCharacter(Difficulty.Hard, this.gameEngine, "Dragon", 1000, 90, 70, 90, 70, 35, 60, 400, 400, new HashSet<Element> { Element.Fire, Element.Earth, Element.Water, Element.Wind }, new HashSet<Element> { Element.Light, Element.Dark }, enemyBehaviour, 2));

      enemyBehaviour = new Dictionary<EnemyAbility, int>();
      enemyBehaviour.Add(this._enemyAbilities[8], 40);
      enemyBehaviour.Add(this._enemyAbilities[9], 30);
      enemyBehaviour.Add(this._enemyAbilities[10], 20);
      enemyBehaviour.Add(this._enemyAbilities[11], 10);
      this._enemyCharacters[Difficulty.VeryHard].Add(new EnemyCharacter(Difficulty.VeryHard, this.gameEngine, "Demon Lord", 3000, 120, 110, 100, 80, 60, 90, 1000, 1500, new HashSet<Element> { Element.Dark }, new HashSet<Element> { Element.Light }, enemyBehaviour, 3));
    }

    private void InitializeConsumables()
    {
      GameEngine gameEngine = this.gameEngine;

      this._consumables.Add(1, new Consumable(gameEngine, "Potion", 10, "Heal one ally HP by 25", 25, TargetType.Ally, 1, Effect.HealHP));
      this.partyInfo.AddItem(this._consumables[1], 3);

      this._consumables.Add(3, new Consumable(gameEngine, "Hi-Potion", 30, "Heal one ally HP by 50", 50, TargetType.Ally, 3, Effect.HealHP));
      this._consumables.Add(5, new Consumable(gameEngine, "Mega-Potion", 80, "Fully heal one ally", 50, TargetType.Ally, 5, Effect.HealHP));
      this._consumables.Add(2, new Consumable(gameEngine, "Ether", 30, "Restore 15 MP", 50, TargetType.Ally, 2, Effect.HealMP));
      this.partyInfo.AddItem(this._consumables[2], 2);

      this._consumables.Add(4, new Consumable(gameEngine, "Hi-Ether", 70, "Restore 30 MP", 50, TargetType.Ally, 4, Effect.HealMP));
      this._consumables.Add(6, new Consumable(gameEngine, "Mega Ether", 150, "Fully restore MP", 50, TargetType.Ally, 6, Effect.HealMP));
    }

    private void InitializeEquipment()
    {
      GameEngine gameEngine = this.gameEngine;
      this._equipment.Add(new Equipment(gameEngine, 10, EquipmentType.MainHand, "Chipped Long Sword", this._playerProfessions[0], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 6) }));
      this._equipment.Add(new Equipment(gameEngine, 100, EquipmentType.MainHand, "Fine Long Sword", this._playerProfessions[0], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 15) }));
      this._equipment.Add(new Equipment(gameEngine, 500, EquipmentType.MainHand, "Magical Long Sword", this._playerProfessions[0], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 25) }));
      this._equipment.Add(new Equipment(gameEngine, 10, EquipmentType.OffHand, "Dented Small Shield", this._playerProfessions[0], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 3) }));
      this._equipment.Add(new Equipment(gameEngine, 80, EquipmentType.OffHand, "Kite Shield", this._playerProfessions[0], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 8) }));
      this._equipment.Add(new Equipment(gameEngine, 350, EquipmentType.OffHand, "Spiked Shield", this._playerProfessions[0], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 12), new StatModifier(StatModifierType.Attack, 5) }));
      this._equipment.Add(new Equipment(gameEngine, 10, EquipmentType.Armor, "Battered Chain Mail", this._playerProfessions[0], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 5) }));
      this._equipment.Add(new Equipment(gameEngine, 150, EquipmentType.Armor, "Scale Mail", this._playerProfessions[0], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 12) }));
      this._equipment.Add(new Equipment(gameEngine, 700, EquipmentType.Armor, "Plate Mail", this._playerProfessions[0], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 25) }));

      this._equipment.Add(new Equipment(gameEngine, 10, EquipmentType.MainHand, "Chipped Short Sword", this._playerProfessions[1], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 4) }));
      this._equipment.Add(new Equipment(gameEngine, 100, EquipmentType.MainHand, "Sharp Short Sword", this._playerProfessions[1], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 10) }));
      this._equipment.Add(new Equipment(gameEngine, 500, EquipmentType.MainHand, "Lethal Short Sword", this._playerProfessions[1], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 18), new StatModifier(StatModifierType.Dexterity, 6) }));
      this._equipment.Add(new Equipment(gameEngine, 10, EquipmentType.OffHand, "Tarnished Parrying Dagger", this._playerProfessions[1], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 1), new StatModifier(StatModifierType.Agility, 4) }));
      this._equipment.Add(new Equipment(gameEngine, 100, EquipmentType.OffHand, "Fine Parrying Dagger", this._playerProfessions[1], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 3), new StatModifier(StatModifierType.Agility, 8) }));
      this._equipment.Add(new Equipment(gameEngine, 400, EquipmentType.OffHand, "Magical Parrying Dagger", this._playerProfessions[1], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 5), new StatModifier(StatModifierType.Agility, 12) }));
      this._equipment.Add(new Equipment(gameEngine, 10, EquipmentType.Armor, "Ragged Clothes", this._playerProfessions[1], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 3) }));
      this._equipment.Add(new Equipment(gameEngine, 100, EquipmentType.Armor, "Reinforced Clothes", this._playerProfessions[1], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 9) }));
      this._equipment.Add(new Equipment(gameEngine, 600, EquipmentType.Armor, "Light Mail", this._playerProfessions[1], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 15) }));

      this._equipment.Add(new Equipment(gameEngine, 10, EquipmentType.MainHand, "Cracked Staff", this._playerProfessions[2], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 3) }));
      this._equipment.Add(new Equipment(gameEngine, 70, EquipmentType.MainHand, "Fine Staff", this._playerProfessions[2], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 10) }));
      this._equipment.Add(new Equipment(gameEngine, 450, EquipmentType.MainHand, "Magical Staff", this._playerProfessions[2], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 15), new StatModifier(StatModifierType.Magic, 5) }));
      this._equipment.Add(new Equipment(gameEngine, 10, EquipmentType.OffHand, "Plain Wand", this._playerProfessions[2], new List<StatModifier> { new StatModifier(StatModifierType.Magic, 1) }));
      this._equipment.Add(new Equipment(gameEngine, 150, EquipmentType.OffHand, "Magic Wand", this._playerProfessions[2], new List<StatModifier> { new StatModifier(StatModifierType.Magic, 6) }));
      this._equipment.Add(new Equipment(gameEngine, 500, EquipmentType.OffHand, "Sorcerer Wand", this._playerProfessions[2], new List<StatModifier> { new StatModifier(StatModifierType.Magic, 15) }));
      this._equipment.Add(new Equipment(gameEngine, 10, EquipmentType.Armor, "Ragged Robes", this._playerProfessions[2], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 1) }));
      this._equipment.Add(new Equipment(gameEngine, 100, EquipmentType.Armor, "Fine Robes", this._playerProfessions[2], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 6) }));
      this._equipment.Add(new Equipment(gameEngine, 600, EquipmentType.Armor, "Sorcerer Robes", this._playerProfessions[2], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 10), new StatModifier(StatModifierType.Magic, 5) }));


      this._equipment.Add(new Equipment(gameEngine, 10, EquipmentType.MainHand, "Warped Mace", this._playerProfessions[3], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 6) }));
      this._equipment.Add(new Equipment(gameEngine, 100, EquipmentType.MainHand, "Fine Mace", this._playerProfessions[3], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 12) }));
      this._equipment.Add(new Equipment(gameEngine, 450, EquipmentType.MainHand, "Holy Mace", this._playerProfessions[3], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 16), new StatModifier(StatModifierType.Will, 6) }));
      this._equipment.Add(new Equipment(gameEngine, 10, EquipmentType.OffHand, "Scratched Buckler", this._playerProfessions[3], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 2) }));
      this._equipment.Add(new Equipment(gameEngine, 100, EquipmentType.OffHand, "Fine Buckler", this._playerProfessions[3], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 6) }));
      this._equipment.Add(new Equipment(gameEngine, 400, EquipmentType.OffHand, "Imbued Buckler", this._playerProfessions[3], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 8), new StatModifier(StatModifierType.Will, 6) }));
      this._equipment.Add(new Equipment(gameEngine, 10, EquipmentType.Armor, "Ragged Vestment", this._playerProfessions[3], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 1), new StatModifier(StatModifierType.Will, 2) }));
      this._equipment.Add(new Equipment(gameEngine, 150, EquipmentType.Armor, "Holy Vestment", this._playerProfessions[3], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 8), new StatModifier(StatModifierType.Will, 6) }));
      this._equipment.Add(new Equipment(gameEngine, 600, EquipmentType.Armor, "Radiant Vestment", this._playerProfessions[3], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 12), new StatModifier(StatModifierType.Will, 10) }));
    }

    private async UniTask<PlayerCharacter> CreateCharacter(int playerNumber)
    {
      PlayerProfession chosenProfession = null;
      string characterName;

      this.uiController.WriteLine($"Please enter player {playerNumber}'s name:");

      characterName = await this.gameEngine.WaitForPlayerInput();

      foreach (var player in this._playerCharacters)
      {
        if (player.Name.ToLower() == characterName.ToLower())
        {
          this.uiController.WriteLine("There is already a player character with that name.");
          return await CreateCharacter(playerNumber);
        }
      }

      this.uiController.WriteLine($"Please choose player {playerNumber}'s class:");
      int keyBind = 1;
      Dictionary<string, PlayerProfession> availableProfessions = new Dictionary<string, PlayerProfession>();

      foreach (PlayerProfession playerProfession in this._playerProfessions)
      {
        availableProfessions.Add(keyBind.ToString(), playerProfession);
        this.uiController.WriteLine($"[{keyBind++}] {playerProfession.Name}");
      }

      await this.gameEngine.WaitForPlayerKeyPress(() =>
        {
          string keyPressed = this.gameEngine.CurrentKeyPressed;

          if (availableProfessions.ContainsKey(keyPressed))
          {
            chosenProfession = availableProfessions[keyPressed];
            return true;
          }

          return false;
        });

      return new PlayerCharacter(this.gameEngine, characterName, chosenProfession);
    }
  }
}
