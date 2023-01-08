using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River : MonoBehaviour, Interactable
{
    public float Health { get; set; }
    public int NumberOfFish { get; set; }

    public float WaterAmount { get; set; }
    public float WaterAcidity { get; set; }

    public Vector3 RainPosition { get; set; }
    [SerializeField] Transform rainPositionTransform;
    [SerializeField] float minWaterLevel;
    [SerializeField] float maxWaterLevel;
    [SerializeField] Gradient waterColorGradient;
    Material material;

    private float lastTimeOfFoodOutput;
    public float FoodOutputDelay { get; set; }
    private float lastTimeOfDecay;
    public float DecayDelay { get; set; }

    private bool isSelected = false;
    // Start is called before the first frame update
    void Start()
    {
        GameplayManager.Instance.OnFoodChanged += OnFoodChanged;
        //GameplayManager.Instance.GameOver += OnGameOver;
        material = GetComponent<MeshRenderer>().material;
        WaterAmount = 100f;
        WaterAcidity = 0f;
        Health = WaterAmount;
    }

    private void OnGameOver(object sender, System.EventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void OnFoodChanged(float newResourceValue)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameplayManager.Instance.CurrentGameplayState == GameplayManager.GameplayState.DISTRIBUTING_WATER)
        {
            if (Time.time - lastTimeOfFoodOutput >= FoodOutputDelay)
            {
                
                float foodOutput = (WaterAmount / 100f) * (1 - WaterAcidity);
                GameplayManager.Instance.UpdateTotalFood(foodOutput);
                lastTimeOfFoodOutput = Time.time;
            }
            if (Time.time - lastTimeOfDecay >= DecayDelay)
            {
                ReceiveWater(-1f, 0f);
                lastTimeOfDecay = Time.time;
            }
        }
    }



    public void ReceiveWater(float addedWater, float acidityOfAddedWater)
    {
        
        if (addedWater > 0f)
        {
            WaterAcidity = ((WaterAmount) * (WaterAcidity) + (addedWater) * (acidityOfAddedWater)) / (WaterAmount + addedWater);
        }
        WaterAmount += addedWater;        
        WaterAmount = Mathf.Clamp(WaterAmount, 0f, 100f);
        Health = WaterAmount;
        material.SetColor("_BaseColor", waterColorGradient.Evaluate(WaterAcidity));
        if (isSelected)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, -Mathf.Lerp(Mathf.Abs(maxWaterLevel), Mathf.Abs(minWaterLevel), 1f - WaterAmount / 100f), transform.localPosition.z);
        }
    }

    public void UpdateFishCount(int change)
    {
        NumberOfFish += change;
    }

    public void ToggleOutline(bool enable)
    {
        if (enable)
        {
            isSelected = true;
            LeanTween.moveLocalY(gameObject, -Mathf.Lerp(Mathf.Abs(maxWaterLevel), Mathf.Abs(minWaterLevel), 1f - WaterAmount / 100f) + 1f, .3f);
        }
        else
        {
            LeanTween.moveLocalY(gameObject, -Mathf.Lerp(Mathf.Abs(maxWaterLevel), Mathf.Abs(minWaterLevel), 1f - WaterAmount / 100f) - 1f, .3f);
            isSelected = false;
        }
    }
}
