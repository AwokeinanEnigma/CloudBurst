﻿using EntityStates;
using Cloudburst.Cores.HAND.Components;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using Cloudburst.Cores.Components;

namespace Cloudburst.Cores.States.Wyatt
{
    class TrashOut : BaseSkillState
    {
        enum ActionStage
        {
            StartUp,
            NoTarget,
            FoundTarget,
            HitTarget
        }

        private ActionStage stage;
        private HANDDroneTracker tracker;
        private HurtBox target;

        private float _stopwatch;

        private GameObject _winch;

        public override void OnEnter()
        {
            base.OnEnter();
            tracker = base.gameObject.GetComponent<HANDDroneTracker>();

            if (base.isAuthority)
            {
                stage = ActionStage.StartUp;
                target = tracker.GetTrackingTarget();

                if (characterBody)
                {
                    characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
                }

                if (!isGrounded)
                {
                    //we good
                }
                else
                {
                    //We have to unground ourselves so we can actually hit the target.
                    base.SmallHop(base.characterMotor, 10f);
                }

                if (target && target.healthComponent && target.healthComponent.alive)
                {
                    stage = ActionStage.FoundTarget;
                    FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                    {
                        crit = false,
                        damage =0,
                        damageColorIndex = DamageColorIndex.Default,
                        force = 0f,
                        owner = base.gameObject,
                        position = GetAimRay().origin,
                        procChainMask = default(ProcChainMask),
                        projectilePrefab = ProjectileCore.winch,
                        rotation = Util.QuaternionSafeLookRotation(GetAimRay().direction),
                        target = target.gameObject,
                        useSpeedOverride = true,
                        speedOverride = 500
                    };
                    EffectManager.SimpleMuzzleFlash(Resources.Load<GameObject>("prefabs/effects/muzzleflashes/MuzzleflashWinch"), base.gameObject, "WinchHole", true);
                    LogCore.LogI(outer.customName);
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                    base.PlayAnimation("Fullbody, Override", "kick");
                }

                //LogCore.LogI("Stage: " + stage.ToString());
            }
        }

        public void SetHookReference(GameObject winch) {
            _winch = winch;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            //LogCore.LogI("Stage: " + stage.ToString());
            _stopwatch += Time.deltaTime;

            if (stage == ActionStage.FoundTarget)
            {
                if (this.target)
                {
                    Vector3 velocity = (target.transform.position - base.transform.position).normalized * 120f;
                    base.characterMotor.velocity = velocity;
                    base.characterDirection.forward = base.characterMotor.velocity.normalized;
                    float distance = Util.SphereVolumeToRadius(target.volume);

                    if (_stopwatch > 2)
                    {
                        //LogCore.LogI(stopwatch);
                        this.activatorSkillSlot.AddOneStock();
                        characterMotor.velocity = Vector3.zero;         
                        LogCore.LogI("Can't reach target, skill refunded!");
                        this.outer.SetNextStateToMain();
                    }


                    if (Vector3.Distance(base.transform.position, target.transform.position) < distance + 5f && target)
                    {

                        base.PlayCrossfade("Fullbody, Override", "BufferEmpty",0.5f);
                        new BlastAttack
                        {
                            position = target.transform.position,
                            baseForce = 3000,
                            attacker = base.gameObject,
                            inflictor = base.gameObject,
                            teamIndex = base.GetTeam(),
                            baseDamage = 5 * this.damageStat,
                            attackerFiltering = default,
                            bonusForce = new Vector3(0, -3000, 0),
                            damageType = DamageType.Stun1s | DamageTypeCore.spiked,
                            crit = RollCrit(),
                            damageColorIndex = DamageColorIndex.Default,
                            falloffModel = BlastAttack.FalloffModel.None,
                            //impactEffect = Resources.Load<GameObject>("prefabs/effects/impacteffects/PulverizedEffect").GetComponent<EffectIndex>(),
                            procCoefficient = 1f,
                            radius = 5
                        }.Fire();
                        if (_winch)
                        {
                            _winch.GetComponent<WyattWinchManager>().OnHit();
                        }
                        EffectData effectData = new EffectData
                        {
                            rotation = Quaternion.identity,
                            scale = 20f,
                            //start = base.transform.position,
                            origin = target.transform.position
                        };
                        EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/MaulingRockImpact"), effectData, true);
                        EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/impacteffects/ExplosionSolarFlare"), effectData, true);

                        base.characterMotor.velocity = Vector3.up * 20f;
                        //characterMotor.ApplyForce(-(GetAimRay().direction * (-characterMotor.mass * 10)), true, false);
                        stage = ActionStage.HitTarget;


                        //LogCore.LogI("Stage: " + stage.ToString());

                        this.outer.SetNextStateToMain();
                    }
                }
                else
                {
                    outer.SetNextStateToMain();
                }
            }
            else
            {
                LogCore.LogE("Something is seriously fucked. Stage: " + stage.ToString());
                characterMotor.velocity = Vector3.zero;
                this.outer.SetNextStateToMain();

            }
        }

        public override void OnExit()
        {
            base.OnExit();
            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
