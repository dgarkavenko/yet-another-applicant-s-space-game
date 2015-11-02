using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDHealth : MonoBehaviour {

    [SerializeField]
    RectTransform _back;
    [SerializeField]

    RectTransform _filler;


    void Start()
    {

    }

    public void SetHP(float hp)
    {
        _filler.anchorMax = new Vector2(hp, 1);
    }


}
