using UnityEngine;
public class HUDManager : MonoBehaviour {
    public static HUDManager Instance;

    private void Awake() {
        Instance = this;
    }
}