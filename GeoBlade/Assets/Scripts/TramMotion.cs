using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class TramMotion : MonoBehaviour {
    public float speed;
    public Vector3 destination;
    public bool isMoving;
    public bool isPlayerOnTram;
    private GameObject _player;

    private void Start() {
        _player = PlayerManager.Instance.player;
    }

    private async void Update() {
        _player = PlayerManager.Instance.player;
        if (_player != null) {
            var playerPos = _player.transform.position;
            var tramPos = transform.position;
            var playerController = _player.GetComponent<CharacterController>();
        
            isPlayerOnTram = playerPos.x < tramPos.x + 15 && playerPos.x > tramPos.x - 15 &&
                             playerPos.z < tramPos.z + 10 && playerPos.z > tramPos.z - 10 &&
                             playerController.isGrounded;

            if (transform.position.x > destination.x) {
                isMoving = false;
            } else if (!isMoving && isPlayerOnTram) {
                isMoving = true;
                await StartDialogue();
            }

            if (isMoving) {
                // TODO: Find a way to do this without using direct translation
                transform.Translate(speed * Time.deltaTime, 0.0f, 0.0f);
                if (isPlayerOnTram) {
                    playerController.Move(new Vector3(speed * Time.deltaTime, 0.0f, 0.0f));
                }
            }
        } else {
            Debug.Log("player is null");
        }
    }
    
    private static async Task StartDialogue() {
        await Task.Delay(1000);
        await DialogueManager.Instance.PlayDialogueSequence("lvl1_platformA_iris_03");
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
