using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeoPuzzle : MonoBehaviour {
    public static GeoPuzzle Instance { get; private set; }
    public Transform pickupDestination;
    public bool isPuzzleActive = true;
    public bool isPickupInHand;
    public List<GeoReceptacle> receptacles;
    public List<GeoPickup> pickups;
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
        if (isPuzzleActive) {
            var player = PlayerManager.Instance.player;
            
            // Switch to new input system (move to player controller?)
            if (Input.GetKeyDown(KeyCode.E)) {
                var minDist = double.MaxValue;
                
                foreach (var receptacle in receptacles) {
                    var dist = Vector3.Distance(player.transform.position, receptacle.transform.position);
                    
                    if (dist < minDist) {
                        _nearestReceptacle = receptacle;
                        minDist = dist;
                    }
                }

                var isReceptacleInRange = minDist < 5;
                
                if (isPickupInHand) {
                    var pickupTransform = _currentPickup.transform;
                    
                    if (isReceptacleInRange) {
                        Debug.Log("Place in receptacle");
                        _nearestReceptacle.AddPickup(_currentPickup);
                        var targetPos = _nearestReceptacle.GetNextOpenPosition();
                        pickupTransform.position = targetPos;
                        pickupTransform.parent = null;
                        // Apply rotational force
                    } else {
                        Debug.Log("Drop on floor");
                        pickupTransform.parent = null;
                        _currentPickup.GetComponent<Rigidbody>().useGravity = true; 
                        _currentPickup.GetComponent<BoxCollider>().enabled = true;
                    }

                    _currentPickup = null;
                    isPickupInHand = false;
                } else {

                    var colliders = new Collider[5];
                    var size = Physics.OverlapSphereNonAlloc(player.transform.position, 2, colliders);
                    Debug.Log("Searching for nearby pickups...");
                    foreach (var coll in colliders) {
                        Debug.Log("Found collider with tag: " + coll.tag);
                    }
                    
                    // Maybe don't use this here. Should probably prioritize nearest pickup even if within range of receptacle.
                    if (isReceptacleInRange) {
                        Debug.Log("Grab from receptacle");
                        _currentPickup = _nearestReceptacle.RemovePickup();
                    } else {
                        Debug.Log("Pick up from floor");
                        
                    }

                    var pickupTransform = _currentPickup.transform;
                    
                    _currentPickup.GetComponent<Rigidbody>().useGravity = false;
                    _currentPickup.GetComponent<BoxCollider>().enabled = false;
                    pickupTransform.position = pickupDestination.position;
                    pickupTransform.parent = pickupDestination;
                    
                    isPickupInHand = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.P)) {
                Debug.Log("Current puzzle status:");
                foreach (var receptacle in receptacles) {
                    Debug.Log("Receptacle " + receptacle.id + ": " + receptacle.totalEnergy + "/" + receptacle.targetEnergy);
                }
            }
            
            if (receptacles.Count(receptacle => receptacle.targetReached) == receptacles.Count) {
                Debug.Log("Puzzle complete!");
            }
        }
    }
}
