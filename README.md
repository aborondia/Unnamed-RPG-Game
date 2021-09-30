# Unnamed RPG Game - [Andrew Borondia](https://cranky-beaver-6bfa9c.netlify.app/projects)

This is a project I made as part of the Software Developer program at the Manitoba Institute of trades and Technology.

It is an RPG game built using C#, for the C# console. The requirements for the project were very basic, but I have added many extra features.

## The basic requirements for the project were as follows:
* Show statistics (games played, won, etc.)
* Show player inventory, including equipped items
* Ability to Equip weapon/armor
* Enable fights between player characters and monsters (victory, game over, etc.)
* Receive gold for winning battles which can be used to restore health
* A hero with:
  * Name
  * Base Strength
  * Base Defense
  * Health
  * Equipped Weapon
  * Equipped Armor
  * Armor Bag
  * Weapon Bag
* A monster class with the same base attributes (no items/equipment) as the player

## The extra attributes/functionality, etc. I added:
* Character classes (different stats/equipment/abilities)
* Experience & leveling system
* Elements (extra/less damage depending on resistances)
* Cheat Menu (give characters max level, max gold, strongest equipment)
* Consumable items
* Four character player party
* Extra player attributes/functions:
   * Mana
   * Magic power
   * Magic defense
   * Agility
   * Dexterity
   * Abilities, which include:
     * Damage multiplier (based on different character stat depending on ability)
     * Type (damage, healing, buff)
     * Health/mana cost (physical = health cost, magic = mana cost)
     * Element type
   * Extra enemy attributes/functions:
     * Same stats as player (excluding mana)
     * Abilities, with same properties (other than cost) as player, and including odds of enemy using ability

* Town where the player can use different facilities in exchange for gold:
  * Inn - Restores health
  * Arms Dealer: Purchase new equipment (main hand, offhand, armor)
  * Apothecary: Purchase healing items
  * Doctor: Revive fallen party members
* Battle UI showing available commands (mapped to keyboard keys), health/mana, current buffs/debuffs on player characters and enemies, and enemy ASCII art
