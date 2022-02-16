using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
    public string lineId;
    private bool _hasDialogueBeenTriggered;
    
    private async void OnTriggerEnter(Collider other) {
        if (_hasDialogueBeenTriggered || !other.gameObject.CompareTag("Player")) return;
        
        _hasDialogueBeenTriggered = true;
        await DialogueManager.Instance.PlayDialogueSequence(lineId);
    }
}
