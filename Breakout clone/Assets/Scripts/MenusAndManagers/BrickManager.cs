using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class BrickManager : NetworkBehaviour {
    #region Vars
    [SyncVar]
    public int score;
    [SyncVar]
    public int level;
    [SyncVar]
    public int lifes;
    public float ballSpeed { get; set; }

    //Bricks
    public float columns { get; set; } //Use half of desired number
    public float rows { get; set; }
    public float missingBrickChance { get; set; }

    //UI
    public GameObject menu;
    public GameObject waitForHost;
    public Text scoreUI;
    public Text levelUI;
    public Text livesUI;
    public GameObject levelCompleteUI;
    public GameObject gameOverUI;

    public SpriteRenderer background;
    public List<Sprite> bgs;
    #endregion

    #region menu
    // calls to client to turn menus on and off
    [ClientRpc]
    public void MenuOn() {
        var paddles = FindObjectsOfType<Paddle>();
        foreach (Paddle p in paddles) {
            p.inMenu = true;
        }
        if (isServer) {
            menu.SetActive(true);
            waitForHost.SetActive(false);
        }
        if (!isServer) {
            menu.SetActive(false);
            waitForHost.SetActive(true);
        }
        scoreUI.enabled = false;
        levelUI.enabled = false;
        livesUI.enabled = false;
        levelCompleteUI.SetActive(false);
        gameOverUI.SetActive(false);
    }
    [ClientRpc]
    public void MenuOff() {
        var paddles = FindObjectsOfType<Paddle>();
        foreach (Paddle p in paddles) {
            p.inMenu = false;
        }
        menu.SetActive(false);
        waitForHost.SetActive(false);

        scoreUI.enabled = true;
        levelUI.enabled = true;
        livesUI.enabled = true;
        levelCompleteUI.SetActive(false);
        gameOverUI.SetActive(false);
    }
    #endregion

    #region Gameover
    // calls to client to return to the menu and resets points
    [ClientRpc]
    public void GameOver() {
        var balls = FindObjectsOfType<Ball>();
        foreach (Ball b in balls) {
            Destroy(b.gameObject);
        }

        var paddles = FindObjectsOfType<Paddle>();
        foreach (Paddle p in paddles) {
            p.inMenu = true;
        }
        StartCoroutine(GameOverState());
    }
    IEnumerator GameOverState() {
        gameOverUI.SetActive(true);
        yield return new WaitForSeconds(5);
        gameOverUI.SetActive(false);
        MenuOn();
    }
    #endregion

    #region Complete level
    // calls to client to return to the menu without resetting points
    [ClientRpc]
    public void CompleteLevel() {
        var balls = FindObjectsOfType<Ball>();
        foreach (Ball b in balls) {
            Destroy(b.gameObject);
        }

        var paddles = FindObjectsOfType<Paddle>();
        foreach (Paddle p in paddles) {
            p.inMenu = true;
        }
        StartCoroutine(CompleteLevelState());
    }
    IEnumerator CompleteLevelState() {
        levelCompleteUI.SetActive(true);
        yield return new WaitForSeconds(5);
        levelCompleteUI.SetActive(false);
        MenuOn();
    }
    #endregion

    [ClientRpc]
    public void UpdateBG(int bgNum) {
            background.sprite = bgs[bgNum];
    }
    public void startGame() {        
        FindObjectOfType<NetworkManagerBreakout>().StartGame();
    }
}
