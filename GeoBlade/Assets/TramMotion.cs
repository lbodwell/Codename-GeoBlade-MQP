using UnityEngine;

public class TramMotion : MonoBehaviour {
    public float speed = 2.0f;
    public bool isMoving;
    public bool isPlayerOnTram;

    private void Update() {
        var player = PlayerManager.Instance.player;
        var playerPos = player.transform.position;
        var tramPos = transform.position;
        var playerController = player.GetComponent<CharacterController>();
        
        isPlayerOnTram = playerPos.x < tramPos.x + 10 && playerPos.x > tramPos.x - 10 &&
                         playerPos.z < tramPos.z + 5 && playerPos.z > tramPos.z - 5 &&
                         playerController.isGrounded;

        if (transform.position.x >= 0) {
            isMoving = false;
        } else if (!isMoving && isPlayerOnTram) {
            isMoving = true;
        }

        if (!isMoving) return;
        // TODO: Find a way to do this without using direct translation
        transform.Translate(10 * Time.deltaTime, 0.0f, 0.0f);
        if (isPlayerOnTram) {
            playerController.Move(new Vector3(10 * Time.deltaTime, 0.0f, 0.0f));
        }
    }
    
    // Events not working
    
    // private void OnCollisionEnter(Collision other) {
    //     Debug.Log("collision");
    //     if (other.gameObject.CompareTag("Player")) {
    //         Debug.Log("player got on");
    //         
    //     }
    //
    //     isMoving = true;
    // }
    //
    // private void OnCollisionExit(Collision other) {
    //     if (other.gameObject.CompareTag("Player")) {
    //         Debug.Log("player got off");
    //         PlayerManager.Instance.player.transform.parent = null;
    //     }
    //
    //     isMoving = false;
    // }
}
