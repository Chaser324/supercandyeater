using UnityEngine;
using System.Collections;

public class FruitPairController : MonoBehaviour {

	private FruitController[] fruit = new FruitController[2];
	private PlayerController player;

	private bool grounded = false;

	void Start () {
		player = this.transform.parent.parent.GetComponent<PlayerController>();

		this.transform.localPosition = new Vector3(3f * PlayerController.CellSize, 0, 0);

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

	void Update () {
	
	}

	public void Rotate(int direction) {
	}

	public void Translate(int direction) {
		Vector3 nextPos = this.transform.localPosition;
		nextPos.x += direction * PlayerController.CellSize;

		int fruit1X = Mathf.CeilToInt(nextPos.x / PlayerController.CellSize);
		int fruit1Y = -1 * Mathf.CeilToInt(nextPos.y / PlayerController.CellSize) + 1;
		int fruit2X = fruit1X + fruit[1].xPos;
		int fruit2Y = fruit1Y - fruit[1].yPos + 1;

		bool occupied = player.CellOccupied(fruit1X,fruit1Y) || player.CellOccupied(fruit2X,fruit2Y);

		if (!occupied) {
			this.transform.localPosition = nextPos;
		}
	}

	public void ApplyGravity() {
		Vector3 nextPos = this.transform.localPosition;
		nextPos.y -= PlayerController.TickSize;

		int fruit1X = Mathf.CeilToInt(nextPos.x / PlayerController.CellSize);
		int fruit1Y = -1 * Mathf.CeilToInt(nextPos.y / PlayerController.CellSize) + 1;
		int fruit2X = fruit1X + fruit[1].xPos;
		int fruit2Y = fruit1Y - fruit[1].yPos + 1;
		
		bool occupied = player.CellOccupied(fruit1X,fruit1Y) || player.CellOccupied(fruit2X,fruit2Y);

		if (!occupied) {
			this.transform.localPosition = nextPos;
		}
		else {
			grounded = true;
		}
	}

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
