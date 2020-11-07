﻿using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace Cloudburst.Cores.States.Bombardier
{
    //oh oh she wants me (to be loved)
    class RocketJump : FireRocket
    {
        public override GameObject GrabProjectile()
        {
            return ProjectileCore.bombardierFireBombProjectile;
        }

        //aimRay.direction * (-characterMotor.mass * 35)

        public override float GetBloom()
        {
            return 800;
        }

        public override bool ShouldPopStickies()
        {
            return false;
        }

        public override void ModifyProjectileInfo(FireProjectileInfo info)
        {
            base.ModifyProjectileInfo(info);
        }

        public override FireProjectileInfo GetInfo(Ray aimRay)
        {
            var damage = base.isGrounded ? 7 * damageStat : 5 * damageStat;
            FireProjectileInfo info = new FireProjectileInfo()
            {
                crit = RollCrit(),
                damage = damage,
                damageColorIndex = RoR2.DamageColorIndex.Default,
                damageTypeOverride = DamageType.Stun1s,
                force = -2500,
                owner = gameObject,
                position = transform.position,
                procChainMask = default,
                projectilePrefab = ProjectileCore.bombardierFireBombProjectile,
                rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                target = null,
                useFuseOverride = false,
                useSpeedOverride = true,
                _speedOverride = 100
            };
            return info;
        }

        public override Vector3 GetKnockback(Ray aimRay)
        {
            return aimRay.direction * (-characterMotor.mass * 10);
        }

        public override bool ShouldHaveKnockback()
        {
            return !isGrounded;
        }
    }
}