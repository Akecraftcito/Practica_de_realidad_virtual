using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{
    public TextMesh infoText;
    public GameObject ball;
    public Player player;
    public Cup[] cups;

    public PauseMenuController pauseMenu;

    private int aciertos = 0;
    private int intentos = 0;

    private GameObject pausePanel;
    private Text scoreText;

    void Start()
    {
        infoText.text = "�Elige el vaso correcto!";

        CreatePauseCanvas();
        StartCoroutine(ShuffleRoutine());
    }

    void Update()
    {
        if (player.picked)
        {
            intentos++;

            if (player.won)
            {
                aciertos++;
                infoText.text = "�Ganaste!";
            }
            else
            {
                infoText.text = "Perdiste, �int�ntalo de nuevo!";
            }

            player.picked = false;
            StartCoroutine(ShowPauseMenuAfterDelay(3f));
        }
    }

    private IEnumerator ShowPauseMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (pauseMenu != null)
        {
            pauseMenu.Show(aciertos, intentos);
        }
    }

    private void CreatePauseCanvas()
    {
        GameObject canvasObject = new GameObject("PauseCanvas");
        canvasObject.transform.SetParent(null, false);
        canvasObject.transform.position = new Vector3(0f, 3.2f, 7.2f);
        canvasObject.transform.rotation = Quaternion.identity;
        canvasObject.transform.localScale = new Vector3(0.012f, 0.012f, 0.012f);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = 1f;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        pausePanel = CreateUiObject("PausePanel", canvas.transform);
        Image panelImage = pausePanel.AddComponent<Image>();
        panelImage.color = new Color(0.05f, 0.05f, 0.08f, 0.88f);
        SetRectTransform(pausePanel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(920f, 520f), new Vector2(0f, 40f));

        Text title = CreateText("Resultado", pausePanel.transform, 50);
        title.text = "Resultado";
        title.color = new Color(1f, 1f, 1f, 1f);
        SetRectTransform(title.gameObject, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(720f, 90f), new Vector2(0f, -65f));

        scoreText = CreateText("Score", pausePanel.transform, 38);
        scoreText.text = "Aciertos: 0 / Intentos: 0";
        scoreText.color = new Color(0.85f, 0.95f, 1f, 1f);
        SetRectTransform(scoreText.gameObject, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(720f, 90f), new Vector2(0f, 90f));

        Button continueButton = CreateButton("Continuar", pausePanel.transform);
        continueButton.targetGraphic.color = new Color(0.22f, 0.58f, 0.89f, 1f);
        continueButton.gameObject.tag = "Interactable";
        SetRectTransform(continueButton.gameObject, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(420f, 90f), new Vector2(0f, -25f));
        UpdateButtonCollider(continueButton.gameObject);

        Button exitButton = CreateButton("Salir", pausePanel.transform);
        exitButton.targetGraphic.color = new Color(0.74f, 0.24f, 0.24f, 1f);
        exitButton.gameObject.tag = "Interactable";
        SetRectTransform(exitButton.gameObject, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(420f, 90f), new Vector2(0f, -160f));
        UpdateButtonCollider(exitButton.gameObject);

        pauseMenu = GetComponent<PauseMenuController>();
        if (pauseMenu == null)
        {
            pauseMenu = gameObject.AddComponent<PauseMenuController>();
        }

        pauseMenu.panel = pausePanel;
        pauseMenu.scoreText = scoreText;
        pauseMenu.gameController = this;

        continueButton.onClick.AddListener(pauseMenu.OnContinuar);
        exitButton.onClick.AddListener(pauseMenu.OnSalir);

        pausePanel.SetActive(false);
    }

    private GameObject CreateUiObject(string objectName, Transform parent)
    {
        GameObject uiObject = new GameObject(objectName);
        uiObject.transform.SetParent(parent, false);
        return uiObject;
    }

    private Text CreateText(string objectName, Transform parent, int fontSize)
    {
        Text text = CreateUiObject(objectName, parent).AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        return text;
    }

    private Button CreateButton(string label, Transform parent)
    {
        GameObject buttonObject = CreateUiObject(label + "Button", parent);
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.12f, 0.45f, 0.75f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        BoxCollider boxCollider = buttonObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = false;
        boxCollider.size = new Vector3(1f, 1f, 1f);

        buttonObject.AddComponent<UIElementXR>();

        Text text = CreateText("Label", buttonObject.transform, 22);
        text.text = label;
        text.color = Color.white;
        SetRectTransform(text.gameObject, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        return button;
    }

    private void UpdateButtonCollider(GameObject buttonObject)
    {
        BoxCollider boxCollider = buttonObject.GetComponent<BoxCollider>();
        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();

        if (boxCollider != null && rectTransform != null)
        {
            boxCollider.size = new Vector3(rectTransform.rect.width, rectTransform.rect.height, 1f);
            boxCollider.center = Vector3.zero;
        }
    }

    private void SetRectTransform(GameObject target, Vector2 anchorMin, Vector2 anchorMax, Vector2 size, Vector2 position)
    {
        RectTransform rectTransform = target.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = position;
    }

    public void ReiniciarRonda()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (infoText != null)
        {
            infoText.gameObject.SetActive(true);
        }

        player.picked = false;
        player.won = false;
        player.canPick = false;

        foreach (Cup cup in cups)
        {
            cup.ball = null;
            cup.ResetForRound();
        }

        infoText.text = "�Elige el vaso correcto!";
        StartCoroutine(ShuffleRoutine());
    }

    private IEnumerator ShuffleRoutine()
    {
        yield return new WaitForSeconds(1f);

        foreach (Cup cup in cups)
        {
            cup.MoveUp();
        }

        yield return new WaitForSeconds(0.5f);

        Cup targetCup = cups[Random.Range(0, cups.Length)];
        targetCup.ball = ball;
        ball.transform.position = new Vector3(
            targetCup.transform.position.x,
            ball.transform.position.y,
            targetCup.transform.position.z
        );

        yield return new WaitForSeconds(1.0f);

        foreach (Cup cup in cups)
        {
            cup.MoveDown();
        }

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < 5; i++)
        {
            Cup cup1 = cups[Random.Range(0, cups.Length)];
            Cup cup2 = cup1;

            while (cup2 == cup1)
            {
                cup2 = cups[Random.Range(0, cups.Length)];
            }

            Vector3 cup1Position = cup1.targetPosition;
            cup1.targetPosition = cup2.targetPosition;
            cup2.targetPosition = cup1Position;

            yield return new WaitForSeconds(0.75f);
        }

        player.canPick = true;
    }
}
