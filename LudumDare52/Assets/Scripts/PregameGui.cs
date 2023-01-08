using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PregameGui : MonoBehaviour
{

    public static PregameGui Instance;


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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleVisibility(bool visible)
    {
        if (visible)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void OnStartButtonClicked()
    {
        GameplayManager.Instance.PregameFinished();
    }
}
