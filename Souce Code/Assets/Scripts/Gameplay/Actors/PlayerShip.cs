using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayerShip : SpaceObject
{

    public float Speed;
    public float MouseSpeed;
    public float MaxSpeed = 3;
    public float HP = 30;
    public float HP_MAX = 30;

    [SerializeField]
    public ShipEffects _shipView;

    private IWeapon _currentWeapon;
    private List<BaseWeapon> _weapons = new List<BaseWeapon>();
    private int _currentWeaponIndex = 0;

    public event Action<IWeapon> OnWeaponChanged;
    public event Action<SpaceObject> OnCollision;
    public event Action<Projectile> OnBeingHit;



    void Start()
    {
        GetComponentInChildren<SimpleSpriteAnimator>().enabled = true;
        RecalculateSizeByCollider();
        VFX.Instance.ParentEffectIf("burst", transform, _shipView.RightEnginePosition, 0, MovingLeft);
        VFX.Instance.ParentEffectIf("burst", transform, _shipView.LeftEnginePosition, Mathf.PI, MovingRight);
    }

    bool MovingLeft()
    {
        return InternalVelocity.x < -0.1f;
    }

    bool MovingRight()
    {
        return InternalVelocity.x > 0.1f;
    }

    void InstallWeapon(BaseWeapon w)
    {
        if (_weapons == null)
            _weapons = new List<BaseWeapon>();

        _weapons.Add(w);
        SwapWeapon();
    }

    public void SwapWeapon()
    {
        _currentWeaponIndex = (_currentWeaponIndex + 1) % _weapons.Count;
        _currentWeapon = _weapons[_currentWeaponIndex];

        if (OnWeaponChanged != null)
            OnWeaponChanged(_currentWeapon);
    }

    protected Vector2 InternalVelocity;

    // Update is called once per frame
    public void Update()
    {

        InternalVelocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Speed + new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * MouseSpeed;

        InternalVelocity = Vector2.ClampMagnitude(InternalVelocity, MaxSpeed);

        var newX = Mathf.Clamp(transform.position.x + InternalVelocity.x, -CameraController.Bounds.x + Size.x / 2, CameraController.Bounds.x - Size.x / 2);
        var newY = Mathf.Clamp(transform.position.y + InternalVelocity.y, -CameraController.Bounds.y + Size.y / 2, CameraController.Bounds.y - Size.y / 2);
        transform.position = new Vector3(newX, newY, transform.position.z);

        if (_weapons != null && _weapons.Count > 1)        
            if (Input.GetKeyDown(KeyCode.Q)) SwapWeapon();
        

        if (_currentWeapon == null) return;
        
        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftControl))
                _currentWeapon.Hold(0);

        if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.LeftControl))
                _currentWeapon.Release(0);
            
        

       
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        var wc = collision.gameObject.GetComponent<WeaponContainer>();
        wc.OnAttachComplete += InstallWeapon;
        wc.Attach(transform);       
    }

  

    public void OnCollisionEnter2D(Collision2D collision)
    {
       

        var proj = collision.gameObject.GetComponent<Projectile>();
        if(proj != null)
        {

            proj.OnHit();

            if (OnBeingHit != null)
                OnBeingHit(proj);

            AddHitEffect(transform.InverseTransformPoint(collision.contacts[0].point));
        }
        else
        {
            var so = collision.gameObject.GetComponent<RelativeSpaceObject>();
            if (so != null)
            {
                if (OnCollision != null)
                    OnCollision(so);
            }

            AddHitEffect(transform.InverseTransformPoint(collision.contacts[0].point));
        }
    }

    public void AddHitEffect(Vector2 point)
    {

        if (UnityEngine.Random.Range(0, 100) > 50) return;

        point.y = Mathf.Clamp(point.y, -Size.y / 2, Size.y / 2);
        point.x = Mathf.Clamp(point.y, -Size.x / 2, Size.x / 2);
        VFX.Instance.ParentEffectIf("sparks", transform, point, 0, () => { return true; });
    }

    [Serializable]
    public class ShipEffects
    {

        public Vector2 LeftEnginePosition;
        public Vector2 RightEnginePosition;

    }


}














