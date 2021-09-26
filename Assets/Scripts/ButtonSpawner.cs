using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ButtonSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] greenButtons;
    [SerializeField] private Transform[] redButtons;
    [SerializeField] private GraphInput graphInput;

    private Transform currentButton;
    Sequence sequence;

    private Vector2 spawnBounds;
    private int offset;

    private bool wasPressed;

    private void Awake()
    {
        Vector2 topRightCorner = new Vector2(Screen.width, Screen.height);

        spawnBounds = new Vector2(topRightCorner.x, topRightCorner.y);
        offset = Screen.width / 8;

        for (int i = 0; i < greenButtons.Length; i++)
        {
            greenButtons[i].localScale = Vector2.zero;
            redButtons[i].localScale = Vector2.zero;
        }
    }

    private void Start()
    {
        StartCoroutine(SpawnButton());
    }

    private void OnDisable()
    {
        sequence.Kill();
    }

    private IEnumerator SpawnButton()
    {
        while(true)
        {
            yield return new WaitForSeconds(2f);
            bool isRed = Random.Range(0, 2) > 0 ? true : false;
            int randomIndex = Random.Range(0, redButtons.Length);
            if(isRed)
            {
                ShowButton(redButtons[randomIndex]);
            }
            else
            {
                ShowButton(greenButtons[randomIndex]);
            }
        }
    }

    private void ShowButton(Transform button)
    {
        currentButton = button;
        button.GetComponent<Button>().interactable = true;
        wasPressed = false;
        button.position = new Vector3(Random.Range(0 + offset, spawnBounds.x - offset),
                                      Random.Range(0 + offset, spawnBounds.y - offset), 0f);
        sequence = DOTween.Sequence()
        .Append(button.DOScale(1f, 0.3f))
        .AppendInterval(0.3f)
        .Append(button.DOScale(0f, 1f))
        .AppendCallback(PressCheck);
    }

    private void PressCheck()
    {
        if(currentButton == greenButtons[0] && !wasPressed && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 1)
        {
            graphInput.MakeStep(0);
        }
    }

    public void Pressed()
    {
        wasPressed = true;
    }
}
