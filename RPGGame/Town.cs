using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
  static class Town
  {
    public static GameDataBase GameData = GameEngine.GameData;
    public static void StartTown()
    {
      TownMenu();
    }

    private static void TownMenu()
    {
      Console.WriteLine("Where would you like to go?");
      Console.WriteLine("[1] Inn");
      Console.WriteLine("[2] Arms Dealer");
      Console.WriteLine("[3] Apothecary");
      Console.WriteLine("[4] Doctor");
      //Console.WriteLine("[5] Adventurer's Guild"); - sorry, I didn't have enough time to implement this
      Console.WriteLine("[Esc] Return to menu");

      while (GameEngine.CurrentGameState == GameState.Town)
      {
        switch (Console.ReadKey(true).Key)
        {
          case ConsoleKey.D1:

            Console.Clear();
            InnMenu();
            break;
          case ConsoleKey.D2:

            Console.Clear();
            ArmsDealerMenu();
            break;
          case ConsoleKey.D3:

            Console.Clear();
            ApothecaryMenu();
            break;
          case ConsoleKey.D4:

            Console.Clear();
            DoctorMenu();
            break;
          case ConsoleKey.Escape:

            Console.Clear();
            GameEngine.SwitchGameState(GameState.Menu);
            break;
          default: continue;
        }
      }
    }

    private static void InnMenu()
    {

      int costToRest = GetInnCost();
      // Taken from https://www.asciiart.eu
      Console.WriteLine(@"
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

      Console.WriteLine("How can I help you today?");
      Console.WriteLine();
      Console.WriteLine($"Party Gold: {PartyInfo.Gold}");
      Console.WriteLine($"[Space] Rest at the inn - {costToRest} gold");
      Console.WriteLine($"[Esc] Return to town");


      while (GameEngine.CurrentGameState == GameState.Town)
      {
        switch (Console.ReadKey(true).Key)
        {
          case ConsoleKey.Spacebar:

            Console.Clear();
            RestAtInn(costToRest);
            break;
          case ConsoleKey.Escape:

            Console.Clear();
            TownMenu();
            break;
        }
      }
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

    private static void RestAtInn(int costToRest)
    {
      if (PartyInfo.Gold < costToRest)
      {
        Console.WriteLine("You do not have enough money to rest here for the night.");
        GameEngine.Pause();
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

        Console.WriteLine("Your party rested for the night...");
        GameEngine.Pause(1600);
        Console.Clear();
        TownMenu();
      }
    }

    private static void ArmsDealerMenu()
    {

      // Taken from https://www.asciiart.eu
      Console.WriteLine(@"
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
      Console.WriteLine("You looking for something in particular?");
      Console.WriteLine();
      Console.WriteLine("Who would you like to buy new equipment for?");
      Console.WriteLine();
      Console.WriteLine($"[1] {PartyInfo.PartyMembers[0].Name}");
      Console.WriteLine($"[2] {PartyInfo.PartyMembers[1].Name}");
      Console.WriteLine($"[3] {PartyInfo.PartyMembers[2].Name}");
      Console.WriteLine($"[4] {PartyInfo.PartyMembers[3].Name}");
      Console.WriteLine($"[Esc] Return to town");

      while (GameEngine.CurrentGameState == GameState.Town)
      {
        switch (Console.ReadKey(true).Key)
        {
          case ConsoleKey.D1:

            Console.Clear();
            ShowEquipmentForSale(PartyInfo.PartyMembers[0]);
            break;
          case ConsoleKey.D2:

            Console.Clear();
            ShowEquipmentForSale(PartyInfo.PartyMembers[1]);
            break;
          case ConsoleKey.D3:

            Console.Clear();
            ShowEquipmentForSale(PartyInfo.PartyMembers[2]);
            break;
          case ConsoleKey.D4:

            Console.Clear();
            ShowEquipmentForSale(PartyInfo.PartyMembers[3]);
            break;
          case ConsoleKey.Escape:

            Console.Clear();
            TownMenu();
            break;
        }
      }
    }

    private static void ShowEquipmentForSale(PlayerCharacter character)
    {
      int keyBind = 1;

      Dictionary<ConsoleKey, Equipment> equipmentKeyBinds = new Dictionary<ConsoleKey, Equipment>();

      Console.WriteLine($"{character.Name}'s current equipment:");
      character.MainHand.PrintEquipmentInfo();
      Console.WriteLine();
      character.OffHand.PrintEquipmentInfo();
      Console.WriteLine();
      character.Armor.PrintEquipmentInfo();
      Console.WriteLine();

      Console.WriteLine();
      Console.WriteLine("Available equipment:");
      foreach (Equipment item in GameData.Equipment)
      {
        if (character.PlayerProfession == item.CanBeUsedBy)
        {
          equipmentKeyBinds.Add(GameEngine.IntToConsoleKey(keyBind), item);
          Console.Write($"[{keyBind++}]");
          item.PrintEquipmentInfo(false);
          Console.WriteLine();
        }
      }
      Console.WriteLine("[Esc] Return to town");

      ConsoleKey keyPressed;

      while (GameEngine.CurrentGameState == GameState.Town)
      {
        keyPressed = Console.ReadKey(true).Key;

        if (keyPressed == ConsoleKey.Escape)
        {
          Console.Clear();
          TownMenu();
        }

        if (equipmentKeyBinds.ContainsKey(keyPressed))
        {
          PurchaseItem(equipmentKeyBinds[keyPressed], character);
        }
      }
    }

    private static void ApothecaryMenu()
    {
      int keyBind = 1;

      Dictionary<ConsoleKey, Consumable> consumableKeyBinds = new Dictionary<ConsoleKey, Consumable>();
      // Taken from https://www.asciiart.eu
      // Art by THE LOCKER GNOME
      Console.WriteLine(@"
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

      Console.WriteLine("Hello friend. What can I get you today?");
      Console.WriteLine();

      foreach (Consumable item in GameData.Consumables.Values)
      {
        consumableKeyBinds.Add(GameEngine.IntToConsoleKey(keyBind), item);
        Console.Write($"[{keyBind++}] ");
        item.PrintItemInfo(true);
      }
      Console.WriteLine("[Esc] Return to town");

      while (GameEngine.CurrentGameState == GameState.Town)
      {
        ConsoleKey keyPressed = Console.ReadKey(true).Key;

        if (keyPressed == ConsoleKey.Escape)
        {
          Console.Clear();
          TownMenu();
        }

        if (consumableKeyBinds.ContainsKey(keyPressed))
        {
          PurchaseItem(consumableKeyBinds[keyPressed]);
        }
      }
    }

    private static void PurchaseItem(Equipment item, PlayerCharacter character)
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

      Console.WriteLine();

      if (PartyInfo.Gold < finalPrice)
      {
        Console.WriteLine("You don't have enough gold! Don't waste my time!");
        GameEngine.Pause();
        Console.Clear();
        ArmsDealerMenu();
      }

      Console.WriteLine($"That will be {finalPrice}G with your trade in.");
      Console.WriteLine();
      Console.WriteLine("[Spacebar] Purchase equipment");
      Console.WriteLine("[Esc] On second thought...");

      while (GameEngine.CurrentGameState == GameState.Town)
      {
        switch (Console.ReadKey(true).Key)
        {
          case ConsoleKey.Spacebar:

            PartyInfo.ModifyGold(-finalPrice);
            character.EquipItem(item);
            Console.WriteLine("Take good care of it.");
            GameEngine.Pause();
            Console.Clear();
            ArmsDealerMenu();
            break;
          case ConsoleKey.Escape:

            Console.Clear();
            ArmsDealerMenu();
            break;
        }
      }
    }

    private static void PurchaseItem(Consumable item)
    {
      if (PartyInfo.Gold < item.Price)
      {
        Console.WriteLine("I'm afraid you're short on gold my friend...");
        GameEngine.Pause();
        Console.Clear();
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
      Console.WriteLine();
      Console.WriteLine("Thank you for your patronage!");
      GameEngine.Pause();
      Console.Clear();
      ApothecaryMenu();
    }

    private static void DoctorMenu()
    {
      int keyBind = 1;

      Dictionary<ConsoleKey, PlayerCharacter> deadCharacters = new Dictionary<ConsoleKey, PlayerCharacter>();
      // Taken from https://www.asciiart.eu
      Console.WriteLine(@"
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

      Console.WriteLine("Hello there!");

      foreach (PlayerCharacter character in PartyInfo.PartyMembers)
      {
        if (character.CharacterStatus == CharacterStatus.Dead)
        {
          deadCharacters.Add(GameEngine.IntToConsoleKey(keyBind++), character);
        }
      }

      if (deadCharacters.Count <= 0)
      {
        GameEngine.Pause();
        Console.WriteLine("I'm glad to see that none of you are in need of my services!");
        GameEngine.Pause();
        Console.WriteLine("Have a nice day!");
        GameEngine.Pause();
        Console.Clear();
        TownMenu();
      }
      else
      {
        foreach (var character in deadCharacters)
        {
          Console.WriteLine($"[{GameEngine.ConsoleKeyToInt(character.Key)}] {character.Value.Name} - {GetReviveCost(character.Value)}G");
        }
        Console.WriteLine("[Esc] Return to town");
      }

      while (GameEngine.CurrentGameState == GameState.Town)
      {
        ConsoleKey keyPressed = Console.ReadKey(true).Key;

        if (keyPressed == ConsoleKey.Escape)
        {
          Console.Clear();
          TownMenu();
        }

        if (deadCharacters.ContainsKey(keyPressed))
        {
          ReviveCharacter(deadCharacters[keyPressed], deadCharacters);
        }
      }
    }

    private static int GetReviveCost(PlayerCharacter character)
    {
      return character.Level * 100;
    }

    private static void ReviveCharacter(PlayerCharacter character, Dictionary<ConsoleKey, PlayerCharacter> deadCharacters)
    {
      int reviveCost = GetReviveCost(character);

      if (PartyInfo.Gold < reviveCost)
      {
        Console.WriteLine("It looks like you don't have enough...");
        GameEngine.Pause(2400);
        foreach (var deadCharacter in deadCharacters.Values)
        {
          deadCharacter.CharacterStatus = CharacterStatus.Alive;
          deadCharacter.CurrentHealth = 1;
        }
        Console.WriteLine("Still, I can't let you leave like that. Pay me what you can and I'll take care of you.");
        GameEngine.Pause(2400);
        Console.Clear();
        Console.WriteLine("You waited patiently while your injured allies wounds were treated...");
        GameEngine.Pause(2400);
        Console.Clear();
        TownMenu();
      }
      else
      {
        PartyInfo.ModifyGold(-reviveCost);
        character.CharacterStatus = CharacterStatus.Alive;
        character.CurrentHealth = 1;
        Console.WriteLine("I'll patch you up right away!");
        GameEngine.Pause(2400);
        DoctorMenu();
      }
    }
  }
}
