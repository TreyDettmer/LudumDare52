using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreesManager : MonoBehaviour
{

    [SerializeField] private float decayDelay = 1f;
    [SerializeField] private float decayAmount = 1f;
    [SerializeField] private float oxygenOutputDelay = 1f;

    private List<Tree> trees = new List<Tree>();
    private float lastTimeOfOxygenOutput;
    private float lastTimeOfDecay;

    // Start is called before the first frame update
    void Start()
    {
        Tree[] _trees = FindObjectsOfType<Tree>();
        trees = new List<Tree>(_trees);
        foreach (Tree tree in trees)
        {
            tree.OnTreeDestroyed += HandleTreeDestroyed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameplayManager.Instance.CurrentGameplayState == GameplayManager.GameplayState.DISTRIBUTING_WATER)
        {
            if (Time.time - lastTimeOfOxygenOutput >= oxygenOutputDelay)
            {
                float oxygenOutput = 0f;
                foreach (Tree tree in trees)
                {
                    oxygenOutput += tree.OxygenOutput;
                }
                GameplayManager.Instance.UpdateTotalOxygen(oxygenOutput);
                lastTimeOfOxygenOutput = Time.time;
            }
            if (Time.time - lastTimeOfDecay >= decayDelay)
            {
                foreach (Tree tree in trees)
                {
                    tree.UpdateHealth(-decayAmount);
                }
                lastTimeOfDecay = Time.time;
            }
        }
    }


    void HandleTreeDestroyed(Tree tree)
    {
        trees.Remove(tree);
    }
}
