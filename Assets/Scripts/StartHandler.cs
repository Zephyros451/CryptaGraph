using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartHandler : MonoBehaviour
{
    [SerializeField] private Button button;

    private void OnEnable()
    {
        button.interactable = true;
    }

    public void StartGame()
    {
        SceneManager.instance.LoadGameScene();
    }
}
