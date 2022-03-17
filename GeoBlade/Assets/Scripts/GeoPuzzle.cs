using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeoPuzzle : MonoBehaviour {
    public static GeoPuzzle Instance { get; private set; }
    public Transform pickupDestination;
    public bool isPuzzleActive = true;
    public bool isPickupInHand;
    public List<GeoReceptacle> receptacles;
    private GeoReceptacle _nearestReceptacle;
    private GeoPickup _currentPickup;

    private void Awake() {
        // TODO: Apply this pattern to all singletons
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }
    
    private void Update() {
        if (!isPuzzleActive) return;
        
        var player = PlayerManager.Instance.player;
            
        // Switch to new input system (move to player controller?)
        if (Input.GetKeyDown(KeyCode.E)) {

            var minReceptacleDist = double.MaxValue;
                
            foreach (var receptacle in receptacles) {
                var dist = Vector3.Distance(player.transform.position, receptacle.transform.position);
                    
                if (dist < minReceptacleDist) {
                    _nearestReceptacle = receptacle;
                    minReceptacleDist = dist;
                }
            }

            Transform pickupTransform;
            var isReceptacleInRange = minReceptacleDist < 5;
            
            if (isPickupInHand) {
                pickupTransform = _currentPickup.transform;
                    
                if (isReceptacleInRange) {
                    _nearestReceptacle.AddPickup(_currentPickup);
                    var targetPos = _nearestReceptacle.GetNextOpenPosition();
                    pickupTransform.position = targetPos;
                    pickupTransform.SetParent(null);
                    // Apply rotational force
                } else {
                    pickupTransform.SetParent(null);
                    _currentPickup.GetComponent<Rigidbody>().useGravity = true; 
                    _currentPickup.GetComponent<BoxCollider>().enabled = true;
                }
                
                _currentPickup = null;
                isPickupInHand = false;
            } else {
                if (isReceptacleInRange && !_nearestReceptacle.IsEmpty()) {
                    _currentPickup = _nearestReceptacle.RemovePickup();
                } else {
                    // TODO: Tune size of colliders buffer for optimal performance while ensuring all pickups are found.
                    var colliders = new Collider[10];
                    Physics.OverlapSphereNonAlloc(player.transform.position, 5, colliders);

                    GeoPickup nearestPickup = null;
                    var minPickupDist = double.MaxValue;

                    foreach (var coll in colliders) {
                        if (coll != null && coll.CompareTag("GeoPickup")) {
                            var dist = Vector3.Distance(player.transform.position, coll.transform.position);

                            if (dist < minPickupDist) {
                                nearestPickup = coll.gameObject.GetComponent<GeoPickup>();
                                minPickupDist = dist;
                            }
                        }
                    }

                    _currentPickup = nearestPickup;
                }

                if (_currentPickup != null) {
                    pickupTransform = _currentPickup.transform;
                    pickupTransform.position = pickupDestination.position;
                    pickupTransform.SetParent(pickupDestination);
                    
                    _currentPickup.GetComponent<Rigidbody>().useGravity = false;
                    _currentPickup.GetComponent<BoxCollider>().enabled = false;
                    
                    var body = _currentPickup.GetComponent<Rigidbody>();
                    body.velocity = Vector3.zero;
                    body.angularVelocity = Vector3.zero;

                    isPickupInHand = true;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            DisplayPuzzleStatus();
        }
            
        if (receptacles.Count(receptacle => receptacle.targetReached) == receptacles.Count) {
            Debug.Log("Puzzle complete!");
        }
    }

    private void DisplayPuzzleStatus() {
        Debug.Log("Current puzzle status:");
        foreach (var receptacle in receptacles) {
            Debug.Log("Receptacle " + receptacle.id + ": " + receptacle.totalEnergy + "/" + receptacle.targetEnergy);
        }
    }
}
