using UnityEngine;

public class PlayerManager : MonoBehaviour {
    public static PlayerManager Instance;
    public GameObject player;
    public GameObject geoBlade;

    private void Awake() {
        Instance = this;
    }
}
