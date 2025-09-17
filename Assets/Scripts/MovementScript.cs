
using System.Collections;
using UnityEngine;
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Collider2D))]
    public class MovementScript : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 5.0f;
        [SerializeField] float dashSpeed = 20f;
        [SerializeField] float dashDuration = 0.2f;
        [SerializeField] private SpriteRenderer spriteRenderer;

        //[Header("Physics Settings")]
        //[SerializeField] float linearDrag = 2f;

        private Rigidbody2D rb;
        private bool isDashing;
        private float dashTimer;
        private Vector2 moveInput;
        private Vector2 lastMoveDir = Vector2.up;
        private Vector2 dashDir;
        private float dragCache;
        private float originalMoveSpeed;
        private Coroutine speedBuffCoroutine;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        originalMoveSpeed = moveSpeed;
       
        }

        private void Start()
        {
            rb.linearDamping = 2f;
            rb.gravityScale = 0;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        }

        void Update()
        {
            Vector2 raw  = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            moveInput = raw.normalized;

            if (raw.sqrMagnitude > 0.01f)
                lastMoveDir = moveInput;


            if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
            {
                isDashing = true;
                dashTimer = dashDuration;
                dashDir = (lastMoveDir.sqrMagnitude > 0f) ? lastMoveDir : Vector2.up;

                dragCache = rb.linearDamping;
                rb.linearDamping = 0f;

            }
        }
        private void FixedUpdate()
        {
            if (isDashing)
            {
                rb.linearVelocity = dashDir * dashSpeed;
                dashTimer -= Time.fixedDeltaTime;

                if (dashTimer <= 0f)
                {   
                    isDashing = false;
                    rb.linearDamping = dragCache;
                }
            }
            else
            {
                rb.linearVelocity = moveInput * moveSpeed;
            }
        }   
        public void ApplyBiscuitBuff(float multiplier , float duration)
    {
        if (speedBuffCoroutine != null)
            StopCoroutine(speedBuffCoroutine);

        speedBuffCoroutine = StartCoroutine(SpeedBuffRoutine(multiplier, duration));
    }

        private IEnumerator SpeedBuffRoutine(float multiplier , float duration)
    {
        moveSpeed = originalMoveSpeed * multiplier;
        yield return new WaitForSeconds(duration);
        moveSpeed = originalMoveSpeed;
    }
    }
