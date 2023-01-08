using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WaterDistributorGui : MonoBehaviour
{

    public static WaterDistributorGui Instance;

    [SerializeField] LeanTweenType showEaseType;
    [SerializeField] LeanTweenType hideEaseType;
    [SerializeField] float showAnimationTime = .2f;
    [SerializeField] float hideAnimationTime = .2f;
    [SerializeField] TextMeshProUGUI prompt;
    [SerializeField] GameObject distributePanel;

    [SerializeField] TextMeshProUGUI distributePanelTitle;
    [SerializeField] TMP_InputField waterAmountInputField;
    private GameObject interactedObject;


    [Header("Stats Bar")]
    [SerializeField] GameObject statsBar;
    [SerializeField] TextMeshProUGUI availableWaterAmount;
    [SerializeField] TextMeshProUGUI oxygenText;
    [SerializeField] TextMeshProUGUI foodText;
    [SerializeField] TextMeshProUGUI dayText;

    [Header("Time Section")]
    [SerializeField] GameObject timeSection;
    [SerializeField] TextMeshProUGUI timeUntilNextRainText;

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
        WaterDistributor.Instance.OnInteraction += OnInteraction;
        GameplayManager.Instance.OnFoodChanged += UpdateFoodValue;
        GameplayManager.Instance.OnOxygenChanged += UpdateOxygenValue;
        GameplayManager.Instance.OnTimeOfDayChanged += UpdateTimeUntilNextRain;
        GameplayManager.Instance.OnDayComplete += UpdateCurrentDay;
        distributePanel.SetActive(false);
        statsBar.SetActive(false);
    }

    private void UpdateCurrentDay(float newDayIndex)
    {
        dayText.text = "Day " + (int)newDayIndex;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnInteraction(GameObject interactedObject)
    {
        this.interactedObject = interactedObject;
        distributePanelTitle.text = this.interactedObject.name;
        if (!distributePanel.activeSelf)
        {
            ShowDistributePanel();
        }
    }

    public void ToggleVisibility(bool visible)
    {
        if (visible)
        {
            ShowMainGui();
            ShowTimeSection();
        }
        else
        {
            HideTimeSection();
            HideMainGui();
            if (distributePanel.activeSelf)
            {
                HideDistributePanel();
            }
        }
    }

    public void ShowDistributePanel()
    {
        waterAmountInputField.text = "";
        distributePanel.transform.localScale = Vector3.zero;
        distributePanel.SetActive(true);
        LeanTween.scale(distributePanel, new Vector3(1, 1, 1), showAnimationTime).setEase(showEaseType);
    }

    public void HideDistributePanel()
    {
        LeanTween.scale(distributePanel, new Vector3(0f, 0f, 0f), hideAnimationTime).setEase(hideEaseType).setOnComplete(_ => { distributePanel.SetActive(false); });
        Debug.Log("Hiding distribute panel so enabling distributor action map");
        GameplayManager.Instance.ToggleActionMaps(false, true);
    }

    public void ShakeDistributePanel()
    {
        LeanTween.moveLocalX(distributePanel, 2f, .1f).setLoopPingPong(1);
    }

    public void ShowMainGui()
    {
        availableWaterAmount.text = "Water Available: " + (Mathf.Round(WaterDistributor.Instance.AvailableWater * 100f) / 100f).ToString() + " gal (" + (Mathf.Round(WaterDistributor.Instance.AcidityOfAvailableWater * 10000f) / 100f).ToString("F1") + "% acid)";
        statsBar.transform.localScale = new Vector3(1, 0f, 1f);
        prompt.gameObject.transform.localScale = new Vector3(1, 0f, 1f);
        dayText.gameObject.transform.localScale = new Vector3(1, 0f, 1f);
        prompt.enabled = true;
        dayText.enabled = true;
        statsBar.SetActive(true);
        LeanTween.scaleY(statsBar, 1f, showAnimationTime).setEase(showEaseType);
        LeanTween.scaleY(prompt.gameObject, 1f, showAnimationTime).setEase(showEaseType);
        LeanTween.scaleY(dayText.gameObject, 1f, showAnimationTime).setEase(showEaseType);
    }

    public void HideMainGui()
    {
        LeanTween.scaleY(statsBar, 0f, hideAnimationTime).setEase(hideEaseType).setOnComplete(_ => { statsBar.SetActive(false); });
        LeanTween.scaleY(prompt.gameObject, 0f, hideAnimationTime).setEase(hideEaseType).setOnComplete(_ => { prompt.enabled = false; });
        LeanTween.scaleY(dayText.gameObject, 0f, hideAnimationTime).setEase(hideEaseType).setOnComplete(_ => { dayText.enabled = false; });
    }

    void ShowTimeSection()
    {
        timeSection.transform.localScale = new Vector3(1, 0f, 1f);
        timeSection.SetActive(true);
        LeanTween.scaleY(timeSection, 1f, showAnimationTime).setEase(hideEaseType);
    }

    void HideTimeSection()
    {
        LeanTween.scaleY(timeSection, 0f, hideAnimationTime).setEase(hideEaseType).setOnComplete(_ => { timeSection.SetActive(false); });
    }



    public void SubmitWaterAmount()
    {
        if (waterAmountInputField.text == string.Empty)
        {
            ShakeDistributePanel();
            return;
        }
        float waterAmount = float.Parse(waterAmountInputField.text);
        if (WaterDistributor.Instance.AttemptToDistributeWater(waterAmount))
        {
            HideDistributePanel();
            availableWaterAmount.text = "Water Available: " + (Mathf.Round(WaterDistributor.Instance.AvailableWater * 100f) / 100f).ToString() + " gal (" + (Mathf.Round(WaterDistributor.Instance.AcidityOfAvailableWater * 10000f) / 100f).ToString("F1") + "% acid)";
        }
        else
        {
            ShakeDistributePanel();
        }
    }

    void UpdateOxygenValue(float newValue)
    {
        oxygenText.text = "Oxygen: " + newValue.ToString("F1") + "%";
    }

    void UpdateFoodValue(float newValue)
    {
        foodText.text = "Food: " + newValue.ToString("F1") + "%";
    }

    void UpdateTimeUntilNextRain(float newValue)
    {
        timeUntilNextRainText.text = "Time Until Rain: " + ((int)newValue).ToString() + "s";
    }

    public void StartFastForward()
    {
        Debug.Log("Start fast forward");
        GameplayManager.Instance.ToggleFastForwardTime(true);
    }

    public void StopFastForward()
    {
        Debug.Log("End fast forward");
        GameplayManager.Instance.ToggleFastForwardTime(false);
    }
}
