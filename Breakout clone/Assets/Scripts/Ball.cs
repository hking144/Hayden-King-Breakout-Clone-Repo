using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Ball : NetworkBehaviour {

    [HideInInspector] public Rigidbody2D rb;
    TrailRenderer trail;
    BrickManager manager;

    [SyncVar]
    public bool held;
    [SyncVar]
    [HideInInspector] public bool bricksActive;    

    void Awake() {
        manager = FindObjectOfType<BrickManager>();
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();      
    }

    public void LaunchBall() {
        if (manager.ballSpeed < 2)
            manager.ballSpeed = 2;

        rb.bodyType = RigidbodyType2D.Dynamic;
        Vector2 force = new Vector2(Random.Range(-1f, 1f), 1);
        rb.AddForce(force.normalized * manager.ballSpeed * 100);
        trail.time = 1;
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == "LossZone") {
            NetworkManagerBreakout manager = FindObjectOfType<NetworkManagerBreakout>();
            manager.brickManager.lifes--;
            if (manager.brickManager.lifes < 0)
                FindObjectOfType<NetworkManagerBreakout>().GameOver();
            manager.livesUI.text = "LIVES: " + manager.brickManager.lifes.ToString();
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter2D(Collision2D col) {
        if (col.transform.tag == "Paddle") {
            rb.velocity = Vector2.zero;
            col.transform.GetComponent<Paddle>().anim.SetTrigger("Hit");

            Vector3 point = col.contacts[0].point;
            float xDiff = point.x - col.transform.position.x;

            Vector2 force = new Vector2(xDiff, 1);
            rb.AddForce(force.normalized * manager.ballSpeed * 100);
        }
        if (col.transform.tag == "Brick") {
            col.transform.GetComponent<Brick>().Break();
        }
    }
}
