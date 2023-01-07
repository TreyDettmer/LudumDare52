using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainManager : MonoBehaviour
{

    struct Shower
    {
        public int goodRainDropletCount;
        public int acidRainDropletCount;

        public Shower(int goodRainDropletCount, int acidRainDropletCount)
        {
            this.goodRainDropletCount = goodRainDropletCount;
            this.acidRainDropletCount = acidRainDropletCount;
        }
    }

    public event EventHandler ShowerFinished;

    [SerializeField] private float skyheight;
    [SerializeField] private GameObject goodRainDropletPrefab;
    [SerializeField] private GameObject acidRainDropletPrefab;

    private Shower currentShower = new Shower(20, 12);

    private List<RainDroplet> rainDroplets = new List<RainDroplet>();
    

    // Start is called before the first frame update
    void Start()
    {
        GameplayManager.Instance.StartGame += HandleStartGame;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartRaining()
    {
        int goodRainDropletCount = currentShower.goodRainDropletCount;
        int acidRainDropletCount = currentShower.acidRainDropletCount;
        while (goodRainDropletCount > 0 || acidRainDropletCount > 0)
        {
            if (goodRainDropletCount > 0 && acidRainDropletCount > 0)
            {
                bool shouldSpawnRainDroplet = UnityEngine.Random.value > .5f ? true : false;
                if (shouldSpawnRainDroplet)
                {
                    SpawnRainDroplet();
                    goodRainDropletCount--;
                }
                else
                {
                    SpawnRainDroplet(true);
                    acidRainDropletCount--;
                }
            }
            else if (goodRainDropletCount > 0)
            {
                SpawnRainDroplet();
                goodRainDropletCount--;
            }
            else
            {
                SpawnRainDroplet(true);
                acidRainDropletCount--;
            }
            yield return new WaitForSeconds(UnityEngine.Random.Range(.1f,0.8f));
        }
    }

    void SpawnRainDroplet(bool isAcidRain = false)
    {
        float randomXPosition = UnityEngine.Random.Range(-6f, 6f);
        float randomYPosition = UnityEngine.Random.Range(skyheight, skyheight+3f);
        RainDroplet instantiatedDroplet = Instantiate(isAcidRain ? acidRainDropletPrefab :  goodRainDropletPrefab, new Vector3(randomXPosition, randomYPosition, 0f), Quaternion.identity, transform).GetComponent<RainDroplet>();
        rainDroplets.Add(instantiatedDroplet);
        instantiatedDroplet.OnRainDropletDestroyed += HandleRainDropletDestroyed;
    }

    void StopRaining()
    {

    }

    void HandleStartGame(object sender, EventArgs eventArgs)
    {
        StartCoroutine(StartRaining());
    }

    void HandleRainDropletDestroyed(RainDroplet rainDroplet)
    {
        rainDroplets.Remove(rainDroplet);
        if (rainDroplets.Count <= 0)
        {
            ShowerFinished?.Invoke(this, EventArgs.Empty);
        }
    }
}
