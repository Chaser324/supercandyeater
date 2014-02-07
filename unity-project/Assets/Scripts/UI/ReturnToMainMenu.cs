using UnityEngine;
using System.Collections;

public class ReturnToMainMenu : MonoBehaviour {

	void OnClick() {
        Application.LoadLevel("mainmenu"); 
    }
}
