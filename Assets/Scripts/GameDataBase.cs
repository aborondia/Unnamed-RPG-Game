using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
  enum Element
  {
    None,
    Fire,
    Water,
    Wind,
    Earth,
    Light,
    Dark
  }
  enum AttackType
  {
    Physical,
    Magical,
    Almighty,
  }
  enum TargetType
  {
    Enemy,
    Ally,
    Self,
    Party
  }
  class GameDataBase
  {
    private Dictionary<ConsoleKey, Consumable> _consumables;
    private List<PlayerProfession> _playerProfessions;
    private List<Equipment> _equipment;
    private List<PlayerCharacter> _playerCharacters;
    private List<PlayerAbility> _playerAbilities;
    private Dictionary<Difficulty, List<EnemyCharacter>> _enemyCharacters;
    private List<EnemyAbility> _enemyAbilities;
    private List<string> _enemyModels;

    public List<PlayerCharacter> PlayerCharacters { get => this._playerCharacters; }
    public List<PlayerProfession> PlayerProfessions { get => this._playerProfessions; }
    public List<PlayerAbility> PlayerAbilities { get => this._playerAbilities; }
    public Dictionary<Difficulty, List<EnemyCharacter>> EnemyCharacters { get => this._enemyCharacters; }
    public List<string> EnemyModels { get => this._enemyModels; }
    public List<Equipment> Equipment { get => this._equipment; }
    public Dictionary<ConsoleKey, Consumable> Consumables { get => this._consumables; }

    public GameDataBase()
    {
      this._playerProfessions = new List<PlayerProfession>();
      this._playerAbilities = new List<PlayerAbility>();
      this._consumables = new Dictionary<ConsoleKey, Consumable>();
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
      this._enemyModels = new List<string>();
    }

    public void InitializeData(bool standardParty)
    {
      PartyInfo.ResetData();
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
      InitializePlayerCharacters(standardParty);
      InitializeEnemyAbilities();
      InitializeEnemyCharacters();
    }

    private void InitializePlayerCharacters(bool standardParty)
    {
      PlayerCharacter player1 = standardParty ?
        new PlayerCharacter("Terra", _playerProfessions[2]) :
        CreateCharacter(1);
      this._playerCharacters.Add(player1);
      PartyInfo.PartyMembers.Add(player1);

      PlayerCharacter player2 = standardParty ?
        new PlayerCharacter("Cyan", _playerProfessions[0]) :
        CreateCharacter(2);
      this._playerCharacters.Add(player2);
      PartyInfo.PartyMembers.Add(player2);

      PlayerCharacter player3 = standardParty ?
        new PlayerCharacter("Locke", _playerProfessions[1]) :
        CreateCharacter(3);
      this._playerCharacters.Add(player3);
      PartyInfo.PartyMembers.Add(player3);

      PlayerCharacter player4 = standardParty ?
        new PlayerCharacter("Celes", _playerProfessions[3]) :
        CreateCharacter(4);
      this._playerCharacters.Add(player4);
      PartyInfo.PartyMembers.Add(player4);
    }

    private void InitializePlayerAbilities()
    {
      Dictionary<StatModifierType, int> statModifiers = new Dictionary<StatModifierType, int>();

      this._playerAbilities.Add(new PlayerAbility("Attack", "attacked!", "", 0, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Enemy, Effect.DamageHp, PoolUsed.HP, 0, ConsoleKey.D0));

      this._playerAbilities.Add(new PlayerAbility("Tackle", "charged forward!", "Deal light physical damage", .2, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Enemy, Effect.DamageHp, PoolUsed.HP, .05, ConsoleKey.D1));
      this._playerAbilities.Add(new PlayerAbility("Fire Slash", "enveloped their blade in flames and slashed!", "Deal light fire damage", .4, StatUsed.Strength, Element.Fire, AttackType.Magical, TargetType.Enemy, Effect.DamageHp, PoolUsed.MP, 3, ConsoleKey.D2));
      this._playerAbilities.Add(new PlayerAbility("Ice Slash", "enveloped their blade in ice and slashed!", "Deal light ice damage", .4, StatUsed.Strength, Element.Water, AttackType.Magical, TargetType.Enemy, Effect.DamageHp, PoolUsed.MP, 3, ConsoleKey.D3));
      this._playerAbilities.Add(new PlayerAbility("Take Down", "slammed the enemy into the ground!", "Deal medium physical damage", .8, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Enemy, Effect.DamageHp, PoolUsed.HP, .15, ConsoleKey.D4));

      statModifiers.Add(StatModifierType.Attack, 1);
      this._playerAbilities.Add(new PlayerAbility("Power Charge", "focused their strength.", "Massively increase your own attack power for a single turn", 2.5, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Self, Effect.Buff, PoolUsed.MP, 6, ConsoleKey.D5, statModifiers, 1));
      this._playerAbilities.Add(new PlayerAbility("War Cry", "let loose a primal scream!", "Increase the attack power of all allies for 3 turns", .6, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Party, Effect.Buff, PoolUsed.MP, 10, ConsoleKey.D6, statModifiers, 3));

      this._playerAbilities.Add(new PlayerAbility("Ground Splitter", "swung down and split the earth beneath the enemy!", "Deal heavy earth damage", 1, StatUsed.Strength, Element.Earth, AttackType.Physical, TargetType.Enemy, Effect.DamageHp, PoolUsed.HP, .25, ConsoleKey.D7));
      this._playerAbilities.Add(new PlayerAbility("Dimension Slash", "slashed with such force it split the fabric of time!", "Deal massive damage - Ignores defense", 1.5, StatUsed.Strength, Element.None, AttackType.Almighty, TargetType.Enemy, Effect.DamageHp, PoolUsed.HP, .30, ConsoleKey.D8));

      this._playerAbilities.Add(new PlayerAbility("Back Stab", "quietly snuck up behind the enemy and struck!", "Deal light physical damage", .5, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Enemy, Effect.DamageHp, PoolUsed.HP, .15, ConsoleKey.D1));

      statModifiers.Clear();
      statModifiers.Add(StatModifierType.Attack, 1);
      statModifiers.Add(StatModifierType.Magic, 1);
      statModifiers.Add(StatModifierType.Defense, -1);
      statModifiers.Add(StatModifierType.MagiDefense, -1);
      this._playerAbilities.Add(new PlayerAbility("Taunt", "hurled several colourful insults towards the enemy", "greatly lower enemy defense, and increase their attack for 2 turns", .6, StatUsed.Dexterity, Element.None, AttackType.Magical, TargetType.Enemy, Effect.Buff, PoolUsed.MP, .2, ConsoleKey.D2, statModifiers, 2));

      statModifiers.Clear();
      statModifiers.Add(StatModifierType.Agility, 1);
      this._playerAbilities.Add(new PlayerAbility("Fleet Foot", "danced around the battlefield", "Increase own agility for 3 turns", .5, StatUsed.Dexterity, Element.None, AttackType.Magical, TargetType.Self, Effect.Buff, PoolUsed.MP, 3, ConsoleKey.D3, statModifiers, 3));

      statModifiers.Clear();
      statModifiers.Add(StatModifierType.Dexterity, 1);
      this._playerAbilities.Add(new PlayerAbility("Pinpoint Accuracy", "took a deep breath...", "Increase dexterity for 3 turns", .5, StatUsed.Dexterity, Element.None, AttackType.Magical, TargetType.Self, Effect.Buff, PoolUsed.MP, 3, ConsoleKey.D4, statModifiers, 3));

      this._playerAbilities.Add(new PlayerAbility("Sword Dance", "rapidly swuncg their blade at the enemy", "Deal moderate physical damage", 1.25, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Enemy, Effect.DamageHp, PoolUsed.HP, .20, ConsoleKey.D5));

      statModifiers.Clear();
      statModifiers.Add(StatModifierType.Dexterity, 1);
      statModifiers.Add(StatModifierType.Agility, 1);
      this._playerAbilities.Add(new PlayerAbility("Encourage", "gave the party a pep talk!", "Raise the entire party's agility and dexterity for 3 turns", .5, StatUsed.Dexterity, Element.None, AttackType.Magical, TargetType.Party, Effect.Buff, PoolUsed.MP, 6, ConsoleKey.D6, statModifiers, 3));

      this._playerAbilities.Add(new PlayerAbility("Swallow Cut", "slashed the enemy faster than the eye can see!", "Deal massive wind damage", 2.5, StatUsed.Dexterity, Element.Wind, AttackType.Physical, TargetType.Enemy, Effect.DamageHp, PoolUsed.HP, .30, ConsoleKey.D7));

      statModifiers.Clear();
      statModifiers.Add(StatModifierType.Agility, 1);
      this._playerAbilities.Add(new PlayerAbility("One With the Shadows", "disappeared into the shadows...", "Become almost impossible to hit for 2 turns", 1000, StatUsed.Agility, Element.Dark, AttackType.Magical, TargetType.Self, Effect.Buff, PoolUsed.MP, 10, ConsoleKey.D8, statModifiers, 2));

      this._playerAbilities.Add(new PlayerAbility("Magic Missiles", "fired magical missiles at the enemy!", "Deal light magical damage", .4, StatUsed.Magic, Element.None, AttackType.Magical, TargetType.Enemy, Effect.DamageHp, PoolUsed.MP, 4, ConsoleKey.D1));
      this._playerAbilities.Add(new PlayerAbility("Firebolt", "sent forth a fiery bolt of magic!", "Deal light fire damage", .5, StatUsed.Magic, Element.Fire, AttackType.Magical, TargetType.Enemy, Effect.DamageHp, PoolUsed.MP, 6, ConsoleKey.D2));
      this._playerAbilities.Add(new PlayerAbility("Icebolt", "sent forth an icy bolt of magic!", "deal light water damage", .5, StatUsed.Magic, Element.Water, AttackType.Magical, TargetType.Enemy, Effect.DamageHp, PoolUsed.MP, 6, ConsoleKey.D3));
      this._playerAbilities.Add(new PlayerAbility("Wind Shear", "sent forth slicing blades of wind! ", "deal light wind damage", .5, StatUsed.Magic, Element.Wind, AttackType.Magical, TargetType.Enemy, Effect.DamageHp, PoolUsed.MP, 6, ConsoleKey.D4));
      this._playerAbilities.Add(new PlayerAbility("Stalagmite", "called forth sharp blades of earth from beneath!", "deal light earth damage", .5, StatUsed.Magic, Element.Earth, AttackType.Magical, TargetType.Enemy, Effect.DamageHp, PoolUsed.MP, 6, ConsoleKey.D5));
      statModifiers.Clear();
      statModifiers.Add(StatModifierType.Magic, 1);
      this._playerAbilities.Add(new PlayerAbility("Mind Charge", "focused their mind...", "Massively increase magic for one turn", 3.5, StatUsed.Magic, Element.None, AttackType.Magical, TargetType.Self, Effect.Buff, PoolUsed.MP, 10, ConsoleKey.D6, statModifiers, 1));
      this._playerAbilities.Add(new PlayerAbility("Dark Wave", "called forth a wave of chilling darkness!", "deal high dark damage", 1, StatUsed.Magic, Element.Dark, AttackType.Magical, TargetType.Enemy, Effect.DamageHp, PoolUsed.MP, 10, ConsoleKey.D7));
      this._playerAbilities.Add(new PlayerAbility("Cataclysm", "summoned forth all the destructive elements of the earth!", "Deal insane magic damage", 5, StatUsed.Magic, Element.None, AttackType.Almighty, TargetType.Enemy, Effect.DamageHp, PoolUsed.MP, 30, ConsoleKey.D8));

      this._playerAbilities.Add(new PlayerAbility("Heal", "sent forth a healing light.", "Light healing on one ally", .5, StatUsed.Will, Element.Light, AttackType.Magical, TargetType.Ally, Effect.HealHP, PoolUsed.MP, 4, ConsoleKey.D1));
      this._playerAbilities.Add(new PlayerAbility("Holy Smite", "focused their faith into their mace and struck!", "Deal average light damage", 1, StatUsed.Strength, Element.Light, AttackType.Physical, TargetType.Enemy, Effect.DamageHp, PoolUsed.HP, .15, ConsoleKey.D2));

      statModifiers.Clear();
      statModifiers.Add(StatModifierType.Defense, 1);
      this._playerAbilities.Add(new PlayerAbility("Wall", "erected a magical wall around the party!", "Raise all allies defense for 3 turns", .5, StatUsed.Will, Element.Light, AttackType.Magical, TargetType.Party, Effect.Buff, PoolUsed.MP, 10, ConsoleKey.D3, statModifiers, 3));

      this._playerAbilities.Add(new PlayerAbility("High Heal", "sent forth a brilliant healing light!", "Moderate healing on one ally", 1.5, StatUsed.Will, Element.Light, AttackType.Magical, TargetType.Ally, Effect.HealHP, PoolUsed.MP, 8, ConsoleKey.D4));

      statModifiers.Clear();
      statModifiers.Add(StatModifierType.MagiDefense, 1);
      this._playerAbilities.Add(new PlayerAbility("Barrier", "erected a magical barrier around the party!", "Raise all allies magic defense for 3 turns", .5, StatUsed.Will, Element.Light, AttackType.Magical, TargetType.Party, Effect.Buff, PoolUsed.MP, 10, ConsoleKey.D5, statModifiers, 3));

      this._playerAbilities.Add(new PlayerAbility("Healing Wind", "enveloped the party in a soothing wind...", "Moderate healing on all allies", 1.5, StatUsed.Will, Element.Light, AttackType.Magical, TargetType.Party, Effect.HealHP, PoolUsed.MP, .15, ConsoleKey.D6));

      statModifiers.Clear();
      statModifiers.Add(StatModifierType.MagiDefense, 1);
      statModifiers.Add(StatModifierType.Defense, 1);
      this._playerAbilities.Add(new PlayerAbility("Aegis", "erected a powerful barrier around the party", "Raise all allies defense and magic defense for 3 turns", .5, StatUsed.Will, Element.Light, AttackType.Magical, TargetType.Party, Effect.Buff, PoolUsed.MP, 20, ConsoleKey.D7, statModifiers, 3));

      statModifiers.Clear();
      statModifiers.Add(StatModifierType.Attack, -1);
      statModifiers.Add(StatModifierType.Defense, -1);
      statModifiers.Add(StatModifierType.Magic, -1);
      statModifiers.Add(StatModifierType.MagiDefense, -1);
      statModifiers.Add(StatModifierType.Agility, -1);
      statModifiers.Add(StatModifierType.Dexterity, -1);
      this._playerAbilities.Add(new PlayerAbility("Debilitate", "sent debilitating waves toward the enemy!", "Reduce all enemy stats for 2 turns", .6, StatUsed.Will, Element.Light, AttackType.Magical, TargetType.Enemy, Effect.Buff, PoolUsed.MP, 25, ConsoleKey.D8, statModifiers, 2));
    }

    private void InitializePlayerProfessions()
    {
      this._playerProfessions.Add(new Warrior());
      this._playerProfessions.Add(new Rouge());
      this._playerProfessions.Add(new Wizard());
      this._playerProfessions.Add(new Cleric());

    }

    private void InitializeEnemyAbilities()
    {
      this._enemyAbilities.Add(new EnemyAbility("Punch", "lashed out with its fist!", "", 0, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Enemy, Effect.DamageHp));
      this._enemyAbilities.Add(new EnemyAbility("Goblin Punch", "pummeled furiously!", "", .2, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Enemy, Effect.DamageHp));

      this._enemyAbilities.Add(new EnemyAbility("Stomp", "stomped down!", "", 0, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Enemy, Effect.DamageHp));
      this._enemyAbilities.Add(new EnemyAbility("Rush", "rushed forward!", "", .5, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Enemy, Effect.DamageHp));
      this._enemyAbilities.Add(new EnemyAbility("Tremor", "split the earth beneath the party!", "", .5, StatUsed.Strength, Element.Earth, AttackType.Physical, TargetType.Party, Effect.DamageHp));

      this._enemyAbilities.Add(new EnemyAbility("Claw", "lashed out with its claw!", "", 0, StatUsed.Strength, Element.None, AttackType.Physical, TargetType.Enemy, Effect.DamageHp));
      this._enemyAbilities.Add(new EnemyAbility("Rend", "gnashed its teeth!", "", 0, StatUsed.Strength, Element.None, AttackType.Almighty, TargetType.Enemy, Effect.DamageHp));
      this._enemyAbilities.Add(new EnemyAbility("Fire Breath", "spewed a torrent of flames at the party!", "", 1, StatUsed.Magic, Element.Fire, AttackType.Magical, TargetType.Party, Effect.DamageHp));

      this._enemyAbilities.Add(new EnemyAbility("Evil Beam", "sent a beam of pure darkness forth!", "", .4, StatUsed.Magic, Element.Dark, AttackType.Magical, TargetType.Enemy, Effect.DamageHp));
      this._enemyAbilities.Add(new EnemyAbility("Lightning Fall", "called forth lightning from the skies!", "", 0, StatUsed.Strength, Element.Light, AttackType.Almighty, TargetType.Party, Effect.DamageHp));
      this._enemyAbilities.Add(new EnemyAbility("Dark Charge", "is focusing its magical energy...", "", 1.5, StatUsed.Magic, Element.Dark, AttackType.Magical, TargetType.Self, Effect.Buff, new Dictionary<StatModifierType, int> { [StatModifierType.Attack] = 1, [StatModifierType.Magic] = 1 }, 1));
      this._enemyAbilities.Add(new EnemyAbility("Darkness Falls", "enveloped the world in pure darkness... horrors sprang forth from the void!", "", 1.5, StatUsed.Magic, Element.Dark, AttackType.Magical, TargetType.Party, Effect.DamageHp));
    }

    private void InitializeEnemyCharacters()
    {
      // All models in this method taken from https://www.asciiart.eu/
      Dictionary<EnemyAbility, int> enemyBehaviour = new Dictionary<EnemyAbility, int>();

      enemyBehaviour.Add(this._enemyAbilities[0], 60);
      enemyBehaviour.Add(this._enemyAbilities[1], 40);
      this._enemyModels.Add(@"
        .-"""".
       /       \
   __ /   .-.  .\
  /  `\  /   \/  \
  |  _ \/   .==.==.
  | (   \  /____\__\
   \ \      (_()(_()
    \ \            '---._
     \                   \_
  /\ |`       (__)________/
 /  \|     /\___/
|    \     \||VV
|     \     \|"""",
|      \     ______)
\       \  /`
jgs      \(
");
      this._enemyCharacters[Difficulty.Easy].Add(new EnemyCharacter("Goblin", 200, 30, 10, 2, 1, 10, 10, 80, 50, null, new HashSet<Element> { Element.Water }, enemyBehaviour, 0));

      enemyBehaviour = new Dictionary<EnemyAbility, int>();
      enemyBehaviour.Add(this._enemyAbilities[2], 50);
      enemyBehaviour.Add(this._enemyAbilities[3], 30);
      enemyBehaviour.Add(this._enemyAbilities[4], 20);
      this._enemyModels.Add(@"
                                             ,--,  ,.-.
               ,                   \,       '-,-`,'-.' | ._
              /|           \    ,   |\         }  )/  / `-,',
              [ ,          |\  /|   | |        /  \|  |/`  ,`
              | |       ,.`  `,` `, | |  _,...(   (      .',
              \  \  __ ,-` `  ,  , `/ |,'      Y     (   /_L\
               \  \_\,``,   ` , ,  /  |         )         _,/
                \  '  `  ,_ _`_,-,<._.<        /         /
                 ', `>.,`  `  `   ,., |_      |         /
                   \/`  `,   `   ,`  | /__,.-`    _,   `\
               -,-..\  _  \  `  /  ,  / `._) _,-\`       \
                \_,,.) /\    ` /  / ) (-,, ``    ,        |
               ,` )  | \_\       '-`  |  `(               \
              /  /```(   , --, ,' \   |`<`    ,            |
             /  /_,--`\   <\  V /> ,` )<_/)  | \      _____)
       ,-, ,`   `   (_,\ \    |   /) / __/  /   `----`
      (-, \           ) \ ('_.-._)/ /,`    /
      | /  `          `/ \\ V   V, /`     /
   ,--\(        ,     <_/`\\     ||      /
  (   ,``-     \/|         \-A.A-`|     /
 ,>,_ )_,..(    )\          -,,_-`  _--`
(_ \|`   _,/_  /  \_            ,--`
 \( `   <.,../`     `-.._   _,-`
");
      this._enemyCharacters[Difficulty.Average].Add(new EnemyCharacter("Minotaur", 500, 50, 40, 10, 25, 15, 25, 150, 200, new HashSet<Element> { Element.Fire, Element.Earth }, new HashSet<Element> { Element.Wind, Element.Dark }, enemyBehaviour, 1));

      enemyBehaviour = new Dictionary<EnemyAbility, int>();
      enemyBehaviour.Add(this._enemyAbilities[5], 40);
      enemyBehaviour.Add(this._enemyAbilities[6], 30);
      enemyBehaviour.Add(this._enemyAbilities[7], 30);
      this._enemyModels.Add(@"
                                             __----~~~~~~~~~~~------___
                                  .  .   ~~//====......          __--~ ~~
                  -.            \_|//     |||\\  ~~~~~~::::... /~
               ___-==_       _-~o~  \/    |||  \\            _/~~-
       __---~~~.==~||\=_    -_--~/_-~|-   |\\   \\        _/~
   _-~~     .=~    |  \\-_    '-~7  /-   /  ||    \      /
 .~       .~       |   \\ -_    /  /-   /   ||      \   /
/  ____  /         |     \\ ~-_/  /|- _/   .||       \ /
|~~    ~~|--~~~~--_ \     ~==-/   | \~--===~~        .\
         '         ~-|      /|    |-~\~~       __--~~
                     |-~~-_/ |    |   ~\_   _-~            /\
                          /  \     \__   \/~                \__
                      _--~ _/ | .-~~____--~-/                  ~~==.
                     ((->/~   '.|||' -_|    ~~-/ ,              . _||
                                -_     ~\      ~~---l__i__i__i--~~_/
                                _-~-__   ~)  \--______________--~~
                              //.-~~~-~_--~- |-------~~~~~~~~
                                     //.-~~~--\
");
      this._enemyCharacters[Difficulty.Hard].Add(new EnemyCharacter("Dragon", 1000, 90, 70, 90, 70, 35, 60, 400, 400, new HashSet<Element> { Element.Fire, Element.Earth, Element.Water, Element.Wind }, new HashSet<Element> { Element.Light, Element.Dark }, enemyBehaviour, 2));


      enemyBehaviour = new Dictionary<EnemyAbility, int>();
      enemyBehaviour.Add(this._enemyAbilities[8], 40);
      enemyBehaviour.Add(this._enemyAbilities[9], 30);
      enemyBehaviour.Add(this._enemyAbilities[10], 20);
      enemyBehaviour.Add(this._enemyAbilities[11], 10);
      this._enemyModels.Add(@"
                            ,-.                               
       ___,---.__          /'|`\          __,---,___          
    ,-'    \`    `-.____,-'  |  `-.____,-'    //    `-.       
  ,'        |           ~'\     /`~           |        `.      
 /      ___//              `. ,'          ,  , \___      \    
|    ,-'   `-.__   _         |        ,    __,-'   `-.    |    
|   /          /\_  `   .    |    ,      _/\          \   |   
\  |           \ \`-.___ \   |   / ___,-'/ /           |  /  
 \  \           | `._   `\\  |  //'   _,' |           /  /      
  `-.\         /'  _ `---'' , . ``---' _  `\         /,-'     
     ``       /     \    ,='/ \`=.    /     \       ''          
             |__   /|\_,--.,-.--,--._/|\   __|                  
             /  `./  \\`\ |  |  | /,//' \,'  \                  
eViL        /   /     ||--+--|--+-/-|     \   \                 
           |   |     /'\_\_\ | /_/_/`\     |   |                
            \   \__, \_     `~'     _/ .__/   /            
             `-._,-'   `-._______,-'   `-._,-'
");
      this._enemyCharacters[Difficulty.VeryHard].Add(new EnemyCharacter("Demon Lord", 3000, 120, 110, 100, 80, 60, 90, 1000, 1500, new HashSet<Element> { Element.Dark }, new HashSet<Element> { Element.Light }, enemyBehaviour, 3));
    }

    private void InitializeConsumables()
    {
      this._consumables.Add(ConsoleKey.D1, new Consumable("Potion", 10, "Heal one ally HP by 25", 25, TargetType.Ally, ConsoleKey.D1, Effect.HealHP));
      PartyInfo.AddItem(this._consumables[ConsoleKey.D1], 3);

      this._consumables.Add(ConsoleKey.D3, new Consumable("Hi-Potion", 30, "Heal one ally HP by 50", 50, TargetType.Ally, ConsoleKey.D3, Effect.HealHP));
      this._consumables.Add(ConsoleKey.D5, new Consumable("Mega-Potion", 80, "Fully heal one ally", 50, TargetType.Ally, ConsoleKey.D5, Effect.HealHP));
      this._consumables.Add(ConsoleKey.D2, new Consumable("Ether", 30, "Restore 15 MP", 50, TargetType.Ally, ConsoleKey.D2, Effect.HealMP));
      PartyInfo.AddItem(this._consumables[ConsoleKey.D2], 2);

      this._consumables.Add(ConsoleKey.D4, new Consumable("Hi-Ether", 70, "Restore 30 MP", 50, TargetType.Ally, ConsoleKey.D4, Effect.HealMP));
      this._consumables.Add(ConsoleKey.D6, new Consumable("Mega Ether", 150, "Fully restore MP", 50, TargetType.Ally, ConsoleKey.D6, Effect.HealMP));
    }

    private void InitializeEquipment()
    {
      this._equipment.Add(new Equipment(10, EquipmentType.MainHand, "Chipped Long Sword", this._playerProfessions[0], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 6) }));
      this._equipment.Add(new Equipment(100, EquipmentType.MainHand, "Fine Long Sword", this._playerProfessions[0], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 15) }));
      this._equipment.Add(new Equipment(500, EquipmentType.MainHand, "Magical Long Sword", this._playerProfessions[0], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 25) }));
      this._equipment.Add(new Equipment(10, EquipmentType.OffHand, "Dented Small Shield", this._playerProfessions[0], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 3) }));
      this._equipment.Add(new Equipment(80, EquipmentType.OffHand, "Kite Shield", this._playerProfessions[0], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 8) }));
      this._equipment.Add(new Equipment(350, EquipmentType.OffHand, "Spiked Shield", this._playerProfessions[0], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 12), new StatModifier(StatModifierType.Attack, 5) }));
      this._equipment.Add(new Equipment(10, EquipmentType.Armor, "Battered Chain Mail", this._playerProfessions[0], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 5) }));
      this._equipment.Add(new Equipment(150, EquipmentType.Armor, "Scale Mail", this._playerProfessions[0], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 12) }));
      this._equipment.Add(new Equipment(700, EquipmentType.Armor, "Plate Mail", this._playerProfessions[0], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 25) }));

      this._equipment.Add(new Equipment(10, EquipmentType.MainHand, "Chipped Short Sword", this._playerProfessions[1], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 4) }));
      this._equipment.Add(new Equipment(100, EquipmentType.MainHand, "Sharp Short Sword", this._playerProfessions[1], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 10) }));
      this._equipment.Add(new Equipment(500, EquipmentType.MainHand, "Lethal Short Sword", this._playerProfessions[1], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 18), new StatModifier(StatModifierType.Dexterity, 6) }));
      this._equipment.Add(new Equipment(10, EquipmentType.OffHand, "Tarnished Parrying Dagger", this._playerProfessions[1], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 1), new StatModifier(StatModifierType.Agility, 4) }));
      this._equipment.Add(new Equipment(100, EquipmentType.OffHand, "Fine Parrying Dagger", this._playerProfessions[1], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 3), new StatModifier(StatModifierType.Agility, 8) }));
      this._equipment.Add(new Equipment(400, EquipmentType.OffHand, "Magical Parrying Dagger", this._playerProfessions[1], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 5), new StatModifier(StatModifierType.Agility, 12) }));
      this._equipment.Add(new Equipment(10, EquipmentType.Armor, "Ragged Clothes", this._playerProfessions[1], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 3) }));
      this._equipment.Add(new Equipment(100, EquipmentType.Armor, "Reinforced Clothes", this._playerProfessions[1], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 9) }));
      this._equipment.Add(new Equipment(600, EquipmentType.Armor, "Light Mail", this._playerProfessions[1], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 15) }));

      this._equipment.Add(new Equipment(10, EquipmentType.MainHand, "Cracked Staff", this._playerProfessions[2], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 3) }));
      this._equipment.Add(new Equipment(70, EquipmentType.MainHand, "Fine Staff", this._playerProfessions[2], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 10) }));
      this._equipment.Add(new Equipment(450, EquipmentType.MainHand, "Magical Staff", this._playerProfessions[2], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 15), new StatModifier(StatModifierType.Magic, 5) }));
      this._equipment.Add(new Equipment(10, EquipmentType.OffHand, "Plain Wand", this._playerProfessions[2], new List<StatModifier> { new StatModifier(StatModifierType.Magic, 1) }));
      this._equipment.Add(new Equipment(150, EquipmentType.OffHand, "Magic Wand", this._playerProfessions[2], new List<StatModifier> { new StatModifier(StatModifierType.Magic, 6) }));
      this._equipment.Add(new Equipment(500, EquipmentType.OffHand, "Sorcerer Wand", this._playerProfessions[2], new List<StatModifier> { new StatModifier(StatModifierType.Magic, 15) }));
      this._equipment.Add(new Equipment(10, EquipmentType.Armor, "Ragged Robes", this._playerProfessions[2], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 1) }));
      this._equipment.Add(new Equipment(100, EquipmentType.Armor, "Fine Robes", this._playerProfessions[2], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 6) }));
      this._equipment.Add(new Equipment(600, EquipmentType.Armor, "Sorcerer Robes", this._playerProfessions[2], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 10), new StatModifier(StatModifierType.Magic, 5) }));


      this._equipment.Add(new Equipment(10, EquipmentType.MainHand, "Warped Mace", this._playerProfessions[3], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 6) }));
      this._equipment.Add(new Equipment(100, EquipmentType.MainHand, "Fine Mace", this._playerProfessions[3], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 12) }));
      this._equipment.Add(new Equipment(450, EquipmentType.MainHand, "Holy Mace", this._playerProfessions[3], new List<StatModifier> { new StatModifier(StatModifierType.Attack, 16), new StatModifier(StatModifierType.Will, 6) }));
      this._equipment.Add(new Equipment(10, EquipmentType.OffHand, "Scratched Buckler", this._playerProfessions[3], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 2) }));
      this._equipment.Add(new Equipment(100, EquipmentType.OffHand, "Fine Buckler", this._playerProfessions[3], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 6) }));
      this._equipment.Add(new Equipment(400, EquipmentType.OffHand, "Imbued Buckler", this._playerProfessions[3], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 8), new StatModifier(StatModifierType.Will, 6) }));
      this._equipment.Add(new Equipment(10, EquipmentType.Armor, "Ragged Vestment", this._playerProfessions[3], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 1), new StatModifier(StatModifierType.Will, 2) }));
      this._equipment.Add(new Equipment(150, EquipmentType.Armor, "Holy Vestment", this._playerProfessions[3], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 8), new StatModifier(StatModifierType.Will, 6) }));
      this._equipment.Add(new Equipment(600, EquipmentType.Armor, "Radiant Vestment", this._playerProfessions[3], new List<StatModifier> { new StatModifier(StatModifierType.Defense, 12), new StatModifier(StatModifierType.Will, 10) }));
    }

    private PlayerCharacter CreateCharacter(int playerNumber)
    {
      string name = "";
      PlayerProfession chosenProfession = null;

      while (string.IsNullOrEmpty(name))
      {
        Console.WriteLine($"Please enter player {playerNumber}'s name:");
        name = Console.ReadLine();

        foreach (var player in this._playerCharacters)
        {
          if (player.Name.ToLower() == name.ToLower())
          {
            name = "";
            Console.WriteLine("There is already a player character with that name.");
          }
        }
      }

      Console.WriteLine($"Please choose player {playerNumber}'s class:");
      int keyBind = 1;
      bool deciding = true;
      Dictionary<ConsoleKey, PlayerProfession> availableProfessions = new Dictionary<ConsoleKey, PlayerProfession>();

      foreach (PlayerProfession playerProfession in this._playerProfessions)
      {
        availableProfessions.Add(GameEngine.IntToConsoleKey(keyBind), playerProfession);
        Console.WriteLine($"[{keyBind++}] {playerProfession.Name}");
      }

      while (deciding)
      {
        ConsoleKey keyPressed = Console.ReadKey(true).Key;
        if (availableProfessions.ContainsKey(keyPressed))
        {
          chosenProfession = availableProfessions[keyPressed];
          deciding = false;
        }
      }

      return new PlayerCharacter(name, chosenProfession);
    }
  }
}
