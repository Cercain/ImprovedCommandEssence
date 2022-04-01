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
using PickupIndex = RoR2.PickupIndex;
using PickupTransmutationManager = RoR2.PickupTransmutationManager;

namespace ImprovedCommandEssence
{
    //[BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    
    public class ImprovedCommandEssence : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Cercain";
        public const string PluginName = "ImprovedCommandEssence";
        public const string PluginVersion = "1.0.1";

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

            Config.SettingChanged += ConfigOnSettingChanged;
            On.RoR2.PickupPickerController.SetOptionsFromPickupForCommandArtifact += SetOptions;

            On.RoR2.ChestBehavior.ItemDrop += ItemDrop;
            On.RoR2.PickupDropletController.OnCollisionEnter += DropletCollisionEnter;

            if (!onInDropShip.Value)
                On.RoR2.ShopTerminalBehavior.DropPickup += TerminalDrop;
        }

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

            var track = new TrackBehaviour() { ItemTag = self.requiredItemTag, DropTable = self.dropTable };

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
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void RoR2.ShopTerminalBehavior::DropPickup()' called on client");
                return;
            }
            var track = new TrackBehaviour() { isTerminal = true };
            self.SetHasBeenPurchased(true);
            CreatePickupDroplet(self.pickupIndex, (self.dropTransform ? self.dropTransform : self.transform).position, self.transform.TransformVector(self.dropVelocity), track);

        }

        public void CreatePickupDroplet(PickupIndex pickupIndex, Vector3 position, Vector3 velocity, TrackBehaviour track)
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

            behav.isTerminal = track.isTerminal;
            behav.DropTable = track.DropTable;
            behav.ItemTag = track.ItemTag;
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
                self.alive = false;
                self.createPickupInfo.position = self.transform.position;
                bool flag = true;

                if ((BazaarController.instance != null && !onInBazaar.Value) || 
                    (!onInDropShip.Value && self.gameObject.TryGetComponent<TrackBehaviour>(out var track) && track.isTerminal))
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

            int itemAmount = 1;
            switch (pickupIndex.pickupDef.itemTier)
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
                    if (pickupIndex.pickupDef.equipmentIndex != EquipmentIndex.None)
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

            PickupIndex[] itemSelection = null;
            if(tracking != null && (tracking.ItemTag == ItemTag.Damage || tracking.ItemTag == ItemTag.Healing || tracking.ItemTag == ItemTag.Utility))
            {
                try
                {
                    var choices = (tracking.DropTable as BasicPickupDropTable).selector.choices;
                    itemSelection = choices.Where(x => x.value.pickupDef.itemTier == pickupIndex.pickupDef.itemTier).Select(x => x.value).ToArray();
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                }
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

    }
    
    public class TrackBehaviour : MonoBehaviour
    {
        public ItemTag ItemTag { get; set; }
        public PickupDropTable DropTable { get; set; }
        public bool isTerminal { get; set; }
    }
}