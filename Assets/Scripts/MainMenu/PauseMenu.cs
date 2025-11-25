using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private PlayerKeyMaps playerKeyMaps;
    private InputAction menu;

    [SerializeField] private GameObject pauseMenuUI;
    public static bool isPaused = false;

    void Awake()
    {
        playerKeyMaps = new PlayerKeyMaps();
    }
    private void OnEnable()
    {
        menu = playerKeyMaps.Menu.Escape;
        menu.Enable();

        menu.performed += Pause;
    }
    private void OnDisable()
    {
        menu.Disable();
    }
    public void Pause(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            ActivateMenu();
        }
        else
        {
            DeactivateMenu();
        }
    }
    void ActivateMenu()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }
    public void DeactivateMenu()
    {
        pauseMenuUI.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        DeactivateMenu();
    }
    public void QuitResume()
    {
        SceneManager.LoadScene(0);
        DeactivateMenu();
    }
}