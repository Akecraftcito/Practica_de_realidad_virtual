using UnityEngine;
using System.Collections;

public class Player_pingpong : MonoBehaviour {

	public float ballProximity = 4f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;

		if (Physics.Raycast(transform.position, transform.forward, out hit)) {
			if (hit.transform.GetComponent<Ball_pingpong>() != null) {
				Ball_pingpong ball = hit.transform.GetComponent<Ball_pingpong> ();

				if (ball.transform.position.z - transform.position.z < ballProximity && ball.direction.z < 0) {
					ball.OnPlayerHit ();
				}
			}
		}
	}
}
