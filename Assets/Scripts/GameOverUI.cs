using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{

    public static GameOverUI ui { get; private set; }
    [SerializeField] GameObject gameOverPanel;

    private void Awake()
    {
        if (ui != null && ui != this) { Destroy(gameObject); return; }
        ui = this;
        Time.timeScale = 1f;
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (gameOverPanel && gameOverPanel.activeSelf)
        {
            if (Input.GetKeyUp(KeyCode.R))
                Restart();
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;  //pause
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }
}


