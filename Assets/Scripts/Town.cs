using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPGGame
{
  static class Town
  {
    public static GameDataBase GameData = GameEngine.Active.GameData;
    public static void StartTown()
    {
      TownMenu();
    }

    private static async void TownMenu()
    {
      UIController.Active.WriteLine("Where would you like to go?");
      UIController.Active.WriteLine("[1] Inn");
      UIController.Active.WriteLine("[2] Arms Dealer");
      UIController.Active.WriteLine("[3] Apothecary");
      UIController.Active.WriteLine("[4] Doctor");
      //UIController.Active.WriteLine("[5] Adventurer's Guild"); - sorry, I didn't have enough time to implement this
      UIController.Active.WriteLine("[Esc] Return to menu");

      await GameEngine.Active.WaitForPlayerKeyPress(() =>
      {
        switch (GameEngine.Active.CurrentKeyPressed)
        {
          case "1":
            UIController.Active.Clear();
            InnMenu();
            return true;
          case "2":
            UIController.Active.Clear();
            ArmsDealerMenu();
            return true;
          case "3":
            UIController.Active.Clear();
            ApothecaryMenu();
            return true;
          case "4":
            UIController.Active.Clear();
            DoctorMenu();
            return true;
          case "escape":
            UIController.Active.Clear();
            GameEngine.Active.SwitchGameState(GameState.Menu);
            return true;
        }

        return false;
      });
    }

    private static async void InnMenu()
    {

      int costToRest = GetInnCost();
      // Taken from https://www.asciiart.eu
      UIController.Active.WriteLine(@"
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

      UIController.Active.WriteLine("How can I help you today?");
      UIController.Active.WriteLine();
      UIController.Active.WriteLine($"Party Gold: {PartyInfo.Gold}");
      UIController.Active.WriteLine($"[Space] Rest at the inn - {costToRest} gold");
      UIController.Active.WriteLine($"[Esc] Return to town");

      await GameEngine.Active.WaitForPlayerKeyPress(() =>
      {
        switch (GameEngine.Active.CurrentKeyPressed)
        {
          case "spacebar":
            UIController.Active.Clear();
            RestAtInn(costToRest);
            return true;
          case "escape":
            UIController.Active.Clear();
            TownMenu();
            return true;
        }

        return false;
      });
    }

    private static int GetInnCost()
    {
      int cost = 0;

      foreach (var character in PartyInfo.PartyMembers)
      {
        if (character.CharacterStatus == CharacterStatus.Alive)
          cost += character.MaxHealth - character.CurrentHealth;
        cost += (character.MaxMana - character.CurrentMana) * 2;
      }

      return cost;
    }

    private static async void RestAtInn(int costToRest)
    {
      if (PartyInfo.Gold < costToRest)
      {
        UIController.Active.WriteLine("You do not have enough money to rest here for the night.");
        await GameEngine.Active.Pause();
        TownMenu();
      }
      else
      {
        PartyInfo.ModifyGold(-costToRest);

        foreach (var character in PartyInfo.PartyMembers)
        {
          if (character.CharacterStatus == CharacterStatus.Alive)
          {
            character.CurrentHealth = character.MaxHealth;
            character.CurrentMana = character.MaxMana;
          }
        }

        UIController.Active.WriteLine("Your party rested for the night...");
        await GameEngine.Active.Pause(1600);
        UIController.Active.Clear();
        TownMenu();
      }
    }

    private static async void ArmsDealerMenu()
    {

      // Taken from https://www.asciiart.eu
      UIController.Active.WriteLine(@"
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
      UIController.Active.WriteLine("You looking for something in particular?");
      UIController.Active.WriteLine();
      UIController.Active.WriteLine("Who would you like to buy new equipment for?");
      UIController.Active.WriteLine();
      UIController.Active.WriteLine($"[1] {PartyInfo.PartyMembers[0].Name}");
      UIController.Active.WriteLine($"[2] {PartyInfo.PartyMembers[1].Name}");
      UIController.Active.WriteLine($"[3] {PartyInfo.PartyMembers[2].Name}");
      UIController.Active.WriteLine($"[4] {PartyInfo.PartyMembers[3].Name}");
      UIController.Active.WriteLine($"[Esc] Return to town");

      await GameEngine.Active.WaitForPlayerKeyPress(() =>
      {
        switch (GameEngine.Active.CurrentKeyPressed)
        {
          case "1":
            UIController.Active.Clear();
            ShowEquipmentForSale(PartyInfo.PartyMembers[0]);
            return true;
          case "2":
            UIController.Active.Clear();
            ShowEquipmentForSale(PartyInfo.PartyMembers[1]);
            return true;
          case "3":
            UIController.Active.Clear();
            ShowEquipmentForSale(PartyInfo.PartyMembers[2]);
            return true;
          case "4":
            UIController.Active.Clear();
            ShowEquipmentForSale(PartyInfo.PartyMembers[3]);
            return true;
          case "escape":
            UIController.Active.Clear();
            TownMenu();
            return true;
        }

        return false;
      });
    }

    private static async void ShowEquipmentForSale(PlayerCharacter character)
    {
      int keyBind = 1;

      Dictionary<int, Equipment> equipmentKeyBinds = new Dictionary<int, Equipment>();

      UIController.Active.WriteLine($"{character.Name}'s current equipment:");
      character.MainHand.PrintEquipmentInfo();
      UIController.Active.WriteLine();
      character.OffHand.PrintEquipmentInfo();
      UIController.Active.WriteLine();
      character.Armor.PrintEquipmentInfo();
      UIController.Active.WriteLine();

      UIController.Active.WriteLine();
      UIController.Active.WriteLine("Available equipment:");

      foreach (Equipment item in GameData.Equipment)
      {
        if (character.PlayerProfession == item.CanBeUsedBy)
        {
          equipmentKeyBinds.Add(keyBind, item);
          UIController.Active.Write($"[{keyBind++}]");
          item.PrintEquipmentInfo(false);
          UIController.Active.WriteLine();
        }
      }
      UIController.Active.WriteLine("[Esc] Return to town");

      string keyPressed;
      await GameEngine.Active.WaitForPlayerKeyPress(() =>
        {
          keyPressed = GameEngine.Active.CurrentKeyPressed;

          if (keyPressed == "escape")
          {
            UIController.Active.Clear();
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

    private static async void ApothecaryMenu()
    {
      int keyBind = 1;

      Dictionary<int, Consumable> consumableKeyBinds = new Dictionary<int, Consumable>();
      // Taken from https://www.asciiart.eu
      // Art by THE LOCKER GNOME
      UIController.Active.WriteLine(@"
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

      UIController.Active.WriteLine("Hello friend. What can I get you today?");
      UIController.Active.WriteLine();

      foreach (Consumable item in GameData.Consumables.Values)
      {
        consumableKeyBinds.Add(keyBind, item);
        UIController.Active.Write($"[{keyBind++}] ");
        item.PrintItemInfo(true);
      }
      UIController.Active.WriteLine("[Esc] Return to town");

      await GameEngine.Active.WaitForPlayerKeyPress(() =>
        {
          string keyPressed = GameEngine.Active.CurrentKeyPressed;

          if (keyPressed == "escape")
          {
            UIController.Active.Clear();
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

    private static async void PurchaseItem(Equipment item, PlayerCharacter character)
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

      UIController.Active.WriteLine();

      if (PartyInfo.Gold < finalPrice)
      {
        UIController.Active.WriteLine("You don't have enough gold! Don't waste my time!");
        await GameEngine.Active.Pause();
        UIController.Active.Clear();
        ArmsDealerMenu();
        return;
      }

      UIController.Active.WriteLine($"That will be {finalPrice}G with your trade in.");
      UIController.Active.WriteLine();
      UIController.Active.WriteLine("[Spacebar] Purchase equipment");
      UIController.Active.WriteLine("[Esc] On second thought...");

      await GameEngine.Active.WaitForPlayerKeyPress(() =>
       {
         switch (GameEngine.Active.CurrentKeyPressed)
         {
           case "spacebar":
             PartyInfo.ModifyGold(-finalPrice);
             character.EquipItem(item);
             UIController.Active.WriteLine("Take good care of it.");
             GameEngine.Active.PerformActionAfterPause(() =>
             {
               UIController.Active.Clear();
               ArmsDealerMenu();
             });
             return true;
           case "escape":
             UIController.Active.Clear();
             ArmsDealerMenu();
             return true;
         }

         return false;
       });
    }

    private static async void PurchaseItem(Consumable item)
    {
      if (PartyInfo.Gold < item.Price)
      {
        UIController.Active.WriteLine("I'm afraid you're short on gold my friend...");
        await GameEngine.Active.Pause();
        UIController.Active.Clear();
        ApothecaryMenu();
        return;
      }

      PartyInfo.ModifyGold(-item.Price);

      if (PartyInfo.UsableItems.ContainsKey(item))
      {
        PartyInfo.UsableItems[item]++;
      }
      else
      {
        PartyInfo.UsableItems[item] = 1;
      }

      UIController.Active.WriteLine();
      UIController.Active.WriteLine("Thank you for your patronage!");
      await GameEngine.Active.Pause();
      UIController.Active.Clear();
      ApothecaryMenu();
    }

    private static async void DoctorMenu()
    {
      int keyBind = 1;

      Dictionary<int, PlayerCharacter> deadCharacters = new Dictionary<int, PlayerCharacter>();
      // Taken from https://www.asciiart.eu
      UIController.Active.WriteLine(@"
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

      UIController.Active.WriteLine("Hello there!");

      foreach (PlayerCharacter character in PartyInfo.PartyMembers)
      {
        if (character.CharacterStatus == CharacterStatus.Dead)
        {
          deadCharacters.Add(keyBind++, character);
        }
      }

      if (deadCharacters.Count <= 0)
      {
        await GameEngine.Active.Pause();
        UIController.Active.WriteLine("I'm glad to see that none of you are in need of my services!");
        await GameEngine.Active.Pause();
        UIController.Active.WriteLine("Have a nice day!");
        await GameEngine.Active.Pause();
        UIController.Active.Clear();
        TownMenu();
      }
      else
      {
        foreach (var character in deadCharacters)
        {
          UIController.Active.WriteLine($"[{character.Key}] {character.Value.Name} - {GetReviveCost(character.Value)}G");
        }

        UIController.Active.WriteLine("[Esc] Return to town");
      }

      await GameEngine.Active.WaitForPlayerKeyPress(() =>
        {
          string keyPressed = GameEngine.Active.CurrentKeyPressed;

          if (keyPressed == "escape")
          {
            UIController.Active.Clear();
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

    private static int GetReviveCost(PlayerCharacter character)
    {
      return character.Level * 100;
    }

    private static async void ReviveCharacter(PlayerCharacter character, Dictionary<int, PlayerCharacter> deadCharacters)
    {
      int reviveCost = GetReviveCost(character);

      if (PartyInfo.Gold < reviveCost)
      {
        UIController.Active.WriteLine("It looks like you don't have enough...");
        await GameEngine.Active.Pause(2400);

        foreach (var deadCharacter in deadCharacters.Values)
        {
          deadCharacter.CharacterStatus = CharacterStatus.Alive;
          deadCharacter.CurrentHealth = 1;
        }

        UIController.Active.WriteLine("Still, I can't let you leave like that. Pay me what you can and I'll take care of you.");

        await GameEngine.Active.Pause(2400);

        UIController.Active.Clear();
        UIController.Active.WriteLine("You waited patiently while your injured allies wounds were treated...");

        await GameEngine.Active.Pause(2400);

        UIController.Active.Clear();
        TownMenu();
      }
      else
      {
        PartyInfo.ModifyGold(-reviveCost);
        character.CharacterStatus = CharacterStatus.Alive;
        character.CurrentHealth = 1;
        UIController.Active.WriteLine("I'll patch you up right away!");

        await GameEngine.Active.Pause(2400);

        DoctorMenu();
      }
    }
  }
}
