using UnityEngine;
using System.Collections;
using System;

public class WeaponContainer : MonoBehaviour {
    public Action<BaseWeapon> OnAttachComplete { get; internal set; }

    public BaseWeapon Weapon;
    Collider2D _collider;
    Transform _target;
    // Use this for initialization
    void Start () {
        _collider = GetComponent<Collider2D>();
        _collider.isTrigger = true;
    }
	
	// Update is called once per frame
	void Update () {

        if (_target != null) {

            transform.position = Vector3.Lerp(transform.position, _target.position, Time.deltaTime * 7);
            if(Vector2.Distance(transform.position, _target.position) < 0.1f)
            {
                if (OnAttachComplete != null)
                    OnAttachComplete(Weapon);

                transform.parent = _target;
                transform.localPosition = Vector2.zero;

                _target = null;
                enabled = false;
            }
        }
        else
        {
            if(transform.position.y > 0)
                transform.position -= Vector3.up * 0.045f;
        }

	}

    internal void Attach(Transform transform)
    {
        _collider.enabled = false;
        _target = transform;
    }
}
