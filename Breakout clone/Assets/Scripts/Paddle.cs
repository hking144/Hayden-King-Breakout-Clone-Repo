using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Paddle : NetworkBehaviour {
    #region Vars
    [SyncVar]
    public float moveSpeed;
    public float movementRange;
    [SerializeField] GameObject ballPrefab;
    [SyncVar]
    public Ball ball;
    NetworkManagerBreakout manager;
    [HideInInspector] public Animator anim;

    public bool inMenu;
    #endregion

    void Start() {
        manager = FindObjectOfType<NetworkManagerBreakout>();
        anim = GetComponent<Animator>();
    }
    void Update() {
        if (isLocalPlayer) {
            var pos = transform.position;
            pos.x += Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
            transform.position = new Vector3(Mathf.Clamp(pos.x, -movementRange, movementRange), pos.y, pos.z);
            if (ball == null  && !inMenu)
                manager.hostMustLaunch.SetActive(true);
            else
                manager.hostMustLaunch.SetActive(false);
        } // movement
        if (Input.GetKeyDown(KeyCode.Space) && isServer && manager.bricksUp) {
            FindObjectOfType<NetworkManagerBreakout>().LaunchBalls(name);
        }// if host launch ball from unballed paddles
    }
}
