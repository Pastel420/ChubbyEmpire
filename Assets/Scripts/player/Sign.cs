using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sign : MonoBehaviour
{
    private PlayerInputControl playerInput;
    private Animator anim;
    public GameObject signSprite;
    private Iinteractable targetItem;
    public Transform playerTrans;
    private bool canPress;

    private void Awake()
    {
        anim = signSprite.GetComponent<Animator>();
        playerInput = new PlayerInputControl();
        playerInput.Enable();
    }

    private void OnEnable()
    {
        anim.Play("keyboard");
        playerInput.Gameplay.Confirm.started += OnConfirm;
    }

 

    private void Update()
    {
        signSprite.SetActive(canPress);
        signSprite.transform.localScale = playerTrans.localScale;
    }
   private void OnConfirm(InputAction.CallbackContext context)
    {
        if (canPress)
        {
            targetItem.TriggerAction();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            canPress = true;
            targetItem = other.GetComponent<Iinteractable>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        canPress = false;
    }
}
