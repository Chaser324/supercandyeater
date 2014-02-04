using UnityEngine;
using System.Collections;

public class FruitController : MonoBehaviour {

	public Sprite[] fruitSprites;

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

	void Start () {
		SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
		sr.sprite = fruitSprites[(int)color];
	}

	void Update () {
	
	}

	public void jelly(float intensity) {
		Hashtable ht = new Hashtable();
		ht.Add("x", 1.0f + intensity);
		ht.Add("y", 1.0f - intensity);
		ht.Add("time", 0.5f);
		ht.Add("easetype", iTween.EaseType.easeOutElastic);

		iTween.ScaleFrom(this.gameObject, ht);
	}
}
