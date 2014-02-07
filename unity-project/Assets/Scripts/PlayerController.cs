using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    #region Constants

    public enum FieldPosition {
        P1 = 1,
        P2 
    };

    public enum FieldPlayer {
        Human,
        AI
    };

    public const float CellSize = 0.15f;
    public const int FieldCellWidth = 6;
    public const int FieldCellHeight = 12;
    public const float FieldWidth = FieldCellWidth * CellSize;
    public const float FieldHeight = FieldCellHeight * CellSize;

    public const float TickSize = 0.03f;
    public const float TickTime = 0.4f;

    public const float MaxCrashChance = 0.2f;
    public const float CrashTickSize = 0.05f;

    #endregion

    #region Public Variables

    public FieldPosition slot;
    public FieldPlayer player;

    public Transform fruitPrefab;
    public Transform fruitControllerPrefab;
    public Transform fruitContainer;
    public Transform loseSpew;
    public Transform winSpew;


    public float speed = 1.0f;

    public AudioClip turnSound = null;
    public AudioClip slideSound = null;
    public AudioClip instantDropSound = null;
    public AudioClip gulp1 = null;
    public AudioClip gulp2 = null;

    public bool nearDefeat;

    #endregion

    #region Private Variables

    private Vector2 lastScreenSize;

    private System.Random rng;

    private FruitController[,] playfield = new FruitController[FieldCellWidth, FieldCellHeight];
    private FruitPairController currentPair = null;
    private FruitPairController nextPair = null;

    private bool tumbling = false;
    private int tumblePhase = 0;

    private float elapsedTime;

    private float lastHorizontalInput = 0f;
    private float lastVerticalInput = 0f;
    private float holdingTime = 0f;

    private float crashChance = 0.0f;

    public bool lost = false;
    public bool won = false;

    private AudioSource playerAudio = null;

    #endregion
    
    #region Event Handlers

    void Start() {
        playerAudio = this.gameObject.AddComponent<AudioSource>();

        rng = new System.Random(GameManager.Instance.CurrentMatch.RandomSeed);

        for (int i=0; i<FieldCellWidth; i++) {
            for (int j=0; j<FieldCellHeight; j++) {
                playfield[i, j] = null;
            }
        }
    }

    void Update() {
        UpdatePosition();

        if (GameManager.Instance.Paused) { return; }

        elapsedTime += Time.deltaTime * speed;

        // Handle player input
        HandleInput();
        
        if (elapsedTime >= TickTime && !lost && !won) {
            elapsedTime -= TickTime;

            CheckForNearDefeat();

            if (!tumbling) {
                if (currentPair == null) {
                    currentPair = nextPair;

                    if (currentPair != null) {
                        currentPair.MakeCurrent();
                    }

                    GenerateNewPair();
                }
                else {
                    // Apply gravity
                    currentPair.ApplyGravity();

                    if (currentPair.transform.localPosition.y <= 0) {
                        nextPair.MakeVisible();
                    }

                    if (currentPair.Grounded) {
                        GroundPair();
                    }
                }
            }
            else {
                Tumble();
            }
        }
        else if (won && !winSpew.gameObject.activeSelf) {
            Win();
        }

    }

    #endregion

    #region Public Methods

    public bool CellOccupied(int x, int y) {
        if (y < 0 && x >= 0 && x < FieldCellWidth) {
            return false;
        }
        if (x >= 0 && y >= 0 && x < FieldCellWidth && y < FieldCellHeight && playfield[x, y] == null) {
            return false;
        }
        else {
            return true;
        }
    }

    #endregion

    #region Private Methods

    private void UpdatePosition() {
        Vector2 screenSize = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);

        if (this.lastScreenSize != screenSize) {
            this.lastScreenSize = screenSize;

            float screenHeight = Camera.main.pixelHeight;
            float screenWidth = Camera.main.pixelWidth;
            float screenAspect = (16f/9f) / Camera.main.aspect;
            
            if (slot == FieldPosition.P1) {
                transform.position = 
                    Camera.main.ScreenToWorldPoint(new Vector3(25 * screenWidth/1280 * screenAspect, 
                                                               screenHeight - 35 * screenHeight/720 * screenAspect, 
                                                               10));
            }
            else {
                transform.position = 
                    Camera.main.ScreenToWorldPoint(new Vector3(screenWidth - 315 * screenWidth/1280 * screenAspect, 
                                                               screenHeight - 35 * screenHeight/720 * screenAspect, 
                                                               10));
            }
        }
    }

    private void Tumble() {
        if (tumblePhase == 0) {
            speed = 100.0f;
            if (!ApplyGravity()) {
                ++tumblePhase;
            }
        }
        else if (tumblePhase == 1) {
            // Join pieces
            ++tumblePhase;
        }
        else if (tumblePhase == 2) {
            // Remove fruit
            int crashed = RemoveFruit();
            if (crashed > 0) {
                Gulp(1f);
                RemoveFruit();
                tumblePhase = 0;
            }
            else if (crashed == 0) {
                ++tumblePhase;
            }
        }
        else {
            speed = 1.0f;
            elapsedTime = 0;
            tumbling = false;
            tumblePhase = 0;
        }
    }
    
    private void Lose() {
        lost = true;

        nearDefeat = false;
        iTween.Stop(this.gameObject);
        UpdatePosition();

        for (int col=0; col<FieldCellWidth; col++) {
            for (int row=FieldCellHeight-1; row>=0; row--) {
                FruitController fruit = playfield[col, row];
                if (fruit != null) {
                    fruit.Explode();
                }
            }
        }

        currentPair[0].Explode();
        currentPair[1].Explode();

        loseSpew.gameObject.SetActive(true);
    }

    private void Win() {
        won = true;
        
        nearDefeat = false;
        iTween.Stop(this.gameObject);
        UpdatePosition();
        
        for (int col=0; col<FieldCellWidth; col++) {
            for (int row=FieldCellHeight-1; row>=0; row--) {
                FruitController fruit = playfield[col, row];
                if (fruit != null) {
                    fruit.Explode();
                }
            }
        }
        
        currentPair[0].Explode();
        currentPair[1].Explode();
        
        winSpew.gameObject.SetActive(true);
    }
    
    private void HandleInput() {
        if (currentPair != null) {
            float horizontal = Input.GetAxis("Horizontal-P" + (int)slot);
            float vertical = Input.GetAxis("Vertical-P" + (int)slot);
            bool instantDrop = Input.GetButtonDown("InstantDrop-P" + (int)slot);
            bool rotateRight = Input.GetButtonDown("RotateRight-P" + (int)slot);
            bool rotateLeft = Input.GetButtonDown("RotateLeft-P" + (int)slot);

            if (horizontal != lastHorizontalInput || vertical != lastVerticalInput) {
                holdingTime = 0f;
            }
            else if (horizontal != 0 || vertical != 0) {
                holdingTime += Time.deltaTime;
            }

            lastHorizontalInput = horizontal;
            lastVerticalInput = vertical;

            if (vertical == 0f) {
                speed = 1.0f;
            }

            if (instantDrop) {
                currentPair.InstantDrop();
                playerAudio.PlayOneShot(instantDropSound);
                if (elapsedTime < TickTime) {
                    elapsedTime = TickTime;
                }
            }
            else if (rotateRight) {
                currentPair.Rotate(1);
                playerAudio.pitch = 1f;
                playerAudio.PlayOneShot(turnSound);
            }
            else if (rotateLeft) {
                currentPair.Rotate(-1);
                playerAudio.pitch = 1f;
                playerAudio.PlayOneShot(turnSound);
            }
            else if (horizontal == 1.0f && holdingTime == 0f) {
                currentPair.Translate(1);
                playerAudio.pitch = 1f;
                playerAudio.PlayOneShot(slideSound);
            }
            else if (horizontal == -1.0f && holdingTime == 0f) {
                currentPair.Translate(-1);
                playerAudio.pitch = 1f;
                playerAudio.PlayOneShot(slideSound);
            }
            else if (vertical == -1.0f) {
                speed = 10.0f;
            }

        }
    }

    private void Gulp(float pitch) {
        playerAudio.pitch *= pitch;
        if (Random.Range(0f,1f) > 0.5f) {
            playerAudio.PlayOneShot(gulp1);
        }
        else {
            playerAudio.PlayOneShot(gulp2);
        }
    }

    private bool ApplyGravity() {
        bool falling = false;

        for (int col=0; col<FieldCellWidth; col++) {
            for (int row=FieldCellHeight-1; row>=0; row--) {
                FruitController fruit = playfield[col, row];
                if (fruit != null) {
                    if (fruit.falling) {
                        Vector3 nextPos = fruit.transform.localPosition;
                        nextPos.y -= PlayerController.TickSize;

                        int newY = -1 * Mathf.CeilToInt(nextPos.y / PlayerController.CellSize) + 1;

                        if (newY == fruit.yPos) {
                            if (!CellOccupied(col, row + 1)) {
                                fruit.falling = true;
                                fruit.yPos += 1;
                                playfield[col, row] = null;
                                playfield[col, row + 1] = fruit;
                                fruit.transform.localPosition = nextPos;
                            }
                            else {
                                fruit.falling = false;
                            }
                        }
                        else {
                            fruit.transform.localPosition = nextPos;
                        }
                    }
                    else if (!CellOccupied(col, row + 1)) {
                        fruit.falling = true;
                        fruit.yPos += 1;
                        playfield[col, row] = null;
                        playfield[col, row + 1] = fruit;
                    }

                    falling |= fruit.falling;
                }
            }
        }

        return falling;
    }

    private int RemoveFruit() {
        int crashCount = 0;

        for (int col=0; col<FieldCellWidth; col++) {
            for (int row=FieldCellHeight-1; row>=0; row--) {
                FruitController fruit = playfield[col, row];
                if (fruit != null) {
                    if (!fruit.crashing && fruit.type == FruitController.FruitType.Crash) {
                        crashCount += CrashAdjacent(col, row, fruit);
                    }
                    else if (fruit.crashing) {
                        playfield[col, row] = null;
                        Destroy(fruit.gameObject);
                    }
                }
            }
        }

        return crashCount;
    }

    private int CrashAdjacent(int x, int y, FruitController fruit) {
        int crashCount = 0;

        crashCount += CrashCheck(x + 1, y, fruit.color);
        crashCount += CrashCheck(x - 1, y, fruit.color);
        crashCount += CrashCheck(x, y + 1, fruit.color);
        crashCount += CrashCheck(x, y - 1, fruit.color);

        return crashCount;
    }

    private int CrashCheck(int x, int y, FruitController.FruitColor color) {
        int crashCount = 0;

        if (x >= 0 && y >= 0 && x < FieldCellWidth && y < FieldCellHeight && playfield[x, y] != null) {
            FruitController fruit = playfield[x, y];
            if (fruit.crashing) {
            }
            else if (fruit.color == color) {
                fruit.crashing = true;
                crashCount = 1 + CrashAdjacent(x, y, fruit);
            }
        }

        return crashCount;
    }

    private void GenerateNewPair() {
        Transform newPair = Instantiate(fruitControllerPrefab) as Transform;
        newPair.parent = fruitContainer.transform;
        nextPair = newPair.gameObject.GetComponent<FruitPairController>();
        
        crashChance = Mathf.MoveTowards(crashChance, MaxCrashChance, CrashTickSize);
        
        for (int i=0; i<2; i++) {
            Transform newFruit = Instantiate(fruitPrefab) as Transform;
            newFruit.parent = nextPair.transform;
            
            FruitController newFruitController = newFruit.gameObject.GetComponent<FruitController>();
            newFruitController.color = (FruitController.FruitColor)(rng.Next((int)FruitController.FruitColor.MAX));
            
            if (rng.NextDouble() < crashChance) {
                newFruitController.type = FruitController.FruitType.Crash;
            }
            else {
                newFruitController.type = FruitController.FruitType.Standard;
            }
            
            nextPair[i] = newFruitController;
        }
        
        if (nextPair[0].type == FruitController.FruitType.Crash || 
            nextPair[1].type == FruitController.FruitType.Crash) {
            crashChance = 0.0f;
        }
    }

    private void GroundPair() {
        currentPair[0].transform.parent = fruitContainer;
        currentPair[1].transform.parent = fruitContainer;
        
        int pairX = Mathf.CeilToInt(currentPair.transform.localPosition.x / CellSize);
        int pairY = Mathf.CeilToInt(-1f * currentPair.transform.localPosition.y / CellSize) + 1;
        
        currentPair[0].xPos += pairX;
        currentPair[0].yPos *= -1;
        currentPair[0].yPos += pairY;
        currentPair[1].xPos += pairX;
        currentPair[1].yPos *= -1;
        currentPair[1].yPos += pairY;

        if (currentPair[0].yPos < 1 || currentPair[1].yPos < 1) {
            Lose();
        }
        else {
            playfield[currentPair[0].xPos, currentPair[0].yPos] = currentPair[0];
            playfield[currentPair[1].xPos, currentPair[1].yPos] = currentPair[1];
        }
        
        float jellyIntensity = Input.GetButtonDown("InstantDrop-P" + (int)slot) ? 0.5f : 0.1f;
        currentPair[0].Jelly(jellyIntensity);
        currentPair[1].Jelly(jellyIntensity);
        
        tumbling = true;
        Destroy(currentPair.gameObject);
        currentPair = null;
    }

    private void CheckForNearDefeat() {
        int highestFilledRow = FieldCellHeight;

        for (int row=0; row<FieldCellHeight; row++) {
            bool rowFilled = true;

            for (int col=0; col<FieldCellWidth; col++) {
                if (playfield[col,row] == null) {
                    rowFilled = false;
                    break;
                }
            }

            if (rowFilled) {
                highestFilledRow = row;
                break;
            }
        }

        if (highestFilledRow < 5 && !nearDefeat) {
            nearDefeat = true;

            Hashtable ht = new Hashtable();
            ht.Add("name", "NearDefeat-P" + (int)slot);
            ht.Add("x", 0.05f);
            ht.Add("y", 0.05f);
            ht.Add("time", 0.2f);
            ht.Add("looptype", iTween.LoopType.loop);
            
            iTween.ShakePosition(this.gameObject, ht);
        }
        else if (highestFilledRow >= 5 && nearDefeat) {
            nearDefeat = false;

            iTween.StopByName("NearDefeat-P" + (int)slot);
            UpdatePosition();
        }
    }
    
    #endregion
    
    #region Public Properties

    #endregion
}
