using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameMenu : MonoBehaviour {


    public Button StartButton;
    public Button ContinueButton;

    private Animator _animator;

    public void Awake() {
        _animator = GetComponent<Animator>();             

    }

    public event System.Action OnHideComplete
    {
        add
        {
            Debug.Log("Subscribe " + value);
            _animator.GetBehaviour<HideBehaviour>().OnHideComplete += value;
        }

        remove {
            Debug.Log("Unsubscribe");
            _animator.GetBehaviour<HideBehaviour>().OnHideComplete -= value;
        }
    }

    public void Show(UnityEngine.Events.UnityAction startEvent, UnityEngine.Events.UnityAction continueEvent = null) {

        gameObject.SetActive(true);

        _animator.SetBool("IsActive", true);

        StartButton.onClick.AddListener(startEvent);
        if(continueEvent != null) ContinueButton.onClick.AddListener(continueEvent);
        ContinueButton.interactable = continueEvent != null;
    }

    public void Hide()
    {
        _animator.SetBool("IsActive", false);
        StartButton.onClick.RemoveAllListeners();
        ContinueButton.onClick.RemoveAllListeners();
    }




}
