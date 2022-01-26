using UnityEngine;

public class GeoPickup : MonoBehaviour {
    public int energyValue;
    public Transform dest;
    private bool _isObjectHeld;
    private bool _isReceptacleInRange;
    private bool _inReceptacle;
    private GeoReceptacle _targetReceptacle;
    
    private void OnEnable() {
        GeoReceptacle.OnReceptacleInRange += CheckForGeoReceptacle;
    }
    
    private void OnDisable() {
        GeoReceptacle.OnReceptacleInRange -= CheckForGeoReceptacle;
    }

    private void Update() {
        // TODO: change to raycast
        var playerPos = PlayerManager.Instance.player.transform.position;
        var dist = Vector3.Distance(playerPos, transform.position);
        // TODO: change to new input system
        if (Input.GetKeyDown(KeyCode.E) && dist < 2) {
            print("Interact");
            if (_isObjectHeld) {
                // TODO: re-write this crap
                if (_isReceptacleInRange) {
                    var yPos = _targetReceptacle.transform.position.y + (_targetReceptacle.numPickups + 1) * 0.5;
                    var targetPos = new Vector3(_targetReceptacle.transform.position.x, (float) yPos, _targetReceptacle.transform.position.z);
                    _targetReceptacle.totalEnergy += energyValue;
                    _targetReceptacle.numPickups++;
                    Debug.Log("Receptacle " + _targetReceptacle.id + " is now at " + _targetReceptacle.totalEnergy + "/" + _targetReceptacle.targetEnergy);
                    transform.position = targetPos;
                    transform.parent = null;
                    _isObjectHeld = false;
                    _isReceptacleInRange = false;
                    _inReceptacle = true;
                } else {
                    print("Dropping object");
                    transform.parent = null;
                    GetComponent<Rigidbody>().useGravity = true;
                    GetComponent<BoxCollider>().enabled = true;
                    _isObjectHeld = false;
                }
            } else {
                print("Picking up object");
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<BoxCollider>().enabled = false;
                transform.position = dest.position;
                transform.parent = GameObject.Find("PickupDestination").transform;
                _isObjectHeld = true;
                if (_inReceptacle) {
                    _targetReceptacle.totalEnergy -= energyValue;
                    _targetReceptacle.numPickups--;
                    Debug.Log("Receptacle " + _targetReceptacle.id + " is now at " + _targetReceptacle.totalEnergy + "/" + _targetReceptacle.targetEnergy);
                    _targetReceptacle = null;
                    _inReceptacle = false;
                }
            }
        }
    }

    private void CheckForGeoReceptacle(GeoReceptacle receptacle) {
        if (_isObjectHeld) {
            _isReceptacleInRange = true;
            _targetReceptacle = receptacle;
        }
    }
}
