using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RainCatcher : MonoBehaviour
{

    public static RainCatcher Instance;

    [SerializeField] private float movementSpeed;
    private float horizontalInput;

    private int caughtGoodRainAmount = 0;
    private int caughtAcidRainAmount = 0;

    public delegate void OnCaughtRainDelegate(int caughtGoodRainAmount, int caughtAcidRainAmount);
    public event OnCaughtRainDelegate OnCaughtRain;

    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;


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


        playerInputActions = new PlayerInputActions();
    }

    public void ToggleActionMap(bool enable)
    {
        if (enable)
        {
            playerInputActions.RainCatcher.Enable();
            Debug.Log("Enabled rain catcher");
        }
        else
        {
            playerInputActions.RainCatcher.Disable();
            Debug.Log("Disabled rain catcher");
        }
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameplayManager.Instance.SwitchedToWaterDistributor += OnSwitchedToWaterDistributor;

    }

    private void OnSwitchedToWaterDistributor(object sender, EventArgs e)
    {
        caughtAcidRainAmount = 0;
        caughtGoodRainAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
        Move();
    }

    void Move()
    {
        transform.Translate(new Vector3(horizontalInput, 0f, 0f) * Time.deltaTime * movementSpeed);
        if (transform.position.x > 8f)
        {
            transform.position = new Vector3(8f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -8f)
        {
            transform.position = new Vector3(-8f, transform.position.y, transform.position.z);
        }
    }
    

    public void HandleMovementInput()
    {
        Vector2 movement = playerInputActions.RainCatcher.Move.ReadValue<Vector2>();
        horizontalInput = movement.x;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.GetComponent<RainDroplet>())
        {
            collision.gameObject.GetComponent<RainDroplet>().SpawnImpactParticles();
        }
        Destroy(collision.gameObject);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<RainDroplet>())
        {
            if (other.gameObject.GetComponent<RainDroplet>().IsAcidRain)
            {
                caughtAcidRainAmount += 1;
                AudioManager.Instance.Play("RainDroplet");
                
            }
            else
            {
                caughtGoodRainAmount += 1;
                AudioManager.Instance.Play("RainDroplet");
            }
            OnCaughtRain?.Invoke(caughtGoodRainAmount, caughtAcidRainAmount);
        }
        Debug.Log("Caught rain");

        Destroy(other.gameObject);
    }

    public int GetCaughtGoodRainAmount()
    {
        return caughtGoodRainAmount;
    }

    public int GetCaughtAcidRainAmount()
    {
        return caughtAcidRainAmount;
    }
}
