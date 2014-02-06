using UnityEngine;
using System.Collections;

public class MainMenuPanel : MonoBehaviour {

    public Transform defaultOption;

	void Start() {
	}
	
	void Update() {
	}

    void OnEnable() {
        if (defaultOption != null) {
            UICamera.selectedObject = defaultOption.gameObject;
        }
    }

    private void ForwardAnimComplete() {
        MainMenuButton button = defaultOption.gameObject.GetComponent<MainMenuButton>();

        if ((button.nextPanel && button.nextPanel.gameObject.activeInHierarchy) || 
            (button.previousPanel && button.previousPanel.gameObject.activeInHierarchy)) {
            this.gameObject.SetActive(false);
        }
    }
}
