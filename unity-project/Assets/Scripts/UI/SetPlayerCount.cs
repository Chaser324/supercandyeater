using UnityEngine;
using System.Collections;

public class SetPlayerCount : MonoBehaviour {

    public PlayerController.FieldPlayer player1;
    public PlayerController.FieldPlayer player2;

    public GameManager.GameType gameType;

    void onClick() {
        GameManager.Instance.PlayerOne = player1;
        GameManager.Instance.PlayerTwo = player2;
        GameManager.Instance.SelectedGameType = gameType;
    }
}
