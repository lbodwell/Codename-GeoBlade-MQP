using System;

public class PlayerStats : CharacterStats {
    public int maxGeo;
    public int geo;
    
    public void ConsumeGeo(int amount) {
        health = Math.Max(0, geo - amount);
    }

    public void ReplenishGeo(int amount) {
        health = Math.Min(maxGeo, geo + amount);
    }
}