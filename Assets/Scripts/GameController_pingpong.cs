using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
public class GameController_pingpong : MonoBehaviour {

	public Player_pingpong player;
	public Ball_pingpong ball;
	public TextMesh scoreText;
	[SerializeField] Canvas canvas;

	private float gameOverTimer = 3f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		bool isGameOver = ball.transform.position.z < player.transform.position.z;

		if (isGameOver == false) {
			scoreText.text = "Score: " + ball.score;
		} else {
			scoreText.text = "Game over!\nYour final score: " + ball.score;
            canvas.gameObject.SetActive(true);
        }
	}
	public void Reiniciar() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
	public void MainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}

/*
 Crear un canvas para mostrar los mensajes de score y gameover (3)
 En el canvas cuando sea game over debe tener dos botones 1 boton reiniciar el juego y otro boton regresar el menu principal (nuevo escenario de menu) (5)
 */