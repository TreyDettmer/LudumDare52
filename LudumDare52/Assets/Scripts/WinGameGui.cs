using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinGameGui : MonoBehaviour
{
    public static WinGameGui Instance;
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

}
