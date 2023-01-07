using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RainCatcherGUI : MonoBehaviour
{

    public static RainCatcherGUI Instance;
    [SerializeField] TextMeshProUGUI dropletCounter;

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
            ShowDropletCounter();
        }
        else
        {
            HideDropletCounter();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        RainCatcher.Instance.OnCaughtRain += OnCaughtRain;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ShowDropletCounter()
    {
        dropletCounter.enabled = true;
    }

    void HideDropletCounter()
    {
        dropletCounter.enabled = false;
    }

    void OnCaughtRain(int caughtGoodRainAmount, int caughtAcidRainAmount)
    {
        dropletCounter.text = (caughtGoodRainAmount + caughtAcidRainAmount).ToString();
    }


}
