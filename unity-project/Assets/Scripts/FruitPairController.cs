using UnityEngine;
using System.Collections;

public class FruitPairController : MonoBehaviour {

	#region Private Variables

	private FruitController[] fruit = new FruitController[2];
	private PlayerController player;

	private bool grounded = false;

	#endregion

	#region Event Handlers

	void Start() {
		player = this.transform.parent.parent.GetComponent<PlayerController>();

		this.transform.localPosition = new Vector3(3f * PlayerController.CellSize, 2f * PlayerController.CellSize, 0);

		fruit[0].transform.localPosition = new Vector3(0,0,0);
		fruit[1].transform.localPosition = new Vector3(0, -1f * 4f * PlayerController.CellSize, 0);

		fruit[0].xPos = 0;
		fruit[0].yPos = 0;
		fruit[0].height = 1;
		fruit[0].width = 1;

		fruit[1].xPos = 0;
		fruit[1].yPos = -1;
		fruit[1].height = 1;
		fruit[1].width = 1;
	}

	void Update() {
	}

	#endregion

	#region Public Methods

	public void MakeVisible() {
		fruit[0].Show();
		fruit[1].Show();
	}

	public void MakeCurrent() {
		MakeVisible();
		this.transform.Find("highlight").gameObject.SetActive(true);
    }

	public void Rotate(int direction) {
		int fruit1X = Mathf.CeilToInt(this.transform.localPosition.x / PlayerController.CellSize);
		int fruit1Y = -1 * Mathf.CeilToInt(this.transform.localPosition.y / PlayerController.CellSize) + 1;

		// Get desired coordinates for fruit2
		// (x,y) rotated left is (-y,x)
		// (x,y) rotated right is (y,-x)
		int nextFruit2Xpos = 0;
		int nextFruit2Ypos = 0;
		int nextFruit1Xpos = 0;
		int nextFruit1Ypos = 0;
        if (direction == 1) {
			// rotate right
			nextFruit2Xpos = fruit[1].yPos;
			nextFruit2Ypos = fruit[1].xPos * -1;
		}
		else {
			// rotate left
			nextFruit2Xpos = fruit[1].yPos * -1;
			nextFruit2Ypos = fruit[1].xPos;
		}

		if (player.CellOccupied(fruit1X+nextFruit2Xpos,fruit1Y+nextFruit2Ypos)) {
			// If desired coordinates are occupied, check if fruit1 can rotate opposite direction

			if (direction == -1) {
				// rotate right
				nextFruit1Xpos = fruit[1].yPos;
				nextFruit1Ypos = fruit[1].xPos * -1;
			}
			else {
				// rotate left
				nextFruit1Xpos = fruit[1].yPos * -1;
				nextFruit1Ypos = fruit[1].xPos;
			}

			if (player.CellOccupied(fruit1X+nextFruit1Xpos,fruit1Y+nextFruit1Ypos)) {
				// If possible fruit1 space is occupied, flip fruit1 and fruit2
				nextFruit1Xpos = fruit[1].xPos;
				nextFruit1Ypos = fruit[1].yPos;
				nextFruit2Xpos = fruit[1].xPos * -1;
				nextFruit2Ypos = fruit[1].yPos * -1;
			}
		}
		
		this.transform.localPosition = 
			new Vector3(this.transform.localPosition.x + nextFruit1Xpos * PlayerController.CellSize, 
			            this.transform.localPosition.y + nextFruit1Ypos * PlayerController.CellSize, 
			            0);
		fruit[1].xPos = nextFruit2Xpos;
		fruit[1].yPos = nextFruit2Ypos;
		fruit[1].transform.localPosition = 
			new Vector3(fruit[1].xPos * 4f * PlayerController.CellSize,
			            fruit[1].yPos * 4f * PlayerController.CellSize,
			            0);
	}

	public void InstantDrop() {
		while (!grounded) {
			ApplyGravity();
		}
	}

	public void Translate(int direction) {
		Vector3 nextPos = this.transform.localPosition;
		nextPos.x += direction * PlayerController.CellSize;

		int fruit1X = Mathf.CeilToInt(nextPos.x / PlayerController.CellSize);
		int fruit1Y = -1 * Mathf.CeilToInt(nextPos.y / PlayerController.CellSize) + 2;
		int fruit2X = fruit1X + fruit[1].xPos;
		int fruit2Y = fruit1Y - fruit[1].yPos;

		if (!player.CellOccupied(fruit1X,fruit1Y) && !player.CellOccupied(fruit2X,fruit2Y)) {
			this.transform.localPosition = nextPos;
		}
	}

	public void ApplyGravity() {
		if (grounded) {
			return;
		}

		Vector3 nextPos = this.transform.localPosition;
		nextPos.y -= PlayerController.TickSize;

		int fruit1X = Mathf.CeilToInt(nextPos.x / PlayerController.CellSize);
		int fruit1Y = -1 * Mathf.CeilToInt(nextPos.y / PlayerController.CellSize) + 2;
		int fruit2X = fruit1X + fruit[1].xPos;
		int fruit2Y = fruit1Y - fruit[1].yPos;

		if (!player.CellOccupied(fruit1X,fruit1Y) && !player.CellOccupied(fruit2X,fruit2Y)) {
			this.transform.localPosition = nextPos;
		}
		else {
			grounded = true;
		}
	}

	#endregion

	#region Public Properties

	public FruitController this[int index] {
		get {
			return fruit[index];
		}
		set {
			fruit[index] = value;
		}
	}

	public bool Grounded {
		get {
			return grounded;
		}
	}

	#endregion
}
