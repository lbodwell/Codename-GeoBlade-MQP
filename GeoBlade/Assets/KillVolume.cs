using UnityEngine;

public class KillVolume : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            var player = PlayerManager.Instance.player;
            if (player != null) {
                player.GetComponent<PlayerStats>().DamageCharacter(100);
            }
        }
    }
}
