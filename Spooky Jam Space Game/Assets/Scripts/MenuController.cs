using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject optionsMenu;
    [SerializeField]
    private GameObject controlsMenu;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject pauseDefaultMenu;
    [SerializeField]
    private GameObject pauseOptionsMenu;
    [SerializeField]
    private GameObject pauseControlsMenu;

    [SerializeField]
    private bool gamePaused;
    [SerializeField]
    private bool smoothPause;

    private void Start()
    {
        if (mainMenu != null)
        {
            mainMenu.SetActive(true);
            optionsMenu.SetActive(false);
            controlsMenu.SetActive(false);
        }

        gamePaused = false;
        smoothPause = false;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "EndScreen")
            return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        gamePaused = !gamePaused;

        if (gamePaused)
        {
            if (smoothPause)
                StartCoroutine(ChangeTime(1f, 0f, 1f)); //Time.timeScale = 0f;
            else
            {
                Time.timeScale = 0f;

                pauseDefaultMenu.SetActive(true);
                pauseOptionsMenu.SetActive(false);
                pauseControlsMenu.SetActive(false);
                pauseMenu.SetActive(true);
            }
        }
        else
        {
            if (smoothPause)
            {
                StartCoroutine(ChangeTime(0f, 1f, 1f));
                pauseDefaultMenu.SetActive(true);
                pauseOptionsMenu.SetActive(false);
                pauseControlsMenu.SetActive(false);
                pauseMenu.SetActive(gamePaused);
            }
            else
            {
                Time.timeScale = 1f;

                pauseDefaultMenu.SetActive(true);
                pauseOptionsMenu.SetActive(false);
                pauseControlsMenu.SetActive(false);
                pauseMenu.SetActive(gamePaused);
            }
        }
    }

    private IEnumerator ChangeTime(float start, float end, float time)
    {
        float lastTime = Time.realtimeSinceStartup;
        float timer = 0.0f;

        while (timer < time)
        {
            Time.timeScale = Mathf.Lerp(start, end, timer / time);
            timer += (Time.realtimeSinceStartup - lastTime);
            lastTime = Time.realtimeSinceStartup;

            yield return null;
        }

        Time.timeScale = end;

        if (gamePaused)
        {
            pauseDefaultMenu.SetActive(true);
            pauseOptionsMenu.SetActive(false);
            pauseControlsMenu.SetActive(false);
            pauseMenu.SetActive(true);
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(1);
    }

    public void Back()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        controlsMenu.SetActive(false);
    }

    public void Options()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
        controlsMenu.SetActive(false);
    }

    public void Controls()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResumeGame()
    {
        PauseGame();
    }

    public void PauseBack()
    {
        pauseDefaultMenu.SetActive(true);
        pauseOptionsMenu.SetActive(false);
        pauseControlsMenu.SetActive(false);
    }

    public void PauseOptions()
    {
        pauseDefaultMenu.SetActive(false);
        pauseOptionsMenu.SetActive(true);
        pauseControlsMenu.SetActive(false);
    }

    public void PauseControls()
    {
        pauseDefaultMenu.SetActive(false);
        pauseOptionsMenu.SetActive(false);
        pauseControlsMenu.SetActive(true);
    }
}
