﻿using Cloudburst.Cores.HAND.Components;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace Cloudburst.Cores
{
    public class BuffCore
    {
        public static BuffCore instance;

        protected internal BuffIndex skinIndex;
        protected internal BuffIndex antiGravIndex;
        protected internal BuffIndex antiGravFriendlyIndex;
        protected internal BuffIndex wyattCombatIndex;

        internal bool Loaded { get; private set; } = false;
        public BuffCore() => RegisterBuffs();

        protected void RegisterBuffs()
        {
            LogCore.LogI("Initializing Core: " + base.ToString());

            instance = this;

            /*RegisterBuff(new BuffDef
            {
                buffIndex = BuffIndex.Count,
                //buffColor = Color.yellow,
                canStack = true,
                eliteIndex = EliteIndex.None,
                iconPath = "@EngimaHANDREBOOTED:Assets/Import/HAND_ICONS/Passive.png",
                isDebuff = false,
                name = "Drone",
            });

            RegisterBuff(new BuffDef
            {
                buffIndex = BuffIndex.Count,
                canStack = false,
                eliteIndex = EliteIndex.None,
                iconPath = "@EngimaHANDREBOOTED:Assets/Import/HAND_ICONS/OverclockBuff.png",
                isDebuff = false,
                name = "Overclock"
            });

            RegisterBuff(new BuffDef
            {
                buffColor = new Color(0.3764706f, 0.84313726f, 0.8980392f),
                buffIndex = BuffIndex.Count,
                canStack = false,
                eliteIndex = EliteIndex.None,
                iconPath = "Textures/BuffIcons/texbuffonfireicon",
                isDebuff = false,
                name = "Surge"
            });

            RegisterBuff(new BuffDef
            {
                buffColor = new Color(0.3764706f, 0.84313726f, 0.8980392f),
                buffIndex = BuffIndex.Count,
                canStack = false,
                eliteIndex = EliteIndex.None,
                iconPath = "Textures/BuffIcons/texbuffonfireicon",
                isDebuff = false,
                name = "Sparkle"
            });
            RegisterBuff(new BuffDef()
            {
                buffIndex = BuffIndex.Count,
                canStack = false,
                eliteIndex = EliteIndex.None,
                iconPath = "Textures/BuffIcons/texMovespeedBuffIcon",
                name = "BombardierForce",
                buffColor = new Color(0.8392157f, 0.7882353f, 0.22745098f)
            });
            RegisterBuff(new BuffDef()
            {
                buffIndex = BuffIndex.Count,
                canStack = true,
                isDebuff = false,
                eliteIndex = EliteIndex.None,
                iconPath = "Textures/BuffIcons/texMovespeedBuffIcon",
                name = "BaboonCharge",
                buffColor = new Color(0.8392157f, 0.7882353f, 0.22745098f)
            });*/
            RegisterBuff(new BuffDef()
            {
                buffIndex = BuffIndex.Count,
                canStack = false,
                isDebuff = false,
                eliteIndex = EliteIndex.None,
                iconPath = "Textures/BuffIcons/texBuffBodyArmorIcon",
                name = "SkinStack",
                buffColor = new Color32(219, 224, 198, byte.MaxValue)
            });
            RegisterBuff(new BuffDef()
            {
                buffIndex = BuffIndex.Count,
                canStack = false,
                isDebuff = false,
                eliteIndex = EliteIndex.None,
                iconPath = "Textures/BuffIcons/texBuffPulverizeIcon",
                name = "AntiGrav",
                buffColor = new Color(0.6784314f, 0.6117647f, 0.4117647f)
            });
            RegisterBuff(new BuffDef()
            {
                buffIndex = BuffIndex.Count,
                canStack = true,
                isDebuff = false,
                eliteIndex = EliteIndex.None,
                iconPath = "@Cloudburst:Assets/Cloudburst/BuffIcons/WyattVelocity.png",
                name = "WyattCombat",
                buffColor = new Color(1f, 0.7882353f, 0.05490196f)
            });
            RegisterBuff(new BuffDef()
            {
                buffIndex = BuffIndex.Count,
                canStack = false,
                isDebuff = false,
                eliteIndex = EliteIndex.None,
                iconPath = "Textures/BuffIcons/texBuffGenericShield",
                name = "AntiGravFriendly",
                buffColor = new Color(0.6784314f, 0.6117647f, 0.4117647f)
            });

            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.RoR2.CharacterBody.RemoveBuff += CharacterBody_RemoveBuff;
            On.RoR2.CharacterBody.AddBuff += CharacterBody_AddBuff;
            On.RoR2.CharacterMotor.OnDeathStart += CharacterMotor_OnDeathStart;
            On.RoR2.CharacterMotor.OnHitGround += CharacterMotor_OnHitGround;        }


        private void CharacterMotor_OnDeathStart(On.RoR2.CharacterMotor.orig_OnDeathStart orig, CharacterMotor self)
        {
            self.useGravity = true;
            orig(self);
        }

        private void CharacterMotor_OnHitGround(On.RoR2.CharacterMotor.orig_OnHitGround orig, CharacterMotor self, CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            orig(self, hitGroundInfo);
            if (self.body && self.body.HasBuff(this.antiGravIndex)) {
                self.useGravity = false;
                if (self.lastVelocity.y < -20)
                {
                    EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/impacteffects/BeetleGuardGroundSlam"), new EffectData
                    {
                        scale = 10,
                        rotation = Quaternion.identity,
                        origin = hitGroundInfo.position,
                    }, true);
                    new BlastAttack
                    {
                        position = hitGroundInfo.position,
                        //baseForce = 3000,
                        attacker = null,
                        inflictor = null,
                        teamIndex = TeamIndex.Player,
                        baseDamage = 0.1f * self.body.maxHealth,
                        attackerFiltering = default,
                        //bonusForce = new Vector3(0, -3000, 0),
                        damageType = DamageType.Stun1s | DamageType.NonLethal, //| DamageTypeCore.spiked,
                        crit = false,
                        damageColorIndex = DamageColorIndex.Default,
                        falloffModel = BlastAttack.FalloffModel.None,
                        //impactEffect = Resources.Load<GameObject>("prefabs/effects/impacteffects/PulverizedEffect").GetComponent<EffectIndex>(),
                        procCoefficient = 0,
                        radius = 10
                    }.Fire();
                }
            }

        }

        private void CharacterBody_AddBuff(On.RoR2.CharacterBody.orig_AddBuff orig, CharacterBody self, BuffIndex buffType)
        {
            orig(self, buffType);
            if (buffType == antiGravFriendlyIndex) {
                ICharacterFlightParameterProvider component = self.GetComponent<ICharacterFlightParameterProvider>();
                if (component != null)
                {
                    CharacterFlightParameters flightParameters = component.flightParameters;
                    flightParameters.channeledFlightGranterCount++;
                    //LogCore.LogI("FLIGHT PARAMS: " + flightParameters.channeledFlightGranterCount);
                    component.flightParameters = flightParameters;
                }
                ICharacterGravityParameterProvider component2 = self.GetComponent<ICharacterGravityParameterProvider>();
                if (component2 != null)
                {
                    CharacterGravityParameters gravityParameters = component2.gravityParameters;
                    gravityParameters.environmentalAntiGravityGranterCount++;
                    //LogCore.LogI(gravityParameters.environmentalAntiGravityGranterCount);
                    component2.gravityParameters = gravityParameters;
                }
            }
        }

        private void CharacterBody_RemoveBuff(On.RoR2.CharacterBody.orig_RemoveBuff orig, CharacterBody self, BuffIndex buffType)
        {
            if (buffType == antiGravIndex)
            {
                if (self.characterMotor)
                {
                    self.characterMotor.useGravity = true;
                }
            }
            if (buffType == antiGravFriendlyIndex) {
                ICharacterFlightParameterProvider component = self.GetComponent<ICharacterFlightParameterProvider>();
                if (component != null)
                {
                    CharacterFlightParameters flightParameters = component.flightParameters;
                    flightParameters.channeledFlightGranterCount--;
                    //LogCore.LogI("FLIGHT PARAMS: " + flightParameters.channeledFlightGranterCount);
                    component.flightParameters = flightParameters;
                }
                ICharacterGravityParameterProvider component2 = self.GetComponent<ICharacterGravityParameterProvider>();
                if (component2 != null)
                {
                    CharacterGravityParameters gravityParameters = component2.gravityParameters;
                    gravityParameters.environmentalAntiGravityGranterCount--;
                    //LogCore.LogI(gravityParameters.environmentalAntiGravityGranterCount);
                    component2.gravityParameters = gravityParameters;
                }
            }

            orig(self, buffType);
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            if (self)
            {
                //Reflection.SetPropertyValue<float>(self, "maxHealth", characterBody.maxHealth * 100f);

                var attackSpeed = self.attackSpeed;
                var armor = self.armor;
                var moveSpeed = self.moveSpeed;
                var regen = self.regen;
                var crit = self.crit;

                if (self.HasBuff(skinIndex)) // && controller)
                {
                    var count = 0;
                    if (self.inventory) {
                        count = self.inventory.GetItemCount(ItemCore.instance.barrierOnLevelIndex);
                    }
                    self.SetPropertyValue("armor", armor + (5f + (count * 5)));
                }
                if (self.HasBuff(antiGravIndex))
                {
                    if (self.characterMotor)
                    {
                        self.characterMotor.useGravity = false;
                    }
                    self.SetPropertyValue("attackSpeed", attackSpeed -= (.5f * attackSpeed));
                    self.SetPropertyValue("moveSpeed", moveSpeed -= (.5f * moveSpeed));
                }
                if (self.HasBuff(antiGravFriendlyIndex)) {
                    //LogCore.LogI(self.moveSpeed);
                    self.SetPropertyValue("moveSpeed", moveSpeed * 1);
                    //LogCore.LogI(self.moveSpeed);
                }
                if (self.HasBuff(wyattCombatIndex)) {
                    var buffCount = self.GetBuffCount(wyattCombatIndex);
                    for (int i = 0; i < buffCount; i++) {
                        self.SetPropertyValue("moveSpeed", moveSpeed * (1f + (buffCount * 0.19f)));
                        self.SetPropertyValue("regen", regen * (1f + (buffCount * 0.19f)));
                    }
                }
            }
        }

        protected internal void RegisterBuff(BuffDef buffDef)
        {
            var customBuff = new CustomBuff(buffDef);
            switch (buffDef.name)
            {
                case "SkinStack":
                    skinIndex = BuffAPI.Add(customBuff);
                    break;
                case "AntiGrav":
                    antiGravIndex = BuffAPI.Add(customBuff);
                    break;
                case "WyattCombat":
                    wyattCombatIndex = BuffAPI.Add(customBuff);
                    break;
                case "AntiGravFriendly":
                    antiGravFriendlyIndex = BuffAPI.Add(customBuff);
                    break;
                //throw new System.NotImplementedException("not implemented yet!");
                default:
                    //feel my haunted lust
                    LogCore.LogF(string.Format("{0} doesn't have a case!", buffDef.name));
                    break;
            }
        }
    }
}
