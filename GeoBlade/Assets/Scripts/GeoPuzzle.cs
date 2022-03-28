using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GeoPuzzle : MonoBehaviour {
    public static GeoPuzzle Instance { get; private set; }
    public Transform pickupDestination;
    public bool isPuzzleActive;
    public bool isPickupInHand;
    public List<GeoReceptacle> receptacles;
    private GeoReceptacle _nearestReceptacle;
    private GeoPickup _currentPickup;
    private bool _isPuzzleComplete;
    private bool _lastPlaythroughStarted;
    private int _noteSequenceIndex;
    private int _notesLeftInLastPlaythrough = 5;
    private float _nextNote;
    // Roughly the amount of time that a half note lasts at 112 bpm
    private const float NoteTimeout = 1.0708f;
    private Material _activeGeneratorMaterial;
    private Material _inactiveGeneratorMaterial;

    private void Awake() {
        // TODO: Apply this pattern to all singletons
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }

        _activeGeneratorMaterial = Resources.Load("Materials/GeoPickup1", typeof(Material)) as Material;
        _inactiveGeneratorMaterial = receptacles[0].gameObject.GetComponent<Renderer>().material;
    }
    
    private void Update() {
        if (!isPuzzleActive) return;
        
        var player = PlayerManager.Instance.player;

        if (Time.time > _nextNote) {
            _nextNote = Time.time + NoteTimeout;
            
            var receptacle = receptacles[_noteSequenceIndex];
            var prevReceptacle = _noteSequenceIndex == 0 ? receptacles[3] : receptacles[_noteSequenceIndex - 1];
            
            if (_isPuzzleComplete && !_lastPlaythroughStarted && _noteSequenceIndex == 3) {
                _lastPlaythroughStarted = true;
            }
            
            receptacle.gameObject.GetComponent<Renderer>().material = _activeGeneratorMaterial;
            prevReceptacle.gameObject.GetComponent<Renderer>().material = _inactiveGeneratorMaterial;

            AkSoundEngine.SetState("Generator_Hum_Note", "Note_" + receptacle.targetEnergy);
            AkSoundEngine.PostEvent("Play_Generator_Hum", receptacle.gameObject);
            
            if (!receptacle.IsEmpty()) {
                AkSoundEngine.SetState("Resonant_Frequency_Note", "Note_" + receptacle.totalEnergy);
                AkSoundEngine.PostEvent("Play_Resonant_Frequency", receptacle.gameObject);
            }
            
            _noteSequenceIndex = (_noteSequenceIndex + 1) % 4;

            if (_lastPlaythroughStarted) {
                _notesLeftInLastPlaythrough--;
            }

            if (_notesLeftInLastPlaythrough == 0) {
                isPuzzleActive = false;
                SceneManager.LoadScene("Demo_End");
            }
        }
            
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
                    pickupTransform.rotation = Quaternion.identity;
                    _currentPickup.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 1, 0);
                } else {
                    _currentPickup.GetComponent<Rigidbody>().useGravity = true; 
                    _currentPickup.GetComponent<BoxCollider>().enabled = true;
                }
                
                pickupTransform.SetParent(null);
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
                    body.angularVelocity = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));

                    isPickupInHand = true;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            DisplayPuzzleStatus();
        }
        
        if (!_isPuzzleComplete && receptacles.Count(receptacle => receptacle.targetReached) == receptacles.Count) {
            Debug.Log("Puzzle complete!");
            _isPuzzleComplete = true;
        }
    }

    private void DisplayPuzzleStatus() {
        Debug.Log("Current puzzle status:");
        foreach (var receptacle in receptacles) {
            Debug.Log("Receptacle " + receptacle.id + ": " + receptacle.totalEnergy + "/" + receptacle.targetEnergy);
        }
    }
}
