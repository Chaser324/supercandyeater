using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

    public string nextScene;

    private Transform panel;
    private TweenAlpha tween;

    void Start() {
    }

    void Update() {
        if (tween != null && tween.value == 1f) {
            tween = null;
            TransitionScene();
        }
    }

    void OnEnable() {
        panel = transform.Find("Overlay");
        FadeOut();
    }

    public void FadeOut() {
        Camera mainCamera = Camera.main;
        AudioSource musicPlayer = mainCamera.GetComponent(typeof(AudioSource)) as AudioSource;
        TweenVolume volTween = mainCamera.gameObject.AddComponent<TweenVolume>() as TweenVolume;
        volTween.from = musicPlayer.volume;
        volTween.to = 0f;
        volTween.delay = 2.0f;
        volTween.duration = 2.5f;
        

        panel.gameObject.SetActive(true);
        
        tween = panel.GetComponent<TweenAlpha>() as TweenAlpha;
        tween.from = 0f;
        tween.to = 1f;
        tween.duration = 3.0f;
        tween.delay = 2.0f;

        tween.ResetToBeginning();
        tween.Play(true);
        volTween.Play(true);
    }

    public void TransitionScene() {
        Application.LoadLevel(nextScene);   
    }
}
