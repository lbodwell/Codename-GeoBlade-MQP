using UnityEngine;

public class GeoPickup : MonoBehaviour {
    public int energyValue;
    public Transform dest;
    private bool _isObjectHeld;

    private void Update() {
        // TODO: change to raycast
        var playerPos = PlayerManager.Instance.player.transform.position;
        var dist = Vector3.Distance(playerPos, transform.position);
        // TODO: change to new input system
        if (Input.GetKeyDown(KeyCode.E) && dist < 2) {
            print("Interact");
            if (_isObjectHeld) {
                print("Dropping object");
                transform.parent = null;
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<BoxCollider>().enabled = true;
                _isObjectHeld = false;
            } else {
                print("Picking up object");
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<BoxCollider>().enabled = false;
                transform.position = dest.position;
                transform.parent = GameObject.Find("PickupDestination").transform;
                _isObjectHeld = true;
            }
        }
    }
}
