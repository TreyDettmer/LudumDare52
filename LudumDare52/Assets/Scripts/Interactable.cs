
using UnityEngine;

public interface Interactable
{

    public Vector3 RainPosition { get; set; }
    public void ReceiveWater(float waterAmount, float waterAcidity);

    public void ToggleOutline(bool enable);
}
