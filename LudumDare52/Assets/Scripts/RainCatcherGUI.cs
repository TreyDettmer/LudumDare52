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

    [SerializeField] Gradient waterColorGradient;
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
        dropletCounter.enabled = true;
        dropletCounterLabel.enabled = true;
        prompt.enabled = true;
        LeanTween.scaleY(dropletCounter.gameObject, 1f, .5f).setEase(LeanTweenType.easeOutBounce);
        LeanTween.scaleY(dropletCounterLabel.gameObject, 1f, .5f).setEase(LeanTweenType.easeOutBounce);
        LeanTween.scaleY(prompt.gameObject, 1f, .5f).setEase(LeanTweenType.easeOutBounce);


    }

    void HideGui()
    {
        LeanTween.scaleY(dropletCounter.gameObject, 0f, .5f).setEase(LeanTweenType.easeOutBounce);
        LeanTween.scaleY(dropletCounterLabel.gameObject, 0f, .5f).setEase(LeanTweenType.easeOutBounce);
        LeanTween.scaleY(prompt.gameObject, 0f, .5f).setEase(LeanTweenType.easeOutBounce);
        prompt.enabled = false;
        dropletCounter.enabled = false;
        dropletCounterLabel.enabled = true;
    }

    void OnCaughtRain(int caughtGoodRainAmount, int caughtAcidRainAmount)
    {
        dropletCounter.text = (caughtGoodRainAmount + caughtAcidRainAmount).ToString();
        dropletCounter.color = waterColorGradient.Evaluate(caughtAcidRainAmount / ((caughtGoodRainAmount + caughtAcidRainAmount) == 0 ? 1f : (caughtGoodRainAmount + caughtAcidRainAmount)));
        dropletCounterLabel.color = waterColorGradient.Evaluate(caughtAcidRainAmount / ((caughtGoodRainAmount + caughtAcidRainAmount) == 0 ? 1f : (caughtGoodRainAmount + caughtAcidRainAmount)));

    }


}
