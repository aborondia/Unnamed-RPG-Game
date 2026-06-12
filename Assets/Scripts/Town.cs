using System.Collections.Generic;
using VContainer;

namespace RPGGame
{
  public class Town
  {
    [Inject] private GameEngine gameEngine;
    [Inject] private UIController uiController;
    [Inject] private PartyInfo partyInfo;
    [Inject] private GameDataBase GameData;
    
    public void StartTown()
    {
      TownMenu();
    }

    private async void TownMenu()
    {
      this.uiController.WriteLine("Where would you like to go?");
      this.uiController.WriteLine("[1] Inn");
      this.uiController.WriteLine("[2] Arms Dealer");
      this.uiController.WriteLine("[3] Apothecary");
      this.uiController.WriteLine("[4] Doctor");
      //this.uiController.WriteLine("[5] Adventurer's Guild"); - sorry, I didn't have enough time to implement this
      this.uiController.WriteLine("[Esc] Return to menu");

      await this.gameEngine.WaitForPlayerKeyPress(() =>
      {
        switch (this.gameEngine.CurrentKeyPressed)
        {
          case "1":
            this.uiController.Clear();
            InnMenu();
            return true;
          case "2":
            this.uiController.Clear();
            ArmsDealerMenu();
            return true;
          case "3":
            this.uiController.Clear();
            ApothecaryMenu();
            return true;
          case "4":
            this.uiController.Clear();
            DoctorMenu();
            return true;
          case "escape":
            this.uiController.Clear();
            this.gameEngine.SwitchGameState(GameState.Menu);
            return true;
        }

        return false;
      });
    }

    private async void InnMenu()
    {

      int costToRest = GetInnCost();
      // Taken from https://www.asciiart.eu
      this.uiController.WriteLine(@"
    XXXXXXXXXXXXXXXXXXXXXXXXXXXXX
  XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
 XXXXXXXXXXXXXXXXXX         XXXXXXXX
XXXXXXXXXXXXXXXX              XXXXXXX
XXXXXXXXXXXXX                   XXXXX
 XXX     _________ _________     XXX
  XX    I  _xxxxx I xxxxx_  I    XX
 ( X----I         I         I----X )
( +I    I      00 I 00      I    I+ )
 ( I    I    __0  I  0__    I    I )
  (I    I______ /   \_______I    I)
   I           ( ___ )           I
   I    _  :::::::::::::::  _    i
    \    \___ ::::::::: ___/    /
     \_      \_________/      _/
       \        \___,        /
         \                 /
          |\             /|
          |  \_________/  |");

      this.uiController.WriteLine("How can I help you today?");
      this.uiController.WriteLine();
      this.uiController.WriteLine($"Party Gold: {this.partyInfo.Gold}");
      this.uiController.WriteLine($"[Space] Rest at the inn - {costToRest} gold");
      this.uiController.WriteLine($"[Esc] Return to town");

      await this.gameEngine.WaitForPlayerKeyPress(() =>
      {
        switch (this.gameEngine.CurrentKeyPressed)
        {
          case "space":
            this.uiController.Clear();
            RestAtInn(costToRest);
            return true;
          case "escape":
            this.uiController.Clear();
            TownMenu();
            return true;
        }

        return false;
      });
    }

    private int GetInnCost()
    {
      int cost = 0;

      foreach (var character in this.partyInfo.PartyMembers)
      {
        if (character.CharacterStatus == CharacterStatus.Alive)
          cost += character.MaxHealth - character.CurrentHealth;
        cost += (character.MaxMana - character.CurrentMana) * 2;
      }

      return cost;
    }

    private async void RestAtInn(int costToRest)
    {
      if (this.partyInfo.Gold < costToRest)
      {
        this.uiController.WriteLine("You do not have enough money to rest here for the night.");
        await this.gameEngine.Pause();
        TownMenu();
      }
      else
      {
        this.partyInfo.ModifyGold(-costToRest);

        foreach (var character in this.partyInfo.PartyMembers)
        {
          if (character.CharacterStatus == CharacterStatus.Alive)
          {
            character.CurrentHealth = character.MaxHealth;
            character.CurrentMana = character.MaxMana;
          }
        }

        this.uiController.WriteLine("Your party rested for the night...");
        await this.gameEngine.Pause(1600);
        this.uiController.Clear();
        TownMenu();
      }
    }

    private async void ArmsDealerMenu()
    {

      // Taken from https://www.asciiart.eu
      this.uiController.WriteLine(@"
   .------\ /------.
   |       -       |
   |               |
   |               |
   |               |
_______________________
===========.===========
  / ~~~~~     ~~~~~ \
 /|     |     |\
 W   ---  / \  ---   W
 \.      |o o|      ./
  |                 |
  \    #########    /
   \  ## ----- ##  /
    \##         ##/
     \_____v_____/
");
      this.uiController.WriteLine("You looking for something in particular?");
      this.uiController.WriteLine();
      this.uiController.WriteLine("Who would you like to buy new equipment for?");
      this.uiController.WriteLine();
      this.uiController.WriteLine($"[1] {this.partyInfo.PartyMembers[0].Name}");
      this.uiController.WriteLine($"[2] {this.partyInfo.PartyMembers[1].Name}");
      this.uiController.WriteLine($"[3] {this.partyInfo.PartyMembers[2].Name}");
      this.uiController.WriteLine($"[4] {this.partyInfo.PartyMembers[3].Name}");
      this.uiController.WriteLine($"[Esc] Return to town");

      await this.gameEngine.WaitForPlayerKeyPress(() =>
      {
        switch (this.gameEngine.CurrentKeyPressed)
        {
          case "1":
            this.uiController.Clear();
            ShowEquipmentForSale(this.partyInfo.PartyMembers[0]);
            return true;
          case "2":
            this.uiController.Clear();
            ShowEquipmentForSale(this.partyInfo.PartyMembers[1]);
            return true;
          case "3":
            this.uiController.Clear();
            ShowEquipmentForSale(this.partyInfo.PartyMembers[2]);
            return true;
          case "4":
            this.uiController.Clear();
            ShowEquipmentForSale(this.partyInfo.PartyMembers[3]);
            return true;
          case "escape":
            this.uiController.Clear();
            TownMenu();
            return true;
        }

        return false;
      });
    }

    private async void ShowEquipmentForSale(PlayerCharacter character)
    {
      int keyBind = 1;

      Dictionary<int, Equipment> equipmentKeyBinds = new Dictionary<int, Equipment>();

      this.uiController.WriteLine($"{character.Name}'s current equipment:");
      character.MainHand.PrintEquipmentInfo();
      this.uiController.WriteLine();
      character.OffHand.PrintEquipmentInfo();
      this.uiController.WriteLine();
      character.Armor.PrintEquipmentInfo();
      this.uiController.WriteLine();

      this.uiController.WriteLine();
      this.uiController.WriteLine("Available equipment:");

      foreach (Equipment item in GameData.Equipment)
      {
        if (character.PlayerProfession == item.CanBeUsedBy)
        {
          equipmentKeyBinds.Add(keyBind, item);
          this.uiController.Write($"[{keyBind++}]");
          item.PrintEquipmentInfo(false);
          this.uiController.WriteLine();
        }
      }
      this.uiController.WriteLine("[Esc] Return to town");

      string keyPressed;
      await this.gameEngine.WaitForPlayerKeyPress(() =>
        {
          keyPressed = this.gameEngine.CurrentKeyPressed;

          if (keyPressed == "escape")
          {
            this.uiController.Clear();
            TownMenu();
            return true;
          }

          if (int.TryParse(keyPressed, out int result) && equipmentKeyBinds.ContainsKey(result))
          {
            PurchaseItem(equipmentKeyBinds[result], character);
            return true;
          }

          return false;
        });
    }

    private async void ApothecaryMenu()
    {
      int keyBind = 1;

      Dictionary<int, Consumable> consumableKeyBinds = new Dictionary<int, Consumable>();
      // Taken from https://www.asciiart.eu
      // Art by THE LOCKER GNOME
      this.uiController.WriteLine(@"
                       ,---.
                       /    |
                      /     |
                     /      |
                    /       |
               ___,'        |
             <  -'          :
              `-.__..--'``-,_\_
                 |o/ ` :,.)_`>
                 :/ `     ||/)
                 (_.).__,-` |\
                 /( `.``   `| :
                 \'`-.)  `  ; ;
                 | `       /-<
                 |     `  /   `.
 ,-_-..____     /|  `    :__..-'\
/,'-.__\\  ``-./ :`      ;       \
`\ `\  `\\  \ :  (   `  /  ,   `. \
  \` \   \\   |  | `   :  :     .\ \
   \ `\_  ))  :  ;     |  |      ): :
  (`-.-'\ ||  |\ \   ` ;  ;       | |
   \-_   `;;._   ( `  /  /_       | |
    `-.-.// ,'`-._\__/_,'         ; |
       \:: :     /     `     ,   /  |
        || |    (        ,' /   /   |
        ||                ,'   / SSt|
");

      this.uiController.WriteLine("Hello friend. What can I get you today?");
      this.uiController.WriteLine();

      foreach (Consumable item in GameData.Consumables.Values)
      {
        consumableKeyBinds.Add(keyBind, item);
        this.uiController.Write($"[{keyBind++}] ");
        item.PrintItemInfo(true);
      }
      this.uiController.WriteLine("[Esc] Return to town");

      await this.gameEngine.WaitForPlayerKeyPress(() =>
        {
          string keyPressed = this.gameEngine.CurrentKeyPressed;

          if (keyPressed == "escape")
          {
            this.uiController.Clear();
            TownMenu();
            return true;
          }

          if (int.TryParse(keyPressed, out int result) && consumableKeyBinds.ContainsKey(result))
          {
            PurchaseItem(consumableKeyBinds[result]);
            return true;
          }

          return false;
        });
    }

    private async void PurchaseItem(Equipment item, PlayerCharacter character)
    {
      int finalPrice = int.MinValue;

      switch (item.EquipmentType)
      {
        case EquipmentType.MainHand:
          finalPrice = item.Price - character.MainHand.TradeInPrice;
          break;
        case EquipmentType.OffHand:
          finalPrice = item.Price - character.OffHand.TradeInPrice;
          break;
        case EquipmentType.Armor:
          finalPrice = item.Price - character.Armor.TradeInPrice;
          break;
      }

      this.uiController.WriteLine();

      if (this.partyInfo.Gold < finalPrice)
      {
        this.uiController.WriteLine("You don't have enough gold! Don't waste my time!");
        await this.gameEngine.Pause();
        this.uiController.Clear();
        ArmsDealerMenu();
        return;
      }

      this.uiController.WriteLine($"That will be {finalPrice}G with your trade in.");
      this.uiController.WriteLine();
      this.uiController.WriteLine("[Spacebar] Purchase equipment");
      this.uiController.WriteLine("[Esc] On second thought...");

      await this.gameEngine.WaitForPlayerKeyPress(() =>
       {
         switch (this.gameEngine.CurrentKeyPressed)
         {
           case "space":
             this.partyInfo.ModifyGold(-finalPrice);
             character.EquipItem(item);
             this.uiController.WriteLine("Take good care of it.");
             this.gameEngine.PerformActionAfterPause(() =>
             {
               this.uiController.Clear();
               ArmsDealerMenu();
             });
             return true;
           case "escape":
             this.uiController.Clear();
             ArmsDealerMenu();
             return true;
         }

         return false;
       });
    }

    private async void PurchaseItem(Consumable item)
    {
      if (this.partyInfo.Gold < item.Price)
      {
        this.uiController.WriteLine("I'm afraid you're short on gold my friend...");
        await this.gameEngine.Pause();
        this.uiController.Clear();
        ApothecaryMenu();
        return;
      }

      this.partyInfo.ModifyGold(-item.Price);

      if (this.partyInfo.UsableItems.ContainsKey(item))
      {
        this.partyInfo.UsableItems[item]++;
      }
      else
      {
        this.partyInfo.UsableItems[item] = 1;
      }

      this.uiController.WriteLine();
      this.uiController.WriteLine("Thank you for your patronage!");
      await this.gameEngine.Pause();
      this.uiController.Clear();
      ApothecaryMenu();
    }

    private async void DoctorMenu()
    {
      int keyBind = 1;

      Dictionary<int, PlayerCharacter> deadCharacters = new Dictionary<int, PlayerCharacter>();
      // Taken from https://www.asciiart.eu
      this.uiController.WriteLine(@"
                                     ____________
                               _____/            \_
                    __________/  _/          _____ \__
        ______ ____/            /           /     \___\_
      _/      \____           _/          _/             \_
    _/             \____     /          _/    ___          \
   /    _______         \_   |         /  ___/_____-        |
  /   _/       \__        \_ |       _/__/      \_ \__      |
 /  _/            \______     \     /_/           \   \     |
 |_/                _____\__________/              \        |
 /               __/  __/ _/                          \_   /
 |           ___/    /  _/                              \_ |
 |        __/  /    |  /                  ________   \    \|
 |                  | /                   \XXXXXXXXxx_|    |
 |\                 | |                               |     \___
 | |          |     \ |______                         |         \_
 |  \             ___||XXXXX/                ---_     |           \
 |  |         | xxXXX//                     /___-///  |           |
  \ \        /\     |/                 /   |///OX\\\  |           |
  |  ||    _/  |        __---_         |   | \\XX///   \___      \|
  \ //   _/    |     \\\xxxxx \        |   |\_\---       \ \_      \
   |/  _/      |      | //OXX\\\        \                 \  \_     \
  _/ _/        /\     | \\XXX///\                         |\   \    |
 /__/         /  \     \_-----                            | \   |   /
|/ /              |                       \               /  |   \_/
   |             /|                      _|              |  | __/
  /             | |                  \ -                 / _/_/
  |           _/ _/                         _____       |/
  | _       _/  /\\ \               ________/ / |       /_
  |/       /   /  \_ \_          __/_________/ /       /  \______
   \      |   | \_  \- \_          \__________/      _/          \
    \__   |              \___                      _/\_           |
       \__|\_                \___                _/|   \         /
          \__\_______________/   \___         __/  |\          \/
                       \___  |       \_______/     | \___       |
                        /  \_|                     |   \ \_____/
                _______|____/                       \___\__
");

      this.uiController.WriteLine("Hello there!");

      foreach (PlayerCharacter character in this.partyInfo.PartyMembers)
      {
        if (character.CharacterStatus == CharacterStatus.Dead)
        {
          deadCharacters.Add(keyBind++, character);
        }
      }

      if (deadCharacters.Count <= 0)
      {
        await this.gameEngine.Pause();
        this.uiController.WriteLine("I'm glad to see that none of you are in need of my services!");
        await this.gameEngine.Pause();
        this.uiController.WriteLine("Have a nice day!");
        await this.gameEngine.Pause();
        this.uiController.Clear();
        TownMenu();
      }
      else
      {
        foreach (var character in deadCharacters)
        {
          this.uiController.WriteLine($"[{character.Key}] {character.Value.Name} - {GetReviveCost(character.Value)}G");
        }

        this.uiController.WriteLine("[Esc] Return to town");
      }

      await this.gameEngine.WaitForPlayerKeyPress(() =>
        {
          string keyPressed = this.gameEngine.CurrentKeyPressed;

          if (keyPressed == "escape")
          {
            this.uiController.Clear();
            TownMenu();
            return true;
          }

          if (int.TryParse(keyPressed, out int result) && deadCharacters.ContainsKey(result))
          {
            ReviveCharacter(deadCharacters[result], deadCharacters);
            return true;
          }

          return false;
        });
    }

    private int GetReviveCost(PlayerCharacter character)
    {
      return character.Level * 100;
    }

    private async void ReviveCharacter(PlayerCharacter character, Dictionary<int, PlayerCharacter> deadCharacters)
    {
      int reviveCost = GetReviveCost(character);

      if (this.partyInfo.Gold < reviveCost)
      {
        this.uiController.WriteLine("It looks like you don't have enough...");
        await this.gameEngine.Pause(2400);

        foreach (var deadCharacter in deadCharacters.Values)
        {
          deadCharacter.CharacterStatus = CharacterStatus.Alive;
          deadCharacter.CurrentHealth = 1;
        }

        this.uiController.WriteLine("Still, I can't let you leave like that. Pay me what you can and I'll take care of you.");

        await this.gameEngine.Pause(2400);

        this.uiController.Clear();
        this.uiController.WriteLine("You waited patiently while your injured allies wounds were treated...");

        await this.gameEngine.Pause(2400);

        this.uiController.Clear();
        TownMenu();
      }
      else
      {
        this.partyInfo.ModifyGold(-reviveCost);
        character.CharacterStatus = CharacterStatus.Alive;
        character.CurrentHealth = 1;
        this.uiController.WriteLine("I'll patch you up right away!");

        await this.gameEngine.Pause(2400);

        DoctorMenu();
      }
    }
  }
}
