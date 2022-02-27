using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Brick : NetworkBehaviour {
    #region vars
    public SpriteRenderer rend;
    NetworkManagerBreakout manager;
    Rigidbody2D rb;
    Collider2D col;
    [SerializeField] GameObject breakParticle;
    [SyncVar] public Color brickColor;
    #endregion
    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        manager = FindObjectOfType<NetworkManagerBreakout>();
    }

    public void Break() {
        rend.color = manager.breakColor;
        rend.sortingOrder = 1;
        if (isServer) {
            GameObject particle = Instantiate(breakParticle, transform.position, Quaternion.identity, null);
            NetworkServer.Spawn(particle);
        }
        col.enabled = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(new Vector2(Random.Range(-300, 300), Random.Range(100, 300)));
        rb.AddTorque(1 * Random.Range(-100, 100));

        manager.activeBricks.Remove(this);
        manager.brickManager.score += 100;
        if (isServer)
            manager.CheckLevel();

        StartCoroutine(Remove());
    } // Disables collider, instantiates particles, sends brick flying and adds points
    IEnumerator Remove() {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    } // destroys brick after 5 seconds
}
