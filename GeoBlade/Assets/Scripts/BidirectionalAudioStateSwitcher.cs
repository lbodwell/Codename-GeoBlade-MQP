using UnityEngine;

public class BidirectionalAudioStateSwitcher : MonoBehaviour {
    public AK.Wwise.State state1;
    public AK.Wwise.State state2;

    private void Start() {
        // Initial state defaults to "None" if not specified
        state1.SetValue();
    }

    private void OnCollisionEnter(Collision collision) {
        if (!collision.gameObject.CompareTag("Player") || state1.GroupId != state2.GroupId) return;
        
        AkSoundEngine.GetState(state1.GroupId, out var currentState);

        if (currentState == state1.Id) {
            AkSoundEngine.SetState(state1.GroupId, state2.Id);
        } else if (currentState == state2.Id) {
            AkSoundEngine.SetState(state1.GroupId, state1.Id);
        }
    }
}
