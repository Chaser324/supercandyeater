using UnityEngine;
using System.Collections;

public class WelcomeHandler : MonoBehaviour {
    public Transform bite1;
    public Transform bite2;
    public Transform bite3;
    public float bite1Delay;
    public float bite2Delay;
    public float bite3Delay;
    public AudioClip bite3Sound;
    public Color nextColor;
    public Transform nextPanel;
    private float elapsedTime;
    private AudioSource welcomeAudio;
    private int phase = 0;

    void Awake() {
        elapsedTime = 0;
        welcomeAudio = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        elapsedTime += Time.deltaTime;

        if (elapsedTime > bite3Delay + 1f && phase == 3) {
            nextPanel.gameObject.SetActive(true);
            Destroy(this.gameObject);
        }
        else if (elapsedTime > bite3Delay && phase == 2) {
            phase++;
            welcomeAudio.PlayOneShot(bite3Sound, 1f);
            bite3.gameObject.SetActive(false);
            bite2.gameObject.SetActive(false);
            bite1.gameObject.SetActive(false);
            Camera.main.backgroundColor = nextColor;
        }
        else if (elapsedTime > bite2Delay && phase == 1) {
            phase++;
            bite2.gameObject.SetActive(true);
        }
        else if (elapsedTime > bite1Delay && phase == 0) {
            phase++;
            bite1.gameObject.SetActive(true);
        }

    }
}
