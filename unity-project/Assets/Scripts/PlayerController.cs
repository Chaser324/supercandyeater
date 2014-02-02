using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	#region Constants

	public enum FieldPosition { P1, P2 };
	public enum FieldPlayer { Human, AI };

	#endregion

	#region Public Variables

	public FieldPosition position;
	public FieldPlayer player;

	#endregion

	#region Private Variables

	private Vector2 lastScreenSize;

	private FruitController[,] playfield = new FruitController[6,12];
	private FruitPairController currentPiece;

	#endregion



	#region Event Handlers

	void Start () {
	}

	void Update () {
		UpdatePosition();

		// Handle player input

		// Apply gravity

		// Join pieces

		// Remove pieces



	}

	#endregion

	#region Private Methods

	private void UpdatePosition() {
		Vector2 screenSize = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);

		if (this.lastScreenSize != screenSize) {
			this.lastScreenSize = screenSize;

			float screenHeight = Camera.main.pixelHeight;
			float screenWidth = Camera.main.pixelWidth;
			
			if (position == FieldPosition.P1) {
				transform.position = Camera.main.ScreenToWorldPoint(new Vector3(25,screenHeight-25,10));
			}
			else {
				transform.position = Camera.main.ScreenToWorldPoint(new Vector3(screenWidth-(98*3)-20,screenHeight-25,10));
			}
		}
	}

	#endregion
}
