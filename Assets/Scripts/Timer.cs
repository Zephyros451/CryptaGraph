using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Text text;

    private float gameTime;

    private void OnEnable()
    {
        gameTime = Random.Range(30,121);
        text.text = gameTime.ToString();
        StartCoroutine(GameTimer());
    }

    private IEnumerator GameTimer()
    {
        while (true)
        {
            gameTime--;
            text.text = gameTime.ToString();
            yield return new WaitForSeconds(1);

            if (gameTime == 0)
            {
                AudioPlayer.instance.PlayWin();
                SceneManager.instance.LoadWinScene();
            }
        }
    }
}
