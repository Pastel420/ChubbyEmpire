// PlayerInteraction.cs 修复版
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("交互设置")]
    public float interactRange = 2f;
    public LayerMask interactableLayer;

    private PlayerInputControl inputControl;  // 新输入系统
    private Iinteractable currentInteractable;

    void Awake()
    {
        inputControl = new PlayerInputControl();
    }

    void OnEnable()
    {
        inputControl.Enable();
        // 绑定交互按键（E键）
       // inputControl.Gameplay.Interact.performed += OnInteractPerformed;
    }

    void OnDisable()
    {
        //inputControl.Gameplay.Interact.performed -= OnInteractPerformed;
        inputControl.Disable();
    }

    void Update()
    {
        CheckInteractable();
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (currentInteractable != null)
        {
            currentInteractable.TriggerAction();
        }
    }

    void CheckInteractable()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactRange, interactableLayer);

        float closestDistance = float.MaxValue;
        Iinteractable closest = null;

        foreach (var col in colliders)
        {
            var interactable = col.GetComponent<Iinteractable>();
            if (interactable != null)
            {
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closest = interactable;
                }
            }
        }

        currentInteractable = closest;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}