using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RPGGame
{
  static class Battle
  {
    public static GameDataBase GameData = GameEngine.GameData;
    private static bool _initializedNewTurn = false;
    private static Dictionary<ConsoleKey, PlayerCharacter> _battleParty = new Dictionary<ConsoleKey, PlayerCharacter>();
    private static EnemyCharacter _currentEnemy;
    private static Character _currentCharacterTurn;
    private static List<Character> _characterTurns = new List<Character>();
    private static List<Character> _targets = new List<Character>();
    private static List<int> _actionEffectValues = new List<int>();
    private static List<TemporaryBuff> _temporaryBuffValues = new List<TemporaryBuff>();
    private static Random _rng = new Random();

    public static void StartBattle()
    {
      ResetData();
      _currentEnemy.PrintModel();
      Console.WriteLine($"A {_currentEnemy.Name} has appeared!");
      GameEngine.Pause(1600);
      StartNewRound();
    }

    private static EnemyCharacter GetNewEnemy()
    {
      List<EnemyCharacter> possibleEnemies = new List<EnemyCharacter>();

      switch (GameEngine.Difficulty)
      {
        case Difficulty.Easy:
          possibleEnemies.AddRange(GameData.EnemyCharacters[Difficulty.Easy]);
          break;
        case Difficulty.Average:
          possibleEnemies.AddRange(GameData.EnemyCharacters[Difficulty.Average]);
          break;
        case Difficulty.Hard:
          possibleEnemies.AddRange(GameData.EnemyCharacters[Difficulty.Hard]);
          break;
        case Difficulty.VeryHard:
          possibleEnemies.AddRange(GameData.EnemyCharacters[Difficulty.VeryHard]);
          break;
      }

      int maxIndex = possibleEnemies.Count;

      return possibleEnemies[_rng.Next(0, maxIndex)];
    }

    private static void ProcessBattleTurn()
    {
      if (!_initializedNewTurn)
      {
        _currentCharacterTurn.UpdateTemporaryBuffs();
        _targets.Clear();
        _actionEffectValues.Clear();
        _temporaryBuffValues.Clear();
        _currentCharacterTurn.IsDefending = false;
        _initializedNewTurn = true;
      }

      Console.Clear();
      _currentEnemy.PrintModel();
      _currentEnemy.PrintBuffs();
      Console.WriteLine();

      PreventMultipleKeyPresses();

      GameEngine.ColorText(ConsoleColor.Blue, $"It is {_currentCharacterTurn.Name}'s turn.");

      if (_currentCharacterTurn == _currentEnemy)
      {
        ProcessEnemyTurn();
        return;
      }

      GameEngine.ColorText(ConsoleColor.Magenta, $"[A] ", false);
      Console.WriteLine("Attack");
      GameEngine.ColorText(ConsoleColor.Magenta, $"[D] ", false);
      Console.WriteLine("Defend");
      GameEngine.ColorText(ConsoleColor.Magenta, $"[S] ", false);
      Console.WriteLine("Use Special Ability");
      GameEngine.ColorText(ConsoleColor.Magenta, $"[I] ", false);
      Console.WriteLine("Use Item");
      Console.WriteLine();

      foreach (var character in _battleParty.Values)
      {
        character.PrintBattleInfo();
      }

      Console.WriteLine();
      Console.WriteLine("What is your command?");

      while (GameEngine.CurrentGameState == GameState.Battle)
      {
        switch (Console.ReadKey(true).Key)
        {
          case ConsoleKey.A:
            UseAbility();
            break;
          case ConsoleKey.D:
            Defend();
            break;
          case ConsoleKey.S:
            ShowAbilitiesList();
            break;
          case ConsoleKey.I:
            ShowItemList();
            break;
          default:
            continue;
        }
      }
    }

    private static void Defend()
    {
      _currentCharacterTurn.IsDefending = true;

      Console.WriteLine($"{_currentCharacterTurn.Name} is defending.");

      GameEngine.Pause();
      NextCharacterTurn();
    }

    private static void ShowAbilitiesList()
    {
      Dictionary<ConsoleKey, PlayerAbility> abilities = (_currentCharacterTurn as PlayerCharacter).SpecialAbilities;

      Console.WriteLine($"{_currentCharacterTurn.Name}'s Abilities:");

      foreach (var ability in abilities.Values)
      {
        double cost = ability.PoolUsed == PoolUsed.HP ? Math.Ceiling(ability.Cost * _currentCharacterTurn.MaxHealth) : ability.Cost;

        GameEngine.ColorText(ConsoleColor.Magenta, $"[{GameEngine.ConsoleKeyToInt(ability.KeyBind)}] ", false);
        ability.PrintAbilityInfo(_currentCharacterTurn as PlayerCharacter);
        Console.WriteLine();
      }

      Console.WriteLine();
      GameEngine.ColorText(ConsoleColor.Magenta, $"[Esc] ", false);
      Console.WriteLine("Return to previous menu");

      while (GameEngine.CurrentGameState == GameState.Battle)
      {
        ConsoleKey keyPressed = Console.ReadKey(true).Key;

        if (keyPressed == ConsoleKey.Escape)
        {
          ProcessBattleTurn();
          return;
        }

        if (abilities.ContainsKey(keyPressed))
        {
          PlayerAbility abilityToUse = abilities[keyPressed];
          int cost = abilityToUse.GetAbilityCost(_currentCharacterTurn as PlayerCharacter);

          if (abilityToUse.AbilityCanBeUsed(cost, _currentCharacterTurn as PlayerCharacter))
          {
            UseAbility(keyPressed);
          }
        }
      }
    }

    private static void UseAbility(ConsoleKey abilityKey = ConsoleKey.D0)
    {
      PlayerAbility ability;

      if (abilityKey == ConsoleKey.D0)
      {
        ability = GameData.PlayerAbilities[0];
      }
      else
      {
        ability = (_currentCharacterTurn as PlayerCharacter).SpecialAbilities[abilityKey];
      }

      double abilityPower = ability.GetAbilityPower(_currentCharacterTurn);

      ConfirmTarget(ability);

      switch (ability.Effect)
      {
        case Effect.Buff:
          GetBuffAmount(ability, abilityPower);
          break;
        case Effect.DamageHp:
          GetDamage(abilityPower, ability.AttackType, ability.Element);
          break;
      }

      if (ability.PoolUsed == PoolUsed.HP)
      {
        _currentCharacterTurn.CurrentHealth -= ability.GetAbilityCost(_currentCharacterTurn as PlayerCharacter);
      }
      else
      {
        (_currentCharacterTurn as PlayerCharacter).CurrentMana -= ability.GetAbilityCost(_currentCharacterTurn as PlayerCharacter);
      }

      Console.WriteLine($"{_currentCharacterTurn.Name} {ability.ActionText}");

      GameEngine.Pause();
      CompleteAction(ability);
    }

    private static void ShowItemList()
    {
      Console.WriteLine("Item Inventory:");
      foreach (var item in PartyInfo.UsableItems)
      {
        GameEngine.ColorText(ConsoleColor.Magenta, $"[{GameEngine.ConsoleKeyToInt(item.Key.KeyBind)}] ", false);
        Console.WriteLine($"{item.Key.Name} x{item.Value} - {item.Key.Description}");
      }
      Console.ForegroundColor = ConsoleColor.Magenta;
      Console.Write("[Esc] ");
      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine("Return to previous menu");


      while (GameEngine.CurrentGameState == GameState.Battle)
      {
        ConsoleKey keyPressed = Console.ReadKey(true).Key;

        if (keyPressed == ConsoleKey.Escape)
        {
          ProcessBattleTurn();
          return;
        }

        if (GameData.Consumables.ContainsKey(keyPressed) && PartyInfo.UsableItems.ContainsKey(GameData.Consumables[keyPressed]))
        {
          UseItem(keyPressed);
        }
      }
    }

    private static void UseItem(ConsoleKey keyPressed)
    {
      Consumable itemUsed = GameData.Consumables[keyPressed];

      ConfirmTarget(itemUsed);
      PartyInfo.UsableItems[itemUsed]--;

      if (PartyInfo.UsableItems[itemUsed] <= 0)
      {
        PartyInfo.UsableItems.Remove(itemUsed);
      }

      CompleteAction(itemUsed);
    }

    private static void GetDamage(double power, AttackType attacktype, Element element)
    {
      if (_targets[0] == _currentEnemy)
      {
        if (_targets[0].Weaknesses.Contains(element))
        {
          power *= 1.5;
          GameEngine.Pause();
        }
        else if (_targets[0].Resistances.Contains(element))
        {
          power *= .5;
          GameEngine.Pause();
        }
      }

      int powerToInt = (int)Math.Ceiling(power);

      foreach (Character target in _targets)
      {
        int damageToCharacter = 0;

        switch (attacktype)
        {
          case AttackType.Physical:
            damageToCharacter = powerToInt - target.DefensePower;
            break;
          case AttackType.Magical:
            damageToCharacter = powerToInt - target.MagicDefense;
            break;
          case AttackType.Almighty:
            damageToCharacter = powerToInt;
            break;
        }

        if (target.IsDefending)
        {
          damageToCharacter = (int)Math.Ceiling(damageToCharacter * .5);
        }

        if (damageToCharacter <= 0)
        {
          damageToCharacter = 1;
        }

        int randomNumberMin = (int)Math.Floor(-(damageToCharacter * .15));
        int randomNumberMax = (int)Math.Ceiling((damageToCharacter * .20));
        damageToCharacter += _rng.Next(randomNumberMin, randomNumberMax);

        if (damageToCharacter <= 0)
        {
          damageToCharacter = 1;
        }

        if (Dodged(target))
        {
          damageToCharacter = 0;
        }

        _actionEffectValues.Add(damageToCharacter);
      }
    }

    private static void GetBuffAmount(SpecialAbility ability, double power)
    {
      foreach (var statToBuff in ability.StatModifiers)
      {
        switch (statToBuff.Key)
        {
          case StatModifierType.Attack:
            _temporaryBuffValues.Add(new TemporaryBuff(ability.Duration, new StatModifier(StatModifierType.Attack, (int)power * statToBuff.Value)));
            break;
          case StatModifierType.Defense:
            _temporaryBuffValues.Add(new TemporaryBuff(ability.Duration, new StatModifier(StatModifierType.Defense, (int)power * statToBuff.Value)));
            break;
          case StatModifierType.Magic:
            _temporaryBuffValues.Add(new TemporaryBuff(ability.Duration, new StatModifier(StatModifierType.Magic, (int)power * statToBuff.Value)));
            break;
          case StatModifierType.MagiDefense:
            _temporaryBuffValues.Add(new TemporaryBuff(ability.Duration, new StatModifier(StatModifierType.MagiDefense, (int)power * statToBuff.Value)));
            break;
          case StatModifierType.Agility:
            _temporaryBuffValues.Add(new TemporaryBuff(ability.Duration, new StatModifier(StatModifierType.Agility, (int)power * statToBuff.Value)));
            break;
          case StatModifierType.Dexterity:
            _temporaryBuffValues.Add(new TemporaryBuff(ability.Duration, new StatModifier(StatModifierType.Dexterity, (int)power * statToBuff.Value)));
            break;
        }
      }
    }

    private static bool Dodged(Character target)
    {
      int chanceToHit = 70;
      chanceToHit += _currentCharacterTurn.Dexterity - target.Agility;

      if (_rng.Next(0, 101) <= chanceToHit)
      {
        return false;
      }

      return true;
    }

    private static void ProcessEnemyTurn()
    {
      EnemyAbility abilityUsed = GetEnemyAbilityToUse();

      if (abilityUsed == null)
      {
        NextCharacterTurn();
        return;
      }

      double abilityPower = abilityUsed.GetAbilityPower(_currentEnemy);

      GetEnemyTarget(abilityUsed);
      GetDamage(abilityPower, abilityUsed.AttackType, abilityUsed.Element);

      Console.WriteLine($"{_currentEnemy.Name} {abilityUsed.ActionText}");

      GameEngine.Pause();

      CompleteAction(abilityUsed);
      NextCharacterTurn();
    }

    private static EnemyAbility GetEnemyAbilityToUse()
    {
      int chanceOfUsing = 0;
      int randomNumber = _rng.Next(0, 101);

      foreach (var ability in _currentEnemy.EnemyBehaviour)
      {
        chanceOfUsing += ability.Value;

        if (randomNumber <= chanceOfUsing)
        {
          return ability.Key;
        }
      }

      return null;
    }

    private static void GetEnemyTarget(EnemyAbility abilityUsed)
    {
      RemoveDeadMembers();

      switch (abilityUsed.TargetType)
      {
        case TargetType.Enemy:
          int randomTarget = _rng.Next(1, _battleParty.Count);
          _targets.Add(_battleParty[GameEngine.IntToConsoleKey(randomTarget)]);
          break;
        case TargetType.Self:
          _targets.Add(_currentEnemy);
          break;
        case TargetType.Party:
          _targets.AddRange(_battleParty.Values);
          break;
      }
    }

    public static void StartNewRound()
    {
      _characterTurns.Clear();
      _characterTurns.Add(_currentEnemy);
      _characterTurns.AddRange(GameData.PlayerCharacters);

      _characterTurns.Sort((character1, character2) =>
      {
        if (character1.Agility > character2.Agility)
        {
          return -1;
        }
        else
        {
          return 1;
        }
      });

      NextCharacterTurn();
    }

    private static void NextCharacterTurn()
    {
      CheckIfBattleOver();
      _initializedNewTurn = false;

      while (_characterTurns.Count > 0 && _characterTurns[0].CharacterStatus == CharacterStatus.Dead)
      {
        _characterTurns.RemoveAt(0);
      }

      if (_characterTurns.Count > 0)
      {
        _currentCharacterTurn = _characterTurns[0];
        _characterTurns.RemoveAt(0);

        ProcessBattleTurn();
      }
      else
      {
        StartNewRound();
      }
    }

    private static void ConfirmTarget(Consumable itemUsed)
    {
      switch (itemUsed.TargetType)
      {
        case TargetType.Ally:
          _targets.Add(PickTarget(itemUsed.TargetType));
          _actionEffectValues.Add(itemUsed.EffectValue);

          string targetText = _targets[0] == _currentCharacterTurn ? "themself" : _targets[0].Name;

          Console.WriteLine($"{_currentCharacterTurn.Name} used a {itemUsed.Name.ToLower()} on {targetText}.");
          break;
        case TargetType.Self:
          _targets.Add(_currentCharacterTurn);
          _actionEffectValues.Add(itemUsed.EffectValue);

          Console.WriteLine($"{_currentCharacterTurn.Name} used a {itemUsed.Name.ToLower()} on themself.");
          break;
        case TargetType.Enemy:
          _targets.Add(_currentEnemy);
          _actionEffectValues.Add(itemUsed.EffectValue);

          Console.WriteLine($"{_currentCharacterTurn.Name} used a {itemUsed.Name.ToLower()} on {_currentEnemy.Name}.");
          break;
        case TargetType.Party:
          foreach (var playerCharacter in _battleParty) { _targets.Add(playerCharacter.Value); _actionEffectValues.Add(itemUsed.EffectValue); }

          Console.WriteLine($"{_currentCharacterTurn.Name} used a {itemUsed.Name.ToLower()} on the party.");
          break;
      }
    }

    private static void ConfirmTarget(PlayerAbility ability)
    {
      switch (ability.TargetType)
      {
        case TargetType.Ally:
          _targets.Add(PickTarget(ability.TargetType));
          break;
        case TargetType.Self:
          _targets.Add(_currentCharacterTurn);
          break;
        case TargetType.Enemy:
          _targets.Add(_currentEnemy);
          break;
        case TargetType.Party:
          foreach (var playerCharacter in _battleParty) { _targets.Add(playerCharacter.Value); }
          break;
      }
    }

    private static Character PickTarget(TargetType targetType)
    {
      foreach (var character in _battleParty)
      {
        Console.WriteLine($"[{GameEngine.ConsoleKeyToInt(character.Key)}] {character.Value.Name} HP: {character.Value.CurrentHealth}/{character.Value.MaxHealth} MP: {character.Value.CurrentMana}/{character.Value.MaxMana}");
      }

      while (GameEngine.CurrentGameState == GameState.Battle)
      {
        ConsoleKey keyPressed = Console.ReadKey(true).Key;

        if (_battleParty.ContainsKey(keyPressed))
        {
          return _battleParty[keyPressed];
        }
      }

      return null;
    }

    private static void CompleteAction(Consumable itemUsed)
    {
      switch (itemUsed.SpecialEffect)
      {
        case Effect.HealHP:
          Effects.HealHP(_targets, itemUsed.EffectValue);
          break;
        case Effect.HealMP:
          Effects.HealMP(_targets[0] as PlayerCharacter, itemUsed.EffectValue);
          break;
      }

      NextCharacterTurn();
    }

    private static void CompleteAction(SpecialAbility ability)
    {
      switch (ability.Effect)
      {
        case Effect.HealHP:
          Effects.HealHP(_targets, (int)ability.GetAbilityPower(_currentCharacterTurn));
          break;
        case Effect.DamageHp:
          Effects.DamageHP(_targets, _actionEffectValues);
          break;
        case Effect.Buff:
          Effects.ModifyStats(_targets, _temporaryBuffValues);
          break;
      }

      RemoveDeadMembers();

      NextCharacterTurn();
    }

    private static void CheckIfBattleOver()
    {
      if (_battleParty.Count <= 0)
      {
        EndBattle(true);
        return;
      }

      if (_currentEnemy.CharacterStatus == CharacterStatus.Dead)
      {
        EndBattle(false);
        return;
      }
    }

    private static void EndBattle(bool playerVictorious)
    {
      Console.Clear();

      foreach (var player in _battleParty)
      {
        player.Value.RemoveAllBuffs();
      }

      if (playerVictorious)
      {
        // ascii art from http://www.asciiworld.com/-Death-Co-.html
        Console.WriteLine(@"
	
 _;~)                  (~;_
(   |                  |   )
 ~', ',    ,''~'',   ,' ,'~
     ', ','       ',' ,'
       ',: {'} {'} :,'
         ;   /^\   ;
          ~\  ~  /~
        ,' ,~~~~~, ',
      ,' ,' ;~~~; ', ',
    ,' ,'    '''    ', ',
  (~  ;               ;  ~)
   -;_)               (_;-
");
        Console.WriteLine("The party has been slain. Your journey is over...");

        GameEngine.Pause(3200);
        Console.Clear();
        GameEngine.StartGame();
      }
      else
      {
        Console.WriteLine("The party has emerged victorious!");

        PartyInfo.UpdateGameStats(_currentEnemy.ExperienceReward, _currentEnemy.GoldReward);
        AddRewards();
        GameEngine.Pause(1800);
        GameEngine.SwitchGameState(GameState.Menu);
      }
    }

    private static void AddRewards()
    {
      int experienceReward = (int)_currentEnemy.ExperienceReward / _battleParty.Count;

      Console.WriteLine($"The party gained {experienceReward} experience.");
      GameEngine.Pause();
      Console.WriteLine($"The party gained {_currentEnemy.GoldReward} gold.");
      GameEngine.Pause();


      foreach (var character in _battleParty.Values)
      {
        character.UpdateExperience(experienceReward);
      }

      PartyInfo.Gold += _currentEnemy.GoldReward;
    }

    private static void RemoveDeadMembers()
    {
      int partyMemberIndex = 1;

      _battleParty.Clear();

      foreach (PlayerCharacter playerCharacter in GameData.PlayerCharacters)
      {
        if (playerCharacter.CharacterStatus != CharacterStatus.Dead)
        {
          _battleParty.Add(GameEngine.IntToConsoleKey(partyMemberIndex++), playerCharacter);
        }
      }
    }

    private static void ResetData()
    {
      int partyMemberIndex = 1;

      _battleParty.Clear();

      foreach (PlayerCharacter playerCharacter in GameData.PlayerCharacters)
      {
        if (playerCharacter.CharacterStatus != CharacterStatus.Dead)
        {
          _battleParty.Add(GameEngine.IntToConsoleKey(partyMemberIndex++), playerCharacter);
        }
      }

      _currentEnemy = GetNewEnemy();
      _currentEnemy.CurrentHealth = _currentEnemy.MaxHealth;
      _currentEnemy.CharacterStatus = CharacterStatus.Alive;
    }

    private static void PreventMultipleKeyPresses()
    {
      // Thanks to CoolDadTx for this https://social.msdn.microsoft.com/Forums/vstudio/en-US/9da27ed7-1453-414c-b17f-b056b83f5a21/ignoringdiscarding-stacked-keyboard-inputs-after-a-threadsleep?forum=csharpgeneral
      while (Console.KeyAvailable)
      {
        Console.ReadKey(true);
      }
    }
  }
}
