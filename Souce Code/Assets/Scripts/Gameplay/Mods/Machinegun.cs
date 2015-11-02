using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Machinegun : BaseWeapon
{

    void Awake()
    {
        ProjectileCacheType = Spawner.ESpaceObjects.Bullet;
    }

    public float Rate = 200;

    int _currentPivot;

    public override void Init(Spawner spawner)
    {
        base.Init(spawner);

        VFX.Instance.ParentEffectIf("fire", transform, new Vector2(-0.85f, 1.7f), -Mathf.PI / 2, IsShooting);
        VFX.Instance.ParentEffectIf("fire", transform, new Vector2(0.75f, 1.7f), -Mathf.PI / 2, IsShooting);
    }

    public bool IsShooting()
    {
        return _shooting;
    }

    bool _shooting;

    public override void Hold(int mb)
    {
        _shooting = true;

        base.Hold(mb);
        Shoot();
    }

    public override void Release(int mb)
    {
        _shooting = false;
    }

    public override void Shoot()
    {
        if (Time.time < _nextShotAvailableAt) return;
        var projectile = GetProjectile();

        projectile.transform.position = Pivots[_currentPivot % Pivots.Length].position;
        _currentPivot++;

        _nextShotAvailableAt = Time.time + Rate;
    }

    public override string GetTitle()
    {
        return "MG";
    }

}
