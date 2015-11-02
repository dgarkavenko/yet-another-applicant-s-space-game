using UnityEngine;
using System.Collections;

public class Wreckage : RelativeSpaceObject
{
    public float AngularSpeed = 2;
    public Sprite[] Sprites;
    public float HP;
    public Vector2 InitialRotation = new Vector2(0, 0);
    public Vector2 Locomotion = new Vector2(0, 1);

    private float _angularSpeed = 0;
    private SpriteRenderer _renderer;
    private BoxCollider2D _collider;

    public bool DontRecalculateSize;



    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();

        _collider = GetComponent<BoxCollider2D>();

        if (_collider == null)
            _collider = gameObject.AddComponent<BoxCollider2D>();
    }

    public override void Spawn()
    {
        base.Spawn();
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(InitialRotation.x, InitialRotation.y));
        
        if(Sprites.Length > 0)
        {
            _renderer.sprite = Sprites[Random.Range(0, Sprites.Length)];
            RecalculateSizeBySprite();
        }
            

        if (!DontRecalculateSize) _collider.size = Size;
        HP = Size.magnitude * 2;

        _angularSpeed = Random.Range(-AngularSpeed, AngularSpeed);
    }


    public virtual void Update()
    {
        transform.position += GetRelativeSpeed() + transform.rotation * Locomotion;
        transform.Rotate(Vector3.forward, _angularSpeed);
    }

    public override void PlayerImpact()
    {
        base.PlayerImpact();

        Explode();


    }

    public override void ExplosionWave(float dmg)
    {
        HP -= dmg;
        if (HP <= 0)
            Explode();
    }


    public override void ProjectileHit(Projectile projectile)
    {
        base.ProjectileHit(projectile);

       
            projectile.OnHit();
            HP -= projectile.Damage;
            if (HP <= 0)
                Explode();
        
    }
}
