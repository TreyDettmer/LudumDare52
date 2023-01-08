using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forest : MonoBehaviour, Interactable
{
    private List<Tree> trees = new List<Tree>();
    private Outline outline;
    [SerializeField] Transform rainPositionTransform;
    public float Health { get; set; }
    public Vector3 RainPosition { get; set; }
    
    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponent<Outline>();
        RainPosition = rainPositionTransform.position;
        ToggleOutline(false);
        Tree[] _trees = GetComponentsInChildren<Tree>();
        trees = new List<Tree>(_trees);
        float totalHealth = 0f;
        foreach (Tree tree in trees)
        {
            tree.OnTreeDestroyed += HandleTreeDestroyed;
            totalHealth += tree.GetHealth();
        }
        Health = totalHealth / trees.Count;
    }

    // Update is called once per frame
    void Update()
    {
        float totalHealth = 0f;
        foreach (Tree tree in trees)
        {
            totalHealth += tree.GetHealth();
        }
        Health = totalHealth / trees.Count;
    }

    public void ReceiveWater(float waterAmount, float waterAcidity)
    {
        foreach (Tree tree in trees)
        {
            if (waterAcidity > .5f)
            {
                float acidityFactor = (waterAcidity - .5f) / .5f;
                tree.UpdateHealth(-acidityFactor * waterAmount);
            }
            else if (waterAcidity < .5f)
            {
                // .5 = 0
                // .25 = .5f
                // 0 = 1f

                float acidityFactor = 1 - (2f * waterAcidity);
                tree.UpdateHealth(acidityFactor * waterAmount);
            }
            else
            {
                tree.UpdateHealth(0f);
            }
        }
    }

    void HandleTreeDestroyed(Tree tree)
    {
        //trees.Remove(tree);
    }

    public void ToggleOutline(bool enable)
    {
        if (enable)
        {
            LeanTween.moveLocalY(gameObject, 1f, .3f);
        }
        else
        {
            LeanTween.moveLocalY(gameObject, -1f, .3f);
        }
        

    }
}
