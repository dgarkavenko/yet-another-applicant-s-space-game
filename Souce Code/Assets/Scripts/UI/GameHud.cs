using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System;

public class GameHud : MonoBehaviour {

    public HUDHealth HPBar;
    public Text Points;
    public Text Weapon;
    public FinalScore FinalResult;
    private PlayerShip _player;

    public void SetPoints(int points)
    {
        var stringPoints = points.ToString();

        while(stringPoints.Length < 5)        
            stringPoints = "0" + stringPoints;

        Points.text = stringPoints;
        FinalResult.Points.text = stringPoints;
    }

    public void FinalScoreMode(bool e)
    {
        FinalResult.gameObject.SetActive(e);
        HPBar.gameObject.SetActive(!e);
        Weapon.gameObject.SetActive(!e);
        Points.gameObject.SetActive(!e);

    }

    public void SetWeapon(string weapon)
    {
        Weapon.text = weapon;
    }

    internal void Init(PlayerShip player)
    {
        HPBar.SetHP(player.HP / player.HP_MAX);
        SetWeapon("UNARMED");

        _player = player;

        player.OnCollision += OnPlayerCollision;
        player.OnWeaponChanged += OnWeaponChanged;
        _player.OnBeingHit += OnPlayerCollision;

        SetPoints(0);

    }

    public void Unsub()
    {
        _player.OnCollision -= OnPlayerCollision;
        _player.OnBeingHit -= OnPlayerCollision;
        _player.OnWeaponChanged -= OnWeaponChanged;
    }

    void OnPlayerCollision(SpaceObject so)
    {
        HPBar.SetHP(_player.HP / _player.HP_MAX);
    }

    void OnWeaponChanged(IWeapon w)
    {
        SetWeapon(w.GetTitle());
    }
}
