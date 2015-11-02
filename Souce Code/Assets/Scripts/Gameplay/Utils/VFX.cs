using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VFX : MonoBehaviour {

    private static VFX _instance;
    public static VFX Instance
    {
        get
        {
            return _instance;
        }
    }

    public void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
        {
            Debug.Log("There are another instance of Effects");
            Object.Destroy(this);
        }

        ConvertToDictionary();
    }


    private List<LiveEffect> _liveEffects = new List<LiveEffect>();
    private Dictionary<string, SimpleCache<ParticleSystem>> EffectsCached = new Dictionary<string, SimpleCache<ParticleSystem>>();
    public VFXContainer[] Effects;
    private Dictionary<string, VFXContainer> _effectsInternal;    


    public void At(string alias, Vector2 position, float normalSize = 1, float normalCount = 1) {

        VFXContainer vfx = GetEffect(alias);

        var size = vfx.VFXData.RandomSize * normalSize;
        var count = vfx.VFXData.RandomCount * normalCount;

        for (int i = 0; i < count; i++)
            vfx.System.Emit(new Vector3(position.x, 0, position.y), Vector3.zero, size, vfx.VFXData.Lifetime, Color.white);
        
    }

    public void At(string alias, Vector2 position, Vector2 normal, float normalSize = 1, float normalCount = 1)
    {

        VFXContainer vfx = GetEffect(alias);

        var size = vfx.VFXData.RandomSize * normalSize;
        var count = vfx.VFXData.RandomCount * normalCount;

        var c1 = vfx.VFXData.Color.Evaluate(0);
        var c2 = vfx.VFXData.Color.Evaluate(1);


        for (int i = 0; i < count; i++)
        {

            Color c = new Color(Random.Range(c1.r, c2.r), Random.Range(c1.g, c2.g), Random.Range(c1.b, c2.b), 1);
            vfx.System.Emit(new Vector3(position.x, position.y, 0), vfx.VFXData.GetRandomVelocity(normal), size, vfx.VFXData.Lifetime, c);

        }
    }

    public delegate bool EffectConditionDelegate();

    public class LiveEffect
    {
        public ParticleSystem Effect;
        public EffectConditionDelegate Condition;
        public Transform Parent;
        public Vector2 Position;
        public float Rotation;
        public SimpleCache<ParticleSystem> Cache;

        public void On() {


            Effect = Cache.Pop();

            Effect.transform.parent = Parent;
            Effect.transform.localPosition = Position;
            Effect.gameObject.SetActive(true);

            foreach (var ps in Effect.gameObject.GetComponentsInChildren<ParticleSystem>())
                ps.startRotation = Rotation;

            Effect.Play(true);
        }

        public void Off() {
            if (Effect == null) return;
            Effect.transform.parent = null;
            Cache.Push(Effect);
            Effect = null;
        }
    }


    public void Update()
    {
        for (int i = 0; i < _liveEffects.Count; i++)
        {
            var e = _liveEffects[i];

            if (e.Condition() && e.Effect == null) e.On();
            else if (e.Effect != null)
            {
                if (!e.Condition())
                {
                    e.Off();
                    continue;
                }
                else
                {
                    //e.Effect.startRotation = e.Parent.rotation.eulerAngles.z * Mathf.Deg2Rad;                    
                }
            }           
        }
    }


    public LiveEffect ParentEffectIf(string alias, Transform parent, Vector2 position, float rotation, EffectConditionDelegate condition)
    {
        SimpleCache<ParticleSystem> cache;

        if (EffectsCached.ContainsKey(alias))
            cache = EffectsCached[alias];
        else
        {
            cache = new SimpleCache<ParticleSystem>(GetEffect(alias).System, 3);
            EffectsCached.Add(alias, cache);
        }

        LiveEffect parentedEffect = new LiveEffect
        {            
            Cache = cache,
            Condition = condition,
            Parent = parent,
            Position = position,
            Rotation = rotation
        };

        _liveEffects.Add(parentedEffect);

        return parentedEffect;
    }

    internal void UnparentAll(Transform ptransform, bool full)
    {

        if (full)
        {
            var transforms = ptransform.GetComponentsInChildren<Transform>();
            foreach (var t in transforms)
            {
                UnparentAll(t);
            }
        }
        else {
            UnparentAll(ptransform);
        }

        
    }

    internal void UnparentAll(Transform ptransform)
    {
        for (int i = _liveEffects.Count - 1; i >= 0; i--)
        {
            if (_liveEffects[i].Parent == ptransform)
            {
                _liveEffects[i].Off();
                _liveEffects[i].Condition = null;
                _liveEffects[i].Cache = null;
                _liveEffects.RemoveAt(i);
            }
        }
    }




    private VFXContainer GetEffect(string alias)
    {        
        return _effectsInternal.ContainsKey(alias) ? _effectsInternal[alias] : null;
    }

    public void OnValidate()
    {
        ConvertToDictionary();
    }

    public void ConvertToDictionary() {
        _effectsInternal = new Dictionary<string, VFXContainer>();        
        for (int i = 0; i < Effects.Length; i++)
        {
            _effectsInternal.Add(Effects[i].Alias, Effects[i]);
        }
    }
 

    [System.Serializable]
    public class VFXContainer {
        public string Alias;
        public ParticleSystem System;
        public VFXData VFXData;
        public bool NeedsParenting;
    }

    [System.Serializable]
    public class VFXData
    {
        public int Count;
        public int DeltaCount;
        public float Size;
        public float DeltaSize;
        public int Lifetime;
        public Gradient Color;
        public float Velocity;
        public float DeltaVelocity;

        public float VelocityDispersion;

        public Vector2 GetRandomVelocity(Vector2 normal) {

            var randomRotation = Quaternion.Euler(0, 0, Random.Range(-VelocityDispersion, VelocityDispersion));
            return randomRotation * normal * (Velocity + Random.Range(0, VelocityDispersion));
        }
     
        internal int RandomCount
        {
            get
            { return DeltaCount > 0 ? Count + Random.Range(0, DeltaCount) : Count; }
        }

        internal float RandomSize
        {
            get
            {
                return DeltaSize > 0 ? Size + Random.Range(0, DeltaSize) : Size;
            }

        }
    }

}
