using UnityEngine;
using System.Collections;

public class MainMenuButton : MonoBehaviour {

    public Transform nextPanel = null;
    public Transform currentPanel = null;
    public Transform previousPanel = null;

    public Transform selectedLabel = null;
    public Transform deselectedLabel = null;

    public bool anyKeyConfirms = false;

    public AudioClip selectedSound = null;

    private bool selected = false;

    private AudioSource buttonAudio;

    void Start () {
        buttonAudio = gameObject.AddComponent<AudioSource>();
    }

    void Update () {
        if (anyKeyConfirms && Input.anyKeyDown) {
            GoForward();
        }
        else if (Input.GetButtonDown("MenuConfirm") && selected && nextPanel) {
            GoForward();
        }
        else if (Input.GetButtonDown("MenuBack") && selected && previousPanel) {
            GoBack();
        }
    }

    public void OnHover(bool isOver) {

    }

    public void OnSelect(bool state) {
        selected = state;

        if (selected) {
            ScalePanel(this.transform, 1.3f, 1.3f, 0.1f, 0f, iTween.EaseType.easeOutElastic, "");
            selectedLabel.gameObject.SetActive(true);
            deselectedLabel.gameObject.SetActive(false);
            buttonAudio.PlayOneShot(selectedSound, 1f);
        }
        else {
            this.transform.localScale = new Vector3(1f, 1f, 1f);
            selectedLabel.gameObject.SetActive(false);
            deselectedLabel.gameObject.SetActive(true);
        }
    }
    
    public void OnClick() {
        if (nextPanel && selected) {
            GoForward();
        }
    }

    private void GoForward() {
        nextPanel.localScale = new Vector3(0.1f, 0.1f, 1f);
        nextPanel.gameObject.SetActive(true);

        ScalePanel(currentPanel.transform, 0.1f, 0.1f, 0.1f, 0f, iTween.EaseType.linear, "ForwardAnimComplete");
        ScalePanel(nextPanel, 1.0f, 1.0f, 0.2f, 0.1f, iTween.EaseType.easeOutElastic, "");
    }

    private void GoBack() {
        previousPanel.localScale = new Vector3(0.1f, 0.1f, 1f);
        previousPanel.gameObject.SetActive(true);
        
        ScalePanel(currentPanel.transform, 0.1f, 0.1f, 0.1f, 0f, iTween.EaseType.linear, "ForwardAnimComplete");
        ScalePanel(previousPanel, 1.0f, 1.0f, 0.2f, 0.1f, iTween.EaseType.easeOutElastic, "");
    }

    private void ScalePanel(Transform target, float scaleX, float scaleY, float time, float delay, iTween.EaseType easing, string onComplete) {
        Hashtable ht = new Hashtable();
        ht.Add("x", scaleX);
        ht.Add("y", scaleY);
        ht.Add("time", time);
        ht.Add("delay", delay);
        ht.Add("easetype", easing);
        ht.Add("oncomplete", onComplete);
        
        iTween.ScaleTo(target.gameObject, ht);
    }

}
