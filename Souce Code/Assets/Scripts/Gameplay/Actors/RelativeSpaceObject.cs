using UnityEngine;
using System.Collections;

public class RelativeSpaceObject : SpaceObject {

    [SerializeField]
    protected Vector2 RelativeVelocity;
    public float RelativeSpeed;

    public int RewardPTS;

    public event System.Action<int> OnReward;

    public Vector3 GetRelativeSpeed()
    {
        return RelativeVelocity * RelativeSpeed;
    }

    public virtual void Spawn()
    {
        RelativeSpeed = 1;
        transform.rotation = Quaternion.identity;
    }

    public event System.Action<RelativeSpaceObject> OnDestroy;

    public void Release()
    {
        if (OnDestroy != null)
            OnDestroy(this);
    }

    public void Explode()
    {
        if (OnReward != null && RewardPTS > 0)
            OnReward(RewardPTS);

        int count = 1 + (int)(Size.y / 5f);

        if (count == 1)
            VFX.Instance.At("explosion", transform.position);
        else
            for (int i = 0; i < count; i++)
            {
                Vector3 offset = new Vector3(0, -Size.y / 2 + Size.y / (count) * i);
                offset = transform.rotation * offset;
                VFX.Instance.At("explosion", transform.position - offset, Random.Range(0.8f, 1.5f));
            }

        Release();
    }

    public virtual void ExplosionWave(float dmg)
    {

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {


        var layer = LayerMask.LayerToName(collision.gameObject.layer);

        if (layer == "PlayerShip")
        {
            PlayerImpact();
        }
        else if(layer == "Projectile" || layer == "Rocket")
        {
            ProjectileHit(collision.gameObject.GetComponent<Projectile>());            
        }
        
    }

    public virtual void PlayerImpact()
    {
        
    }


    public virtual void ProjectileHit(Projectile projectile)
    {
        
    }
}
