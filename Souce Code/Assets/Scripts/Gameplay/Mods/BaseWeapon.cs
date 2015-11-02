using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class BaseWeapon : MonoBehaviour, IWeapon {


   

    protected float _nextShotAvailableAt;
    public Transform[] Pivots;

    public Spawner.ESpaceObjects ProjectileCacheType {
        get; set;
    }

    public Vector2 Offset = new Vector2(-2.15f, 0);

    private Spawner _spawner;

    public virtual void Hold(int mb)
    {
    }

    public virtual void Init(Spawner spawner)
    {
        _spawner = spawner;
    }

    public virtual void Release(int mb)
    {
    }

    public virtual void Shoot()
    {
      
    }

    public virtual string GetTitle()
    {
        return "UNARMED";
    }


    public virtual Projectile GetProjectile()
    {
        return _spawner.SpawnSingle(ProjectileCacheType, Vector2.zero) as Projectile;       
    }

 
}


public interface IWeapon
{
    void Hold(int mb);
    void Release(int mb);
    void Init(Spawner spawner);
    string GetTitle();
    Spawner.ESpaceObjects ProjectileCacheType
    {
        get; set;
    }

}
