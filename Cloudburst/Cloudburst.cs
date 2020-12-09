﻿using BepInEx;
using BepInEx.Configuration;
using Cloudburst.Cores;
using Cloudburst.Cores.Components;
using Cloudburst.Cores.HAND;
using R2API.Utils;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Cloudburst
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(guid, modName, version)]
    [R2APISubmoduleDependency(new string[]
    {
        "ItemAPI",
        "SurvivorAPI",
        "ResourcesAPI",
        "PrefabAPI",
        "LanguageAPI",
        "BuffAPI",
        "EffectAPI",
        "LoadoutAPI",
        "DirectorAPI",
        "LanguageAPI",
        "ResourcesAPI",
        "UnlockablesAPI",
        "DotAPI",
        "EliteAPI",
        //gotta stop using this one, orbCore should be able to support itself.
        "OrbAPI"
    })]

    public class Cloudburst : BaseUnityPlugin
    {
        //be unique, though you're the same here.

        const string guid = "com.TeamCloudburst.Cloudburst";
        const string modName = "Cloudburst";
        const string version = "1.0.0";

        public static Cloudburst instance;

        /// <summary>
        /// Called BEFORE the first frame of the game.
        /// </summary>
        public static event Action awake;
        /// <summary>
        /// Called on the first frame of the game.
        /// </summary>
        public static event Action start;
        /// <summary>
        /// Called when the mod is enabled
        /// </summary>
        public static event Action onEnable;
        /// <summary>
        /// Called when the mod is disabled
        /// </summary>
        public static event Action onDisable;
        /// <summary>
        /// Called on the mod's FixedUpdate
        /// </summary>
        public static event Action onFixedUpdate;

        #region Cores
        //Support cores; Cannot be disabled.
        private AssetsCore assetCore;
        private OrbCore orbCore;
        private BuffCore buffCore;
        private DoTCore dotCore;
        private EffectCore effectCore;
        private DebuggingCore debugCore;
        private PrefabCore prefabCore;
        //private ShitpostCore lolCore;

        //Content Cores; Can be disabled
        private ItemCore itemCore;
        private EquipmentCore equipCore;
        private WyattCore wyattCore;
        private EliteCore eliteCore;
        //private MegaMushrum mushrum;
        //private WyattCore han_dCore;
        private EngineerCore engineerCore;
        private QOLCore qol;
        private CommandoCore commandoCore;
        //private AncientWispCore ancientWisp;
        //private ClayMenCore clayCore;
        private SipSipCore sipsipCore;
        //private A rchaicWispCore archwispCore;
        private RexCore rexCore;
        private HuntressCore wipCore;
        //private BombardierCore bombManCore;
        private AchievementCore achiveCore;
        private ProjectileCore projectileCore;
        private DamageTypeCore damageTypeCore;
        //shut up vs
        //private ArtifactsCore artifactCore;
        #endregion

        #region Config
        public static ConfigEntry<bool> EnableHAND;
        public static ConfigEntry<bool> EnableClaymen;
        public static ConfigEntry<bool> EnableSipSip;
        public static ConfigEntry<bool> EnableArchaicWisps;
        public static ConfigEntry<bool> EnableElites;
        public static ConfigEntry<bool> EnableCommando;
        public static ConfigEntry<bool> EnableEngineer;
        public static ConfigEntry<bool> EnableRex;
        public static ConfigEntry<bool> EnableItems;
        public static ConfigEntry<bool> EnableEquipment;

        public static ConfigEntry<bool> EnableWIP;
        public static ConfigEntry<bool> EnableUnlockAll;
        public static ConfigEntry<bool> Enabled;
        #endregion

        public Cloudburst()
        {
            LogCore.logger = Logger;

            DefineConfig();
            InitializeCores();

            /*if (!gameObject.HasComponent<ProgressTracker>() || !ProgressTracker.instance)
            {
                LogCore.LogI("Bepinex object does not have a progress tracker or there is no instance of the progress tracker. Attaching now.");
                gameObject.AddComponent<ProgressTracker>();
            }*/

            LogCore.LogM("Cloudburst loaded!");
        }

        private void DefineConfig()
        {
            //learn a smile
            //as every good day goes on by
            EnableHAND = Config.Bind("Cloudburst :: HAN-D", "Enabled", true, "Enables HAN-D as a survivor. Set to false to disable HAN-D.");
            EnableClaymen = Config.Bind("Cloudburst :: Clay Men", "Enabled", true, "Enables Clay Men as enemies. Set to false to disable Clay Men.");
            EnableSipSip = Config.Bind("Cloudburst :: SipSip", "Enabled", true, "Enables SipSip as a potential 5th lunar scavenger. Set to false to disable SipSip.");
            EnableArchaicWisps = Config.Bind("Cloudburst :: Archaic Wisps", "Enabled", true, "Enables Archaic Wisps as enemies. Set to false to disable Arcahic Wisps.");
            EnableElites = Config.Bind("Cloudburst :: Elites", "Enabled", true, "Enables custom elites. Set to false to disable Cloudburst's elites.");
            EnableRex = Config.Bind("Cloudburst :: REX", "Enabled", true, "Enables Cloudburst's modifications to REX. Set to false to disable Cloudburst's modifications to REX.");
            EnableCommando = Config.Bind("Cloudburst :: Commando", "Enabled", true, "Enables Cloudburst's modifications to Commando. Set to false to disable Cloudburst's modifications to Commando.");
            EnableEngineer = Config.Bind("Cloudburst :: Engineer", "Enabled", true, "Enables Cloudburst's modifications to Engineer. Set to false to disable Cloudburst's modifications to Engineer.");
            EnableItems = Config.Bind("Cloudburst :: Items", "Enabled", true, "Enables Cloudburst's items. Set to false to disable Cloudburst's items.");
            EnableEquipment = Config.Bind("Cloudburst :: Equipment", "Enabled", true, "Enables Cloudburst's equipment. Set to false to disable Cloudburst's equipment.");
            EnableWIP = Config.Bind("Cloudburst :: WIP", "Enabled", false, "WARNING: CONTENT ADDED BY THIS MODULE MAY NOT BE STABLE. ENABLE AT YOUR OWN RISK! Enables Cloudburst's WIP (work in progress) content. Set to false to disable Cloudburst's WIP content.");
            EnableUnlockAll = Config.Bind("Cloudburst :: Achivements", "Enabled", true, "Enables Cloudburst's unlocks for unlockable content. Set to false to unlock all of Cloudburst's unlockable content.");
            Enabled = Config.Bind("Cloudburst", "Enabled", true, "Enables the mod. Set to false to disable the mod entirely.");
        }

        private void InitializeCores()
        {
            if (Enabled.Value)
            {
                //if i allow users to disable these cores they're gonna be stupid
                //so i can't do that
                //plus most cores rely on these
                assetCore = new AssetsCore();
                CommonAssets.Load();

                SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

                damageTypeCore = new DamageTypeCore();
                orbCore = new OrbCore();
                buffCore = new BuffCore();
                effectCore = new EffectCore();
                dotCore = new DoTCore();
                prefabCore = new PrefabCore();
                projectileCore = new ProjectileCore();
                achiveCore = new AchievementCore();

                //got this feeling
                //i'm losing you
                //it's got me reeling
                //i need a clue
                if (EnableItems.Value)
                {
                    itemCore = new ItemCore();
                }
                if (EnableEquipment.Value)
                {
                    equipCore = new EquipmentCore();
                }
                if (EnableElites.Value)
                {
                    eliteCore = new EliteCore();
                }
                if (EnableHAND.Value)
                {
                    // han_dCore = new HAN_DCore();
                }
                if (EnableEngineer.Value)
                {
                    engineerCore = new EngineerCore();
                }
                if (EnableCommando.Value)
                {
                    commandoCore = new CommandoCore();
                }
                if (EnableSipSip.Value)
                {
                    sipsipCore = new SipSipCore();
                }
                if (EnableClaymen.Value)
                {
                    // clayCore = new ClayMenCore();
                }
                if (EnableArchaicWisps.Value)
                {
                    // archwispCore = new ArchaicWispCore();
                }
                wyattCore = new WyattCore();
                if (EnableRex.Value)
                {
                    rexCore = new RexCore();
                }
                if (EnableWIP.Value)
                {
                    wipCore = new HuntressCore();
                }


                //and thus
                //we reach the end
                qol = new QOLCore();
                // mushrum = new MegaMushrum();
                // ancientWisp = new AncientWispCore();
                // bombManCore = new BombardierCore();
#if DEBUG
                //unlock!
                debugCore = new DebuggingCore();
#endif
            }
            else
            {
                LogCore.LogW("You have disabled ALL of Cloudburst. If this was not desired, you can re-enable it in Cloudburst's config file.");
            }
        }

        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            LogCore.LogI(arg1.name);
            if (arg1.name == "title")
            {
                var menu = GameObject.Find("MainMenu");
                //LogCore.LogI(menu.name)
                var title = menu.transform.Find("MENU: Title/TitleMenu/SafeZone/ImagePanel (JUICED)/LogoImage");
                title.GetComponent<Image>().sprite = AssetsCore.mainAssetBundle.LoadAsset<Sprite>("Assets/Cloudburst/cloudburstlogo.png");
                //LogCore.LogI(title.name);

                //var rock = GameObject.Find("HOLDER: Title Background/Rocks/RockModular2(3)");
                var broom = Instantiate<GameObject>(AssetsCore.mainAssetBundle.LoadAsset<GameObject>("Broom"));

                broom.transform.Find("BroomRig/Handle/GyroBall").AddComponent<Spinner>();
                broom.transform.Find("BroomRig/Handle/GyroRing").AddComponent<Spinner>();

                var transform = broom.transform;
                transform.position = new Vector3(81, 4, 62);
                transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                transform.rotation = Quaternion.Euler(new Vector3(340, 340, 70));
            }
        }

        public void Awake()
        {
            Action awake = Cloudburst.awake;
            if (awake == null)
            {
                return;
            }
            awake();
        }

        public void FixedUpdate()
        {
            Action fixedUpdate = Cloudburst.onFixedUpdate;
            if (fixedUpdate == null)
            {
                return;
            }
            fixedUpdate();
        }

        public void Start()
        {
            Action awake = Cloudburst.start;
            if (awake == null)
            {
                return;
            }
            awake();
        }

        public void OnEnable()
        {
            SingletonHelper.Assign<Cloudburst>(Cloudburst.instance, this);
            LogCore.LogI("Cloudburst instance assigned.");
            Action awake = Cloudburst.onEnable;
            if (awake == null)
            {
                return;
            }
            awake();
        }

        public void OnDisable()
        {
            SingletonHelper.Unassign<Cloudburst>(Cloudburst.instance, this);
            LogCore.LogI("Cloudburst instance unassigned.");
            Action awake = Cloudburst.onDisable;
            if (awake == null)
            {
                return;
            }
            awake();
        }
    }
}