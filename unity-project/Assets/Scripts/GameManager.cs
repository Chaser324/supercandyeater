using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private PlayerController.FieldPlayer playerOne = PlayerController.FieldPlayer.Human;
	private PlayerController.FieldPlayer playerTwo = PlayerController.FieldPlayer.Human;

	private MatchManager currentMatch;

	private static GameManager instance;

	#region Event Handlers

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}

	void Start () {
	}

	void OnApplicationQuit() {
		instance = null;
	}

	void Update () {
	}

	#endregion

	#region Public Properties

	public static GameManager Instance {
		get {
			if (!instance) {
				instance = new GameObject("GameManager").AddComponent<GameManager>();
			}

			return instance;
		}
	}

	public PlayerController.FieldPlayer PlayerOne {
		get { return playerOne; }
		set { playerOne = value; }
	}

	public PlayerController.FieldPlayer PlayerTwo {
		get { return playerTwo; }
		set { playerTwo = value; }
    }

	public MatchManager CurrentMatch {
		get { return currentMatch; }
		set { currentMatch = value; }
	}

	#endregion
}
