using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public GameObject panel;
    public Text scoreText;
    public GameController gameController;

    void Awake()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    public void Show(int aciertos, int intentos)
    {
        if (panel == null || scoreText == null)
        {
            return;
        }

        if (gameController != null && gameController.infoText != null)
        {
            gameController.infoText.gameObject.SetActive(false);
        }

        panel.SetActive(true);
        scoreText.text = "Aciertos: " + aciertos + " / Intentos: " + intentos;
        Time.timeScale = 0f;
    }

    public void OnContinuar()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        Time.timeScale = 1f;

        if (gameController != null)
        {
            gameController.ReiniciarRonda();
        }
    }

    public void OnSalir()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
