using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
    public string lineId;
    
    private async void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            await DialogueManager.Instance.PlayDialogueSequence(lineId);
        }
    }
}
