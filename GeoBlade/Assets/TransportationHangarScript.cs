using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TransportationHangarScript : MonoBehaviour {
    public static TransportationHangarScript Instance { get; private set; }
    public bool isEncounterActive;
    public bool isEncounterComplete;
    public bool areDroidsAggro;
    public float droidActivationTime;
    public List<GameObject> securityDroids;
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    private void Update() {
        if (isEncounterActive) {
            if (areDroidsAggro) {
                var numDroidsAlive = securityDroids.Count(droid => droid != null);

                if (numDroidsAlive == 0) {
                    Debug.Log("All droids killed");
                    isEncounterComplete = true;
                    isEncounterActive = false;
                }
            } else if (Time.time > droidActivationTime && droidActivationTime != 0) {
                foreach (var droid in securityDroids) {
                    droid.GetComponent<SecurityDroidController>().isAggro = true;
                    areDroidsAggro = true;
                    Debug.Log("droids aggro");
                }

                isEncounterActive = true;
            }
        }
    }
}
