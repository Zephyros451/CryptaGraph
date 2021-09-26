using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestartHandler : MonoBehaviour
{
    [SerializeField] private Button button;

    private void OnEnable()
    {
        button.interactable = true;
    }

    public void Restart()
    {
        SceneManager.instance.LoadMainMenuScene();
        AudioPlayer.instance.UnPauseBackground();
    }
}
