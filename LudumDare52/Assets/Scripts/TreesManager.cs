using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreesManager : MonoBehaviour
{

    [SerializeField] private float treeDecayDelay = 1f;
    [SerializeField] private float treeOxygenOutputDelay = 1f;
    [SerializeField] private float treeBaseOxygenOutput;

    private List<Tree> trees = new List<Tree>();
    private float lastTimeOfOxygenOutput;
    private float lastTimeOfDecay;
    private bool isRunning = true;
    // Start is called before the first frame update
    void Start()
    {
        GameplayManager.Instance.GameOver += OnGameOver;
        Tree[] _trees = FindObjectsOfType<Tree>();
        trees = new List<Tree>(_trees);
        foreach (Tree tree in trees)
        {
            tree.OnTreeDestroyed += HandleTreeDestroyed;
            tree.SetBaseOxygenOutput(treeBaseOxygenOutput);
        }
    }

    private void OnGameOver(string reasonForGameOver, int daysLasted)
    {
        isRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRunning) return;
        if (GameplayManager.Instance.CurrentGameplayState == GameplayManager.GameplayState.DISTRIBUTING_WATER)
        {
            if (Time.time - lastTimeOfOxygenOutput >= treeOxygenOutputDelay)
            {
                float oxygenOutput = 0f;
                foreach (Tree tree in trees)
                {
                    oxygenOutput += tree.OxygenOutput;
                }
                GameplayManager.Instance.UpdateTotalOxygen(oxygenOutput);
                lastTimeOfOxygenOutput = Time.time;
            }
            if (Time.time - lastTimeOfDecay >= treeDecayDelay)
            {
                foreach (Tree tree in trees)
                {
                    tree.UpdateHealth(-1f);
                }
                lastTimeOfDecay = Time.time;
            }
        }
    }


    void HandleTreeDestroyed(Tree tree)
    {
        //trees.Remove(tree);
    }
}
