using UnityEngine;

public class RocketLauncher : BaseWeapon
{

    public float ReloadTime;
    public int Ammo = 4;
    int _currentPivot;

    private int _ammo = 0;

    void Awake()
    {
        ProjectileCacheType = Spawner.ESpaceObjects.Rocket;
    }

    public override void Init(Spawner spawner)
    {
        base.Init(spawner);
        _ammo = Ammo;
    }

    

    public override string GetTitle()
    {
        return "RLNCH";
    }

    public override void Release(int mb)
    {
        Shoot();
    }

    public override void Shoot()
    {
        if (_ammo <= 0)
        {
            _nextShotAvailableAt = Time.time + ReloadTime;
            _ammo = Ammo;
        }

        if (Time.time < _nextShotAvailableAt) return;
        var p = GetProjectile();
        p.transform.position = Pivots[_currentPivot].position;
        _currentPivot = (++_currentPivot) % Pivots.Length;


        _ammo--;
    }
}