using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class NetworkManagerBreakout : NetworkManager {
    #region Var spawn and launch
    public Transform Player01Spawn;
    public Transform Player02Spawn;

    [SerializeField] GameObject ballPrefab;
    Ball ball;

    public BrickManager brickManager;
    #endregion
    #region Spawn and launch
    public override void OnServerAddPlayer(NetworkConnection conn) {
        Transform start = numPlayers == 0 ? Player01Spawn : Player02Spawn;
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        player.gameObject.name = numPlayers.ToString();
        NetworkServer.AddPlayerForConnection(conn, player); 
        if(numPlayers == 2 && bricksUp) {
            GameOver();
        }
        else
            brickManager.MenuOn();

    }
    public void LaunchBalls(string nameCheck) {
        var paddles = FindObjectsOfType<Paddle>();
        foreach (Paddle p in paddles) {
            if (p.ball == null) {
                ball = Instantiate((ballPrefab).GetComponent<Ball>(),
                    new Vector2(p.transform.position.x, p.transform.position.y + 0.6f), Quaternion.identity, null);
                NetworkServer.Spawn
                    (ball.gameObject);
                ball.LaunchBall();
                p.ball = ball;
            }
        }
    } // for each paddle without a ball 
    #endregion

    #region vars
    [HideInInspector] public bool bricksUp;

    public Brick brick;
    [HideInInspector] public List<Brick> activeBricks;
    public Color breakColor;
    public List<ColorSet> colSets;
    [HideInInspector]public ColorSet chosenColSet;
    public SpriteRenderer backGround;

    public GameObject menu;
    public GameObject waitForHost;
    public Text scoreUI;
    public Text levelUI;
    public Text livesUI;
    public GameObject levelCompleteUI;
    public GameObject gameOverUI;


    public GameObject hostMustLaunch;    
    #endregion
    void Start() {
        bricksUp = false;
        brickManager.score = 0;
        brickManager.level = 1;
        brickManager.lifes = 3;

        brickManager.rows = 5;
        brickManager.columns = 5;
        brickManager.ballSpeed = 4;

        levelCompleteUI.SetActive(false);
        gameOverUI.SetActive(false);
    }

    public void StartGame() {
        DestroyExistingGrid();
        StartCoroutine(CreateBrickGrid());
        StartCoroutine(fadeinBG());
        brickManager.MenuOff();
        brickManager.lifes = 3;
        livesUI.text = "LIVES: " + brickManager.lifes.ToString();
    }

    IEnumerator CreateBrickGrid() {
        // get level colorset
        int colSetNum = Random.Range(0, colSets.Count);
        chosenColSet = colSets[colSetNum];
        backGround.sprite = chosenColSet.background;
        brickManager.UpdateBG(colSetNum);
        bricksUp = false;

        if (brickManager.rows < 1)
            brickManager.rows = 1;
        if (brickManager.columns < 1)
            brickManager.columns = 1;
        //instantiate symetrical bricks
        for (int r = 0; r < brickManager.rows; r++) {
            for (int c = 0; c < brickManager.columns; c++) {
                Color color;
                yield return new WaitForEndOfFrame();
                if (brickManager.missingBrickChance < Random.Range(1, 100)) {
                    //Right Brick
                    color = chosenColSet.cols[Random.Range(0, chosenColSet.cols.Count)];
                    Brick brickR = Instantiate(brick, new Vector3(transform.position.x + 0.75f + c * 1.5f,
                        transform.position.y + r * 0.5f, 0), Quaternion.identity, transform);

                    NetworkServer.Spawn(brickR.gameObject);

                    brickR.rend.color = color;
                    activeBricks.Add(brickR);
                    brickR.brickColor = color;

                    //Left Brick
                    Brick brickL = Instantiate(brick, new Vector3(transform.position.x - 0.75f - c * 1.5f,
                        transform.position.y + r * 0.5f, 0), Quaternion.identity, transform);

                    NetworkServer.Spawn(brickL.gameObject);

                    brickL.rend.color = color;
                    activeBricks.Add(brickL);
                    brickL.brickColor = color;
                }
            }
        }
        //instantiate one brick if none are present
        if (activeBricks.Count == 0) {
            Color color;
            color = chosenColSet.cols[Random.Range(0, chosenColSet.cols.Count)];
            Brick brickC = Instantiate(brick, new Vector3(transform.position.x + 0.75f + Random.Range(0, brickManager.columns) * 1.5f,
                transform.position.y + Random.Range(0, brickManager.rows) * 0.5f, 0), Quaternion.identity, transform);

            NetworkServer.Spawn(brickC.gameObject);

            brickC.rend.color = color;
            activeBricks.Add(brickC);
        }

        bricksUp = true;
    }
    public void DestroyExistingGrid() {
        foreach (Brick b in activeBricks)
            Destroy(b.gameObject);
        activeBricks.Clear();
    }

    public IEnumerator fadeoutBG() {
        float elapsedTime = 0;
        float waitTime = 3;
        while (elapsedTime < waitTime) {
            backGround.color = Color.Lerp(backGround.color, Color.black, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    public IEnumerator fadeinBG() {
        float elapsedTime = 0;
        float waitTime = 3;
        while (elapsedTime < waitTime) {
            backGround.color = Color.Lerp(backGround.color, Color.white, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void GameOver() {
        livesUI.text = "LIVES: " + brickManager.lifes.ToString();
        bricksUp = false;
        brickManager.score = 0;
        brickManager.level = 1;
        brickManager.lifes = 0;
        scoreUI.text = "SCORE: " + brickManager.score;
        levelUI.text = "LEVEL: " + brickManager.level.ToString();        
        brickManager.GameOver();
        StartCoroutine(fadeoutBG());
        DestroyExistingGrid();
    }

    public void CheckLevel() {
        scoreUI.text = "SCORE: " + brickManager.score;
        levelUI.text = "LEVEL: " + brickManager.level;
        livesUI.text = "LIVES: " + brickManager.lifes.ToString();
        if (activeBricks.Count == 0)
            NextLevel();
    }
    public void NextLevel() {
        bricksUp = false;
        brickManager.level++;
        levelUI.text = "LEVEL: " + brickManager.level.ToString();
        livesUI.text = "LIVES: " + brickManager.lifes.ToString();
        brickManager.CompleteLevel();
        StartCoroutine(fadeoutBG());
        DestroyExistingGrid();
    }

}
