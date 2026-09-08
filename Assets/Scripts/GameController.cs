using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {

	public TextMesh infoText;
	public GameObject ball;
	public Player player;
	public Cup[] cups;

	private int hits;
	private int attempts;
	private bool roundEnded;
	private GameObject pausePanel;
	private Text scoreText;

	// Use this for initialization
	void Start () {
		infoText.text = "Escoje la copa correcta!";

		StartCoroutine (ShuffleRoutine());
		CreatePauseCanvas();
	}

	// Update is called once per frame
	void Update () {
		if (player.picked && !roundEnded) {
			roundEnded = true;
			attempts++;

			if (player.won) {
				hits++;
				infoText.text = "Ganaste :D!";
			} else {
				infoText.text = "Perdiste :( intenta de nuevo!";
			}

			StartCoroutine (ShowPausePanelAfterDelay());
		}
	}

	private void CreatePauseCanvas () {
		GameObject canvasObject = new GameObject("PauseCanvas");
		Canvas canvas = canvasObject.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvasObject.AddComponent<CanvasScaler>();
		canvasObject.AddComponent<GraphicRaycaster>();

		pausePanel = CreateUiObject("PausePanel", canvas.transform);
		Image panelImage = pausePanel.AddComponent<Image>();
		panelImage.color = new Color(0f, 0f, 0f, 0.82f);
		SetRectTransform(pausePanel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(420f, 300f), Vector2.zero);

		Text title = CreateText("Resultado", pausePanel.transform, 28);
		title.text = "Resultado";
		SetRectTransform(title.gameObject, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(360f, 50f), new Vector2(0f, -35f));

		scoreText = CreateText("Score", pausePanel.transform, 22);
		SetRectTransform(scoreText.gameObject, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(360f, 45f), new Vector2(0f, -95f));

		Button continueButton = CreateButton("Continuar", pausePanel.transform);
		continueButton.onClick.AddListener(ContinueRound);
		SetRectTransform(continueButton.gameObject, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(240f, 55f), new Vector2(0f, 20f));

		Button exitButton = CreateButton("Salir", pausePanel.transform);
		exitButton.onClick.AddListener(ExitGame);
		SetRectTransform(exitButton.gameObject, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(240f, 55f), new Vector2(0f, -55f));

		pausePanel.SetActive(false);
	}

	private GameObject CreateUiObject (string objectName, Transform parent) {
		GameObject uiObject = new GameObject(objectName);
		uiObject.transform.SetParent(parent, false);
		return uiObject;
	}

	private Text CreateText (string objectName, Transform parent, int fontSize) {
		Text text = CreateUiObject(objectName, parent).AddComponent<Text>();
		text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		text.fontSize = fontSize;
		text.color = Color.white;
		text.alignment = TextAnchor.MiddleCenter;
		return text;
	}

	private Button CreateButton (string label, Transform parent) {
		GameObject buttonObject = CreateUiObject(label + "Button", parent);
		Image image = buttonObject.AddComponent<Image>();
		image.color = new Color(0.12f, 0.45f, 0.75f, 1f);
		Button button = buttonObject.AddComponent<Button>();
		Text text = CreateText("Label", buttonObject.transform, 20);
		text.text = label;
		SetRectTransform(text.gameObject, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
		return button;
	}

	private void SetRectTransform (GameObject target, Vector2 anchorMin, Vector2 anchorMax, Vector2 size, Vector2 position) {
		RectTransform rectTransform = target.GetComponent<RectTransform>();
		rectTransform.anchorMin = anchorMin;
		rectTransform.anchorMax = anchorMax;
		rectTransform.sizeDelta = size;
		rectTransform.anchoredPosition = position;
	}

	private void ShowPausePanel () {
		scoreText.text = "Aciertos: " + hits + " / Intentos: " + attempts;
		pausePanel.SetActive(true);
		Time.timeScale = 0f;
	}

	private IEnumerator ShowPausePanelAfterDelay () {
		yield return new WaitForSeconds (3f);
		ShowPausePanel();
	}

	public void ContinueRound () {
		Time.timeScale = 1f;
		pausePanel.SetActive(false);
		player.picked = false;
		player.won = false;
		player.canPick = false;
		roundEnded = false;

		foreach (Cup cup in cups) {
			cup.ResetForRound();
		}

		infoText.text = "Escoje la copa correcta!";
		StartCoroutine (ShuffleRoutine());
	}

	private void ExitGame () {
		Time.timeScale = 1f;

#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	private IEnumerator ShuffleRoutine () {
		yield return new WaitForSeconds (1f);

		foreach (Cup cup in cups) {
			cup.MoveUp ();
		}

		yield return new WaitForSeconds (0.5f);

		Cup targetCup = cups[Random.Range(0, cups.Length)];
		targetCup.ball = ball;
		ball.transform.position = new Vector3 (
			targetCup.transform.position.x,
			ball.transform.position.y,
			targetCup.transform.position.z
		);

		yield return new WaitForSeconds (1.0f);

		foreach (Cup cup in cups) {
			cup.MoveDown ();
		}

		yield return new WaitForSeconds (1.0f);

		for (int i = 0; i < 5; i++) {
			Cup cup1 = cups[Random.Range(0, cups.Length)];
			Cup cup2 = cup1;

			while (cup2 == cup1) {
				cup2 = cups[Random.Range(0, cups.Length)];
			}

			Vector3 cup1Position = cup1.targetPosition;

			cup1.targetPosition = cup2.targetPosition;
			cup2.targetPosition = cup1Position;

			yield return new WaitForSeconds (0.75f);
		}

		player.canPick = true;
	}
}