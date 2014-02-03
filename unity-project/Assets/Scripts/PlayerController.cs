﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	#region Constants

	public enum FieldPosition { P1, P2 };
	public enum FieldPlayer { Human, AI };

	public const float CellSize = 0.15f;
	public const int FieldCellWidth = 6;
	public const int FieldCellHeight = 12;
	public const float FieldWidth = FieldCellWidth * CellSize;
	public const float FieldHeight = FieldCellHeight * CellSize;

	public const float TickSize = 0.03f;
	public const float TickTime = 0.4f;

	#endregion

	#region Public Variables

	public FieldPosition slot;
	public FieldPlayer player;

	public Transform fruitPrefab;
	public Transform fruitControllerPrefab;
	public Transform fruitContainer;

	public float speed = 1.0f;

	#endregion

	#region Private Variables

	private Vector2 lastScreenSize;

	private System.Random rng;

	private FruitController[,] playfield = new FruitController[FieldCellWidth,FieldCellHeight];
	private FruitPairController currentPair = null;

	private bool tumbling = false;

	private float elapsedTime;

	private float lastHorizontalInput = 0f;
	private float lastVerticalInput = 0f;
	private float holdingTime = 0f;

	#endregion
	
	#region Event Handlers

	void Start () {
		rng = new System.Random(GameManager.Instance.CurrentMatch.RandomSeed);

		for (int i=0; i<FieldCellWidth; i++) {
			for (int j=0; j<FieldCellHeight; j++) {
				playfield[i,j] = null;
			}
		}
	}

	void Update () {
		UpdatePosition();

		elapsedTime += Time.deltaTime * speed;

		// Handle player input
		HandleInput();
        
        if (elapsedTime > TickTime) {
			elapsedTime -= TickTime;

			// Spawn new piece
			if (!tumbling) {
				if (currentPair == null) {
					Transform newPair = Instantiate(fruitControllerPrefab) as Transform;
					newPair.parent = fruitContainer.transform;
					currentPair = newPair.gameObject.GetComponent<FruitPairController>();

					for (int i=0; i<2; i++) {
						Transform newFruit = Instantiate(fruitPrefab) as Transform;
						newFruit.parent = currentPair.transform;
						currentPair[i] = newFruit.gameObject.GetComponent<FruitController>();
					}
				}
				else {
		            // Apply gravity
					currentPair.ApplyGravity();

					if (currentPair.Grounded) {
						currentPair[0].transform.parent = fruitContainer;
						currentPair[1].transform.parent = fruitContainer;

						int pairX = Mathf.CeilToInt(currentPair.transform.localPosition.x / CellSize);
						int pairY = Mathf.CeilToInt(-1f * currentPair.transform.localPosition.y / CellSize) + 1;

						playfield[pairX,pairY] = currentPair[0];
						playfield[pairX + currentPair[1].xPos, pairY - currentPair[1].yPos] = currentPair[1];

						tumbling = true;
						Destroy(currentPair.gameObject);
						currentPair = null;
					}
				}
			}
			else {
	            // Apply gravity

				// Join pieces

				// Remove pieces

				tumbling = false;
			}
		}

	}

	#endregion

	#region Public Methods

	public bool CellOccupied(int x, int y) {
		if (x >= 0 && y >= 0 && x < FieldCellWidth && y < FieldCellHeight && playfield[x,y] == null) {
			return false;
		}
		else {
			return true;
		}
	}

	#endregion

	#region Private Methods

	private void UpdatePosition() {
		Vector2 screenSize = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);

		if (this.lastScreenSize != screenSize) {
			this.lastScreenSize = screenSize;

			float screenHeight = Camera.main.pixelHeight;
			float screenWidth = Camera.main.pixelWidth;
			
			if (slot == FieldPosition.P1) {
				transform.position = Camera.main.ScreenToWorldPoint(new Vector3(25,screenHeight-25,10));
			}
			else {
				transform.position = Camera.main.ScreenToWorldPoint(new Vector3(screenWidth-(98*3)-20,screenHeight-25,10));
			}
		}
	}

	private void HandleInput() {
		if (currentPair != null) {
			float horizontal = Input.GetAxis("Horizontal");
			float vertical = Input.GetAxis("Vertical");

			if (horizontal != lastHorizontalInput || vertical != lastVerticalInput) {
				holdingTime = 0f;
			}
			else if (horizontal != 0 || vertical != 0) {
				holdingTime += Time.deltaTime;
			}

			lastHorizontalInput = horizontal;
			lastVerticalInput = vertical;

			if (vertical == 0f) {
				speed = 1.0f;
			}

			if (horizontal == 1.0f && holdingTime == 0f) {
				currentPair.Translate(1);
			}
			else if (horizontal == -1.0f && holdingTime == 0f) {
				currentPair.Translate(-1);
			}
			else if (vertical == -1.0f) {
				speed = 10.0f;
			}

		}
	}

	private void ApplyGravity() {

	}

	#endregion

	#region Public Properties


	#endregion
}