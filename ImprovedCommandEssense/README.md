## Why?

The Command Artifact is too strong, but having better control over your build is fun.

This mod gives you more control over your run while not being imba

Similar mods exists but do not have the configurability or additional QOL this does

## What it do?

This mod changes the amount of options that you can choose from the Command Essences that drop with the Command Artifact.

Also provides some QOL options like having the Bazaar and Terminal shops drop the item they are displaying


## Config

Improved Command Essence is highly configurable, you can change the amount of options you can choose from for each tier individually <br/>
The default values are how I like to play, change these up to fit your playstyle

### Defaults:
#### You can change the option amount for each tier
| Rarity/Tier             |    Options      |
|----------|:-------------:|
| Common (White)   |  6 |
| Uncommon (Green) |  4  |
| Legendary (Red)  |  2 |
| Lunar (Blue)  |  2 |
| Boss (Yellow)  |  2 |
| Equipment (Orange)  |  4 |
| Void (Purple)  |  2 |
| Void Potential (White Sphere)  |  6 |
| Void Cache (Purple Sphere)  |  4 |

(All void tiers use the same config)<br />
(Lunar items and lunar equipment use the same config)<br/>
(Void Potential and Void Cache config is used when onForPotential is set to false)

#### Toggle Options
| Name             |    default      | Description |
|----------|:-------------:|------------|
| keepCategory   |  true | If true Category Chests will only contain options of the same category |
| onInBazaar   |  false | If false item drops in the Bazaar will drop as the actual item not a Command Essence |
| onInDropShip |  false  | If false item drops from terminals (like the Drop Ship) will drop as the actual item not a Command Essence |
| sameBossDrops | true | If true the Command Essences dropped from the teleporter boss of the same rarity will contain the same options |
| onForTrophy | false | If false bosses killed with the Trophy Hunter's Tricorn will drop the related item and not a Command Essence |
| onForAdaptive | false | Set if Adaptive Chests drop their items (false) or Command Essence (true) |
| onForPotential | false | Set if Void Cache and Void Triples drop a Void Potential (false) Command Essence (true) |
| enableScrappers | false | Set if Scrappers spawn |
| enablePrinters | false | Set if Printers spawn (onInDropShip must be on to drop as the item) |
| enableMultishop | false | Set if Multishops spawn (onInDropShip must be on to drop as the item) |
| scrappersDropEssence | false | Set if Scrappers drop scrap (false) or Command Essence (true) |
| essenceChance | 100 | Set the chance for chests to drop Command Essences (0-100) |

(Note that Category chests are bugged in v1.2.2.0 and will only contain item with ONLY the same tag. Use https://thunderstore.io/package/Cercain/FixedCategoryChests/ to fix this)<br/>
(By default the only Terminal Shops that spawn in Command runs are Drop Ships but this toggle affects all of them)


## Changelog

**1.3.1**

* Rebuilt with updated dlls to support v1.2.3

**1.3.0**

* (Feature) Added a toggle to enable Void Potentials (default enables them)
* (Feature) Added options for the amount of items a Void Potential offers (split normal and void cache)
* Removing dependency for [FixedCategoryChests](https://thunderstore.io/package/Cercain/FixedCategoryChests/) as the fix has been made to vanilla

**1.2.1**

* (Compatability Fix) Fixed the exclusion list to be compatible with mods that add new items

**1.2.0**

* (Feature) Added new toggles to enable Scrappers, Multishops, and Printers (default off)
* (Feature) Added a config to set the chance for chests to drop as Command Essences (default 100%)
* (Feature) Added a toggle for Scrappers to drop as scrap(false) or Essence(true)
* (Bug Fix) Fixed boss drops from the teleporter event sometimes not having the same items with the sameBossDrops config on
* Added a soft dependency for [FixedCategoryChests](https://thunderstore.io/package/Cercain/FixedCategoryChests/) (Mod will load without it but R2 will ask to download it)

**1.1.4**

* (Bugfix) Keep Category chest now works correctly

**1.1.3**

* (BugFix) Lunar equipment now correctly drops as a Command Essence containing Lunar equipment
* (BugFix) Special boss drops (ie. Aurelionite) now actually correctly drop as their items instead of as a Command Essence

**1.1.2**

* (Compatability Fix) ZetAspects now works with the Item drop mode

**1.1.1**

* (BugFix) Lunar Coins now drop correctly
* (BugFix) Special items like Aspects and the Halcyon Seed now correctly drop as their items instead of Command Essences

**1.1.0**

* (Togglable) Command Essences dropped from the teleporter boss of the same rarity to contain the same options
* (Togglable) Trophy Hunter's Tricorn to drop the related boss item not the command essence

**1.0.1**

* Hook fix

**1.0.0**

* Initial Release

## Known Bugs & Compatability Issues

* [Current Issues List](https://github.com/Cercain/ImprovedCommandEssence/issues)

## Future Plans


## Contact for bugs and suggestions

Lilly.Varous#7620 on Discord