using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WaterDistributor : MonoBehaviour
{
    public static WaterDistributor Instance;

    private PlayerInputActions playerInputActions;

    private Vector2 cursorPosition;
    [SerializeField] private LayerMask interactablesLayerMask;


    public delegate void OnInteractionDelegate(GameObject interactedObject);
    public event OnInteractionDelegate OnInteraction;
    public delegate void OnDistributedWaterDelegate(GameObject interactedObject,float waterAmount);
    public event OnDistributedWaterDelegate OnDistributedWater;
    private GameObject interactedObject = null;
    private Interactable highlightedInteractable;

    public float AvailableWater { get; private set; }
    public float AcidityOfAvailableWater { get; private set; }

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

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }

    public void ToggleActionMap(bool enable)
    {
        if (enable)
        {
            playerInputActions.WaterDistributor.Enable();
        }
        else
        {
            playerInputActions.WaterDistributor.Disable();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        playerInputActions.WaterDistributor.Interact.performed += _ => OnInteract();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerInputActions.WaterDistributor.enabled) return;


        cursorPosition = playerInputActions.WaterDistributor.Position.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(cursorPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, interactablesLayerMask))
        {
            highlightedInteractable = hit.collider.gameObject.GetComponent<Interactable>();
            highlightedInteractable.ToggleOutline(true);
        }
        else if (highlightedInteractable != null)
        {
            highlightedInteractable.ToggleOutline(false);
            highlightedInteractable = null;
        }

    }


    void OnInteract()
    {
        Ray ray = Camera.main.ScreenPointToRay(cursorPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, interactablesLayerMask))
        {
            Debug.Log("Clicked on " + hit.collider.gameObject.name);
            interactedObject = hit.collider.gameObject;
            OnInteraction?.Invoke(hit.collider.gameObject);
        }

    }

    public void SetAvailableWaterAndAcidity(float availableWater,float acidityOfAvailableWater)
    {
        AvailableWater = availableWater;
        AcidityOfAvailableWater = acidityOfAvailableWater;
    }

    public bool AttemptToDistributeWater(float waterAmount)
    {
        if (waterAmount <= AvailableWater)
        {
            OnDistributedWater?.Invoke(interactedObject, waterAmount);
            interactedObject = null;
            AvailableWater -= waterAmount;
            return true;
        }
        return false;
    }
}
