using UnityEngine;

public class StartTransportationHangarFight : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        var encounter = TransportationHangarScript.Instance;
        
        if (!encounter.isEncounterActive && other.gameObject.CompareTag("Player")) {
            encounter.isEncounterActive = true;
            encounter.droidActivationTime = Time.time + 36;
            Debug.Log("Started transportation hangar encounter");
        }
    }
}
