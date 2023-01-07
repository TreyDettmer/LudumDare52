using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River : MonoBehaviour, Interactable
{

    public int NumberOfFish { get; set; }

    public float WaterAmount { get; set; }

    private Outline outline;
    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineHidden;
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void RecieveWater(float waterAmount, float waterAcidity)
    {
        WaterAmount += waterAmount;
    }

    public void UpdateFishCount(int change)
    {
        NumberOfFish += change;
    }

    public void ToggleOutline(bool enable)
    {
        if (enable)
        {
            outline.OutlineMode = Outline.Mode.OutlineAll;
        }
        else
        {
            outline.OutlineMode = Outline.Mode.OutlineHidden;
        }
    }
}
