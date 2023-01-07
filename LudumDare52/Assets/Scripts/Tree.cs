using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{

    [SerializeField] private float health;
    [SerializeField] private float baseOxygenOutput;
    public float OxygenOutput { get; private set; }

    public delegate void OnTreeDestroyedDelegate(Tree tree);
    public event OnTreeDestroyedDelegate OnTreeDestroyed;

    [SerializeField] Material[] materials;
    MeshRenderer meshRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (materials.Length == 0) Debug.LogError("No materials assigned");
        meshRenderer.material = materials[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealth(float change)
    {
        float previousHealth = health;
        health += change;
        health = Mathf.Clamp(health, 0f, 100f);
        if (health == 0)
        {
            // TODO: spawn dead tree
            Destroy(gameObject);
        }
        else
        {
            if (health <= 75f)
            {
                if (health > 30f && previousHealth >= 75f)
                {
                    meshRenderer.material = materials[1];
                }
                else if (health > 0f && previousHealth >= 30f)
                {
                    meshRenderer.material = materials[2];
                }
            }
            else if (previousHealth < 75f)
            {
                meshRenderer.material = materials[0];
            }
            OxygenOutput = baseOxygenOutput * (health / 100f);
        }
        
    }



    private void OnDestroy()
    {
        OnTreeDestroyed?.Invoke(this);
    }

}
