using UnityEngine;

public class Potato : MonoBehaviour {
    void Start() {
        AkSoundEngine.PostEvent("Stasis_Pod_Hiss", gameObject);
    }
}
