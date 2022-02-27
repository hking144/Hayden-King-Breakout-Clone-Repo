using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ShareColors : NetworkBehaviour {

    [SyncVar] public Color currentColor;
    public SpriteRenderer rend;

    int delay = 4;
    int counter = 0;

    [ClientRpc]
    private void RpcSyncColorWithClients(Color col) {
        currentColor = col;
    }

    private void Update() {
        counter++;
        if (counter > delay) {
            counter = Random.Range(-2, 2);
            if (isServer) {
                RpcSyncColorWithClients(rend.color);
            }
        }
    }

    private void LateUpdate() {
        if (!isServer) {
            rend.color = currentColor;
        }
    }
}
