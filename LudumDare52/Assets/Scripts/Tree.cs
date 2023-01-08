using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{

    [SerializeField] private float health;
    private float baseOxygenOutput;
    public float OxygenOutput { get; private set; }

    public delegate void OnTreeDestroyedDelegate(Tree tree);
    public event OnTreeDestroyedDelegate OnTreeDestroyed;

    [SerializeField] MeshRenderer leavesRenderer;
    [SerializeField] Material leavesMaterial;
    [SerializeField] Gradient leavesColorGradient;

    private bool isDead = false;
    
    // Start is called before the first frame update
    void Start()
    {
        leavesMaterial = Instantiate(leavesRenderer.material);
        leavesRenderer.material = leavesMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealth(float change)
    {
        if (isDead) return;
        float previousHealth = health;
        health += change;
        health = Mathf.Clamp(health, 0f, 100f);
        if (health == 0)
        {
            Die();
        }
        else
        {
            leavesMaterial.color = leavesColorGradient.Evaluate(1f - health / 100f);
            OxygenOutput = baseOxygenOutput * (health / 100f);
        }
        
    }

    public float GetHealth()
    {
        return health;
    }

    private void Die()
    {
        leavesRenderer.enabled = false;
        OxygenOutput = 0f;
        isDead = true;
        OnTreeDestroyed?.Invoke(this);
    }

    public void SetBaseOxygenOutput(float baseOxygenOutput)
    {
        this.baseOxygenOutput = baseOxygenOutput;
    }

}
