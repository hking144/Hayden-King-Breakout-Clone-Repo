using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DestroyOnTime : NetworkBehaviour {

    public float time;

    void Start() {
        StartCoroutine(DestroyTime());
    }

    IEnumerator DestroyTime() {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
