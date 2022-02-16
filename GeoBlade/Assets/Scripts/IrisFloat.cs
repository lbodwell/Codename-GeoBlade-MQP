using System;
using UnityEngine;

public class IrisFloat : MonoBehaviour {
    private void Update() {
        var xVel = (float) Math.Cos(Time.time / 2) / 4;
        var yVel = (float) -Math.Cos(Time.time * 2) / 2;
        
        transform.Translate(xVel * Time.deltaTime, yVel * Time.deltaTime, 0);
    }
}
