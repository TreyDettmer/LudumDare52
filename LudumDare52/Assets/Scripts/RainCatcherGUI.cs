using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RainCatcherGUI : MonoBehaviour
{

    public static RainCatcherGUI Instance;
    [SerializeField] TextMeshProUGUI dropletCounter;
    [SerializeField] TextMeshProUGUI dropletCounterLabel;
    [SerializeField] TextMeshProUGUI prompt;
    [SerializeField] TextMeshProUGUI promptDetail;
    [SerializeField] Gradient waterColorGradient;
    [SerializeField] TextMeshProUGUI multiplierText;
    [SerializeField] Color multiplierTextGoodColor;
    [SerializeField] Color multiplierTextBadColor;
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


    public void ToggleVisibility(bool visible)
    {
        if (visible)
        {
            ShowGui();
        }
        else
        {
            HideGui();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        RainCatcher.Instance.OnCaughtRain += OnCaughtRain;
        RainCatcher.Instance.OnMultiplierChanged += OnMultiplierChanged;
        OnCaughtRain(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ShowGui()
    {
        dropletCounter.text = "0";
        dropletCounter.gameObject.transform.localScale = new Vector3(1, 0f, 1f);
        dropletCounterLabel.gameObject.transform.localScale = new Vector3(1, 0f, 1f);
        prompt.gameObject.transform.localScale = new Vector3(1, 0f, 1f);
        promptDetail.gameObject.transform.localScale = new Vector3(1, 0f, 1f);
        multiplierText.gameObject.transform.localScale = new Vector3(1, 0f, 1f);
        dropletCounter.enabled = true;
        dropletCounterLabel.enabled = true;
        prompt.enabled = true;
        promptDetail.enabled = true;
        multiplierText.enabled = true;
        LeanTween.scaleY(dropletCounter.gameObject, 1f, .5f).setEase(LeanTweenType.easeOutBounce);
        LeanTween.scaleY(dropletCounterLabel.gameObject, 1f, .5f).setEase(LeanTweenType.easeOutBounce);
        LeanTween.scaleY(prompt.gameObject, 1f, .5f).setEase(LeanTweenType.easeOutBounce);
        LeanTween.scaleY(promptDetail.gameObject, 1f, .5f).setEase(LeanTweenType.easeOutBounce);
        LeanTween.scaleY(multiplierText.gameObject, 1f, .5f).setEase(LeanTweenType.easeOutBounce);


    }

    void HideGui()
    {
        LeanTween.scaleY(dropletCounter.gameObject, 0f, .5f).setEase(LeanTweenType.easeOutBounce);
        LeanTween.scaleY(dropletCounterLabel.gameObject, 0f, .5f).setEase(LeanTweenType.easeOutBounce);
        LeanTween.scaleY(prompt.gameObject, 0f, .5f).setEase(LeanTweenType.easeOutBounce);
        LeanTween.scaleY(promptDetail.gameObject, 0f, .5f).setEase(LeanTweenType.easeOutBounce);
        LeanTween.scaleY(multiplierText.gameObject, 0f, .5f).setEase(LeanTweenType.easeOutBounce);
        prompt.enabled = false;
        multiplierText.enabled = false;
        promptDetail.enabled = false;
        dropletCounter.enabled = false;
        dropletCounterLabel.enabled = true;
    }

    void OnCaughtRain(int caughtGoodRainAmount, int caughtAcidRainAmount)
    {
        dropletCounter.text = (caughtGoodRainAmount + caughtAcidRainAmount).ToString();
        dropletCounter.color = waterColorGradient.Evaluate(caughtAcidRainAmount / ((caughtGoodRainAmount + caughtAcidRainAmount) == 0 ? 1f : (caughtGoodRainAmount + caughtAcidRainAmount)));
        dropletCounterLabel.color = waterColorGradient.Evaluate(caughtAcidRainAmount / ((caughtGoodRainAmount + caughtAcidRainAmount) == 0 ? 1f : (caughtGoodRainAmount + caughtAcidRainAmount)));

    }

    void OnMultiplierChanged(int newMultiplierValue)
    {
        if (newMultiplierValue == 1)
        {
            multiplierText.color = multiplierTextBadColor;
            LeanTween.scaleY(multiplierText.gameObject, 0f, .5f).setEase(LeanTweenType.easeOutBounce);
            multiplierText.text = "";
        }
        else
        {
            string previousText = multiplierText.text;
            multiplierText.text = "x" + newMultiplierValue;
            multiplierText.color = multiplierTextGoodColor;
            if (previousText == "")
            {
                LeanTween.scaleY(multiplierText.gameObject, 1f, .5f).setEase(LeanTweenType.easeOutBounce);

            }

        }
    }

    
    


}
