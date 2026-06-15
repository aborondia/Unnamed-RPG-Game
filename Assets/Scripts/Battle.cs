using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

    public async void StartBattle()
    {
      ResetData();
      _currentEnemy.PrintModel();
      this.uiController.WriteLine($"A {_currentEnemy.Name} has appeared!");
      await this.gameEngine.Pause(1600);
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
      string keyPressed = String.Empty;

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

      this.uiController.WriteColorText(ConsoleColor.Blue, $"It is {_currentCharacterTurn.Name}'s turn.");

      if (_currentCharacterTurn == _currentEnemy)
      {
        ProcessEnemyTurn();
        return;
      }

      this.uiController.WriteColorText(ConsoleColor.Magenta, $"[A] ");
      this.uiController.Write("Attack");
      this.uiController.WriteColorText(ConsoleColor.Magenta, $"[D] ");
      this.uiController.Write("Defend");
      this.uiController.WriteColorText(ConsoleColor.Magenta, $"[S] ");
      this.uiController.Write("Use Special Ability");
      this.uiController.WriteColorText(ConsoleColor.Magenta, $"[I] ");
      this.uiController.Write("Use Item");
      this.uiController.WriteLine();

      foreach (var character in _battleParty.Values)
      {
        character.PrintBattleInfo();
      }

      this.uiController.WriteLine();
      this.uiController.WriteLine("What is your command?");

      await this.gameEngine.WaitForPlayerKeyPress(() =>
        {
          keyPressed = this.gameEngine.CurrentKeyPressed;

          switch (keyPressed)
          {
            case "a":
            case "d":
            case "s":
            case "i":
              return true;
          }

          return false;
        });

      switch (keyPressed)
      {
        case "a":
          await UseAbility();
          break;
        case "d":
          await Defend();
          break;
        case "s":
          ShowAbilitiesList();
          break;
        case "i":
          await ShowItemList();
          break;
      }
    }

    private async UniTask Defend()
    {
      _currentCharacterTurn.IsDefending = true;

      this.uiController.WriteLine($"{_currentCharacterTurn.Name} is defending.");

      await this.gameEngine.Pause();
      NextCharacterTurn();
    }

    private async void ShowAbilitiesList()
    {
      Dictionary<int, PlayerAbility> abilities = (_currentCharacterTurn as PlayerCharacter).SpecialAbilities;
      string keyPressed = String.Empty;
      Action actionToTake = null;
      int abilityKey;
      this.uiController.ClearText();
      this.uiController.WriteLine($"{_currentCharacterTurn.Name}'s Abilities:");

      foreach (var ability in abilities.Values)
      {
        double cost = ability.PoolUsed == PoolUsed.HP ? Math.Ceiling(ability.Cost * _currentCharacterTurn.MaxHealth) : ability.Cost;

        this.uiController.WriteColorText(ConsoleColor.Magenta, $"[{ability.KeyBind}]: ");
        ability.PrintAbilityInfo(_currentCharacterTurn as PlayerCharacter);
      }

      this.uiController.WriteColorText(ConsoleColor.Magenta, $"[Esc] ");
      this.uiController.Write("Return to previous menu");

      await this.gameEngine.WaitForPlayerKeyPress(() =>
      {
        keyPressed = this.gameEngine.CurrentKeyPressed;

        if (keyPressed == "escape")
        {
          actionToTake = () => ProcessBattleTurn();
          return true;
        }

        if (int.TryParse(this.gameEngine.CurrentKeyPressed, out abilityKey) && abilities.ContainsKey(abilityKey))
        {
          PlayerAbility abilityToUse = abilities[abilityKey];
          int cost = abilityToUse.GetAbilityCost(_currentCharacterTurn as PlayerCharacter);

          if (abilityToUse.AbilityCanBeUsed(cost, _currentCharacterTurn as PlayerCharacter))
          {
            actionToTake = async () => await UseAbility(abilityKey);
            return true;
          }
        }

        return false;
      });

      actionToTake.Invoke();
    }

    private async UniTask UseAbility(int abilityKey = 0)
    {
      PlayerAbility ability;

      if (abilityKey == 0)
      {
        ability = GameData.PlayerAbilities[0];
      }
      else
      {
        ability = (_currentCharacterTurn as PlayerCharacter).SpecialAbilities[abilityKey];
      }

      double abilityPower = ability.GetAbilityPower(_currentCharacterTurn);

      await ConfirmTarget(ability);

      if (this._targets.Count <= 0)
      {
        ProcessBattleTurn();
        return;
      }

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

    private async Task ShowItemList()
    {
      string keyPressed = String.Empty;

      this.uiController.ClearText();
      this.uiController.WriteLine("Item Inventory:");

      foreach (var item in this.partyInfo.UsableItems)
      {
        this.uiController.WriteColorText(ConsoleColor.Magenta, $"[{item.Key.KeyBind}] ");
        this.uiController.Write($"{item.Key.Name} x{item.Value} - {item.Key.Description}");
      }

      this.uiController.WriteColorText(ConsoleColor.Magenta, "[Esc] ");
      this.uiController.Write("Return to previous menu");

      await this.gameEngine.WaitForPlayerKeyPress(() =>
      {
        keyPressed = this.gameEngine.CurrentKeyPressed;

        if (keyPressed == "escape")
        {
          ProcessBattleTurn();
          return true;
        }

        if (int.TryParse(keyPressed, out int value) && GameData.Consumables.ContainsKey(value)
        && this.partyInfo.UsableItems.ContainsKey(GameData.Consumables[value]))
        {
          UseItem(value);
          return true;
        }

        return false;
      });
    }

    private async void UseItem(int keyPressed)
    {
      Consumable itemUsed = GameData.Consumables[keyPressed];

      await ConfirmTarget(itemUsed);

      if (this._targets.Count <= 0)
      {
        ProcessBattleTurn();
        return;
      }

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
        }
        else if (_targets[0].Resistances.Contains(element))
        {
          power *= .5;
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

    private async void ProcessEnemyTurn()
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

      await this.gameEngine.Pause();

      CompleteAction(abilityUsed);
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
      if (BattleIsOver())
      {
        return;
      }

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
          _targets.Add(await PickTarget());
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
      Character target;

      switch (ability.TargetType)
      {
        case TargetType.Ally:
          target = await PickTarget();
          break;
        case TargetType.Self:
          target = _currentCharacterTurn;
          break;
        case TargetType.Enemy:
          target = _currentEnemy;
          break;
        case TargetType.Party:
          foreach (var playerCharacter in _battleParty)
          {
            target = playerCharacter.Value;

            if (target != null)
            {
              this._targets.Add(target);
            }
          }
          return;
        default:
          return;
      }

      if (target != null)
      {
        this._targets.Add(target);
      }
    }

    private async UniTask<Character> PickTarget()
    {
      Character target = null;

      this.uiController.ClearText();

      foreach (var character in _battleParty)
      {
        this.uiController.WriteColorText(ConsoleColor.Magenta, $"[{character.Key}]");
        this.uiController.Write($" {character.Value.Name} HP: {character.Value.CurrentHealth}/{character.Value.MaxHealth} MP: {character.Value.CurrentMana}/{character.Value.MaxMana}");
      }

      this.uiController.WriteColorText(ConsoleColor.Magenta, "[Esc] ");
      this.uiController.Write("Return to previous menu");

      await this.gameEngine.WaitForPlayerKeyPress(() =>
          {
            string keyPressed = this.gameEngine.CurrentKeyPressed;

            if (keyPressed == "escape")
            {
              return true;
            }

            if (int.TryParse(keyPressed, out int result) && _battleParty.ContainsKey(result))
            {
              target = _battleParty[result];
              return true;
            }

            return false;
          });

      return target;
    }

    private async void CompleteAction(Consumable itemUsed)
    {
      switch (itemUsed.SpecialEffect)
      {
        case Effect.HealHP:
          await this.resolver.Resolve<Effects>().HealHP(new List<Character>(_targets), itemUsed.EffectValue);
          break;
        case Effect.HealMP:
          await this.resolver.Resolve<Effects>().HealMP(_targets[0] as PlayerCharacter, itemUsed.EffectValue);
          break;
      }

      NextCharacterTurn();
    }

    private async void CompleteAction(SpecialAbility ability)
    {
      switch (ability.Effect)
      {
        case Effect.HealHP:
          await this.resolver.Resolve<Effects>().HealHP(new List<Character>(_targets), (int)ability.GetAbilityPower(_currentCharacterTurn));
          break;
        case Effect.DamageHp:
          await this.resolver.Resolve<Effects>().DamageHP(new List<Character>(_targets), _actionEffectValues);
          break;
        case Effect.Buff:
          this.resolver.Resolve<Effects>().ModifyStats(new List<Character>(_targets), _temporaryBuffValues);
          break;
      }

      RemoveDeadMembers();
      NextCharacterTurn();
    }

    private bool BattleIsOver()
    {
      if (_battleParty.Count <= 0)
      {
        EndBattle(true);
        return true;
      }

      if (_currentEnemy.CharacterStatus == CharacterStatus.Dead)
      {
        EndBattle(false);
        return true;
      }

      return false;
    }

    private async void EndBattle(bool gameOver)
    {
      this.uiController.Clear();

      foreach (var player in _battleParty)
      {
        player.Value.RemoveAllBuffs();
      }

      if (gameOver)
      {
        this.uiController.DrawGameOverModel();
        this.uiController.WriteLine("The party has been slain. Your journey is over...");
        await this.gameEngine.Pause(3200);
        this.uiController.Clear();
        this.gameEngine.StartGame();
      }
      else
      {
        this.uiController.WriteLine("The party has emerged victorious!");

        await this.gameEngine.Pause();

        this.partyInfo.UpdateGameStats(_currentEnemy.ExperienceReward, _currentEnemy.GoldReward);
        await AddRewards();
        this.gameEngine.SwitchGameState(GameState.Menu);
      }
    }

    private async UniTask AddRewards()
    {
      int experienceReward = (int)_currentEnemy.ExperienceReward / _battleParty.Count;

      this.uiController.WriteLine($"The party gained {experienceReward} experience.");
      await this.gameEngine.Pause();
      this.uiController.WriteLine($"The party gained {_currentEnemy.GoldReward} gold.");
      await this.gameEngine.Pause();


      foreach (var character in _battleParty.Values)
      {
        await character.UpdateExperience(experienceReward);
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
  }
}
