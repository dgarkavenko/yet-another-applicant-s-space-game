using UnityEngine;
using System.Collections;
using System;

public partial class Game {

    

    public interface IGameState
    {
        IGameState NextState { get; set; }

        void Update();
        void Enter();
        void Exit();

        float CurrentStateTime
        {
            get; set;
        }
    }

    public class BaseState : IGameState
    {
        protected Game Game;
        public float CurrentStateTime
        {
            get; set;
        }

        public BaseState(Game game)
        {
            Game = game;
        }

        public virtual void Update()
        {

            CurrentStateTime += Time.deltaTime;           

            Game._camEffects.RelativeSpeed = Game.RelativeSpeed;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                NextState = Game._pauseState;
                Game._pauseState.InterruptedState = this;
            }
        }

        public virtual void Exit()
        {
            NextState = null;
        }

        public virtual void Enter()
        {
            CurrentStateTime = 0;
        }

        public IGameState NextState
        {
            get; set;
        }
    }


    public class IntroState : BaseState, IGameState
    {

        public IntroState(Game game) : base(game)
        {

        }

        public override void Update()
        {
            CurrentStateTime += Time.deltaTime;
            Game._camEffects.RelativeSpeed = Game.RelativeSpeed;


            var currentStateTime = CurrentStateTime;

            if (currentStateTime > TimeAwake)
                AwakePlayer();

            

            if (currentStateTime > TimeInState) NextState = Game._accState;
        }

        public override void Enter()
        {
            base.Enter();

            Debug.Log("Intro state");


            Game._camController.transform.position -= new Vector3(0, CameraController.Bounds.y * 1f, 0);
            Game._camController.Move(Vector2.zero, 0.05f);

            Game.RelativeSpeed = 0;
            Game.Player.enabled = false;                    

            Game.Spawner.Spawn(
                    Spawner.ESpaceObjects.Wreck,
                    15,
                    new Vector2(-CameraController.Bounds.x, 0),
                    new Vector2(CameraController.Bounds.x, CameraController.Bounds.y * 4)
                    );

            Game.Spawner.ForeachLiveObject((x) =>
            {
                x.RelativeSpeed = 0;
            });

        }

        public void AwakePlayer()
        {
            if (Game.Player.enabled) return;
            Game.Player.enabled = true;
            Game._hud.gameObject.SetActive(true);
        }

        

        public float TimeInState;
        public float TimeAwake;
    }

    public class AccState : BaseState, IGameState
    {

        public AccState(Game game) : base(game)
        {
        }

        public IGameState ExitState;

        public override void Enter()
        {
            base.Enter();

            Game.Spawner.ForeachLiveObject((x) =>
            {
                x.RelativeSpeed = 1;
            });
        }

        public override void Update()
        {
            base.Update();

            var stateTime = CurrentStateTime;
            Game.RelativeSpeed = Mathf.Clamp01(stateTime * stateTime / 8);

            if (Game.RelativeSpeed >= 1) NextState = ExitState;
        }

    }

    public class SpawningState : BaseState, IGameState
    {
        public SpawningState(Game game) : base(game)
        {
        }

        protected float nextSpawnTime = 0;
        protected float rareSpawnTime = 0;


        public override void Enter()
        {
            base.Enter();
            nextSpawnTime = 0;
            rareSpawnTime = 0;
        }

    }

    public class UnarmedState : SpawningState, IGameState
    {
        public UnarmedState(Game game) : base(game)
        {
        }        


        public override void Update()
        {
            base.Update();

            if (CurrentStateTime > nextSpawnTime)
            {
                nextSpawnTime = CurrentStateTime + UnityEngine.Random.Range(1, 3);

                Game.Spawner.Spawn(
                    Spawner.ESpaceObjects.Wreck, 
                    UnityEngine.Random.Range(2,6), 
                    new Vector2(-CameraController.Bounds.x, CameraController.Bounds.y), 
                    new Vector2(CameraController.Bounds.x, CameraController.Bounds.y * 2)
                    );

                if (CurrentStateTime > 6) {
                    Game.Spawner.Spawn(
                    Spawner.ESpaceObjects.WreckLoco,
                    UnityEngine.Random.Range(-1, 2),
                    new Vector2(-CameraController.Bounds.x / 2, CameraController.Bounds.y),
                    new Vector2(CameraController.Bounds.x / 2, CameraController.Bounds.y * 1.2f)
                    );
                }
            }    
            
            if(CurrentStateTime > 20)
            {
                NextState = Game._powerUpState;
            }
                   
        }
    }

    public class PowerUpState : BaseState, IGameState
    {

        public PowerUpState(Game game) : base(game)
        {

        }

        public WeaponContainer _weapon;
        public int PowerupIndex;

        public override void Update()
        {
            base.Update();
            Game.RelativeSpeed = Mathf.Lerp(Game.RelativeSpeed, 0, Time.deltaTime);

            if(Game.RelativeSpeed < 0.1f && _weapon == null)
            {
                _weapon = WeaponContainer.Instantiate(Game.PlayerWeapons[PowerupIndex]);
                _weapon.gameObject.SetActive(true);
                _weapon.transform.position = new Vector2(0, CameraController.Bounds.y);
                Game.Player.OnWeaponChanged += OnWeaponChanged;


            }
            
        }

        public void OnWeaponChanged(IWeapon w) {

            _weapon = null;

            Game.Player.OnWeaponChanged -= OnWeaponChanged;
            NextState = Game._accState;
            (Game._accState as AccState).ExitState = PowerupIndex == 0 ? Game._blockadesAndEnemiesState : Game._aliensFinalState;
            PowerupIndex++;

        }


        public override void Enter()
        {
            base.Enter();
        }
    }

    public class BlockadeAndEnemiesState : SpawningState, IGameState
    {
        public BlockadeAndEnemiesState(Game game) : base(game)
        {

        }

        public override void Update()
        {
            base.Update();

            if (CurrentStateTime > nextSpawnTime)
            {
                nextSpawnTime = CurrentStateTime + UnityEngine.Random.Range(1, 3);

                Game.Spawner.Spawn(
                    Spawner.ESpaceObjects.Wreck,
                    UnityEngine.Random.Range(5, 6) - (int)(CurrentStateTime / 10),
                    new Vector2(-CameraController.Bounds.x, CameraController.Bounds.y),
                    new Vector2(CameraController.Bounds.x, CameraController.Bounds.y * 2)
                    );

                Game.Spawner.Spawn(
                    Spawner.ESpaceObjects.WreckLoco,
                    UnityEngine.Random.Range(-1, 2) - (int)(CurrentStateTime / 25),
                    new Vector2(-CameraController.Bounds.x / 2, CameraController.Bounds.y),
                    new Vector2(CameraController.Bounds.x / 2, CameraController.Bounds.y * 1.2f)
                    );              
            }

            if (CurrentStateTime > rareSpawnTime)
            {

                rareSpawnTime = CurrentStateTime + UnityEngine.Random.Range(3, 5);


                if (CurrentStateTime > 6)
                {
                    Game.Spawner.Spawn(
                        Spawner.ESpaceObjects.WreckStatic,
                        UnityEngine.Random.Range(0, 2),
                        new Vector2(-CameraController.Bounds.x / 2, CameraController.Bounds.y * 1.4f),
                        new Vector2(CameraController.Bounds.x / 2, CameraController.Bounds.y * 1.7f)
                    );
                }
                
                Game.Spawner.Spawn(
                    Spawner.ESpaceObjects.Jammer,
                    UnityEngine.Random.Range(0, 2 + (int)(CurrentStateTime / 15)),
                    new Vector2(-CameraController.Bounds.x / 2, CameraController.Bounds.y * 1.2f),
                    new Vector2(CameraController.Bounds.x / 2, CameraController.Bounds.y * 1.5f),
                    Game._jammers
                );
                
            }

            if(CurrentStateTime > 45)
            {
                NextState = Game._aliensState;
            }
        }
    }

    public class AliensState : SpawningState, IGameState
    {
        public AliensState(Game game) : base(game)
        {

        }

        public override void Update()
        {
            base.Update();

            if (Game.pts > 2000 && CurrentStateTime > 10)
            {
                if(CurrentStateTime > nextSpawnTime)
                    NextState = Game._powerUpState;

                return;
            }

            if (CurrentStateTime > nextSpawnTime)
            {
                nextSpawnTime = CurrentStateTime + UnityEngine.Random.Range(6, 8);

                if (Game.Spawner.LiveObjects.Count > 30) return;

                Game.Spawner.Spawn(
                    Spawner.ESpaceObjects.AlienSmall,
                    UnityEngine.Random.Range(2, 3) + (int)(CurrentStateTime / 10),
                    new Vector2(-CameraController.Bounds.x, CameraController.Bounds.y * 1.2f),
                    new Vector2(CameraController.Bounds.x, CameraController.Bounds.y * 1.3f)
                    );

                Game.Spawner.Spawn(
                        Spawner.ESpaceObjects.Jammer,
                        UnityEngine.Random.Range(0, 3),
                        new Vector2(-CameraController.Bounds.x / 2, CameraController.Bounds.y * 1.2f),
                        new Vector2(CameraController.Bounds.x / 2, CameraController.Bounds.y * 1.5f),
                        Game._jammers
                    );

                Game.Spawner.Spawn(
                        Spawner.ESpaceObjects.WreckStatic,
                        UnityEngine.Random.Range(0, 2),
                        new Vector2(-CameraController.Bounds.x / 2, CameraController.Bounds.y * 1.4f),
                        new Vector2(CameraController.Bounds.x / 2, CameraController.Bounds.y * 1.7f)
                    );
            }

           
        }
    }

    public class AliensFinalState : SpawningState, IGameState
    {
        public AliensFinalState(Game game) : base(game)
        {

        }


        public override void Enter()
        {
            base.Enter();
            Game.Spawner.Spawn(
               Spawner.ESpaceObjects.WreckStatic,
               3,
               new Vector2(-CameraController.Bounds.x, CameraController.Bounds.y * 1.6f),
               new Vector2(CameraController.Bounds.x, CameraController.Bounds.y * 2.5f)
               );
        }

        public override void Update()
        {
            base.Update();            

            if (CurrentStateTime > nextSpawnTime)
            {
                nextSpawnTime = CurrentStateTime + UnityEngine.Random.Range(6, 8);


                Game.Spawner.Spawn(
                Spawner.ESpaceObjects.Wreck,
                UnityEngine.Random.Range(3 - (int)(CurrentStateTime / 20), 12 - (int)(CurrentStateTime / 20)),
                new Vector2(-CameraController.Bounds.x, CameraController.Bounds.y * 1.2f),
                new Vector2(CameraController.Bounds.x, CameraController.Bounds.y * 2.5f)
                );        


                if (Game.Spawner.LiveObjects.Count > 30 || CurrentStateTime < 15) return;

                Game.Spawner.Spawn(
                    Spawner.ESpaceObjects.AlienSmall,
                    UnityEngine.Random.Range(2, 3 + (int)(CurrentStateTime / 10)) ,
                    new Vector2(-CameraController.Bounds.x, CameraController.Bounds.y * 1.2f),
                    new Vector2(CameraController.Bounds.x, CameraController.Bounds.y * 1.3f)
                    );

                Game.Spawner.Spawn(
                        Spawner.ESpaceObjects.Jammer,
                        UnityEngine.Random.Range(0, 2 + (int)(CurrentStateTime / 15)),
                        new Vector2(-CameraController.Bounds.x / 2, CameraController.Bounds.y * 1.2f),
                        new Vector2(CameraController.Bounds.x / 2, CameraController.Bounds.y * 1.5f),
                        Game._jammers
                    );

                Game.Spawner.Spawn(
                        Spawner.ESpaceObjects.WreckStatic,
                        UnityEngine.Random.Range(0, 2),
                        new Vector2(-CameraController.Bounds.x / 2, CameraController.Bounds.y * 1.4f),
                        new Vector2(CameraController.Bounds.x / 2, CameraController.Bounds.y * 1.7f)
                    );
            }
        }
    }

    public class PauseState : BaseState, IGameState{

        public PauseState(Game game) : base(game)
        {

        }

        public bool Resuming;

        public IGameState InterruptedState;

        public override void Enter()
        {

            Resuming = false;

            base.Enter();
            Game._hud.gameObject.SetActive(false);

            Game.Spawner.ForeachLiveObject((x) => {
                x.enabled = false;
            });

            foreach (var j in Game._jammers)
                j.enabled = false;

            Game.Player.enabled = false;
            Game._menu.Show(Game.OnStartNewGame, OnContinue);
        }

        public override void Update()
        {
            CurrentStateTime += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Escape) && !Resuming)
            {
                OnContinue();                
            }
        }

        public override void Exit()
        {
            base.Exit();            
        }

        public void OnContinue()
        {
            Game._menu.OnHideComplete += OnHideComlete;
            Game._menu.Hide();
            Resuming = true;
        }

        private void OnHideComlete()
        {
            Game._menu.OnHideComplete -= OnHideComlete;
            Game._hud.gameObject.SetActive(true);
            Game._menu.gameObject.SetActive(false);
            Game.Spawner.ForeachLiveObject( (x)=>{
                x.enabled = true;
            });

            foreach (var j in Game._jammers)            
                j.enabled = true;
            

            Game.Player.enabled = true;
            NextState = InterruptedState;

            InterruptedState = null;
        }
    }

}
