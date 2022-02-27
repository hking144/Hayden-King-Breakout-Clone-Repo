using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberUpdater : MonoBehaviour {

    Text text;
    public bool doubleNum; // used for columns as they are instantiated symetrically
    public float number { get; set; }

    void Awake() {
        text = GetComponent<Text>();
    }

    public void UpdateText() {
        if (doubleNum) {
            float doubledNum = number * 2;
            text.text = doubledNum.ToString();
        }
        else
            text.text = number.ToString();
    }

}
