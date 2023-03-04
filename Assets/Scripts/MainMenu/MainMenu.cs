using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject gameTitle;

    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject backgroud;


    public void playGame() {

        StartCoroutine(WaitBeforeSceneChange());
        gameTitle.GetComponent<CanvasGroup>().alpha = 0;
        mainMenu.GetComponent<CanvasGroup>().alpha = 0;
        backgroud.GetComponent<VideoPlayer>().playbackSpeed = 5;

    }

    public void quitGame() {
        Application.Quit();
    }


    IEnumerator WaitBeforeSceneChange() {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
