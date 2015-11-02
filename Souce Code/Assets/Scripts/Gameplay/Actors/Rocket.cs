using UnityEngine;
using System.Collections;

public class Rocket : Projectile {

    public AnimationCurve SpeedByLifetime;
    public SpriteRenderer Renderer;

    void Start()
    {
        VFX.Instance.ParentEffectIf("burst", transform, new Vector2(0.05f, -0.6f), Mathf.PI / 2, BurstOn);
    }

    public override void OnHit()
    {

        VFX.Instance.At("explosion", transform.position + Random.insideUnitSphere, Random.Range(0.7f, 1.5f));

        var hits = Physics2D.CircleCastAll(transform.position, 4, Vector2.zero, 0.001f, LayerMask.GetMask("Wreck", "Ship"));

        foreach (var hit in hits)
        {
            var so = hit.collider.gameObject.GetComponent<RelativeSpaceObject>();
            if(so != null)
            {
                var dmg = Damage / Mathf.Max(1, Vector3.Distance(hit.collider.transform.position, transform.position)) * 0.75f;
                so.ExplosionWave(dmg);
            }
        }

        Speed = 0;
        Release();
    }

    public bool BurstOn()
    {
        return Time.time - Spawned > 0.5f;
    }

    override public void Update() {

        base.Update();
        Speed = SpeedByLifetime.Evaluate(Time.time - Spawned);
    }

}
