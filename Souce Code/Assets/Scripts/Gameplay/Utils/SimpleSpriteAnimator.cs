using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class SimpleSpriteAnimator : MonoBehaviour {

    public float dt;
    public Sprite[] Sprites;
    private SpriteRenderer _renderer;
    private float _next;
    private int frameIndex;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();        
    }

    void Start()
    {
        _next = Time.time + dt;
    }

    void Update()
    {

        if(Sprites.Length > 0)

        if (Time.time > _next) {
            frameIndex = Mathf.Max(1, ++frameIndex) % Sprites.Length;
            _renderer.sprite = Sprites[frameIndex];
            _next = Time.time + dt;
        }
    }
}
