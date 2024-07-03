using Cinemachine;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, IBoolReceiver
{
    [SerializeField]
    ContactFilter2D m_groundCastFilter;
    [SerializeField]
    ContactFilter2D m_wallCastFilter;
    [SerializeField]
    CinemachineVirtualCamera m_playerCam;

    Rigidbody2D m_rb;
    PlayerInput m_input;
    BoxCollider2D m_col;
    Animator m_anim;

    readonly int m_HashHorizontal = Animator.StringToHash("Horizontal");
    readonly int m_HashHit = Animator.StringToHash("Hit");
    readonly int m_HashDie = Animator.StringToHash("Die");
    readonly int m_HashJump = Animator.StringToHash("Jump");
    readonly int m_HashFalling = Animator.StringToHash("Falling");
    readonly int m_HashDoubleJump = Animator.StringToHash("DoubleJump");
    readonly int m_HashDash = Animator.StringToHash("Dash");
    readonly int m_HashAnimationTime = Animator.StringToHash("AnimationTime");
    readonly int m_HashAttack = Animator.StringToHash("Attack");
    readonly int m_HashAttacking = Animator.StringToHash("Attacking");
    readonly int m_HashDashing = Animator.StringToHash("Dashing");
    readonly int m_HashHeavyAttack = Animator.StringToHash("HeavyAttack");


    public float m_runSpeed = 7f;
    public float m_dashPower = 8;
    public float m_jumpPower = 12f;
    public float m_fallMultiplier = 6f;
    public float m_junpMultiplier = 4f;
    public float m_jumpTime = 0.4f;
    readonly float m_dashCooldown = 2f;
    //distance to detect wall
    readonly float m_wallHitDist = 0.2f;
    //distance to detect ground
    readonly float m_groundHitDist = 0.05f;

    bool m_dead = false;
    bool m_jump = false;

    bool m_dashing = false;
    bool m_jumping = false;
    bool m_doubleJump = false;
    bool m_falling = false;
    bool m_attacking = false;
    bool m_canDash = true;

    int m_currentDir = 1;
    float m_jumpCounter = 0f;
    float m_dashTimer = 0f;
    Vector2 m_gravity;
    float m_gravityScale;
    Vector3 m_checkpoint;
    CinemachineFramingTransposer m_transposer;

    RaycastHit2D[] m_colHits = new RaycastHit2D[5];
    // Start is called before the first frame update
    void Start()
    {
        m_anim = GetComponent<Animator>();
        m_input = GetComponent<PlayerInput>();
        m_rb = GetComponent<Rigidbody2D>();
        m_col = GetComponent<BoxCollider2D>();

        m_gravityScale = m_rb.gravityScale;
        m_gravity = new Vector2(0f, -Physics2D.gravity.y);
        m_checkpoint = transform.position;
        m_transposer = m_playerCam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    // Update is called once per frame
    void Update()
    {
        m_anim.SetBool(m_HashDie,m_dead);
        if (!m_dead)
        {
            m_anim.ResetTrigger(m_HashAttack);
            m_anim.ResetTrigger(m_HashDash);

            if (m_input.Dash&&m_canDash&&!m_attacking)
            {
                Debug.Log("Start dashing");
                m_anim.SetFloat(m_HashHorizontal, 0f);

                m_anim.SetTrigger(m_HashDash);
                m_anim.SetBool(m_HashDashing, true);
                //remove garvity
                m_rb.gravityScale = 0f;
                m_dashing = true;
                // moves horizontly with dash power
                m_rb.AddForce(Vector2.right*m_currentDir * m_dashPower,ForceMode2D.Impulse);
            }

            if (!m_canDash)
            {
                m_dashTimer +=Time.deltaTime;
                if(m_dashTimer >= m_dashCooldown)
                {
                    m_canDash = true;
                    m_dashTimer = 0f;
                }
            }

            if (m_input.Attack&&!m_dashing)
            {
                //if is not attacking -  remove gravity(for jump), remove movement
                if (!m_attacking)
                {
                    Debug.Log("Start attacking");
                    m_anim.SetFloat(m_HashHorizontal, 0f);

                    m_anim.SetBool(m_HashAttacking, true);
                    m_attacking = true;
                    m_rb.velocity = new Vector2(0f, 0f);
                    m_rb.gravityScale = 0;
                }
                //if is attacking - go to another animation
                m_anim.SetTrigger(m_HashAttack);
            }

            if (m_input.HeavyAttack&&!m_attacking)
            {
                m_anim.SetBool(m_HashAttacking, true);
                m_attacking = true;
                m_rb.velocity = new Vector2(0f, 0f);
            }

            // if dash animation is over - set gravity, start falling if air dash
            if (m_dashing && !m_anim.GetBool(m_HashDashing))
            {
                Debug.Log("Stop dashing");

                if (m_jumping)
                    m_falling = true;
                m_rb.gravityScale = m_gravityScale;
                m_dashing = false;
                m_canDash = false;
            }
            // when jump button is up - start falling
            if (m_jump && !m_input.Jump)
            {
                m_falling = true;
                m_jump = false;
            }
            // when attack animation is over - set back gravity 
            if (m_attacking && !m_anim.GetBool(m_HashAttacking))
            {
                Debug.Log("Stop attacking");

                m_rb.gravityScale = m_gravityScale;
                m_attacking = false;
            }
            m_anim.SetBool(m_HashHeavyAttack, m_input.HeavyAttack);
            m_anim.SetFloat(m_HashAnimationTime, Mathf.Repeat(m_anim.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));
            m_anim.SetFloat(m_HashHorizontal, Mathf.Abs(m_input.Move.x));
            m_anim.SetBool(m_HashJump, m_jumping);
            m_anim.SetBool(m_HashFalling, m_falling);
        }
    }


    private void FixedUpdate()
    {
        if (!m_dead)
        {
            // turn around
            if (m_currentDir * m_input.Move.x < 0)
            {
                m_currentDir *= -1;
                transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + m_currentDir * 180f, 0f);
            }
            if (IsWalls() && (m_jumping || m_falling))
            {
                m_rb.velocity = new Vector2(0f, m_rb.velocity.y);
            }
            // if is not in attack or dash - move
            else if (!m_dashing && !m_attacking)
            {
                m_rb.velocity = new Vector2(m_input.Move.x * m_runSpeed, m_rb.velocity.y);
            }
            // if is not in attack or dash  - start jump
            if (!m_dashing && !m_attacking && m_input.Jump && !m_jump && IsGrounded())
            {
                m_jump = true;
                m_jumping = true;
                m_jumpCounter = 0f;
                m_falling = false;
                m_rb.velocity = new Vector2(m_rb.velocity.x, m_jumpPower);
                return;
            }
            // if is not in attack or dash and is jumping - make double jump 
            if (!m_dashing && !m_attacking && m_jumping && m_input.Jump && !m_jump && !m_doubleJump)
            {
                m_jump = true;
                m_doubleJump = true;
                m_anim.SetTrigger(m_HashDoubleJump);
                m_rb.velocity = new Vector2(m_rb.velocity.x, m_jumpPower + 5f);
            }
            // if jumping and moving up - add vertical velocity for m_jumpTime or until jump button is up
            if (!m_dashing && !m_attacking && m_rb.velocity.y > 0f && m_jumping && !m_falling)
            {
                m_jumpCounter += Time.fixedDeltaTime;
                if (m_jumpCounter > m_jumpTime)
                {
                    m_falling = true;
                }
                m_rb.velocity += m_junpMultiplier * Time.fixedDeltaTime * m_gravity;
            }
            // if moving down - set falling and substract vertical velocity
            if (!m_dashing && !m_attacking && m_rb.velocity.y < 0f)
            {
                m_falling = true;
                m_rb.velocity -= m_fallMultiplier * Time.fixedDeltaTime * m_gravity;
            }
            //if is in air and reaches the ground and not touching wall (or touching and standing on the ground) - reset falling and jumping
            if ((m_jumping || m_falling) && IsGrounded() && (!IsWalls()|| Mathf.Approximately(m_rb.velocity.y,0f)))
            {
                m_falling = false;
                m_jumping = false;
                m_doubleJump = false;
            }
        }
    }

    bool IsGrounded()
    {
        return m_col.Cast(-transform.up, m_groundCastFilter, m_colHits, m_groundHitDist) > 0;
    }

    bool IsWalls()
    {
        return m_col.Cast(transform.right, m_wallCastFilter, m_colHits, m_wallHitDist) > 0;
    }

    IEnumerator Reload()
    {
        m_anim.SetTrigger(m_HashHit);
        m_rb.velocity += Vector2.left*0.1f * m_currentDir;
        // if player fell down - reset camera
            yield return new WaitForSeconds(1f);
            m_transposer.m_DeadZoneWidth = 0.083f;
            m_transposer.m_DeadZoneHeight = 0.32f;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if player fell down - move to checkpoint and reset camera
        if (collision.gameObject.CompareTag("bounds"))
        {
            Debug.Log("Bounds");
            transform.SetPositionAndRotation(m_checkpoint, Quaternion.identity);
            m_transposer.m_DeadZoneWidth = 0f;
            m_transposer.m_DeadZoneHeight = 0f;
            m_currentDir = 1;
            StartCoroutine(Reload());
        }
        //reset checkpoint
        else if (collision.gameObject.CompareTag("checkpoint"))
        {
            m_checkpoint = collision.transform.position;
        }
    }

    public void ReceiveBool(bool value)
    {
        if (value)
        {
            m_dead = true;
        }
        else
        {
            m_anim.SetTrigger(m_HashHit);
            m_rb.velocity += Vector2.left * 0.1f * m_currentDir;
        }
    }
}
