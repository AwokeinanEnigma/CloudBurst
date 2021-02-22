﻿using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using EntityStates.Toolbot;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace Cloudburst.Cores.States.Wyatt
{
    public class DRIVEMETOTHEHIGHWAY : AimThrowableBase
    {
        private static AimStunDrone _goodState;
        public DRIVEMETOTHEHIGHWAY()
        {
            if (_goodState == null)
            {
                _goodState = Instantiate(typeof(AimStunDrone)) as AimStunDrone;
            }
            maxDistance = 1000;
            rayRadius = _goodState.rayRadius;
            arcVisualizerPrefab = _goodState.arcVisualizerPrefab;
            projectilePrefab = ProjectileCore.wyattBlinkProjectile;
            endpointVisualizerPrefab = _goodState.endpointVisualizerPrefab;
            endpointVisualizerRadiusScale = _goodState.endpointVisualizerRadiusScale;
            setFuse = false; //_goodSate.setFuse;
            damageCoefficient = FireGrenade.damageCoefficient;
            baseMinimumDuration = 0.1f;
            //rayRadius = _goodState.rayRadius;
            //rayRadius = _goodState.rayRadius;
        }
        public override void ModifyProjectile(ref FireProjectileInfo fireProjectileInfo)
        {
            string muzzleName = "MuzzleCenter";

            if (FireGrenade.effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(FireGrenade.effectPrefab, base.gameObject, muzzleName, false);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}