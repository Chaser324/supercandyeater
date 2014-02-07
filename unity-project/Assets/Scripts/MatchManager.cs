using UnityEngine;
using System.Collections;

public class MatchManager : MonoBehaviour {

    #region Public Variables

    public Transform fieldPrefab;

    public Transform goPanel;
    public Transform optionsPanel;

    public Transform p1WonPanel;
    public Transform p1LosePanel;
    public Transform p2WonPanel;
    public Transform p2LosePanel;

    public AudioClip musicIntro;
    public AudioClip musicLoop;
    public AudioClip tenseMusicIntro;
    public AudioClip tenseMusicLoop;

    #endregion

    #region Private Variables

    private int randomSeed;
    private PlayerController[] players = new PlayerController[2];

    private int phase = 0;
    private bool tense = false;

    private AudioSource matchAudio = null;
    private AudioSource musicAudio = null;
    private AudioSource musicIntroAudio = null;

    #endregion

    #region Event Handlers

    void Start() {
        GameManager.Instance.CurrentMatch = this;

        matchAudio = this.gameObject.AddComponent<AudioSource>();
        musicAudio = this.gameObject.AddComponent<AudioSource>();
        musicIntroAudio = this.gameObject.AddComponent<AudioSource>();

        musicAudio.loop = true;

        randomSeed = (int)System.DateTime.Now.Ticks;
    }

    void Update() {
        if (phase == 0) {
            StartCoroutine(ShowGoPanel());
            ++phase;
        }
        else if (phase == 1) {
            // intentionally blank - waiting for coroutine
        }
        else if (phase == 2) {
            SpawnPlayers();
            ++phase;
        }
        else if (phase == 3) {
            if (Input.GetButtonDown("Pause")) {
                GameManager.Instance.Paused = !GameManager.Instance.Paused;
                optionsPanel.gameObject.SetActive(GameManager.Instance.Paused);
            }

            if (!GameManager.Instance.Paused) {
                SetIntensity(players[0].nearDefeat || players[1].nearDefeat);

                if (players[0].lost) {
                    players[1].won = true;

                    p1LosePanel.gameObject.SetActive(true);
                    p2WonPanel.gameObject.SetActive(true);

                    ++phase;

                }
                else if (players[1].lost) {
                    players[0].won = true;

                    p2LosePanel.gameObject.SetActive(true);
                    p1WonPanel.gameObject.SetActive(true);

                    ++phase;
                }
            }
            else {
                if (!optionsPanel.gameObject.activeInHierarchy) {
                    GameManager.Instance.Paused = false;
                }

                musicAudio.volume = GameManager.Instance.musicVol;
                musicIntroAudio.volume = GameManager.Instance.musicVol;
            }
        }
        else if (phase == 4) {
            StartCoroutine(ReturnToMainMenu());
            ++phase;
        }
    }

    #endregion

    #region Public Methods



    #endregion

    #region Private Methods

    private void SetIntensity(bool tenseMatch) {
        if (tenseMatch != tense) {
            tense = tenseMatch;
            if (tense) {
                PlayTenseMusic();
            }
            else {
                PlayMatchMusic();
            }
        }
    }

    private void PlayMatchMusic() {
        musicAudio.Stop();
        musicIntroAudio.Stop();

        musicAudio.clip = musicLoop;
        musicIntroAudio.clip = musicIntro;

        musicAudio.PlayDelayed(musicIntro.length);
        musicIntroAudio.Play();
    }

    private void PlayTenseMusic() {
        musicAudio.Stop();
        musicIntroAudio.Stop();
        
        musicAudio.clip = tenseMusicLoop;
        musicIntroAudio.clip = tenseMusicIntro;
        
        musicAudio.PlayDelayed(tenseMusicIntro.length);
        musicIntroAudio.Play();
    }

    private void SpawnPlayers() {
        for (int i=0; i<players.Length; i++) {
            Transform newPlayer = Instantiate(fieldPrefab) as Transform;
            players[i] = newPlayer.gameObject.GetComponent<PlayerController>();
        }
        
        players[0].slot = PlayerController.FieldPosition.P1;
        players[0].player = GameManager.Instance.PlayerOne;
        
        players[1].slot = PlayerController.FieldPosition.P2;
        players[1].player = GameManager.Instance.PlayerTwo;
    }

    private IEnumerator ShowGoPanel() {
        goPanel.localScale = new Vector3(4f,4f,1f);
        goPanel.gameObject.SetActive(true);
        
        Hashtable ht = new Hashtable();
        ht.Add("x", 1f);
        ht.Add("y", 1f);
        ht.Add("time", 1.0f);
        ht.Add("delay", .2f);
        ht.Add("easetype", iTween.EaseType.easeOutBounce);
        
        iTween.ScaleTo(goPanel.gameObject, ht);

        yield return new WaitForSeconds(0.25f);

        PlayMatchMusic();

        yield return new WaitForSeconds(2.6f);
        
        goPanel.gameObject.SetActive(false);
        ++phase;
    }

    private IEnumerator ReturnToMainMenu() {
        yield return new WaitForSeconds(5f);

        while (!Input.anyKeyDown)
            yield return 0;

        Application.LoadLevel("mainmenu");
    }

    #endregion

    #region Public Properties

    public int RandomSeed {
        get { return randomSeed; }
    }

    #endregion
}
