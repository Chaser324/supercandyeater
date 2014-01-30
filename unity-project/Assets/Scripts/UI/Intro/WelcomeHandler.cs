using UnityEngine;
using System.Collections;

public class WelcomeHandler : MonoBehaviour {
    public Transform bite1;
    public Transform bite2;
    public Transform bite3;

    public float bite1Delay;
    public float bite2Delay;
    public float bite3Delay;

    public Color nextColor;

    public Transform nextPanel;

    private float elapsedTime;

    void Awake() {
        elapsedTime = 0;
    }

    // Update is called once per frame
    void Update() {
        elapsedTime += Time.deltaTime;

        if (elapsedTime > bite3Delay) {
            bite3.gameObject.SetActive(false);
            Camera.main.backgroundColor = nextColor;
            nextPanel.gameObject.SetActive(true);
            Destroy(this.gameObject);
        }
        else if (elapsedTime > bite2Delay) {
            bite2.gameObject.SetActive(true);
        }
        else if (elapsedTime > bite1Delay) {
            bite1.gameObject.SetActive(true);
        }

    }
}
