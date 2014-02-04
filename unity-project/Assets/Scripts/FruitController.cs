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
		iTween.Init(this.gameObject);
	}

	void Update () {
	
	}

	public void jelly() {
		Hashtable ht = new Hashtable();
		ht.Add("scale", this.transform);
		ht.Add("x", 3.5f);
		ht.Add("y", 4.5f);
		ht.Add("z", 4.5f);
		ht.Add("time", 5.5f);
		ht.Add("easetype", iTween.EaseType.easeOutElastic);
		ht.Add("looptype", iTween.LoopType.none);

		iTween.ScaleFrom(this.gameObject, ht);
	}
}
