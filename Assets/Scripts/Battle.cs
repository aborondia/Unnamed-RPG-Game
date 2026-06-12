using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace RPGGame
{
  public class Battle
  {
    [Inject] private IObjectResolver resolver;
    [Inject] private GameEngine gameEngine;
    [Inject] private UIController uiController;
    [Inject] private PartyInfo partyInfo;
    [Inject] private GameDataBase GameData;
    private bool _initializedNewTurn = false;
    private Dictionary<int, PlayerCharacter> _battleParty = new Dictionary<int, PlayerCharacter>();
    private EnemyCharacter _currentEnemy;
    private Character _currentCharacterTurn;
    private List<Character> _characterTurns = new List<Character>();
    private List<Character> _targets = new List<Character>();
    private List<int> _actionEffectValues = new List<int>();
    private List<TemporaryBuff> _temporaryBuffValues = new List<TemporaryBuff>();
    private System.Random _rng = new System.Random();

    public void StartBattle()
    {
      ResetData();
      _currentEnemy.PrintModel();
      this.uiController.WriteLine($"A {_currentEnemy.Name} has appeared!");
      this.gameEngine.Pause(1600);
      StartNewRound();
    }

    private EnemyCharacter GetNewEnemy()
    {
      List<EnemyCharacter> possibleEnemies = new List<EnemyCharacter>();

      switch (this.gameEngine.Difficulty)
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

    private async void ProcessBattleTurn()
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

      this.uiController.Clear();
      _currentEnemy.PrintModel();
      _currentEnemy.PrintBuffs();
      this.uiController.WriteLine();

      PreventMultipleKeyPresses();

      this.uiController.WriteColorText(ConsoleColor.Blue, $"It is {_currentCharacterTurn.Name}'s turn.");

      if (_currentCharacterTurn == _currentEnemy)
      {
        ProcessEnemyTurn();
        return;
      }

      this.uiController.WriteColorText(ConsoleColor.Magenta, $"[A] ", false);
      this.uiController.WriteLine("Attack");
      this.uiController.WriteColorText(ConsoleColor.Magenta, $"[D] ", false);
      this.uiController.WriteLine("Defend");
      this.uiController.WriteColorText(ConsoleColor.Magenta, $"[S] ", false);
      this.uiController.WriteLine("Use Special Ability");
      this.uiController.WriteColorText(ConsoleColor.Magenta, $"[I] ", false);
      this.uiController.WriteLine("Use Item");
      this.uiController.WriteLine();

      foreach (var character in _battleParty.Values)
      {
        character.PrintBattleInfo();
      }

      this.uiController.WriteLine();
      this.uiController.WriteLine("What is your command?");

      await this.gameEngine.WaitForPlayerKeyPress(() =>
        {
          switch (Console.ReadKey(true).Key)
          {
            case ConsoleKey.A:
              UseAbility();
              return true;
            case ConsoleKey.D:
              Defend();
              return true;
            case ConsoleKey.S:
              ShowAbilitiesList();
              return true;
            case ConsoleKey.I:
              ShowItemList();
              return true;
          }

          return false;
        });
    }

    private async void Defend()
    {
      _currentCharacterTurn.IsDefending = true;

      this.uiController.WriteLine($"{_currentCharacterTurn.Name} is defending.");

      await this.gameEngine.Pause();
      NextCharacterTurn();
    }

    private void ShowAbilitiesList()
    {
      Dictionary<ConsoleKey, PlayerAbility> abilities = (_currentCharacterTurn as PlayerCharacter).SpecialAbilities;

      this.uiController.WriteLine($"{_currentCharacterTurn.Name}'s Abilities:");

      foreach (var ability in abilities.Values)
      {
        double cost = ability.PoolUsed == PoolUsed.HP ? Math.Ceiling(ability.Cost * _currentCharacterTurn.MaxHealth) : ability.Cost;

        this.uiController.WriteColorText(ConsoleColor.Magenta, $"[{this.gameEngine.ConsoleKeyToInt(ability.KeyBind)}] ", false);
        ability.PrintAbilityInfo(_currentCharacterTurn as PlayerCharacter);
        this.uiController.WriteLine();
      }

      this.uiController.WriteLine();
      this.uiController.WriteColorText(ConsoleColor.Magenta, $"[Esc] ", false);
      this.uiController.WriteLine("Return to previous menu");

      while (this.gameEngine.CurrentGameState == GameState.Battle)
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

    private async void UseAbility(ConsoleKey abilityKey = ConsoleKey.D0)
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

      await ConfirmTarget(ability);

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

      this.uiController.WriteLine($"{_currentCharacterTurn.Name} {ability.ActionText}");

      await this.gameEngine.Pause();
      CompleteAction(ability);
    }

    private void ShowItemList()
    {
      this.uiController.WriteLine("Item Inventory:");
      foreach (var item in this.partyInfo.UsableItems)
      {
        this.uiController.WriteColorText(ConsoleColor.Magenta, $"[{this.gameEngine.ConsoleKeyToInt(item.Key.KeyBind)}] ", false);
        this.uiController.WriteLine($"{item.Key.Name} x{item.Value} - {item.Key.Description}");
      }
      Console.ForegroundColor = ConsoleColor.Magenta;
      this.uiController.Write("[Esc] ");
      Console.ForegroundColor = ConsoleColor.White;
      this.uiController.WriteLine("Return to previous menu");


      while (this.gameEngine.CurrentGameState == GameState.Battle)
      {
        ConsoleKey keyPressed = Console.ReadKey(true).Key;

        if (keyPressed == ConsoleKey.Escape)
        {
          ProcessBattleTurn();
          return;
        }

        if (GameData.Consumables.ContainsKey(keyPressed) && this.partyInfo.UsableItems.ContainsKey(GameData.Consumables[keyPressed]))
        {
          UseItem(keyPressed);
        }
      }
    }

    private void UseItem(ConsoleKey keyPressed)
    {
      Consumable itemUsed = GameData.Consumables[keyPressed];

      ConfirmTarget(itemUsed);
      this.partyInfo.UsableItems[itemUsed]--;

      if (this.partyInfo.UsableItems[itemUsed] <= 0)
      {
        this.partyInfo.UsableItems.Remove(itemUsed);
      }

      CompleteAction(itemUsed);
    }

    private void GetDamage(double power, AttackType attacktype, Element element)
    {
      if (_targets[0] == _currentEnemy)
      {
        if (_targets[0].Weaknesses.Contains(element))
        {
          power *= 1.5;
          this.gameEngine.Pause();
        }
        else if (_targets[0].Resistances.Contains(element))
        {
          power *= .5;
          this.gameEngine.Pause();
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

    private void GetBuffAmount(SpecialAbility ability, double power)
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

    private bool Dodged(Character target)
    {
      int chanceToHit = 70;
      chanceToHit += _currentCharacterTurn.Dexterity - target.Agility;

      if (_rng.Next(0, 101) <= chanceToHit)
      {
        return false;
      }

      return true;
    }

    private void ProcessEnemyTurn()
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

      this.uiController.WriteLine($"{_currentEnemy.Name} {abilityUsed.ActionText}");

      this.gameEngine.Pause();

      CompleteAction(abilityUsed);
      NextCharacterTurn();
    }

    private EnemyAbility GetEnemyAbilityToUse()
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

    private void GetEnemyTarget(EnemyAbility abilityUsed)
    {
      RemoveDeadMembers();

      switch (abilityUsed.TargetType)
      {
        case TargetType.Enemy:
          int randomTarget = _rng.Next(1, _battleParty.Count);
          _targets.Add(_battleParty[randomTarget]);
          break;
        case TargetType.Self:
          _targets.Add(_currentEnemy);
          break;
        case TargetType.Party:
          _targets.AddRange(_battleParty.Values);
          break;
      }
    }

    public void StartNewRound()
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

    private void NextCharacterTurn()
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

    private async UniTask ConfirmTarget(Consumable itemUsed)
    {
      switch (itemUsed.TargetType)
      {
        case TargetType.Ally:
          _targets.Add(await PickTarget(itemUsed.TargetType));
          _actionEffectValues.Add(itemUsed.EffectValue);

          string targetText = _targets[0] == _currentCharacterTurn ? "themself" : _targets[0].Name;

          this.uiController.WriteLine($"{_currentCharacterTurn.Name} used a {itemUsed.Name.ToLower()} on {targetText}.");
          break;
        case TargetType.Self:
          _targets.Add(_currentCharacterTurn);
          _actionEffectValues.Add(itemUsed.EffectValue);

          this.uiController.WriteLine($"{_currentCharacterTurn.Name} used a {itemUsed.Name.ToLower()} on themself.");
          break;
        case TargetType.Enemy:
          _targets.Add(_currentEnemy);
          _actionEffectValues.Add(itemUsed.EffectValue);

          this.uiController.WriteLine($"{_currentCharacterTurn.Name} used a {itemUsed.Name.ToLower()} on {_currentEnemy.Name}.");
          break;
        case TargetType.Party:
          foreach (var playerCharacter in _battleParty) { _targets.Add(playerCharacter.Value); _actionEffectValues.Add(itemUsed.EffectValue); }

          this.uiController.WriteLine($"{_currentCharacterTurn.Name} used a {itemUsed.Name.ToLower()} on the party.");
          break;
      }
    }

    private async UniTask ConfirmTarget(PlayerAbility ability)
    {
      switch (ability.TargetType)
      {
        case TargetType.Ally:
          _targets.Add(await PickTarget(ability.TargetType));
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

    private async UniTask<Character> PickTarget(TargetType targetType)
    {
      Character target = null;

      foreach (var character in _battleParty)
      {
        this.uiController.WriteLine($"[{character.Key}] {character.Value.Name} HP: {character.Value.CurrentHealth}/{character.Value.MaxHealth} MP: {character.Value.CurrentMana}/{character.Value.MaxMana}");
      }

      await this.gameEngine.WaitForPlayerKeyPress(() =>
          {
            string keyPressed = this.gameEngine.CurrentKeyPressed;

            if (int.TryParse(keyPressed, out int result) && _battleParty.ContainsKey(result))
            {
              target = _battleParty[result];
              return true;
            }

            return false;
          });

      return target;
    }

    private void CompleteAction(Consumable itemUsed)
    {
      switch (itemUsed.SpecialEffect)
      {
        case Effect.HealHP:
          this.resolver.Resolve<Effects>().HealHP(_targets, itemUsed.EffectValue);
          break;
        case Effect.HealMP:
          this.resolver.Resolve<Effects>().HealMP(_targets[0] as PlayerCharacter, itemUsed.EffectValue);
          break;
      }

      NextCharacterTurn();
    }

    private void CompleteAction(SpecialAbility ability)
    {
      switch (ability.Effect)
      {
        case Effect.HealHP:
          this.resolver.Resolve<Effects>().HealHP(_targets, (int)ability.GetAbilityPower(_currentCharacterTurn));
          break;
        case Effect.DamageHp:
          this.resolver.Resolve<Effects>().DamageHP(_targets, _actionEffectValues);
          break;
        case Effect.Buff:
          this.resolver.Resolve<Effects>().ModifyStats(_targets, _temporaryBuffValues);
          break;
      }

      RemoveDeadMembers();

      NextCharacterTurn();
    }

    private void CheckIfBattleOver()
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

    private void EndBattle(bool playerVictorious)
    {
      this.uiController.Clear();

      foreach (var player in _battleParty)
      {
        player.Value.RemoveAllBuffs();
      }

      if (playerVictorious)
      {
        // ascii art from http://www.asciiworld.com/-Death-Co-.html
        this.uiController.WriteLine(@"
	
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
        this.uiController.WriteLine("The party has been slain. Your journey is over...");

        this.gameEngine.Pause(3200);
        this.uiController.Clear();
        this.gameEngine.StartGame();
      }
      else
      {
        this.uiController.WriteLine("The party has emerged victorious!");

        this.partyInfo.UpdateGameStats(_currentEnemy.ExperienceReward, _currentEnemy.GoldReward);
        AddRewards();
        this.gameEngine.Pause(1800);
        this.gameEngine.SwitchGameState(GameState.Menu);
      }
    }

    private void AddRewards()
    {
      int experienceReward = (int)_currentEnemy.ExperienceReward / _battleParty.Count;

      this.uiController.WriteLine($"The party gained {experienceReward} experience.");
      this.gameEngine.Pause();
      this.uiController.WriteLine($"The party gained {_currentEnemy.GoldReward} gold.");
      this.gameEngine.Pause();


      foreach (var character in _battleParty.Values)
      {
        character.UpdateExperience(experienceReward);
      }

      this.partyInfo.Gold += _currentEnemy.GoldReward;
    }

    private void RemoveDeadMembers()
    {
      int partyMemberIndex = 1;

      _battleParty.Clear();

      foreach (PlayerCharacter playerCharacter in GameData.PlayerCharacters)
      {
        if (playerCharacter.CharacterStatus != CharacterStatus.Dead)
        {
          _battleParty.Add(partyMemberIndex++, playerCharacter);
        }
      }
    }

    private void ResetData()
    {
      int partyMemberIndex = 1;

      _battleParty.Clear();

      foreach (PlayerCharacter playerCharacter in GameData.PlayerCharacters)
      {
        if (playerCharacter.CharacterStatus != CharacterStatus.Dead)
        {
          _battleParty.Add(partyMemberIndex++, playerCharacter);
        }
      }

      _currentEnemy = GetNewEnemy();
      _currentEnemy.CurrentHealth = _currentEnemy.MaxHealth;
      _currentEnemy.CharacterStatus = CharacterStatus.Alive;
    }

    private void PreventMultipleKeyPresses()
    {
      // Thanks to CoolDadTx for this https://social.msdn.microsoft.com/Forums/vstudio/en-US/9da27ed7-1453-414c-b17f-b056b83f5a21/ignoringdiscarding-stacked-keyboard-inputs-after-a-threadsleep?forum=csharpgeneral
      while (Console.KeyAvailable)
      {
        Console.ReadKey(true);
      }
    }
  }
}
