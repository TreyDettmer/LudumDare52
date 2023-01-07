using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainDroplet : MonoBehaviour
{

    [SerializeField] private float fallSpeed;
    public bool IsAcidRain;

    public delegate void OnRainDropletDestroyedDelegate(RainDroplet rainDroplet);
    public event OnRainDropletDestroyedDelegate OnRainDropletDestroyed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0f, -fallSpeed, 0f) * Time.deltaTime);
        //transform.position -= new Vector3(0f, fallSpeed, 0f) * Time.deltaTime;
        if (transform.position.y < 25f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

    }

    public void OnDestroy()
    {
        OnRainDropletDestroyed?.Invoke(this);
    }
}
