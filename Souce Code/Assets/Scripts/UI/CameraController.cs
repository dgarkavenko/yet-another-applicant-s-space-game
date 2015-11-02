using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {


    public int Scale = 1;
    public static int PIXELS_IN_METER = 10;
    public static Vector2 Bounds;

    public Vector2 RelativeBounds
    {
        get {
            return new Vector2(transform.position.x, transform.position.y) - Bounds;
        }
    }

    void Awake()
    {
        var dx = Screen.width * 0.5f / Scale / PIXELS_IN_METER;
        var dy = Screen.height * 0.5f / Scale / PIXELS_IN_METER;
        Bounds = new Vector2(dx, dy);
    }


    Camera _cam;
	// Use this for initialization
	void Start () {
       _cam = GetComponent<Camera>();
        UpdateScale();
    }

    // Update is called once per frame
    void UpdateScale () {
        _cam.orthographicSize = Screen.height * 0.5f / Scale / PIXELS_IN_METER;

    }

    private Vector2 _target;
    private bool _move;
    private float _speed;
    public event Action OnCameraArrived;

    void Update()
    {
        if (_move) {

            var d = Vector2.Distance(_target, transform.position);
            if (d < _speed)
            {
                transform.position = new Vector3(_target.x, _target.y, transform.position.z);
                if (OnCameraArrived != null)
                    OnCameraArrived();
                _move = false;
            }
            else
            {
                var delta = (new Vector3(_target.x, _target.y, transform.position.z) - transform.position);
                transform.position += delta.normalized * _speed;
            }

        }

        var dx = Screen.width * 0.5f / Scale / PIXELS_IN_METER;
        var dy = Screen.height * 0.5f / Scale / PIXELS_IN_METER;
        Bounds = new Vector2(dx, dy);
    }

    internal void Move(Vector2 target, float speed = 0.4f)
    {
        _target = target;
        _speed = speed;
        _move = true;
    }
}
