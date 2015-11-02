using UnityEngine;
using System.Collections;
using System;

public class Projectile : RelativeSpaceObject
{

    public float Speed = 0.1f;
    public float Spawned = 0;
    public float RicochetChance = 0;
    public float Damage = 3;  

    public virtual void Update()
    {
        transform.position = transform.position + transform.up * Speed + GetRelativeSpeed();
        if (transform.position.y > CameraController.Bounds.y || Time.time - Spawned > 3) Release();
    }

    public virtual void OnHit()
    {
        VFX.Instance.At("hit", transform.position, -transform.up);
        Release();
    }

    public override void Spawn()
    {
        base.Spawn();
        Spawned = Time.time;

    }
}