using UnityEngine;
using System.Collections;

public class FruitController : MonoBehaviour {

	public Sprite[] fruitSprites;
	public Sprite[] crashSprites;

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

	public void Show() {
		SpriteRenderer sr = this.GetComponent<SpriteRenderer>();

		sr.color = new Color(1f, 1f, 1f, 1f);
	}

	public void Jelly(float intensity) {
		Hashtable ht = new Hashtable();
		ht.Add("x", 1.0f + intensity);
		ht.Add("y", 1.0f - intensity);
		ht.Add("time", 0.5f);
		ht.Add("easetype", iTween.EaseType.easeOutElastic);

		iTween.ScaleFrom(this.gameObject, ht);
	}
}
