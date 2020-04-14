using com.xenturio.enums;
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
        SceneManager.LoadScene(SceneEnum.MAIN_GAME);
    }

    public void StartNewMultiplayerGame()
    {
        SceneManager.LoadScene(SceneEnum.MULTIPLAYER_MAIN_GAME);
    }

    public void ConnectNetworkScene() {
        SceneManager.LoadScene(SceneEnum.CONNECT_SCENE);
    }

    public void JoinMultiplayerGame() {
        SceneManager.LoadScene(SceneEnum.WAITING_ROOM);
    }

    public void LoadSavedGame()
    {
        SceneManager.LoadScene(PlayerPrefsController.GetSavedGameLevel());
    }

    public void StartBattleField() {
        SceneManager.LoadScene(SceneEnum.BATTLEFIELD, LoadSceneMode.Additive);
    }

    public void ReturnToMainGame() {
        SceneManager.UnloadSceneAsync(SceneEnum.BATTLEFIELD);
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void LoadLoseScreen()
    {
        SceneManager.LoadScene(SceneEnum.LOSE_SCREEN);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void LoadMainMenu()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.LoadScene(SceneEnum.START_SCREEN);
    }

    public void LoadOptionsScreen()
    {
        SceneManager.LoadScene(SceneEnum.OPTIONS_SCREEN);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}