using UnityEngine;

public class GeoPickup : MonoBehaviour {
    public int energyValue;
    public Transform dest;
    // private static bool _isObjectHeld;
    public GeoReceptacle CurrentReceptacle { get; set; }

    private void Update() {
        // TODO: change to raycast
        var playerPos = PlayerManager.Instance.player.transform.position;
        var dist = Vector3.Distance(playerPos, transform.position);
        // TODO: change to new input system

        // if (Input.GetKeyDown(KeyCode.E) && dist < 2) {
        //     Debug.Log("input");
        //     if (_isObjectHeld) {
        //         Debug.Log("In hand");
        //         // TODO: re-write this crap
        //         if (_isReceptacleInRange && _targetReceptacle != null) {
        //             var yPos = _targetReceptacle.transform.position.y + (_targetReceptacle.numPickups + 1) * 0.5;
        //             var targetPos = new Vector3(_targetReceptacle.transform.position.x, (float) yPos, _targetReceptacle.transform.position.z);
        //             _targetReceptacle.totalEnergy += energyValue;
        //             _targetReceptacle.numPickups++;
        //             Debug.Log("Receptacle " + _targetReceptacle.id + " is now at " + _targetReceptacle.totalEnergy + "/" + _targetReceptacle.targetEnergy);
        //             transform.position = targetPos;
        //             transform.parent = null;
        //             _isObjectHeld = false;
        //             _isReceptacleInRange = false;
        //             _inReceptacle = true;
        //             _currentReceptacle = _targetReceptacle;
        //             _targetReceptacle = null;
        //         } else {
        //             Debug.Log("Dropping object");
        //             transform.parent = null;
        //             GetComponent<Rigidbody>().useGravity = true; 
        //             GetComponent<BoxCollider>().enabled = true;
        //             _isObjectHeld = false;
        //             _targetReceptacle = null;
        //         }
        //     } else {
        //         Debug.Log("Not in hand");
        //         
        //         GetComponent<Rigidbody>().useGravity = false;
        //         GetComponent<BoxCollider>().enabled = false;
        //         transform.position = dest.position;
        //         transform.parent = GameObject.Find("PickupDestination").transform;
        //         _isObjectHeld = true;
        //         if (_inReceptacle) {
        //             Debug.Log("removed");
        //             _targetReceptacle.totalEnergy -= energyValue;
        //             _targetReceptacle.numPickups--;
        //             Debug.Log("Receptacle " + _targetReceptacle.id + " is now at " + _targetReceptacle.totalEnergy + "/" + _targetReceptacle.targetEnergy);
        //             _targetReceptacle = null;
        //             _currentReceptacle = null;
        //             _isReceptacleInRange = false;
        //             _inReceptacle = false;
        //         } else {
        //             Debug.Log("Picking up object");
        //         }
        //     }
        // }
    }

    // private void CheckForGeoReceptacle(GeoReceptacle receptacle) {
    //     Debug.Log("event fired");
    //     if (_isObjectHeld) {
    //         _isReceptacleInRange = true;
    //         _targetReceptacle = receptacle;
    //     }
    // }
}
