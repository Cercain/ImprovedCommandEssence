using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
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
        public const string PluginVersion = "1.1.1";

        public static ConfigFile configFile = new ConfigFile(Paths.ConfigPath + "\\ImprovedCommandEssence.cfg", true);

        public static ConfigEntry<int> itemAmountCommon { get; set; }
        public static ConfigEntry<int> itemAmountUncommon { get; set; }
        public static ConfigEntry<int> itemAmountLegendary { get; set; }
        public static ConfigEntry<int> itemAmountLunar { get; set; }
        public static ConfigEntry<int> itemAmountVoid { get; set; }
        public static ConfigEntry<int> itemAmountEquip { get; set; }
        public static ConfigEntry<int> itemAmountBoss { get; set; }
        public static ConfigEntry<bool> keepCategory { get; set; }
        public static ConfigEntry<bool> onInBazaar { get; set; }
        public static ConfigEntry<bool> onInDropShip { get; set; }
        public static ConfigEntry<bool> sameBossDrops { get; set; }
        public static ConfigEntry<bool> onForTrophy { get; set; }

        List<int> eliteEquipIds = new List<int>() { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };
        List<int> specialDropIds = new List<int>() { 171, 5, 125, 150 };
        List<string> zetAspectItem = new List<string>() { "ItemIndex.ZetAspectWhite", "ItemIndex.ZetAspectBlue", "ItemIndex.ZetAspectRed", "ItemIndex.ZetAspectHaunted", "ItemIndex.ZetAspectPoison", "ItemIndex.ZetAspectLunar", "ItemIndex.ZetAspectEarth", "ItemIndex.ZetAspectVoid", "ItemIndex.ZetAspectSanguine" };

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
        }

        public void OnEnable()
        {
            itemAmountCommon = configFile.Bind("ImprovedCommandEssence", "itemAmountCommon", 6, new ConfigDescription("Set the amount of Common (white) options shown when opening a Command Essences."));
            itemAmountUncommon = configFile.Bind("ImprovedCommandEssence", "itemAmountUncommon", 4, new ConfigDescription("Set the amount of Uncommon (Green) options shown when opening a Command Essences."));
            itemAmountLegendary = configFile.Bind("ImprovedCommandEssence", "itemAmountLegendary", 2, new ConfigDescription("Set the amount of Legendary (Yellow) options shown when opening a Command Essences."));
            itemAmountBoss = configFile.Bind("ImprovedCommandEssence", "itemAmountBoss", 2, new ConfigDescription("Set the amount of Yellow items shown when opening a Command Essences."));
            itemAmountLunar = configFile.Bind("ImprovedCommandEssence", "itemAmountLunar", 2, new ConfigDescription("Set the amount of Blue items shown when opening a Command Essences."));
            itemAmountVoid = configFile.Bind("ImprovedCommandEssence", "itemAmountVoid", 2, new ConfigDescription("Set the amount of Blue items shown when opening a Command Essences."));
            itemAmountEquip = configFile.Bind("ImprovedCommandEssence", "itemAmountEquip", 4, new ConfigDescription("Set the amount of Orange items shown when opening a Command Essences."));
            keepCategory = configFile.Bind("ImprovedCommandEssence", "keepCategory", true, new ConfigDescription("Set if category chests only show items of the corresponding category."));
            onInBazaar = configFile.Bind("ImprovedCommandEssence", "onInBazaar", false, new ConfigDescription("Set if the Command Artifact is turn on in the Bazaar."));
            onInDropShip = configFile.Bind("ImprovedCommandEssence", "onInDropShip", false, new ConfigDescription("Set if the Command Artifact is turn on for Drop Ship items."));
            sameBossDrops = configFile.Bind("ImprovedCommandEssence", "sameBossDrops", true, new ConfigDescription("Set if the Command Essences that drop from the Teleporter boss give the same options."));
            onForTrophy = configFile.Bind("ImprovedCommandEssence", "onForTrophy", false, new ConfigDescription("Set if the item dropped by bosses killed via Trophy Hunter's Tricorn drop as a Command Essence (true) or the boss item (false)"));

            Config.SettingChanged += ConfigOnSettingChanged;
            On.RoR2.PickupPickerController.SetOptionsFromPickupForCommandArtifact += SetOptions;

            On.RoR2.ChestBehavior.ItemDrop += ItemDrop;
            On.RoR2.PickupDropletController.OnCollisionEnter += DropletCollisionEnter;

            if (!onInDropShip.Value)
                On.RoR2.ShopTerminalBehavior.DropPickup += TerminalDrop;

            if (sameBossDrops.Value)
                On.RoR2.BossGroup.DropRewards += BossDropRewards;

            if (!onForTrophy.Value)
                On.RoR2.EquipmentSlot.FireBossHunter += BossHunterDrop;
        }

        public void OnDisable()
        {
            On.RoR2.PickupPickerController.SetOptionsFromPickupForCommandArtifact -= SetOptions;
            if (keepCategory.Value)
            {
                On.RoR2.ChestBehavior.ItemDrop -= ItemDrop;
                On.RoR2.PickupDropletController.OnCollisionEnter -= DropletCollisionEnter;
            }
            if (!onInDropShip.Value)
                On.RoR2.ShopTerminalBehavior.DropPickup -= TerminalDrop;

            if (sameBossDrops.Value)
                On.RoR2.BossGroup.DropRewards -= BossDropRewards;

            if (!onForTrophy.Value)
                On.RoR2.EquipmentSlot.FireBossHunter -= BossHunterDrop;
        }

        private bool BossHunterDrop(On.RoR2.EquipmentSlot.orig_FireBossHunter orig, RoR2.EquipmentSlot self)
        {
            if (!RunArtifactManager.instance.IsArtifactEnabled(CommandArtifactManager.myArtifact))
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
                tracking.DropTable = null;
                tracking.PickupSource = PickupSource.BossHunter;
                tracking.Options = new PickupPickerController.Option[0];
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

        private void BossDropRewards(On.RoR2.BossGroup.orig_DropRewards orig, RoR2.BossGroup self)
        {
            if (!RunArtifactManager.instance.IsArtifactEnabled(CommandArtifactManager.myArtifact))
            {
                orig(self);
                return;
            }

            int participatingPlayerCount = Run.instance.participatingPlayerCount;
            if (participatingPlayerCount != 0 && self.dropPosition)
            {
                List<PickupIndex> list;

                if (self.dropTable)
                {
                    list = (from x in (self.dropTable as BasicPickupDropTable).selector.choices where x.value.pickupDef.itemTier != ItemTier.Boss select x.value).ToList();
                }
                else
                {
                    list = Run.instance.availableTier2DropList;
                    if (self.forceTier3Reward)
                    {
                        list = Run.instance.availableTier3DropList;
                    }
                }

                int itemAmount = GetItemAmountFromTier(list.First().pickupDef);

                List<PickupIndex> indexList = (from x in list orderby self.rng.Next() select x).Take(itemAmount).ToList();

                int num = 1 + self.bonusRewardCount;
                if (self.scaleRewardsByPlayerCount)
                {
                    num *= participatingPlayerCount;
                }
                float angle = 360f / (float)num;
                Vector3 vector = Quaternion.AngleAxis((float)UnityEngine.Random.Range(0, 360), Vector3.up) * (Vector3.up * 40f + Vector3.forward * 5f);
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
                
                int i = 0;
                while (i < num)
                {
                    if ((self.bossDrops.Count > 0 || self.bossDropTables.Count > 0) && self.rng.nextNormalizedFloat <= self.bossDropChance)
                    {
                        indexList = (from x in Run.instance.availableBossDropList orderby self.rng.Next() select x).Take(itemAmountBoss.Value).ToList();
                    }

                    var options = new PickupPickerController.Option[indexList.Count];

                    for (int x = 0; x < indexList.Count; x++)
                    {
                        PickupIndex index = indexList[x];
                        options[x] = new PickupPickerController.Option
                        {
                            available = Run.instance.IsPickupAvailable(index),
                            pickupIndex = index
                        };
                    }

                    var track = new TrackBehaviour();
                    track.PickupSource = PickupSource.Boss;
                    track.ItemTag = ItemTag.Any;
                    track.Options = options;

                    CreatePickupDroplet(indexList[0], self.dropPosition.position, vector, track);
                    i++;
                    vector = rotation * vector;
                }
            }
        }

        [Server]
        void ItemDrop(On.RoR2.ChestBehavior.orig_ItemDrop orig, RoR2.ChestBehavior self)
        {
            if (!RunArtifactManager.instance.IsArtifactEnabled(CommandArtifactManager.myArtifact))
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
                CreatePickupDroplet(self.dropPickup, self.dropTransform.position + Vector3.up * 1.5f, vector, track);
                vector = rotation * vector;
                self.Roll();
            }
            self.dropPickup = PickupIndex.none;
        }

        [Server]
        void TerminalDrop(On.RoR2.ShopTerminalBehavior.orig_DropPickup orig, RoR2.ShopTerminalBehavior self)
        {
            if (!RunArtifactManager.instance.IsArtifactEnabled(CommandArtifactManager.myArtifact))
            {
                orig(self);
                return;
            }

            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void RoR2.ShopTerminalBehavior::DropPickup()' called on client");
                return;
            }
            var track = new TrackBehaviour() { PickupSource = PickupSource.Terminal };
            self.SetHasBeenPurchased(true);
            CreatePickupDroplet(self.pickupIndex, (self.dropTransform ? self.dropTransform : self.transform).position, self.transform.TransformVector(self.dropVelocity), track);

        }

        private void CreatePickupDroplet(PickupIndex pickupIndex, Vector3 position, Vector3 velocity, TrackBehaviour track)
        {
            CreatePickupDroplet(new GenericPickupController.CreatePickupInfo
            {
                rotation = Quaternion.identity,
                pickupIndex = pickupIndex
            }, position, velocity, track);
        }

        // Token: 0x06002C0E RID: 11278 RVA: 0x000BBFA4 File Offset: 0x000BA1A4
        public void CreatePickupDroplet(GenericPickupController.CreatePickupInfo pickupInfo, Vector3 position, Vector3 velocity, TrackBehaviour track)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(PickupDropletController.pickupDropletPrefab, position, Quaternion.identity);
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
            if (!RunArtifactManager.instance.IsArtifactEnabled(CommandArtifactManager.myArtifact))
            {
                orig(self, collision);
                return;
            }

            if (NetworkServer.active && self.alive)
            {

                Logger.LogInfo($"Droplet {self.pickupIndex}:{self.pickupIndex.value}");
                self.alive = false;
                self.createPickupInfo.position = self.transform.position;
                bool flag = true;

                var trackBool = self.gameObject.TryGetComponent<TrackBehaviour>(out var track);

                if ((!onInBazaar.Value && BazaarController.instance != null) ||
                    (!onInDropShip.Value && trackBool && track.PickupSource == PickupSource.Terminal) ||
                    (!onForTrophy.Value && trackBool && track.PickupSource == PickupSource.BossHunter) ||
                    (self.pickupIndex.pickupDef == null || (self.pickupIndex.pickupDef.itemIndex == ItemIndex.None && self.pickupIndex.pickupDef.equipmentIndex == EquipmentIndex.None && self.pickupIndex.pickupDef.itemTier == ItemTier.NoTier)) ||
                    (eliteEquipIds.Contains((int)self.pickupIndex.pickupDef.equipmentIndex)) ||
                    (specialDropIds.Contains((int)self.pickupIndex.pickupDef.itemIndex)) ||
                    (zetAspectItem.Contains(self.pickupIndex.ToString())))
                        GenericPickupController.CreatePickup(self.createPickupInfo);
                else
                    OnDropletHitGroundServer(ref self.createPickupInfo, ref flag, self.gameObject.GetComponent<TrackBehaviour>());

                UnityEngine.Object.Destroy(self.gameObject);
            }
        }

        private void OnDropletHitGroundServer(ref GenericPickupController.CreatePickupInfo createPickupInfo, ref bool shouldSpawn, TrackBehaviour trackBehaviour = null)
        {
            PickupIndex pickupIndex = createPickupInfo.pickupIndex;
            PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
            
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
            gameObject.GetComponent<PickupPickerController>().SetOptionsFromPickupForCommandArtifact(pickupIndex);
            
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
            if(tracking != null && sameBossDrops.Value && tracking.PickupSource == PickupSource.Boss)
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
                        if (pickupIndex.pickupDef.equipmentIndex == EquipmentIndex.None)
                            itemSelection = Run.instance.availableLunarItemDropList.ToArray();
                        else
                            itemSelection = Run.instance.availableLunarEquipmentDropList.ToArray();
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
                            itemSelection = Run.instance.availableEquipmentDropList.ToArray();
                        break;
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
                System.Random rnd = new System.Random();
                List<PickupIndex> indexList = (from x in itemSelection.ToList() orderby rnd.Next() select x).Take(itemAmount).ToList();
                options = new PickupPickerController.Option[indexList.Count];

                for (int i = 0; i < indexList.Count; i++)
                {
                    PickupIndex index = indexList[i];
                    options[i] = new PickupPickerController.Option
                    {
                        available = Run.instance.IsPickupAvailable(index),
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
    }

    public enum PickupSource
    {
        Chest,
        Terminal,
        Boss,
        BossHunter
    }
}