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

    private List<Shower> forecastedShowers = new List<Shower>();

    private List<RainDroplet> rainDroplets = new List<RainDroplet>();
    private int maximumShowersDay = 10;

    // Start is called before the first frame update
    void Start()
    {
        GameplayManager.Instance.StartGame += HandleStartGame;
        GameplayManager.Instance.StartRaining += HandleStartGame;
        GenerateFutureShowerForecast();
    }

    void GenerateFutureShowerForecast()
    {
        int currentDay = GameplayManager.Instance.CurrentDay;
        for (int i = 0; i < 4; i++)
        {
            int minimumGoodRainDrops = (int) Mathf.Lerp(90f, 320f, (float)currentDay / maximumShowersDay);
            forecastedShowers.Add(new Shower(UnityEngine.Random.Range(minimumGoodRainDrops, minimumGoodRainDrops+50), UnityEngine.Random.Range((int)(minimumGoodRainDrops/1.3f), (int)((minimumGoodRainDrops+50)/ 1.3f))));
            currentDay++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartRaining()
    {
        yield return new WaitForSeconds(1f);
        int goodRainDropletCount = forecastedShowers[0].goodRainDropletCount;
        int acidRainDropletCount = forecastedShowers[0].acidRainDropletCount;
        List<int> dropletArray = new List<int>();
        for (int i = 0; i < goodRainDropletCount; i++)
        {
            dropletArray.Add(0);
        }
        for (int i = 0; i < acidRainDropletCount; i++)
        {
            dropletArray.Add(1);
        }
        while (dropletArray.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, dropletArray.Count);
            if (dropletArray[randomIndex] == 0)
            {
                SpawnRainDroplet();
                
            }
            else
            {
                SpawnRainDroplet(true);
            }
            dropletArray.RemoveAt(randomIndex);
            yield return new WaitForSeconds(UnityEngine.Random.Range(.05f,0.15f));
        }
    }

    void SpawnRainDroplet(bool isAcidRain = false)
    {
        float randomXPosition = UnityEngine.Random.Range(-7f, 7f);
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
            forecastedShowers.RemoveAt(0);
            if (forecastedShowers.Count < 3)
            {
                GenerateFutureShowerForecast();
            }
          
        }
    }
}
