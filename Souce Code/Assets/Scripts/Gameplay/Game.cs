using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;

public partial class Game : MonoBehaviour {

    private GameMenu _menu;
    private GameHud _hud;
    private CameraController _camController;
    private CameraEffects _camEffects;

    public PlayerShip PlayerPrefab;
    public PlayerShip Player;
    public List<WeaponContainer> PlayerWeapons;
    public Spawner Spawner;

    IGameState _introState;
    AccState _accState;
    IGameState _unarmedState;
    IGameState _blockadesAndEnemiesState;
    PowerUpState _powerUpState;
    IGameState _aliensState;
    IGameState _aliensFinalState;


    private List<RelativeSpaceObject> _jammers = new List<RelativeSpaceObject>();

    PauseState _pauseState;

    void Update()
    {
        if (_currentGameState == null) return;

        if(_currentGameState.NextState != null)
        {
            var t = _currentGameState == _pauseState ? _currentGameState.NextState.CurrentStateTime : 0;


            var nextState = _currentGameState.NextState;
            _currentGameState.Exit();
            _currentGameState = nextState;
            _currentGameState.Enter();

            _currentGameState.CurrentStateTime = t;
        }
        
        _currentGameState.Update();
        _camEffects.RelativeSpeed = RelativeSpeed;

        var treshold = _camController.RelativeBounds.y - 5;

        for (int i = Spawner.LiveObjects.Count-1; i >= 0; i--)
        {
            if(Spawner.LiveObjects[i].transform.position.y < treshold)
                Spawner.LiveObjects[i].Release();            
        }

        float jamStr = 0;

        for (int i = _jammers.Count - 1; i >= 0; i--)
        {
            if (_jammers[i].transform.position.y < treshold)
            {
                _jammers[i].Release();
                continue;
            }
            
            var d = Mathf.Max(3, Vector3.Distance(Player.transform.position, _jammers[i].transform.position));
            jamStr += Mathf.Clamp(6f / d, 0, 2);
        }

        _camEffects.JAMMERIntensity = jamStr;

    }


    public IGameState _currentGameState;
    public float RelativeSpeed = 0;

    public Spawner.GamePrefabs GamePrefabs;
    private int pts;

    void Start()
    {
        _menu = GetComponentInChildren<GameMenu>();
        _hud = GetComponentInChildren<GameHud>();


        _camEffects = Camera.main.GetComponent<CameraEffects>();
        _camController = Camera.main.GetComponent<CameraController>();

        _hud.FinalScoreMode(false);
        _hud.gameObject.SetActive(false);
        _menu.Show(OnStartNewGame, null);

        _introState = new IntroState(this) { TimeInState = 5, TimeAwake = 4f };
        _unarmedState = new UnarmedState(this);
        _accState = new AccState(this);
        _powerUpState = new PowerUpState(this);
        _blockadesAndEnemiesState = new BlockadeAndEnemiesState(this);
        _aliensState = new AliensState(this);
        _pauseState = new PauseState(this);
        _aliensFinalState = new AliensFinalState(this);

        _accState.ExitState = _unarmedState;

        Spawner = new Spawner(GamePrefabs);
        Spawner.OnRewardEvent += (reward_pts) =>
        {
            pts += reward_pts;
            _hud.SetPoints(pts);
        };
    }




    private void OnStartNewGame() {

        if(Player != null)
        {
            StartCoroutine(OnDeathCoroutine());
        }

        _menu.OnHideComplete += StartNewGame;
        _menu.Hide();
    }



    void StartNewGame()
    {

        _menu.OnHideComplete -= StartNewGame;
        _menu.gameObject.SetActive(false);

        Player = PlayerShip.Instantiate(PlayerPrefab);
        Player.transform.position = Vector3.zero;

        Player.OnCollision += OnPlayerCollision;
        Player.OnBeingHit += OnPlayerBeingHit;
        Player.OnWeaponChanged += OnWeaponChanged;

        _hud.Init(Player);

        _currentGameState = _introState;
        _currentGameState.Enter();
    }

    private void OnWeaponChanged(IWeapon obj)
    {
        Spawner.InitPlayersWeapon(obj);
    }

    void OnPlayerCollision(SpaceObject so)
    {
        var dmg = so.Size.magnitude;
        Player.HP -= dmg;
        _camEffects.HITIntensity = Mathf.Clamp(dmg,0,4);

        if (Player.HP <= 0)
        {
            GameOver();
        }
    }

    void OnPlayerBeingHit(Projectile p)
    {
        Player.HP -= p.Damage / 2;
        _camEffects.HITIntensity = p.Damage / 16f;

        if(Player.HP <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        _currentGameState = null;


        _camEffects.JAMMERIntensity = 2;
        _camEffects.RelativeSpeed = 0;
        Player.enabled = false;

        Player.OnCollision -= OnPlayerCollision;
        Player.OnBeingHit -= OnPlayerBeingHit;

        _hud.Unsub();

        Spawner.ForeachLiveObject((x) =>
        {
            x.RelativeSpeed = 0;
        });

        foreach (var item in _jammers)
        {
            item.RelativeSpeed = 0;
        }

        StartCoroutine(OnDeathCoroutine());

    }

    IEnumerator OnDeathCoroutine()
    {

        for (int i = 0; i < 5; i++)
        {
            VFX.Instance.At("explosion", Player.transform.position + UnityEngine.Random.insideUnitSphere * 4, UnityEngine.Random.Range(0.5f, 2f));
            yield return new WaitForSeconds(.2f);
        }

        if (Player != null)
        {
            VFX.Instance.UnparentAll(Player.transform, true);
            Destroy(Player.gameObject);
        }


        yield return new WaitForSeconds(.5f);
        _camEffects.JAMMERIntensity = 0;
        _hud.FinalScoreMode(true);
        yield return new WaitForSeconds(2.5f);
        _camEffects.JAMMERIntensity = 2;
        yield return new WaitForSeconds(1.5f);

        Reset();      
    }

    public void Reset()
    {
        Spawner.ForeachLiveObject((x) =>
        {
            x.enabled = true;
            x.Release();
        });

        for (int i = _jammers.Count - 1; i >= 0; i--)
        {
            _jammers[i].enabled = true;
            _jammers[i].Release();
        }

        _powerUpState.PowerupIndex = 0;
        _accState.ExitState = _unarmedState;
        _hud.FinalScoreMode(false);
        _hud.gameObject.SetActive(false);

        if (Player != null)
        {
            VFX.Instance.UnparentAll(Player.transform, true);
            Destroy(Player.gameObject);
        }
        pts = 0;
        _menu.Show(OnStartNewGame, null);
        _camEffects.JAMMERIntensity = 0;
    }









}
