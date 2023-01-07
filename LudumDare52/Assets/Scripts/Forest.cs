using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forest : MonoBehaviour, Interactable
{
    private List<Tree> trees = new List<Tree>();
    private Outline outline;

    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponent<Outline>();
        ToggleOutline(false);
        Tree[] _trees = GetComponentsInChildren<Tree>();
        trees = new List<Tree>(_trees);
        foreach (Tree tree in trees)
        {
            tree.OnTreeDestroyed += HandleTreeDestroyed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RecieveWater(float waterAmount, float waterAcidity)
    {
        foreach (Tree tree in trees)
        {
            if (waterAcidity > .5f)
            {
                tree.UpdateHealth(-waterAmount);
            }
            else if (waterAcidity < .5f)
            {
                tree.UpdateHealth(waterAmount);
            }
            else
            {
                tree.UpdateHealth(0f);
            }
        }
    }

    void HandleTreeDestroyed(Tree tree)
    {
        trees.Remove(tree);
    }

    public void ToggleOutline(bool enable)
    {
        if (enable)
        {
            outline.OutlineMode = Outline.Mode.OutlineAll;
        }
        else
        {
            outline.OutlineMode = Outline.Mode.OutlineHidden;
        }
    }
}
