using UnityEngine;

public class AttackCollision : MonoBehaviour {
    private void OnCollisionEnter(Collision collision) {
        var playerController = PlayerManager.Instance.player.GetComponent<PlayerController>();
        //if (!playerController.isAttacking || !collision.gameObject.CompareTag("Enemy")) return;
        
        Debug.Log("Collision with enemy");
    }
}