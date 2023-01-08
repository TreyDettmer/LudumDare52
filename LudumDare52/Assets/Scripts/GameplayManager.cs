using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public event EventHandler StartRaining;
    public event EventHandler SwitchedToWaterDistributor;

    private RainManager rainManager;
    private Camera camera;


    public delegate void OnResourceValueChangedDelegate(float newResourceValue);
    public event OnResourceValueChangedDelegate OnOxygenChanged;
    public event OnResourceValueChangedDelegate OnFoodChanged;
    public event OnResourceValueChangedDelegate OnTimeOfDayChanged;
    public event OnResourceValueChangedDelegate OnDayComplete;
    public delegate void OnGameOverDelegate(string reasonForGameOver, int daysLasted);
    public event OnGameOverDelegate GameOver;

    [SerializeField] private Transform waterDistributorCameraTransform;
    [SerializeField] private Transform rainCatcherCameraTransform;
    [SerializeField] ParticleSystem rainParticleSystem;
    [SerializeField] float introOrbitSpeed;

    [SerializeField] private float totalOxygen;
    [SerializeField] private float totalFood;
    [SerializeField] private float totalElectricity;
    [SerializeField] private int dayLengthInSeconds;


    [SerializeField] private float oxygenUsageDelay;
    [SerializeField] private float foodUsageDelay;

    [Header("River Management")]
    [SerializeField] private float foodOutputDelay;
    [SerializeField] private float riverLoseWaterDelay;


    private float lastTimeOfOxygenDecay;
    private float lastTimeOfFoodDecay;
    private float lastTimeOfElectricityDecay;
    private float currentDayStartTime;
    private int timeUntilNextRain;
    private bool isGameRunning = true;
    public int CurrentDay { get; private set; }
    private bool isPregame = true;
    private bool isPostgame = false;

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
        FindObjectOfType<River>().FoodOutputDelay = foodOutputDelay;
        FindObjectOfType<River>().DecayDelay = riverLoseWaterDelay;
        timeUntilNextRain = dayLengthInSeconds;
        AudioManager.Instance.Play("Theme");
        CurrentDay = 1;

        // TODO: Game Intro
        CurrentGameplayState = GameplayState.PREGAME;
        StartCoroutine(PregameIntro());
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameRunning) return;

        if (CurrentGameplayState == GameplayState.DISTRIBUTING_WATER)
        {

            if (Time.time - lastTimeOfFoodDecay >= foodUsageDelay)
            {
                UpdateTotalFood(-1f);
                lastTimeOfFoodDecay = Time.time;
            }
            if (Time.time - lastTimeOfOxygenDecay >= oxygenUsageDelay)
            {
                UpdateTotalOxygen(-1f);
                lastTimeOfOxygenDecay = Time.time;
            }

            UpdateTimeUntilNextRain();


            if (totalOxygen <= 0f || totalFood <= 0f)
            {
                EndGame();
            }

        }
    }

    void UpdateTimeUntilNextRain()
    {
        int currentTimeUntilNextRain = (int)(dayLengthInSeconds - (Time.time - currentDayStartTime));
        if (currentTimeUntilNextRain != timeUntilNextRain)
        {
            OnTimeOfDayChanged?.Invoke(currentTimeUntilNextRain);
        }
        timeUntilNextRain = currentTimeUntilNextRain;
        if (timeUntilNextRain <= 0)
        {
            DayComplete();
            
        }
    }

    public void DayComplete()
    {
        CurrentDay++;
        if (CurrentDay == 11)
        {
            StartCoroutine(GameWasWon());
            return;
        }
        OnDayComplete?.Invoke(CurrentDay);
        SwitchToCatchingRain();
    }

    IEnumerator GameWasWon()
    {
        RainCatcherGUI.Instance.ToggleVisibility(false);
        WaterDistributorGui.Instance.ToggleVisibility(false);
        isGameRunning = false;
        CurrentGameplayState = GameplayState.GAME_OVER;
        camera.transform.position = waterDistributorCameraTransform.position;
        WinGameGui.Instance.ToggleVisibility(true);
        isPostgame = true;
        while (isPostgame)
        {
            // Orbit camera
            camera.transform.RotateAround(new Vector3(0f, -10f, 0f), Vector3.up, introOrbitSpeed);
            Vector3 forward = -(camera.transform.position - new Vector3(0f, -14f, 0f)).normalized;
            camera.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            yield return null;
        }
    }

    IEnumerator PregameIntro()
    {
        RainCatcherGUI.Instance.gameObject.SetActive(false);
        WaterDistributorGui.Instance.gameObject.SetActive(false);
        isPregame = true;
        PregameGui.Instance.ToggleVisibility(true);
        camera.transform.position = waterDistributorCameraTransform.position;
        while (isPregame)
        {
            // Orbit camera
            camera.transform.RotateAround(new Vector3(0f,-10f,0f), Vector3.up, introOrbitSpeed);
            Vector3 forward = -(camera.transform.position - new Vector3(0f, -14f, 0f)).normalized;
            camera.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            yield return null;
        }      
    }

    public void PregameFinished()
    {
        isPregame = false;
        PregameGui.Instance.ToggleVisibility(false);
        SwitchToCatchingRain();
    }

    public void SwitchToCatchingRain()
    {
        RainCatcherGUI.Instance.gameObject.SetActive(true);
        ToggleFastForwardTime(false);
        CurrentGameplayState = GameplayState.CATCHING_RAIN;  
        WaterDistributorGui.Instance.ToggleVisibility(false);
        RainCatcherGUI.Instance.ToggleVisibility(true);
        ToggleActionMaps(true, false);
        LeanTween.move(camera.gameObject, rainCatcherCameraTransform.position, 1.5f).setEase(LeanTweenType.easeInOutCubic);
        LeanTween.rotate(camera.gameObject, rainCatcherCameraTransform.rotation.eulerAngles, 1f);
        
        StartRaining?.Invoke(this, EventArgs.Empty);
    }

    public void HandleShowerFinished(object sender, EventArgs eventArgs)
    {
        int caughtGoodRainAmount = RainCatcher.Instance.GetCaughtGoodRainAmount();
        int caughtAcidRainAmount = RainCatcher.Instance.GetCaughtAcidRainAmount();
        WaterDistributor.Instance.AddToAvailableWaterAndAcidity(caughtGoodRainAmount + caughtAcidRainAmount, caughtAcidRainAmount / ((caughtGoodRainAmount + caughtAcidRainAmount) == 0 ? 1f : (caughtGoodRainAmount + caughtAcidRainAmount)));
        SwitchedToWaterDistributor?.Invoke(this, EventArgs.Empty);
        SwitchToWaterDistributor();
    }

    public void SwitchToWaterDistributor()
    {
        WaterDistributorGui.Instance.gameObject.SetActive(true);
        CurrentGameplayState = GameplayState.DISTRIBUTING_WATER;
        ToggleActionMaps(false, true);
        RainCatcherGUI.Instance.ToggleVisibility(false);
        LeanTween.move(camera.gameObject, waterDistributorCameraTransform.position,1.5f).setEase(LeanTweenType.easeInOutCubic);
        LeanTween.rotate(camera.gameObject, waterDistributorCameraTransform.rotation.eulerAngles, 1f);
        timeUntilNextRain = dayLengthInSeconds;
        currentDayStartTime = Time.time;
        WaterDistributorGui.Instance.ToggleVisibility(true);
    }

    public void UpdateTotalOxygen(float change)
    {
        totalOxygen += change;
        totalOxygen = Mathf.Clamp(totalOxygen, 0f, 100f);
        OnOxygenChanged?.Invoke(totalOxygen);
    }

    public void UpdateTotalFood(float change)
    {
        totalFood += change;
        totalFood = Mathf.Clamp(totalFood, 0f, 100f);
        OnFoodChanged?.Invoke(totalFood);
    }


    void OnDistributorInteraction(GameObject interactedObject)
    {
        Debug.Log("Interacted so disabling distributor action map");
        ToggleActionMaps(false, false);
    }

    void OnDistributedWater(GameObject interactedObject, float waterAmount)
    {  
        ToggleActionMaps(false, true);
        interactedObject.GetComponent<Interactable>().ReceiveWater(waterAmount,WaterDistributor.Instance.AcidityOfAvailableWater);
        StartCoroutine(RainParticlesRoutine(interactedObject.GetComponent<Interactable>()));
    }

    public void ToggleActionMaps(bool rainCatcherToggle, bool waterDistributorToggle)
    {
        RainCatcher.Instance.ToggleActionMap(rainCatcherToggle);
        WaterDistributor.Instance.ToggleActionMap(waterDistributorToggle);
    }


    IEnumerator RainParticlesRoutine(Interactable interactable)
    {
        rainParticleSystem.gameObject.transform.position = interactable.RainPosition;
        rainParticleSystem.Play();
        AudioManager.Instance.Play("Rain");
        yield return new WaitForSeconds(3f);
        rainParticleSystem.Stop();
    }

    public void ToggleFastForwardTime(bool fastForward)
    {
        if (fastForward)
        {
            if (CurrentGameplayState == GameplayState.DISTRIBUTING_WATER)
            {
                Time.timeScale = 3.5f;
            }
        }
        else
        {
            Time.timeScale = 1f;
        }
    }


    public void EndGame()
    {
        isGameRunning = false;
        CurrentGameplayState = GameplayState.GAME_OVER;
        if (totalOxygen <= 0f)
        {
            GameOver?.Invoke("Your town ran out of oxygen.",CurrentDay - 1);
        }
        else if (totalFood <= 0f)
        {
            GameOver?.Invoke("Your town ran out of food.",CurrentDay - 1);
        }
        
        GameOverGui.Instance.ToggleVisibility(true);
    }

    public void RestartGame()
    {
        Destroy(gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }

}
