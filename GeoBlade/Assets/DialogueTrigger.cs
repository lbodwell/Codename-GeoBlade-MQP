using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
    public string lineId;
    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            DialogueManager.Instance.PlayDialogueSequence(lineId);
        }
    }
}
