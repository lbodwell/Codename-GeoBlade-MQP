using UnityEngine;

public class StartPuzzle : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        var puzzle = GeoPuzzle.Instance;
        
        if (!puzzle.isPuzzleActive && other.gameObject.CompareTag("Player")) {
            puzzle.isPuzzleActive = true;
        }
    }
}
