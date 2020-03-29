using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

    [SerializeField] int timeToWait = 4;
    private int currentSceneIndex;

    public void Start()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    IEnumerator LoadStartScreen()
    {
        yield return new WaitForSeconds(timeToWait);
        SceneManager.LoadScene(1);
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene(ScreenEnum.MAIN_GAME);
    }

    public void LoadSavedGame()
    {
        SceneManager.LoadScene(PlayerPrefsController.GetSavedGameLevel());
    }

    public void StartBattleField() {
        SceneManager.LoadScene(ScreenEnum.BATTLEFIELD, LoadSceneMode.Additive);
    }

    public void ReturnToMainGame() {
        SceneManager.LoadScene(ScreenEnum.MAIN_GAME);
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void LoadLoseScreen()
    {
        SceneManager.LoadScene(ScreenEnum.LOSE_SCREEN);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(ScreenEnum.START_SCREEN);
    }

    public void LoadOptionsScreen()
    {
        SceneManager.LoadScene(ScreenEnum.OPTIONS_SCREEN);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}