using UnityEngine;

public class GeoReceptacle : MonoBehaviour {
    public int id;
    public int numPickups;
    public int totalEnergy;
    public int targetEnergy;
    
    public delegate void GeoReceptacleAction(GeoReceptacle receptacle);
    
    public static event GeoReceptacleAction OnReceptacleInRange;

    private void Update() {
        if (Vector3.Distance(PlayerManager.Instance.player.transform.position, transform.position) < 5) {
            // Debug.Log("here");
            OnReceptacleInRange?.Invoke(this);
        }
    }
}