using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject playerUI;
    public Shoot playerShoot;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        playerUI.SetActive(true);
        playerShoot.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameIsPaused = false;
        Time.timeScale = 1;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        playerUI.SetActive(false);
        playerShoot.enabled = false;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        GameIsPaused = true;
        Time.timeScale = 0;
    }

    public void Settings()
    {

    }

    public void Exit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
