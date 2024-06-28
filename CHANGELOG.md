## Changelog

**1.4.5**

* (Bug Fix) Fixed onForVoidPotential causing Void Potentials to have a broken item selection, reverted previous workaround fix forcing cell reward back to a Command Essence of the right rarity

**1.4.4**

* (Bug Fix) Void Fields' Cell rewards now correctly drop a Void Potential (not affected by configs), this is currently the best I can do as the logic around this is really awkward to hook onto and change

**1.4.3**

* (Bug Fix) Fixed non-host players unable to see the Command Essence

**1.4.2**

* (Feature) Added 'onForAspect' & 'itemAmountAspect' to have the option for Aspects to drop as Command Essences containing Aspects
* Changed the way the options are selected to align with vanilla logic

**1.4.1**

* (Bug Fix) Accidentally commented out Aspects dropping as Aspects

**1.4.0**

* (Feature) Added 'onForYellowBossDrops' to have the option for boss drops to give their respective item instead of a command essence
* (Compatability) Further improved compatability with other mods by properly using the 'WorldUnique' item tag rather than Item Index names (custom compatability no longer needed)
* (Bug Fix) Fixed teleporter drop sometimes having the same items when 'sameBossDrops' was false
* (Bug Fix) Fixed Aspect of Earth dropping as a normal equiptment Command Essence
* (Bug Fix) Fixed 'onForHidden' being false forcing all terminals to drop as Command Essence, overwriting 'onForDropShip'

**1.3.6**

* (Bug Fix) Now works with delusion
* (Feature) Added 'onForDelusion' to have delusion drops drop as an essence when true.

**1.3.5**

* (Feature) Added 'onForHidden' which makes the items that display as a ? in terminals drop as a Command Essence (affected by 'essenceChance')
* Reworded some config description to better describe their use

**1.3.4**

* (Compatibility Fix) Forgotten Relics - special item(s) now drop as the item not an essence
* (Feature) Added a new configuration to add custom compatibility to drop any item as the item (see above)

**1.3.3**

* (Compatibility Fix) Fixed 'sameBossDrops' and Starstorm 2 adding white boxes to the drop pool
* (Bug Fix) Fixed 'onForPotential' not being configuration and also affecting non-command runs

**1.3.2**

* (Bug Fix) Void Potentials correctly drop from Simulacrum when enabled

**1.3.1**

* Rebuilt with updated dlls to support v1.2.3

**1.3.0**

* (Feature) Added a toggle to enable Void Potentials (default enables them)
* (Feature) Added options for the amount of items a Void Potential offers (split normal and void cache)
* Removing dependency for [FixedCategoryChests](https://thunderstore.io/package/Cercain/FixedCategoryChests/) as the fix has been made to vanilla

**1.2.1**

* (Compatibility Fix) Fixed the exclusion list to be compatible with mods that add new items

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

* (Compatibility Fix) ZetAspects now works with the Item drop mode

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
