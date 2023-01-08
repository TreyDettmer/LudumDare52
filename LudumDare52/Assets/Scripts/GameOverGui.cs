using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverGui : MonoBehaviour
{
    public static GameOverGui Instance;
    [SerializeField] TextMeshProUGUI reasonForGameOverText;
    [SerializeField] TextMeshProUGUI daysLastedText;
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
        GameplayManager.Instance.GameOver += OnGameOver;
    }

    private void OnGameOver(string reasonForGameOver, int daysLasted)
    {
        reasonForGameOverText.text = reasonForGameOver;
        daysLastedText.text = "You lasted " + daysLasted + " days";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleVisibility(bool visible)
    {
        if (visible)
        {
            transform.localPosition = new Vector3(-1000f, transform.localPosition.y, transform.localPosition.z);
            LeanTween.moveLocalX(gameObject, 0f, 1f).setEase(LeanTweenType.easeInBack);
        }
        else
        {

        }
    }

    public void OnRestartClicked()
    {
        GameplayManager.Instance.RestartGame();
    }

    public void OnReturnToMainMenuClicked()
    {
        GameplayManager.Instance.QuitGame();
    }
}
