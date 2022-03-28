using UnityEngine;

public class DoorScript : MonoBehaviour {
    public bool isOpen;
    public float speed;
    private void Update() {
        var script = TransportationHangarScript.Instance;
        
        if (script != null) {
            if (!isOpen && script.isEncounterComplete) {
                if (transform.position.y >= 45) {
                    isOpen = true;
                } else {
                    transform.Translate(0, speed * Time.deltaTime, 0);
                }
            }
        }
    }
}
