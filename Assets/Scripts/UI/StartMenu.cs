using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public GameObject startMenuUI;
    public GameObject SettingsUI;

    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        //change scene
        SceneManager.LoadScene("GameScene");
    }

    public void Settings()
    {
        //Open setting sub menu
        SettingsUI.SetActive(true);
        startMenuUI.SetActive(false);
    }

    public void ExitGame()
    {
        //close game
        Application.Quit();
    }
}
