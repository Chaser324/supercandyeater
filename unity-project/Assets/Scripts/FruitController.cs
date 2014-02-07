using UnityEngine;
using System.Collections;

public class FruitController : MonoBehaviour {

    public Sprite[] fruitSprites;
    public Sprite[] crashSprites;
    public Sprite[] blobOrangeSprites;
    public Sprite[] blobRedSprites;
    public Sprite[] blobGreenSprites;
    public Sprite[] blobYellowSprites;

    public enum FruitColor {
        Red = 0,
        Green,
        Yellow,
        Orange,
        MAX
    }

    public enum FruitType {
        Standard = 0,
        Crash,
        SuperCrash,
        Counter
    }

    public int xPos;
    public int yPos;
    public int height;
    public int width;
    public FruitType type;
    public FruitColor color;
    public bool falling = false;
    public bool crashing = false;

    private float rotateAmount;

    void Start() {
        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();

        sr.color = new Color(1f, 1f, 1f, 0f);

        if (type == FruitType.Standard) {
            sr.sprite = fruitSprites[(int)color];
        }
        else {
            sr.sprite = crashSprites[(int)color];
        }
    }

    void Update() {
    }

    public void Show(float alpha) {
        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();

        sr.color = new Color(1f, 1f, 1f, alpha);
    }

    public void Explode() {
        float randomOffset = Random.Range(5f,10f);
        randomOffset *= Random.Range(0f,1f) > 0.5f ? 1f : -1f;

        Hashtable ht = new Hashtable();
        ht.Add("x", this.transform.position.x + randomOffset);
        ht.Add("y", -50);
        ht.Add("speed", 0.5f + Random.Range(1f,1.5f));
        ht.Add("onupdate", "Rotate");

        rotateAmount = Random.Range(1f,2f);
        rotateAmount *= Random.Range(0f,1f) > 0.5f ? 1f : -1f;
        
        iTween.MoveTo(this.gameObject, ht);
    }

    public void Jelly(float intensity) {
        Hashtable ht = new Hashtable();
        ht.Add("x", 1.0f + intensity);
        ht.Add("y", 1.0f - intensity);
        ht.Add("time", 0.5f);
        ht.Add("easetype", iTween.EaseType.easeOutElastic);

        iTween.ScaleFrom(this.gameObject, ht);
    }

    private void Rotate() {
        this.transform.localEulerAngles = new Vector3(0,0,this.transform.localEulerAngles.z + rotateAmount);
    }
}
