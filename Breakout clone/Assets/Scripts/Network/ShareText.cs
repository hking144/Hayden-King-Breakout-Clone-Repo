using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ShareText : NetworkBehaviour {
    [SyncVar] public string currentText;
    public Text textbox;

    int delay = 20;
    int counter = 0;

    [ClientRpc]
    private void RpcSyncStringWithClients(string tx) {
        currentText = tx;
    }

    private void Update() {
        counter++;
        if (counter > delay) {
            counter = Random.Range(-5, 5);
            if (isServer) {
                RpcSyncStringWithClients(textbox.text);
            }
        }
    }

    private void LateUpdate() {
        if (!isServer) {
            textbox.text = currentText;
        }
    }
}
