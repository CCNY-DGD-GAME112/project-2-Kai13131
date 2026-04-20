using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuScreen : MonoBehaviour
{
    public GameObject MenuScreen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void StartButton()
    {
        Debug.Log("Start pressed");

        if (MenuScreen == null)
        {
            Debug.LogError("MenuScreen is NOT assigned!");
            return;
        }
        Time.timeScale = 1f;

        SceneManager.LoadScene("GameScene");


    }

    public void ExitButton()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}
