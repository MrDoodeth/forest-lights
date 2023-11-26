using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] TransitionSettings transition;

    public void LoadScene(string _sceneName)
    {
        Time.timeScale = 1;
        if (GameObject.Find("Man").TryGetComponent(out PlayerBehaviour pl))
        {
            pl.outPause.TransitionTo(0.2f);
        }
        TransitionManager.Instance().Transition(_sceneName, transition, 0);
    }

    public void Quit() 
    {
        Application.Quit();
    }
}
