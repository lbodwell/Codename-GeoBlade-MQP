using UnityEngine;

public class PlayerManager : MonoBehaviour {
    public static PlayerManager Instance;
    public GameObject player;
    public GameObject iris;

    private void Awake() {
        Instance = this;
    }
}
