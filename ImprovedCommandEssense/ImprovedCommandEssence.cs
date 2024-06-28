using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using EntityStates.Scrapper;
using RoR2;
using RoR2.Artifacts;
using UnityEngine;
using UnityEngine.Networking;

namespace ImprovedCommandEssence
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]    
    public class ImprovedCommandEssence : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Cercain";
        public const string PluginName = "ImprovedCommandEssence";
        public const string PluginVersion = "1.4.4";

        public static ConfigFile configFile = new ConfigFile(Paths.ConfigPath + "\\ImprovedCommandEssence.cfg", true);

        public static ConfigEntry<int> itemAmountCommon { get; set; }
        public static ConfigEntry<int> itemAmountUncommon { get; set; }
        public static ConfigEntry<int> itemAmountLegendary { get; set; }
        public static ConfigEntry<int> itemAmountLunar { get; set; }
        public static ConfigEntry<int> itemAmountVoid { get; set; }
        public static ConfigEntry<int> itemAmountEquip { get; set; }
        public static ConfigEntry<int> itemAmountAspect { get; set; }
        public static ConfigEntry<int> itemAmountPotential { get; set; }
        public static ConfigEntry<int> itemAmountPotentialCache { get; set; }
        public static ConfigEntry<int> itemAmountBoss { get; set; }
        public static ConfigEntry<bool> keepCategory { get; set; }
        public static ConfigEntry<bool> onInBazaar { get; set; }
        public static ConfigEntry<bool> onInDropShip { get; set; }
        public static ConfigEntry<bool> onForHidden { get; set; }
        public static ConfigEntry<bool> onForAdaptive { get; set; }
        public static ConfigEntry<bool> onForYellowBossDrops { get; set; }
        public static ConfigEntry<bool> onForPotential { get; set; }
        public static ConfigEntry<bool> onForDelusion { get; set; }
        public static ConfigEntry<bool> onForAspect { get; set; }
        public static ConfigEntry<bool> sameBossDrops { get; set; }
        public static ConfigEntry<bool> onForTrophy { get; set; }
        public static ConfigEntry<bool> enableScrappers { get; set; }
        public static ConfigEntry<bool> enablePrinters { get; set; }
        public static ConfigEntry<bool> enableMultishops { get; set; }
        public static ConfigEntry<bool> scrappersDropEssence { get; set; }
        public static ConfigEntry<int> essenceChance { get; set; }
        public static ConfigEntry<string> customCompatibility { get; set; }

        void ConfigOnSettingChanged(object sender, SettingChangedEventArgs e)
        {
            if (itemAmountCommon.Value <= 0)
                itemAmountCommon.Value = 1;

            if (itemAmountUncommon.Value <= 0)
                itemAmountUncommon.Value = 1;

            if (itemAmountLegendary.Value <= 0)
                itemAmountLegendary.Value = 1;

            if (itemAmountLunar.Value <= 0)
                itemAmountLunar.Value = 1;

            if (itemAmountVoid.Value <= 0)
                itemAmountVoid.Value = 1;

            if (itemAmountEquip.Value <= 0)
                itemAmountEquip.Value = 1;

            if (essenceChance.Value > 100)
                essenceChance.Value = 100;
            if (essenceChance.Value < 0)
                essenceChance.Value = 0;
        }

        public void OnEnable()
        {
            itemAmountCommon = configFile.Bind("ImprovedCommandEssence", "itemAmountCommon", 6, new ConfigDescription("Set the amount of Common (white) options shown when opening White Command Essences."));
            itemAmountUncommon = configFile.Bind("ImprovedCommandEssence", "itemAmountUncommon", 4, new ConfigDescription("Set the amount of Uncommon (Green) options shown when opening Green Command Essences."));
            itemAmountLegendary = configFile.Bind("ImprovedCommandEssence", "itemAmountLegendary", 2, new ConfigDescription("Set the amount of Legendary (Red) options shown when opening Red Command Essences."));
            itemAmountBoss = configFile.Bind("ImprovedCommandEssence", "itemAmountBoss", 2, new ConfigDescription("Set the amount of Boss (Yellow) items shown when opening Yellow Command Essences with 'onForYellowBossDrops' set to true."));
            itemAmountLunar = configFile.Bind("ImprovedCommandEssence", "itemAmountLunar", 2, new ConfigDescription("Set the amount of Blue items shown when opening Lunar Command Essences."));
            itemAmountVoid = configFile.Bind("ImprovedCommandEssence", "itemAmountVoid", 2, new ConfigDescription("Set the amount of Blue items shown when opening Void Command Essences."));
            itemAmountEquip = configFile.Bind("ImprovedCommandEssence", "itemAmountEquip", 4, new ConfigDescription("Set the amount of Orange items shown when opening Equiptment Command Essences."));
            itemAmountAspect = configFile.Bind("ImprovedCommandEssence", "itemAmountAspect", 2, new ConfigDescription("Set the amount of Aspect items shown when opening Aspect Command Essences."));
            itemAmountPotential = configFile.Bind("ImprovedCommandEssence", "itemAmountPotential", 6, new ConfigDescription("Set the amount of items shown when opening a Void Potential from a Void Cache."));
            itemAmountPotentialCache = configFile.Bind("ImprovedCommandEssence", "itemAmountPotentialCache", 4, new ConfigDescription("Set the amount of items shown when opening a Void Potential from a Void Triple."));
            essenceChance = configFile.Bind("ImprovedCommandEssence", "essenceChance", 100, new ConfigDescription("Set the chance for item drops to drop as a Command Essence (0-100) from non-explicit chests"));
            keepCategory = configFile.Bind("ImprovedCommandEssence", "keepCategory", true, new ConfigDescription("Set if category chests only show items of the corresponding category."));
            onInBazaar = configFile.Bind("ImprovedCommandEssence", "onInBazaar", false, new ConfigDescription("Set if the Command Artifact is turn on in the Bazaar."));
            onInDropShip = configFile.Bind("ImprovedCommandEssence", "onInDropShip", false, new ConfigDescription("Set if the Command Artifact is turn on for Drop Ship items."));
            onForHidden = configFile.Bind("ImprovedCommandEssence", "onForHidden", true, new ConfigDescription("When 'onInDropShip' is false, set if hidden (?) items drop as a Command Essence or the hidden item."));
            onForAdaptive = configFile.Bind("ImprovedCommandEssence", "onForAdaptive", false, new ConfigDescription("Set if Adaptive Chests will drop a Command Essense (true) or their item (false)."));
            onForYellowBossDrops = configFile.Bind("ImprovedCommandEssence", "onForYellowBossDrops", true, new ConfigDescription("Set if boss items dropped by the teleporter event will drop a Command Essence (true) or their item (false)."));
            onForPotential = configFile.Bind("ImprovedCommandEssence", "onForPotential", false, new ConfigDescription("Set if the Command Artifact is turn on for Void Potentials and Void Caches."));
            onForDelusion = configFile.Bind("ImprovedCommandEssence", "onForDelusion", false, new ConfigDescription("Set if the items dropped by the Delusion artifact drop as their item or a Command Essence."));
            onForAspect = configFile.Bind("ImprovedCommandEssence", "onForAspect", false, new ConfigDescription("Set if the Aspects dropped by Elites drop as their item or a Command Essence."));
            sameBossDrops = configFile.Bind("ImprovedCommandEssence", "sameBossDrops", true, new ConfigDescription("Set if the Command Essences that drop from the Teleporter boss give the same options."));
            onForTrophy = configFile.Bind("ImprovedCommandEssence", "onForTrophy", false, new ConfigDescription("Set if the item dropped by bosses killed via Trophy Hunter's Tricorn drop as a Command Essence (true) or the boss item (false)"));
            enableScrappers = configFile.Bind("ImprovedCommandEssence", "enableScrappers", false, new ConfigDescription("Set if Scrappers spawn"));
            enablePrinters = configFile.Bind("ImprovedCommandEssence", "enablePrinters", false, new ConfigDescription("Set if Printers spawn (onInDropShip must be on to drop as the item)"));
            enableMultishops = configFile.Bind("ImprovedCommandEssence", "enableTerminals", false, new ConfigDescription("Set if Multishops spawn (onInDropShip must be on to drop as the item)"));
            scrappersDropEssence = configFile.Bind("ImprovedCommandEssence", "scrappersDropEssence", false, new ConfigDescription("Set if Scrappers drop scrap (false) or Command Essence (true)"));

            Config.SettingChanged += ConfigOnSettingChanged;
            On.RoR2.PickupPickerController.SetOptionsFromPickupForCommandArtifact += SetOptions;

            On.RoR2.ChestBehavior.BaseItemDrop += BaseItemDrop;
            On.RoR2.PickupDropletController.OnCollisionEnter += DropletCollisionEnter;
            On.RoR2.BossGroup.DropRewards += BossGroupDropRewards;

            if (!onInDropShip.Value)
                On.RoR2.ShopTerminalBehavior.DropPickup += TerminalDrop;

            if (!onForAdaptive.Value)
                On.RoR2.RouletteChestController.EjectPickupServer += RouletteEject;

            if (!onForTrophy.Value)
                On.RoR2.EquipmentSlot.FireBossHunter += BossHunterDrop;

            if (enableScrappers.Value || enableMultishops.Value || enablePrinters.Value)
                On.RoR2.Artifacts.CommandArtifactManager.OnGenerateInteractableCardSelection += OnGenCommandCards;

            if (!scrappersDropEssence.Value)
                On.EntityStates.Scrapper.ScrappingToIdle.OnEnter += ScrapperDrop;

            if (!onForPotential.Value)
            {
                On.RoR2.OptionChestBehavior.Roll += PotentialRoll;
            }

            On.RoR2.OptionChestBehavior.ItemDrop += OptionItemDrop;
        }


        [Server]
        public void PotentialRoll(On.RoR2.OptionChestBehavior.orig_Roll orig, RoR2.OptionChestBehavior self)
        {
            if (!RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.commandArtifactDef))
            {
                orig(self);
                return;
            }

            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void RoR2.OptionChestBehavior::Roll()' called on client");
                return;
            }

            if(self.gameObject.GetComponent<GenericDisplayNameProvider>().displayToken == "LOCKBOXVOID_NAME")                
                self.generatedDrops = self.dropTable.GenerateUniqueDrops(itemAmountPotentialCache.Value, self.rng);
            else
                self.generatedDrops = self.dropTable.GenerateUniqueDrops(itemAmountPotential.Value, self.rng);
        }

        [Server]
        public void OptionItemDrop(On.RoR2.OptionChestBehavior.orig_ItemDrop orig, RoR2.OptionChestBehavior self)
        {
            if (!RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.commandArtifactDef))
            {
                orig(self);
                return;
            }

            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void RoR2.OptionChestBehavior::ItemDrop()' called on client");
                return;
            }
            if (self.generatedDrops == null || self.generatedDrops.Length == 0)
            {
                return;
            }

            var track = new TrackBehaviour() { PickupSource = PickupSource.VoidPotential };
            CreatePickupDroplet(new GenericPickupController.CreatePickupInfo
            {
                pickerOptions = PickupPickerController.GenerateOptionsFromArray(self.generatedDrops),
                prefabOverride = self.pickupPrefab,
                position = self.dropTransform.position,
                rotation = Quaternion.identity,
                pickupIndex = PickupCatalog.FindPickupIndex(self.displayTier),
            }, Vector3.up * self.dropUpVelocityStrength + self.dropTransform.forward * self.dropForwardVelocityStrength,
            track);
            self.generatedDrops = null;
        }

        private bool BossHunterDrop(On.RoR2.EquipmentSlot.orig_FireBossHunter orig, RoR2.EquipmentSlot self)
        {
            if (!RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.commandArtifactDef))
            {
                return orig(self);
            }

            self.UpdateTargets(DLC1Content.Equipment.BossHunter.equipmentIndex, true);
            HurtBox hurtBox = self.currentTarget.hurtBox;
            DeathRewards deathRewards;
            if (hurtBox == null)
            {
                deathRewards = null;
            }
            else
            {
                HealthComponent healthComponent = hurtBox.healthComponent;
                if (healthComponent == null)
                {
                    deathRewards = null;
                }
                else
                {
                    CharacterBody body = healthComponent.body;
                    if (body == null)
                    {
                        deathRewards = null;
                    }
                    else
                    {
                        GameObject gameObject = body.gameObject;
                        deathRewards = ((gameObject != null) ? gameObject.GetComponent<DeathRewards>() : null);
                    }
                }
            }
            DeathRewards deathRewards2 = deathRewards;
            if (hurtBox && deathRewards2)
            {
                var tracking = new TrackBehaviour();
                tracking.ItemTag = ItemTag.Any;
                tracking.PickupSource = PickupSource.BossHunter;
                CharacterBody body2 = hurtBox.healthComponent.body;
                Vector3 vector = body2 ? body2.corePosition : Vector3.zero;
                Vector3 normalized = (vector - self.characterBody.corePosition).normalized;
                CreatePickupDroplet(deathRewards2.bossDropTable.GenerateDrop(self.rng), vector, normalized * 15f, tracking);
                UnityEngine.Object exists;
                if (hurtBox == null)
                {
                    exists = null;
                }
                else
                {
                    HealthComponent healthComponent2 = hurtBox.healthComponent;
                    if (healthComponent2 == null)
                    {
                        exists = null;
                    }
                    else
                    {
                        CharacterBody body3 = healthComponent2.body;
                        exists = ((body3 != null) ? body3.master : null);
                    }
                }
                if (exists)
                {
                    hurtBox.healthComponent.body.master.TrueKill(base.gameObject, null, DamageType.Generic);
                }
                CharacterModel component = hurtBox.hurtBoxGroup.GetComponent<CharacterModel>();
                if (component)
                {
                    TemporaryOverlay temporaryOverlay = component.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 0.1f;
                    temporaryOverlay.animateShaderAlpha = true;
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashBright");
                    temporaryOverlay.AddToCharacerModel(component);
                    TemporaryOverlay temporaryOverlay2 = component.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay2.duration = 1.2f;
                    temporaryOverlay2.animateShaderAlpha = true;
                    temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay2.destroyComponentOnEnd = true;
                    temporaryOverlay2.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matGhostEffect");
                    temporaryOverlay2.AddToCharacerModel(component);
                }
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.attacker = base.gameObject;
                damageInfo.force = -normalized * 2500f;
                self.healthComponent.TakeDamageForce(damageInfo, true, false);
                GameObject effectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BossHunterKillEffect");
                Quaternion rotation = Util.QuaternionSafeLookRotation(normalized, Vector3.up);
                EffectManager.SpawnEffect(effectPrefab, new EffectData
                {
                    origin = vector,
                    rotation = rotation
                }, true);
                ModelLocator component2 = base.gameObject.GetComponent<ModelLocator>();
                CharacterModel characterModel;
                if (component2 == null)
                {
                    characterModel = null;
                }
                else
                {
                    Transform modelTransform = component2.modelTransform;
                    characterModel = ((modelTransform != null) ? modelTransform.GetComponent<CharacterModel>() : null);
                }
                CharacterModel characterModel2 = characterModel;
                if (characterModel2)
                {
                    foreach (GameObject gameObject2 in characterModel2.GetEquipmentDisplayObjects(DLC1Content.Equipment.BossHunter.equipmentIndex))
                    {
                        if (gameObject2.name.Contains("DisplayTricorn"))
                        {
                            EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BossHunterHatEffect"), new EffectData
                            {
                                origin = gameObject2.transform.position,
                                rotation = gameObject2.transform.rotation,
                                scale = gameObject2.transform.localScale.x
                            }, true);
                        }
                        else
                        {
                            EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BossHunterGunEffect"), new EffectData
                            {
                                origin = gameObject2.transform.position,
                                rotation = Util.QuaternionSafeLookRotation(vector - gameObject2.transform.position, Vector3.up),
                                scale = gameObject2.transform.localScale.x
                            }, true);
                        }
                    }
                }
                CharacterBody characterBody = self.characterBody;
                if ((characterBody != null) ? characterBody.inventory : null)
                {
                    CharacterMasterNotificationQueue.PushEquipmentTransformNotification(self.characterBody.master, self.characterBody.inventory.currentEquipmentIndex, DLC1Content.Equipment.BossHunterConsumed.equipmentIndex, CharacterMasterNotificationQueue.TransformationType.Default);
                    self.characterBody.inventory.SetEquipmentIndex(DLC1Content.Equipment.BossHunterConsumed.equipmentIndex);
                }
                self.InvalidateCurrentTarget();
                return true;
            }
            return false;
        }

        private void BossGroupDropRewards(On.RoR2.BossGroup.orig_DropRewards orig, RoR2.BossGroup self)
        {
            if (!RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.commandArtifactDef))
            {
                orig(self);
                return;
            }
            if (!Run.instance)
            {
                Debug.LogError("No valid run instance!");
                return;
            }
            if (self.rng == null)
            {
                Debug.LogError("RNG is null!");
                return;
            }

            int participatingPlayerCount = Run.instance.participatingPlayerCount;
            if (participatingPlayerCount != 0)
            {
                if (self.dropPosition)
                {
                    PickupPickerController.Option[] options;
                    PickupIndex pickupIndex = PickupIndex.none;
                    if (self.dropTable)
                    {
                        pickupIndex = self.dropTable.GenerateDrop(self.rng);
                        options = PickupPickerController.GetOptionsFromPickupIndex(pickupIndex);
                    }
                    else
                    {
                        var list = Run.instance.availableTier2DropList;
                        if (self.forceTier3Reward)
                        {
                            list = Run.instance.availableTier3DropList;
                        }
                        pickupIndex = self.rng.NextElementUniform<PickupIndex>(list);
                        options = PickupPickerController.GetOptionsFromPickupIndex(pickupIndex);
                    }

                    int itemAmount = GetItemAmountFromTier(options[0].pickupIndex.pickupDef);
                    options = (from x in options.ToList() orderby self.rng.Next() select x).Take(itemAmount).ToArray();

                    int num = 1 + self.bonusRewardCount;
                    if (self.scaleRewardsByPlayerCount)
                    {
                        num *= participatingPlayerCount;
                    }
                    float angle = 360f / (float)num;
                    Vector3 vector = Quaternion.AngleAxis((float)UnityEngine.Random.Range(0, 360), Vector3.up) * (Vector3.up * 40f + Vector3.forward * 5f);
                    Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
                    bool flag = self.bossDrops != null && self.bossDrops.Count > 0;
                    bool flag2 = self.bossDropTables != null && self.bossDropTables.Count > 0;
                    int i = 0;
                    PickupPickerController.Option[] yellowOptions = null;
                    while (i < num)
                    {
                        var track = new TrackBehaviour();
                        track.PickupSource = PickupSource.Boss;

                        var options2 = options;
                        PickupIndex pickupIndex2 = pickupIndex;
                        if (self.bossDrops != null && ((flag || flag2) && self.rng.nextNormalizedFloat <= self.bossDropChance))
                        {
                            track.PickupSource = PickupSource.YellowBoss;
                            Debug.Log($"Generated Boss Rewards");
                            if (flag2)
                            {
                                PickupDropTable pickupDropTable = self.rng.NextElementUniform<PickupDropTable>(self.bossDropTables);
                                if (pickupDropTable != null)
                                {
                                    pickupIndex2 = pickupDropTable.GenerateDrop(self.rng);
                                }
                            }
                            else
                            {
                                pickupIndex2 = self.rng.NextElementUniform<PickupIndex>(self.bossDrops);
                            }

                            if (onForYellowBossDrops.Value)
                            {
                                if (sameBossDrops.Value)
                                {
                                    if (yellowOptions == null)
                                        yellowOptions = (from x in PickupPickerController.GetOptionsFromPickupIndex(pickupIndex2) orderby self.rng.Next() select x).Take(itemAmountBoss.Value).ToArray();

                                    options2 = yellowOptions;
                                }
                                else
                                {
                                    options2 = (from x in PickupPickerController.GetOptionsFromPickupIndex(pickupIndex2) orderby self.rng.Next() select x).Take(itemAmountBoss.Value).ToArray();
                                }                                
                            }
                            else
                            {
                                options2 = [ new PickupPickerController.Option {
                                        available = Run.instance.IsPickupAvailable(pickupIndex2),
                                        pickupIndex = pickupIndex2
                                }];
                            }
                        }
                        
                        track.ItemTag = ItemTag.Any;
                        track.Options = options2;

                        CreatePickupDroplet(pickupIndex2, self.dropPosition.position, vector, track);
                        i++;
                        vector = rotation * vector;
                    }
                    return;
                }
                Debug.LogWarning("dropPosition not set for BossGroup! No item will be spawned.");
            }
        }

        private static void OnGenCommandCards(On.RoR2.Artifacts.CommandArtifactManager.orig_OnGenerateInteractableCardSelection orig, SceneDirector sceneDirector, DirectorCardCategorySelection dccs)
        {
            dccs.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(CheckCardsFilter));
        }

        private static bool CheckCardsFilter(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            if (!enableMultishops.Value)
                if (prefab.GetComponent<MultiShopController>())
                    return false;
            if (!enableScrappers.Value)
                if (prefab.GetComponent<ScrapperController>())
                    return false;
            if (!enablePrinters.Value)
                if (prefab.GetComponent<ShopTerminalBehavior>())
                    return false;

            return true;
        }
        
        void ScrapperDrop(On.EntityStates.Scrapper.ScrappingToIdle.orig_OnEnter orig, EntityStates.Scrapper.ScrappingToIdle self)
        {
            if (!RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.commandArtifactDef))
            {
                orig(self);
                return;
            }
            //base
            {
                if (self.characterBody)
                {
                    self.attackSpeedStat = self.characterBody.attackSpeed;
                    self.damageStat = self.characterBody.damage;
                    self.critStat = self.characterBody.crit;
                    self.moveSpeedStat = self.characterBody.moveSpeed;
                }
                self.pickupPickerController = self.GetComponent<PickupPickerController>();
                self.scrapperController = self.GetComponent<ScrapperController>();
                self.pickupPickerController.SetAvailable(self.enableInteraction);

            }
            Util.PlaySound(ScrappingToIdle.enterSoundString, self.gameObject);
            self.PlayAnimation("Base", "ScrappingToIdle", "Scrapping.playbackRate", ScrappingToIdle.duration);
            if (ScrappingToIdle.muzzleflashEffectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(ScrappingToIdle.muzzleflashEffectPrefab, self.gameObject, ScrappingToIdle.muzzleString, false);
            }
            if (!NetworkServer.active)
            {
                return;
            }
            self.foundValidScrap = false;
            PickupIndex pickupIndex = PickupIndex.none;
            ItemDef itemDef = ItemCatalog.GetItemDef(self.scrapperController.lastScrappedItemIndex);
            if (itemDef != null)
            {
                switch (itemDef.tier)
                {
                    case ItemTier.Tier1:
                        pickupIndex = PickupCatalog.FindPickupIndex("ItemIndex.ScrapWhite");
                        break;
                    case ItemTier.Tier2:
                        pickupIndex = PickupCatalog.FindPickupIndex("ItemIndex.ScrapGreen");
                        break;
                    case ItemTier.Tier3:
                        pickupIndex = PickupCatalog.FindPickupIndex("ItemIndex.ScrapRed");
                        break;
                    case ItemTier.Boss:
                        pickupIndex = PickupCatalog.FindPickupIndex("ItemIndex.ScrapYellow");
                        break;
                }
            }
            if (pickupIndex != PickupIndex.none)
            {
                self.foundValidScrap = true;
                Transform transform = self.FindModelChild(ScrappingToIdle.muzzleString);

                var track = new TrackBehaviour();
                track.PickupSource = PickupSource.Scrapper;

                CreatePickupDroplet(pickupIndex, transform.position, Vector3.up * ScrappingToIdle.dropUpVelocityStrength + transform.forward * ScrappingToIdle.dropForwardVelocityStrength, track);
                ScrapperController scrapperController = self.scrapperController;
                int itemsEaten = scrapperController.itemsEaten;
                scrapperController.itemsEaten = itemsEaten - 1;
            }
        }

        void RouletteEject(On.RoR2.RouletteChestController.orig_EjectPickupServer orig, RoR2.RouletteChestController self, PickupIndex pickupIndex)
        {
            if (!RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.commandArtifactDef))
            {
                orig(self,pickupIndex);
                return;
            }
            if (pickupIndex == PickupIndex.none)
            {
                return;
            }
            var track = new TrackBehaviour();
            track.PickupSource = PickupSource.Roulette;
            CreatePickupDroplet(pickupIndex, self.ejectionTransform.position, self.ejectionTransform.rotation * self.localEjectionVelocity, track);
        }

        [Server]
        public void BaseItemDrop(On.RoR2.ChestBehavior.orig_BaseItemDrop orig, RoR2.ChestBehavior self)
        {
            if (!RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.commandArtifactDef))
            {
                orig(self);
                return;
            }

            if (!NetworkServer.active)
            {
                return;
            }
            if (self.dropPickup == PickupIndex.none || self.dropCount < 1)
            {
                return;
            }

            float angle = 360f / (float)self.dropCount;
            Vector3 vector = Vector3.up * self.dropUpVelocityStrength + self.dropTransform.forward * self.dropForwardVelocityStrength;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);

            var track = new TrackBehaviour() { ItemTag = self.requiredItemTag, DropTable = self.dropTable, PickupSource = PickupSource.Chest };

            for (int i = 0; i < self.dropCount; i++)
            {
                CreatePickupDroplet(new GenericPickupController.CreatePickupInfo
                {
                    pickupIndex = self.dropPickup,
                    position = self.dropTransform.position + Vector3.up * 1.5f,
                    chest = self,
                    artifactFlag = (self.isCommandChest ? GenericPickupController.PickupArtifactFlag.COMMAND : GenericPickupController.PickupArtifactFlag.NONE)
                }, vector, track);
                vector = rotation * vector;
                //self.Roll();
            }
            self.dropPickup = PickupIndex.none;
        }

        [Server]
        void TerminalDrop(On.RoR2.ShopTerminalBehavior.orig_DropPickup orig, RoR2.ShopTerminalBehavior self)
        {
            if (!RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.commandArtifactDef))
            {
                orig(self);
                return;
            }

            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void RoR2.ShopTerminalBehavior::DropPickup()' called on client");
                return;
            }

            TrackBehaviour track = new TrackBehaviour();

            if (!(onForHidden.Value && self.hidden))
            {
                track.PickupSource = PickupSource.Terminal;
            }
            self.SetHasBeenPurchased(true);
            CreatePickupDroplet(self.pickupIndex, (self.dropTransform ? self.dropTransform : self.transform).position, self.transform.TransformVector(self.dropVelocity), track);

        }

        private void CreatePickupDroplet(PickupIndex pickupIndex, Vector3 position, Vector3 velocity, TrackBehaviour track)
        {
            CreatePickupDroplet(new GenericPickupController.CreatePickupInfo
            {
                rotation = Quaternion.identity,
                position = position,
                pickupIndex = pickupIndex
            }, velocity, track);
        }

        public void CreatePickupDroplet(GenericPickupController.CreatePickupInfo pickupInfo,  Vector3 velocity, TrackBehaviour track)
        {
            if (CommandArtifactManager.IsCommandArtifactEnabled)
            {
                pickupInfo.artifactFlag &= ~GenericPickupController.PickupArtifactFlag.COMMAND;
            }
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(PickupDropletController.pickupDropletPrefab, pickupInfo.position, Quaternion.identity);
            PickupDropletController component = gameObject.GetComponent<PickupDropletController>();
            var behav = gameObject.AddComponent<TrackBehaviour>();

            behav.PickupSource = track.PickupSource;
            behav.DropTable = track.DropTable;
            behav.ItemTag = track.ItemTag;
            behav.Options = track.Options;
            if (component)
            {
                component.createPickupInfo = pickupInfo;
                component.NetworkpickupIndex = pickupInfo.pickupIndex;
            }

            Rigidbody component2 = gameObject.GetComponent<Rigidbody>();
            component2.velocity = velocity;
            component2.AddTorque(UnityEngine.Random.Range(150f, 120f) * UnityEngine.Random.onUnitSphere);
            NetworkServer.Spawn(gameObject);
        }

        public void DropletCollisionEnter(On.RoR2.PickupDropletController.orig_OnCollisionEnter orig, RoR2.PickupDropletController self, Collision collision)
        {
            if (!RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.commandArtifactDef))
            {
                orig(self, collision);
                return;
            }

            if (NetworkServer.active && self.alive)
            {
                List<EquipmentDef> eliteEquipIds = new List<EquipmentDef>() { RoR2Content.Equipment.AffixBlue, RoR2Content.Equipment.AffixHaunted, RoR2Content.Equipment.AffixLunar, RoR2Content.Equipment.AffixPoison, RoR2Content.Equipment.AffixRed, RoR2Content.Equipment.AffixWhite, DLC1Content.Equipment.EliteVoidEquipment, DLC1Content.Elites.Earth.eliteEquipmentDef };
                //List<ItemDef> specialDropIds = new List<ItemDef>() { RoR2Content.Items.TitanGoldDuringTP, RoR2Content.Items.ArtifactKey, RoR2Content.Items.Pearl, RoR2Content.Items.ShinyPearl };

                Logger.LogInfo($"Droplet {self.pickupIndex}:{self.pickupIndex.pickupDef.itemIndex}:{self.pickupIndex.pickupDef.equipmentIndex}");
                self.alive = false;
                self.createPickupInfo.position = self.transform.position;
                bool flag = true;

                var trackBool = self.gameObject.TryGetComponent<TrackBehaviour>(out var track);
                var pi = PickupCatalog.GetPickupDef(self.pickupIndex);
                bool isWorldUnique = false;

                if ((int)pi.itemIndex != -1)
                {
                    var itemDef = ItemCatalog.GetItemDef(pi.itemIndex);
                    isWorldUnique = itemDef.ContainsTag(ItemTag.WorldUnique);
                }

                bool doChance = UnityEngine.Random.Range(0, 100f) < essenceChance.Value;
                if (!doChance ||
                    isWorldUnique ||
                    (!onForDelusion.Value && self.createPickupInfo.artifactFlag.HasFlag(GenericPickupController.PickupArtifactFlag.DELUSION)) ||
                    (!onInBazaar.Value && BazaarController.instance != null) ||
                    (!scrappersDropEssence.Value && trackBool && track.PickupSource == PickupSource.Scrapper) ||
                    (!onForAdaptive.Value && trackBool && track.PickupSource == PickupSource.Roulette) ||
                    (!onInDropShip.Value && trackBool && track.PickupSource == PickupSource.Terminal) ||
                    (!onForTrophy.Value && trackBool && track.PickupSource == PickupSource.BossHunter) ||
                    (!onForPotential.Value && trackBool && track.PickupSource == PickupSource.VoidPotential) ||
                    (self.pickupIndex.pickupDef == null || (self.pickupIndex.pickupDef.itemIndex == ItemIndex.None && self.pickupIndex.pickupDef.equipmentIndex == EquipmentIndex.None && self.pickupIndex.pickupDef.itemTier == ItemTier.NoTier)) ||
                    (!onForAspect.Value && eliteEquipIds.Any(x => x.equipmentIndex == self.pickupIndex.pickupDef.equipmentIndex)) //||
                    //(specialDropIds.Any(x => x.itemIndex == self.pickupIndex.pickupDef.itemIndex)) //||
                    //(crossModCompatibility.Contains(self.pickupIndex.ToString()))
                    )
                {
                    GenericPickupController.CreatePickup(self.createPickupInfo);
                }
                else if(trackBool && track.Options.Count() == 1)
                {
                    self.createPickupInfo.pickerOptions = track.Options;
                    GenericPickupController.CreatePickup(self.createPickupInfo);
                }
                else
                    CreateCommandCube(ref self.createPickupInfo, ref flag, track);

                UnityEngine.Object.Destroy(self.gameObject);
            }
        }

        private void CreateCommandCube(ref GenericPickupController.CreatePickupInfo createPickupInfo, ref bool shouldSpawn, TrackBehaviour trackBehaviour = null)
        {
            PickupIndex pickupIndex = createPickupInfo.pickupIndex;
            PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);

            Logger.LogInfo($"PickupDef {pickupDef.internalName}:{pickupDef.itemTier}:{pickupDef.itemIndex}");
            if (pickupDef == null || (pickupDef.itemIndex == ItemIndex.None && pickupDef.equipmentIndex == EquipmentIndex.None && pickupDef.itemTier == ItemTier.NoTier))
            {
                return;
            }
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(CommandArtifactManager.commandCubePrefab, createPickupInfo.position, createPickupInfo.rotation);

            if (trackBehaviour != null)
            {
                var track = gameObject.AddComponent<TrackBehaviour>();
                track.ItemTag = trackBehaviour.ItemTag;
                track.DropTable = trackBehaviour.DropTable;
                track.PickupSource = trackBehaviour.PickupSource;
                track.Options = trackBehaviour.Options;
            }
            gameObject.GetComponent<PickupIndexNetworker>().NetworkpickupIndex = pickupIndex;
            PickupPickerController component = gameObject.GetComponent<PickupPickerController>();
            component.SetOptionsFromPickupForCommandArtifact(pickupIndex);
            component.chestGeneratedFrom = createPickupInfo.chest;

            NetworkServer.Spawn(gameObject);
            shouldSpawn = false;
        }

        [Server]
        void SetOptions(On.RoR2.PickupPickerController.orig_SetOptionsFromPickupForCommandArtifact orig, RoR2.PickupPickerController self, PickupIndex pickupIndex)
        {
            var tracking = self.gameObject.GetComponent<TrackBehaviour>();

            if (!NetworkServer.active) return;

            int itemAmount = GetItemAmountFromTier(pickupIndex.pickupDef);

            PickupIndex[] itemSelection = null;
            if (tracking != null && keepCategory.Value && (tracking.ItemTag == ItemTag.Damage || tracking.ItemTag == ItemTag.Healing || tracking.ItemTag == ItemTag.Utility))
            {
                var choices = (tracking.DropTable as BasicPickupDropTable).selector.choices;
                itemSelection = choices.Where(x => x.value.pickupDef.itemTier == pickupIndex.pickupDef.itemTier).Select(x => x.value).ToArray();
            }
            else if(tracking != null && sameBossDrops.Value && tracking.PickupSource == PickupSource.Boss)
            {
                self.SetOptionsServer(tracking.Options);
                return;
            }
            else if (tracking != null && tracking.PickupSource == PickupSource.YellowBoss)
            {
                self.SetOptionsServer(tracking.Options);
                return;
            }
            else
                switch (pickupIndex.pickupDef.itemTier)
                {
                    case ItemTier.Tier1:
                        itemSelection = Run.instance.availableTier1DropList.ToArray();
                        break;
                    case ItemTier.Tier2:
                        itemSelection = Run.instance.availableTier2DropList.ToArray();
                        break;
                    case ItemTier.Tier3:
                        itemSelection = Run.instance.availableTier3DropList.ToArray();
                        break;
                    case ItemTier.Boss:
                        itemSelection = Run.instance.availableBossDropList.ToArray();
                        break;
                    case ItemTier.Lunar:
                        itemSelection = Run.instance.availableLunarItemDropList.ToArray();
                        break;
                    case ItemTier.VoidTier1:
                        itemSelection = Run.instance.availableVoidTier1DropList.ToArray();
                        break;
                    case ItemTier.VoidTier2:
                        itemSelection = Run.instance.availableVoidTier2DropList.ToArray();
                        break;
                    case ItemTier.VoidTier3:
                        itemSelection = Run.instance.availableVoidTier3DropList.ToArray();
                        break;
                    case ItemTier.VoidBoss:
                        itemSelection = Run.instance.availableVoidBossDropList.ToArray();
                        break;
                    case ItemTier.NoTier:
                        if (pickupIndex.pickupDef.equipmentIndex != EquipmentIndex.None)
                            if (pickupIndex.pickupDef.isLunar)
                                itemSelection = Run.instance.availableLunarEquipmentDropList.ToArray();
                            else
                                itemSelection = Run.instance.availableEquipmentDropList.ToArray();
                        break;
                }

            List<EquipmentDef> eliteEquipIds = new List<EquipmentDef>() { RoR2Content.Equipment.AffixBlue, RoR2Content.Equipment.AffixHaunted, RoR2Content.Equipment.AffixLunar, RoR2Content.Equipment.AffixPoison, RoR2Content.Equipment.AffixRed, RoR2Content.Equipment.AffixWhite, DLC1Content.Elites.Earth.eliteEquipmentDef };
            if (onForAspect.Value && eliteEquipIds.Any(x => x.equipmentIndex == pickupIndex.pickupDef.equipmentIndex))
            {
                itemSelection = eliteEquipIds.Select(x => PickupCatalog.FindPickupIndex(x.equipmentIndex)).ToArray();
                itemAmount = itemAmountAspect.Value;
            }

            PickupPickerController.Option[] options;

            if (itemSelection == null)
            {
                options = new PickupPickerController.Option[1]
                {
                    new PickupPickerController.Option
                    {
                        available = true,
                        pickupIndex = pickupIndex
                    }
                };
            }
            else
            {
                var rng = Run.instance.treasureRng;

                //System.Random rnd = new System.Random();
                List<PickupIndex> indexList = (from x in itemSelection.ToList() orderby rng.Next() select x).Take(itemAmount).ToList();
                options = new PickupPickerController.Option[indexList.Count];

                for (int i = 0; i < indexList.Count; i++)
                {
                    PickupIndex index = indexList[i];
                    options[i] = new PickupPickerController.Option
                    {
                        available = true,
                        pickupIndex = index
                    };
                }
            }

            self.SetOptionsServer(options);
        }

        public int GetItemAmountFromTier(PickupDef pickup)
        {
            int itemAmount = 0;
            switch (pickup.itemTier)
            {
                case ItemTier.Tier1:
                    itemAmount = itemAmountCommon.Value;
                    break;
                case ItemTier.Tier2:
                    itemAmount = itemAmountUncommon.Value;
                    break;
                case ItemTier.Tier3:
                    itemAmount = itemAmountLegendary.Value;
                    break;
                case ItemTier.Boss:
                    itemAmount = itemAmountBoss.Value;
                    break;
                case ItemTier.Lunar:
                    itemAmount = itemAmountLunar.Value;
                    break;
                case ItemTier.NoTier:
                    if (pickup.equipmentIndex != EquipmentIndex.None)
                        itemAmount = itemAmountEquip.Value;
                    break;
                case ItemTier.VoidTier1:
                case ItemTier.VoidTier2:
                case ItemTier.VoidTier3:
                case ItemTier.VoidBoss:
                    itemAmount = itemAmountVoid.Value;
                    break;
                default:
                    itemAmount = itemAmountCommon.Value;
                    break;
            }
            return itemAmount;
        }
    }
    
    public class TrackBehaviour : MonoBehaviour
    {
        public ItemTag ItemTag { get; set; }
        public PickupDropTable DropTable { get; set; }
        public PickupSource PickupSource { get; set; }
        public PickupPickerController.Option[] Options { get; set; }

        public TrackBehaviour()
        {
            Options = new PickupPickerController.Option[0];
        }
    }

    public enum PickupSource
    {
        Chest,
        Terminal,
        Boss,
        YellowBoss,
        BossHunter,
        Roulette,
        Scrapper,
        VoidPotential,
        Cell
    }
}