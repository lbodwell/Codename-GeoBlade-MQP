using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
    public string lineId;
    
    private async void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            print("collision with player");
            await DialogueManager.Instance.PlayDialogueSequence(lineId);
        }
    }
}
