using UnityEngine;

public abstract class PowerupBase : ScriptableObject
{
    public Sprite Sprite;
    public string Description;

    public abstract void ApplyPowerUp();
}
