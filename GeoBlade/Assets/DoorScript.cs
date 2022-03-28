using UnityEngine;

public class DoorScript : MonoBehaviour {
    public bool isOpen;
    public float speed;
    private void Update() {
        if (!isOpen && TransportationHangarScript.Instance.isEncounterComplete) {
            if (transform.position.y >= 45) {
                isOpen = true;
            } else {
                transform.Translate(0, speed * Time.deltaTime, 0);
            }
        }
    }
}
