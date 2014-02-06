using UnityEngine;
using System.Collections;

public class MatchManager : MonoBehaviour {

    #region Public Variables

    public Transform fieldPrefab;

    #endregion

    #region Private Variables

    private int randomSeed;
    private PlayerController[] players = new PlayerController[2];

    #endregion

    void Start() {
        GameManager.Instance.CurrentMatch = this;

        randomSeed = (int)System.DateTime.Now.Ticks;

        for (int i=0; i<players.Length; i++) {
            Transform newPlayer = Instantiate(fieldPrefab) as Transform;
            players[i] = newPlayer.gameObject.GetComponent<PlayerController>();
        }

        players[0].slot = PlayerController.FieldPosition.P1;
        players[0].player = GameManager.Instance.PlayerOne;

        players[1].slot = PlayerController.FieldPosition.P2;
        players[1].player = GameManager.Instance.PlayerTwo;
    }

    void Update() {
    }

    #region Public Properties

    public int RandomSeed {
        get { return randomSeed; }
    }

    #endregion
}
