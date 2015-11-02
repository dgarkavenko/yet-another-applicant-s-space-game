using UnityEngine;
using System.Collections;

public class Alien : RelativeSpaceObject {

    public AnimationCurve[] XMovements;
    public Spawner Spawner;

    public Spawner.ESpaceObjects WeaponType = Spawner.ESpaceObjects.Bullet;
       
    private float _eval;
   
    private AnimationCurve _currentMovementPattern;
    private SpriteRenderer _renderer;
    private BoxCollider2D _collider;

    public float FireRate = 0.4f;
    public Sprite[] Sprites;
    public float HP;

    public Vector2 NormalHeight = new Vector2(0.6f, 0.9f);
    private float _targetHeight;

    private float nextShot;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();

        if (_collider == null)
            _collider = gameObject.AddComponent<BoxCollider2D>();
        Spawn();
    }

    public override void Spawn()
    {
        base.Spawn();

        _renderer.sprite = Sprites[Random.Range(0, Sprites.Length)];
        RecalculateSizeBySprite();

        _collider.size = Size;

        HP = Size.magnitude;

        _currentMovementPattern = XMovements[Random.Range(0, XMovements.Length)];
        _eval = Random.Range(0, _currentMovementPattern.keys[_currentMovementPattern.length-1].time);
        transform.position = new Vector3(_currentMovementPattern.Evaluate(_eval) * CameraController.Bounds.x * 0.66f, transform.position.y, transform.position.z);

        nextShot = Time.time + FireRate * 2;
        _targetHeight = Random.Range(NormalHeight.x, NormalHeight.y) * CameraController.Bounds.y;
    }

    public override void ProjectileHit(Projectile projectile)
    {
        base.ProjectileHit(projectile);

        projectile.OnHit();
        HP -= projectile.Damage;
        if (HP <= 0)
            Explode();        

    }

    public override void ExplosionWave(float dmg)
    {
        HP -= dmg;
        if (HP <= 0)
            Explode();
    }



    public void Update()
    {
        if (transform.position.y > _targetHeight)
        {
            transform.position += Vector3.up * -0.1f;
        }

        _eval += Time.deltaTime;
        transform.position = new Vector3(_currentMovementPattern.Evaluate(_eval) * CameraController.Bounds.x * 0.66f, transform.position.y, transform.position.z);
        transform.position += GetRelativeSpeed();

        if (Time.time > nextShot && Spawner != null)
        {
            var proj = Spawner.SpawnSingle(WeaponType, new Vector3(transform.position.x, transform.position.y - Size.y - 0.2f, 0)) as Projectile;

            proj.transform.rotation = Quaternion.Euler(0, 0, 180);
            nextShot = Time.time + FireRate;

        }
        
    }

}
