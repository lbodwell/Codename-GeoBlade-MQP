using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
    public string lineId;
    private async void OnCollisionEnter(Collision collision) {
        if (!collision.gameObject.CompareTag("Player")) return;
        
        print("collision with player");

        await DialogueManager.Instance.PlayLine(lineId);
    }
}
