using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {
    public enum BarType {
        Health,
        Geo
    }
    
    public static HUDManager Instance;
    public Image healthBar;
    public Image geoBar;

    private void Awake() {
        Instance = this;
    }
    
    public void UpdateBar(BarType type, float fill) {
        switch (type) {
            case BarType.Health:
                healthBar.fillAmount = fill;
                break;
            case BarType.Geo:
                geoBar.fillAmount = fill;
                break;
            default:
                throw new NotSupportedException();
        }
    }
}