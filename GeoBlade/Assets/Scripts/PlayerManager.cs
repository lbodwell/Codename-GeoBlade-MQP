using UnityEngine;

public class PlayerManager : MonoBehaviour {
    public static PlayerManager Instance { get; private set; }
    public GameObject player;
    public GameObject camera;
    public GameObject geoBlade;
    public GameObject iris;
    public GameObject securityDroid1;
    public GameObject securityDroid2;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }
}
