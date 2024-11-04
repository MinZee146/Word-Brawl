using UnityEngine;

public abstract class PowerUpBase : ScriptableObject
{
    public Sprite Sprite;
    public string Description;

    public abstract void SetUpPowerUp();
}
