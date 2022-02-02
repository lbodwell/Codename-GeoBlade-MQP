using UnityEngine;

public class TramMotion : MonoBehaviour {
    public float speed = 2.0f;
    public bool isMoving = false;

    private void Update() {
        var player = PlayerManager.Instance.player;
        var playerPos = player.transform.position;
        var playerController = player.GetComponent<CharacterController>();
        // This is really dumb
        if (playerPos.x < transform.position.x + 10 && playerPos.x > transform.position.x - 10 && playerPos.z < transform.position.z + 5 && playerPos.z > transform.position.z - 5 && playerController.isGrounded) {
            // Debug.Log("on tram");
            transform.Translate(10 * Time.deltaTime, 0.0f, 0.0f);
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
