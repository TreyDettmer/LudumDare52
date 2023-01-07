using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaterDistributorGui : MonoBehaviour
{

    public static WaterDistributorGui Instance;

    [SerializeField] LeanTweenType showEaseType;
    [SerializeField] LeanTweenType hideEaseType;
    [SerializeField] float showAnimationTime = .2f;
    [SerializeField] float hideAnimationTime = .2f;
    [SerializeField] GameObject distributePanel;
    [SerializeField] GameObject statsBar;
    [SerializeField] TextMeshProUGUI distributePanelTitle;
    [SerializeField] TMP_InputField waterAmountInputField;
    private GameObject interactedObject;


    [Header("Stats Bar")]
    [SerializeField] TextMeshProUGUI availableWaterAmount;
    [SerializeField] TextMeshProUGUI oxygenText;
    [SerializeField] TextMeshProUGUI foodText;
    [SerializeField] TextMeshProUGUI electricityText;

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
        GameplayManager.Instance.OnElectricityChanged += UpdateElectricityValue;
        GameplayManager.Instance.OnFoodChanged += UpdateFoodValue;
        GameplayManager.Instance.OnOxygenChanged += UpdateOxygenValue;
        distributePanel.SetActive(false);
        statsBar.SetActive(false);
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
            ShowStatsBar();
        }
        else
        {
            HideStatsBar();
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
        GameplayManager.Instance.ToggleActionMaps(false, true);
    }

    public void ShakeDistributePanel()
    {
        LeanTween.moveLocalX(distributePanel, 2f, .2f).setLoopPingPong(1);
    }

    public void ShowStatsBar()
    {
        availableWaterAmount.text = "Water Available: " + (Mathf.Round(WaterDistributor.Instance.AvailableWater * 100f) / 100f).ToString() + " gallons";
        statsBar.transform.localScale = new Vector3(1, 0f, 1f);
        statsBar.SetActive(true);
        LeanTween.scaleY(statsBar, 1f, hideAnimationTime).setEase(hideEaseType);
    }

    public void HideStatsBar()
    {
        LeanTween.scaleY(statsBar, 0f, hideAnimationTime).setEase(hideEaseType).setOnComplete(_ => { statsBar.SetActive(false); });
        
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
            availableWaterAmount.text = "Water Available: " + (Mathf.Round(WaterDistributor.Instance.AvailableWater * 100f) / 100f).ToString() + " gallons";
        }
        else
        {
            ShakeDistributePanel();
        }
    }

    void UpdateElectricityValue(float newValue)
    {
        electricityText.text = "Electricity: " + newValue.ToString("F1") + "%";
    }

    void UpdateOxygenValue(float newValue)
    {
        oxygenText.text = "Oxygen: " + newValue.ToString("F1") + "%";
    }

    void UpdateFoodValue(float newValue)
    {
        foodText.text = "Food: " + newValue.ToString("F1") + "%";
    }
}
