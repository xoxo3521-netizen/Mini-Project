using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Collider2D col;
    private Animator anim;

    Vector2 moveDir;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        float moveX = (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0);
        float moveY = (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0);
        moveDir = new Vector2(moveX, moveY).normalized;

        if(anim != null )
        {
            anim.SetBool("isRun", moveDir.magnitude > 0);
        }

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        float scaleX = (mouseWorldPos.x > transform.position.x) ? -1f : 1f;
        transform.localScale = new Vector3(scaleX, 1f, 1f);

        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveDir * moveSpeed;
    }
    void Attack()
    {
        if(anim != null)
        {
            anim.SetTrigger("doAttack");
        }
        Debug.Log("기본 공격");
    }
}
