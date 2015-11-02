using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner {
        
    [System.Serializable]
    public class GamePrefabs
    {
        public RelativeSpaceObject Wreck;
        public RelativeSpaceObject WreckLoco;
        public RelativeSpaceObject WreckStatic;
        public RelativeSpaceObject Jammer;
        public RelativeSpaceObject AlienSmall;
        public RelativeSpaceObject AlienBig;
        public RelativeSpaceObject AlienShield;
        public RelativeSpaceObject Rocket;
        public RelativeSpaceObject Bullet;
    }

    public enum ESpaceObjects
    {
        Wreck,
        WreckLoco,
        WreckStatic,
        Jammer,
        AlienSmall,
        AlienBig,
        AlienShield,
        Rocket,
        Bullet
    }

    private SimpleCache<RelativeSpaceObject>[] _cache;
    private System.Action<RelativeSpaceObject>[] _destroyDelegates;
    public List<RelativeSpaceObject> LiveObjects = new List<RelativeSpaceObject>();

    public System.Action<int> OnRewardEvent;

    private void OnReward(int reward)
    {
        if (OnRewardEvent != null)
            OnRewardEvent(reward);
    }
    
    public void ForeachLiveObject (System.Action<RelativeSpaceObject> action)
    {
        for (int i = LiveObjects.Count - 1; i >= 0 ; i--)        
            action(LiveObjects[i]);        
    }

    public Spawner(GamePrefabs prefabs)
    {
        _cache = new SimpleCache<RelativeSpaceObject>[System.Enum.GetNames(typeof(ESpaceObjects)).Length];

        _cache[(int)ESpaceObjects.Wreck] = new SimpleCache<RelativeSpaceObject>(prefabs.Wreck, 15);
        _cache[(int)ESpaceObjects.WreckLoco] = new SimpleCache<RelativeSpaceObject>(prefabs.WreckLoco, 4);
        _cache[(int)ESpaceObjects.WreckStatic] = new SimpleCache<RelativeSpaceObject>(prefabs.WreckStatic, 7);
        _cache[(int)ESpaceObjects.Jammer] = new SimpleCache<RelativeSpaceObject>(prefabs.Jammer, 3);
        _cache[(int)ESpaceObjects.AlienSmall] = new SimpleCache<RelativeSpaceObject>(prefabs.AlienSmall, 7);
        _cache[(int)ESpaceObjects.AlienBig] = new SimpleCache<RelativeSpaceObject>(prefabs.AlienBig, 6);
        _cache[(int)ESpaceObjects.AlienShield] = new SimpleCache<RelativeSpaceObject>(prefabs.AlienShield, 5);
        _cache[(int)ESpaceObjects.Rocket] = new SimpleCache<RelativeSpaceObject>(prefabs.Rocket, 8);
        _cache[(int)ESpaceObjects.Bullet] = new SimpleCache<RelativeSpaceObject>(prefabs.Bullet, 10);

        _destroyDelegates = new System.Action<RelativeSpaceObject>[_cache.Length];

        for (int i = 0; i < _cache.Length; i++)
        {
            var cache = _cache[i];
            var index = i;

            _destroyDelegates[i] = (x) =>
            {
                cache.Push(x);
                x.OnDestroy -= _destroyDelegates[index];
                x.OnReward -= OnReward;
                LiveObjects.Remove(x);
            };
        }
    }

    public RelativeSpaceObject SpawnSingle(ESpaceObjects e, Vector2 position)
    {
        var cahce = _cache[(int)e];        
        var so = cahce.Pop();
        so.OnDestroy += _destroyDelegates[(int)e];
        so.OnReward += OnReward;

        so.transform.position = position;
        so.Spawn();

        if (e == ESpaceObjects.AlienBig || e == ESpaceObjects.AlienShield || e == ESpaceObjects.AlienSmall)
        {
            Alien alien = so as Alien;
            alien.Spawner = this;
        }

        LiveObjects.Add(so);

        return so;
        
    }

    public void Spawn(ESpaceObjects e, int count, Vector2 start, Vector2 end)
    {
        if (count < 1) return;

        Debug.Log(string.Format("Spawn {0} {1}", count, e.ToString()));

        var cahce = _cache[(int)e];
        for (int i = 0; i < count; i++)
        {
            var so = cahce.Pop();
            so.OnDestroy += _destroyDelegates[(int)e];
            so.OnReward += OnReward;

            so.transform.position = new Vector2(Random.Range(start.x, end.x), Random.Range(start.y, end.y));
            so.Spawn();

            if (e == ESpaceObjects.AlienBig || e == ESpaceObjects.AlienShield || e == ESpaceObjects.AlienSmall)
            {
                Alien alien = so as Alien;
                alien.Spawner = this;
            }

            LiveObjects.Add(so);
        }
    }

    public void Spawn(ESpaceObjects e, int count, Vector2 start, Vector2 end, List<RelativeSpaceObject> list)
    {
        if (count < 1) return;

        Debug.Log(string.Format("Spawn {0} {1}", count, e.ToString()));

        var cahce = _cache[(int)e];
        for (int i = 0; i < count; i++)
        {
            var so = cahce.Pop();

            System.Action<RelativeSpaceObject> destroyDelegate = null;
            destroyDelegate = (x) =>
            {
                cahce.Push(x);
                list.Remove(x);
                Debug.Log(destroyDelegate);
                x.OnDestroy -= destroyDelegate;
                x.OnReward -= OnReward;
            };

            so.OnDestroy += destroyDelegate;
            so.OnReward += OnReward;


            so.transform.position = new Vector2(Random.Range(start.x, end.x), Random.Range(start.y, end.y));
            so.Spawn();

            if (e == ESpaceObjects.AlienBig || e == ESpaceObjects.AlienShield || e == ESpaceObjects.AlienSmall)
            {
                Alien alien = so as Alien;
                alien.Spawner = this;
            }

            list.Add(so);
        }
    }

    internal void InitPlayersWeapon(IWeapon obj)
    {
        obj.Init(this);
    }


}
