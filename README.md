# Now with fixed Compatibility for StarStorm 2, Forgotten Relics, Zet Aspects, & more
(MOD DEVS: for mod compatability, ensure your special item drops are tagged with 'ItemTag.WorldUnique')

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
| Aspect (Orange)  |  2 |
| Void (Purple)  |  2 |
| Void Potential (White Sphere)  |  6 |
| Void Cache (Purple Sphere)  |  4 |

(Boss is used when 'onForYellowBossDrops' is set to true)<br />
(Aspect is used when 'onForAspect' is set to true)<br />
(All void tiers use the same config)<br />
(Lunar items and lunar equipment use the same config)<br/>
(Void Potential and Void Cache config is used when 'onForPotential' is set to false)

#### Toggle Options
| Name             |    default      | Description |
|----------|:-------------:|------------|
| keepCategory   |  true | If true, Category Chests will only contain options of the same category |
| onInBazaar   |  false | If false, item drops in the Bazaar will drop as the actual item not a Command Essence |
| onInDropShip* |  false  | If false, item drops from terminals (like the Drop Ship) will drop as the actual item not a Command Essence |
| onForHidden |  true  | (When 'onInDropShip' is false) If true, hidden (?) item drops from terminals will drop as a Command Essence |
| sameBossDrops | true | If true, the Command Essences dropped from the teleporter boss of the same rarity will contain the same options |
| onForTrophy | false | If false, bosses killed with the Trophy Hunter's Tricorn will drop the related item and not a Command Essence |
| onForAdaptive | false | Set if Adaptive Chests drop their items (false) or Command Essence (true) |
| onForAspect | false | Set if the Aspects dropped by Elites drop as their item (false) or Command Essence (true) |
| onForYellowBossDrops | true | Set if boss items dropped by the teleporter event will drop a Command Essence (true) or their item (false). |
| onForPotential | false | Set if Void Cache and Void Triples drop a Void Potential (false) or a Command Essence (true) |
| onForDelusion | false | Set if the items dropped by the Delusion artifact drop as their item (false) or a Command Essence (true) |
| enableScrappers | false | Set if Scrappers spawn |
| enablePrinters | false | Set if Printers spawn (onInDropShip must be false to drop as the item) |
| enableMultishop | false | Set if Multishops spawn (onInDropShip must be false to drop as the item) |
| scrappersDropEssence | false | Set if Scrappers drop scrap (false) or a Command Essence (true) |
| essenceChance | 100 | Set the chance for items to drop as Command Essences (0-100) |

* (By default the only Terminal Shops that spawn in Command runs are Drop Ships but this toggle affects all of them)

## Known Bugs & Compatibility Issues

* [Current Issues List](https://github.com/Cercain/ImprovedCommandEssence/issues)

## Future Plans


## Contact for bugs and suggestions

Contact through GitHub Issues page to raise bugs and suggestions