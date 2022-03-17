using System.Collections.Generic;
using UnityEngine;

public class GeoReceptacle : MonoBehaviour {
    public int id;
    public int totalEnergy;
    public int targetEnergy;
    public bool targetReached;
    private readonly Stack<GeoPickup> _pickups = new Stack<GeoPickup>();

    public void AddPickup(GeoPickup pickup) {
        _pickups.Push(pickup);
        totalEnergy += pickup.energyValue;
        targetReached = targetEnergy == totalEnergy;
    }

    public GeoPickup RemovePickup() {
        var pickup = _pickups.Pop();

        totalEnergy -= pickup.energyValue;
        targetReached = targetEnergy == totalEnergy;
        
        return pickup;
    }

    public Vector3 GetNextOpenPosition() {
        var pos = transform.position;
        return new Vector3(pos.x,  pos.y + (_pickups.Count + 1) * 0.5f, pos.z);
    }

    public bool IsEmpty() {
        return _pickups.Count == 0;
    }
}