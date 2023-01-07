using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{

    public static GameplayManager Instance;

    public enum GameplayState
    {
        PREGAME,
        CATCHING_RAIN,
        DISTRIBUTING_WATER,
        GAME_OVER
    }

    public GameplayState CurrentGameplayState { get; private set;}
    public event EventHandler StartGame;
    public event EventHandler SwitchedToWaterDistributor;

    private RainManager rainManager;
    private Camera camera;


    public delegate void OnResourceValueChangedDelegate(float newResourceValue);
    public event OnResourceValueChangedDelegate OnOxygenChanged;
    public event OnResourceValueChangedDelegate OnFoodChanged;
    public event OnResourceValueChangedDelegate OnElectricityChanged;


    [SerializeField] private float totalOxygen;
    [SerializeField] private float totalFood;
    [SerializeField] private float totalElectricity;


    [SerializeField] private float oxygenDecayDelay;
    [SerializeField] private float foodDecayDelay;
    [SerializeField] private float electricityDecayDelay;
    private float lastTimeOfOxygenDecay;
    private float lastTimeOfFoodDecay;
    private float lastTimeOfElectricityDecay;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        camera = FindObjectOfType<Camera>();
        rainManager = GetComponent<RainManager>();
        rainManager.ShowerFinished += HandleShowerFinished;
        WaterDistributor.Instance.OnInteraction += OnDistributorInteraction;
        WaterDistributor.Instance.OnDistributedWater += OnDistributedWater;
        // TODO: Game Intro
        CurrentGameplayState = GameplayState.PREGAME;
        StartCoroutine(PregameIntro());
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentGameplayState == GameplayState.DISTRIBUTING_WATER)
        {
            if (Time.time - lastTimeOfElectricityDecay >= electricityDecayDelay)
            {
                UpdateTotalElectricity(-1f);
                lastTimeOfElectricityDecay = Time.time;
            }
            if (Time.time - lastTimeOfFoodDecay >= foodDecayDelay)
            {
                UpdateTotalFood(-1f);
                lastTimeOfFoodDecay = Time.time;
            }
            if (Time.time - lastTimeOfOxygenDecay >= oxygenDecayDelay)
            {
                UpdateTotalOxygen(-1f);
                lastTimeOfOxygenDecay = Time.time;
            }
        }
    }

    IEnumerator PregameIntro()
    {
        yield return new WaitForSeconds(.5f);
        SwitchToCatchingRain();
        StartGame?.Invoke(this, EventArgs.Empty);
    }

    public void SwitchToCatchingRain()
    {
        CurrentGameplayState = GameplayState.CATCHING_RAIN;
        ToggleActionMaps(true, false);
        WaterDistributorGui.Instance.ToggleVisibility(false);
        RainCatcherGUI.Instance.ToggleVisibility(true);
        LeanTween.move(camera.gameObject, new Vector3(0, 34f, -10), 1.5f).setEase(LeanTweenType.easeInOutCubic);
        LeanTween.rotate(camera.gameObject, new Vector3(0f, 0f, 0f), 1f);
    }

    public void HandleShowerFinished(object sender, EventArgs eventArgs)
    {
        int caughtGoodRainAmount = RainCatcher.Instance.GetCaughtGoodRainAmount();
        int caughtAcidRainAmount = RainCatcher.Instance.GetCaughtAcidRainAmount();
        WaterDistributor.Instance.SetAvailableWaterAndAcidity(caughtGoodRainAmount + caughtAcidRainAmount, caughtAcidRainAmount / ((caughtGoodRainAmount + caughtAcidRainAmount) == 0 ? 1f : (caughtGoodRainAmount + caughtAcidRainAmount)));
        SwitchedToWaterDistributor?.Invoke(this, EventArgs.Empty);
        SwitchToWaterDistributor();
    }

    public void SwitchToWaterDistributor()
    {
        CurrentGameplayState = GameplayState.DISTRIBUTING_WATER;
        ToggleActionMaps(false, true);
        RainCatcherGUI.Instance.ToggleVisibility(false);
        LeanTween.move(camera.gameObject, new Vector3(0, 12f, -10f),1.5f).setEase(LeanTweenType.easeInOutCubic);
        LeanTween.rotate(camera.gameObject, new Vector3(20, 0f, 0f), 1f);
        WaterDistributorGui.Instance.ToggleVisibility(true);
    }

    public void UpdateTotalOxygen(float change)
    {
        totalOxygen += change;
        OnOxygenChanged?.Invoke(totalOxygen);
    }

    public void UpdateTotalFood(float change)
    {
        totalFood += change;
        OnFoodChanged?.Invoke(totalFood);
    }

    public void UpdateTotalElectricity(float change)
    {
        totalElectricity += change;
        OnElectricityChanged?.Invoke(totalElectricity);
    }

    void OnDistributorInteraction(GameObject interactedObject)
    {
        ToggleActionMaps(false, false);
    }

    void OnDistributedWater(GameObject interactedObject, float waterAmound)
    {  
        ToggleActionMaps(false, true);
    }

    public void ToggleActionMaps(bool rainCatcherToggle, bool waterDistributorToggle)
    {
        RainCatcher.Instance.ToggleActionMap(rainCatcherToggle);
        WaterDistributor.Instance.ToggleActionMap(waterDistributorToggle);
    }



}
