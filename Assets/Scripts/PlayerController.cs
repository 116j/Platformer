using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    CinemachineVirtualCamera m_playerCam;
    [SerializeField]
    CatDetectZone m_catZone;
    [SerializeField]
    UnityEvent m_resetEvent;

    Rigidbody2D m_rb;
    PlayerInput m_input;
    BoxCollider2D m_col;
    Animator m_anim;
    TouchingCheck m_touchings;
    SoundController m_sound;

    readonly int m_HashHorizontal = Animator.StringToHash("Horizontal");
    readonly int m_HashHit = Animator.StringToHash("Hit");
    readonly int m_HashDie = Animator.StringToHash("Die");
    readonly int m_HashJump = Animator.StringToHash("Jump");
    readonly int m_HashFalling = Animator.StringToHash("Falling");
    readonly int m_HashDoubleJump = Animator.StringToHash("DoubleJump");
    readonly int m_HashDash = Animator.StringToHash("Dash");
    readonly int m_HashAnimationTime = Animator.StringToHash("AnimationTime");
    readonly int m_HashAttack = Animator.StringToHash("Attack");
    readonly int m_HashPet = Animator.StringToHash("Pet");
    readonly int m_HashHeavyAttack = Animator.StringToHash("HeavyAttack");
    readonly int m_HashCantMove = Animator.StringToHash("CantMove");

    public float m_runSpeed = 7f;
    public float m_dashPower = 8;
    public float m_jumpPower = 12f;
    public float m_fallMultiplier = 6f;
    public float m_junpMultiplier = 4f;
    public float m_jumpTime = 0.4f;
    public float m_dashCooldownTime = 1.5f;

    bool m_dead = false;
    bool m_jump = false;
    bool m_attack = false;
    bool m_dash = false;
    bool m_pet = false;
    bool m_blockMove = false;

    bool m_jumping = false;
    bool m_doubleJump = false;
    bool m_falling = false;
    bool m_canDash = true;
    bool m_isHit = false;
    bool m_canPet = false;
    bool m_onSlope = false;

    int m_currentDir = 1;
    float m_jumpCounter = 0f;
    float m_dashCooldown = 0f;
    Vector2 m_gravity;
    float m_gravityScale;
    Vector3 m_checkpoint;

    public float PlayedTime { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        m_anim = GetComponent<Animator>();
        m_input = GetComponent<PlayerInput>();
        m_rb = GetComponent<Rigidbody2D>();
        m_col = GetComponent<BoxCollider2D>();
        m_touchings = GetComponent<TouchingCheck>();
        m_sound = GetComponent<SoundController>();

        m_gravityScale = m_rb.gravityScale;
        m_gravity = new Vector2(0f, -Physics2D.gravity.y);
        m_checkpoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        m_anim.SetBool(m_HashDie, m_dead);
        if (!m_dead)
        {
            m_isHit = m_anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Hit";
            m_blockMove = m_anim.GetBool(m_HashCantMove);
            // if dash animation is over - set gravity, start falling if air dash
            if (!m_blockMove && m_dash)
            {
                if (m_jumping)
                    m_falling = true;
                m_rb.gravityScale = m_gravityScale;
                m_dash = m_canDash = false;
            }
            // when jump button is up - start falling
            if (m_jump && !m_input.Jump)
            {
                m_falling = true;
                m_jump = false;
            }

            // when attack animation is over - set back gravity 
            if (!m_blockMove && m_attack)
            {
                m_attack = false;
                m_rb.gravityScale = m_gravityScale;
                m_anim.ResetTrigger(m_HashAttack);
            }
            // dash
            if (m_input.Dash && m_canDash && !m_blockMove && !m_isHit)
            {
                m_anim.SetBool(m_HashCantMove, true);
                m_anim.SetTrigger(m_HashDash);
                UIController.Instance.SetDashSprite(0f);
                //remove garvity
                m_rb.gravityScale = 0f;
                m_dash = true;
                // moves horizontly with dash power
                if (m_jumping || m_falling)
                {
                    m_rb.velocity = new Vector2(m_rb.velocity.x, 0f);
                }
                m_rb.AddForce(Vector2.right * m_currentDir * m_dashPower, ForceMode2D.Impulse);
            }
            // dash cooldown after dash
            if (!m_canDash)
            {
                m_dashCooldown += Time.deltaTime;

                if (m_dashCooldown >= m_dashCooldownTime)
                {
                    UIController.Instance.SetDashSprite(1f);
                    m_canDash = true;
                    m_dashCooldown = 0f;
                }
                else
                {
                    UIController.Instance.SetDashSprite(m_dashCooldown / m_dashCooldownTime);
                }
            }

            if (m_input.Attack && !m_blockMove && !m_isHit)
            {
                //if is not attacking -  remove gravity(for jump), remove movement
                if (!m_blockMove)
                {
                    m_anim.SetBool(m_HashCantMove, true);
                    m_attack = true;
                    m_rb.velocity = new Vector2(0f, 0f);
                    m_rb.gravityScale = 0;
                }
                //if is attacking - go to another animation
                m_anim.SetTrigger(m_HashAttack);
            }

            if (m_input.HeavyAttack && !m_blockMove && !m_isHit && !m_jumping && !m_falling)
            {
                m_anim.SetBool(m_HashCantMove, true);
                m_attack = true;
                m_rb.velocity = new Vector2(0f, 0f);
            }

            if (m_canPet && m_input.Pet)
            {
                m_input.LockInput();
                m_catZone.ApplyPet(true);
                m_pet = true;
            }
            PlayedTime += Mathf.Abs(m_input.Move.x) < 0.1 ? 0 : Time.deltaTime;

            m_anim.SetBool(m_HashHeavyAttack, m_input.HeavyAttack);
            m_anim.SetFloat(m_HashAnimationTime, Mathf.Repeat(m_anim.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));
            m_anim.SetFloat(m_HashHorizontal, Mathf.Abs(m_rb.velocity.x));
            m_anim.SetBool(m_HashJump, m_jumping);
            m_anim.SetBool(m_HashFalling, m_falling);
        }
    }

    private void FixedUpdate()
    {
        if (!m_dead)
        {
            //
            if (m_pet)
            {
                Vector2 dir = m_catZone.TargetLocation - transform.position;
                m_rb.velocity = new Vector2(dir.normalized.x * m_runSpeed, 0f);
                if (Mathf.Abs(dir.x) <= 0.01f)
                {
                    m_pet = false;
                    m_rb.velocity = Vector2.zero;
                    m_anim.SetTrigger(m_HashPet);
                }
                return;
            }
            // turn around
            if (m_currentDir * m_input.Move.x < 0 && !m_blockMove)
            {
                m_currentDir *= -1;
                transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + m_currentDir * 180f, 0f);
            }
            // if is touching walls while jumping - don't move horizontly
            if (m_touchings.IsWalls() && (m_jumping || m_falling))
            {
                m_rb.velocity = new Vector2(0f, m_rb.velocity.y);
            }
            // if is not in attack or dash - move
            else if (!m_blockMove && !m_isHit)
            {
                if (m_touchings.IsSlopeUp())
                {
                    m_onSlope = m_rb.isKinematic = true;
                    m_rb.velocity = Vector2.LerpUnclamped(m_rb.velocity, new Vector2(m_input.Move.x * m_runSpeed, Mathf.Abs(m_input.Move.x) * m_runSpeed), 0.7f);
                }
                else if (m_touchings.IsSlopeDown())
                {
                    m_falling  = false;
                    m_onSlope = m_rb.isKinematic = true;
                    m_rb.velocity = Vector2.LerpUnclamped(m_rb.velocity, new Vector2(m_input.Move.x * m_runSpeed, -Mathf.Abs(m_input.Move.x) * m_runSpeed), 0.7f);
                }
                else
                {
                    if (m_onSlope)
                    {
                        m_onSlope = m_rb.isKinematic = false;
                    }
                    m_rb.velocity = Vector2.LerpUnclamped(m_rb.velocity, new Vector2(m_input.Move.x * m_runSpeed, m_rb.velocity.y), 0.7f);
                }
            }
            // if is not in attack or dash  - start jump
            if (!m_blockMove && m_input.Jump && !m_jump && m_touchings.IsGrounded())
            {
                if (m_onSlope)
                {
                    m_onSlope = m_rb.isKinematic = false;
                }
                m_sound.PlaySound("Jump");
                m_rb.velocity = new Vector2(m_rb.velocity.x, m_jumpPower);
                m_jump = true;
                Jump();
                return;
            }
            // if is not in attack or dash and is jumping - make double jump 
            if (!m_blockMove && m_jumping && m_input.Jump && !m_jump && !m_doubleJump)
            {
                m_sound.PlaySound("Jump");
                m_jump = m_doubleJump = true;
                m_anim.SetTrigger(m_HashDoubleJump);
                m_jumpCounter = 0f;
                m_rb.velocity = new Vector2(m_rb.velocity.x, m_jumpPower + 5f);
            }
            // if jumping and moving up - add vertical velocity for m_jumpTime or until jump button is up
            if (!m_blockMove && !m_onSlope && m_rb.velocity.y > 0f && m_jumping && !m_falling)
            {
                m_jumpCounter += Time.fixedDeltaTime;
                if (m_jumpCounter > m_jumpTime)
                {
                    m_falling = true;
                }
                m_rb.velocity += m_junpMultiplier * Time.fixedDeltaTime * m_gravity;
            }
            // if moving down - set falling and substract vertical velocity
            if (!m_blockMove && !m_onSlope && m_rb.velocity.y < 0f && !m_touchings.IsGrounded())
            {
                m_falling = true;
                m_rb.velocity -= m_fallMultiplier * Time.fixedDeltaTime * m_gravity;
            }
            //if is in air and reaches the ground and not touching wall (or touching and standing on the ground) - reset falling and jumping
            if ((m_falling && !m_touchings.IsSlopeDown()&& m_touchings.IsGrounded() && (!m_touchings.IsWalls() || Mathf.Approximately(m_rb.velocity.y, 0f)))|| (m_touchings.IsSlopeDown() || m_touchings.IsSlopeUp())&&m_jumping)
            {
                m_sound.PlaySound("Land");
                m_falling = m_jumping = m_doubleJump = false;
            }
        }
        //stop moving if is dead
        else if (m_touchings.IsGrounded())
        {
            m_rb.gravityScale = 0f;
            m_rb.velocity = Vector2.zero;
            m_col.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if player fell down - move to checkpoint and reset camera
        if (collision.gameObject.CompareTag("bounds"))
        {
            DestroyableTile.Instance.Restart(m_checkpoint);
            transform.SetPositionAndRotation(m_checkpoint, Quaternion.identity);
            m_currentDir = 1;
            m_resetEvent.Invoke();
        }
        //reset checkpoint
        else if (collision.gameObject.CompareTag("checkpoint"))
        {
            m_checkpoint = collision.transform.position;
        }
        else if (collision.gameObject.CompareTag("cat") && m_pet)
        {
            m_pet = false;
            m_rb.velocity = Vector2.zero;
            m_anim.SetTrigger(m_HashPet);
        }
    }
    /// <summary>
    /// Recieve damage 
    /// </summary>
    /// <param name="damage">damage amount</param>
    public void ReceiveDamage(int damage)
    {
        if (damage == 0)
        {
            m_rb.velocity = Vector2.zero;
            m_dead = !m_dead;
        }
        else if (damage < 0)
        {
            m_anim.SetTrigger(m_HashHit);
            m_isHit = true;
            m_rb.velocity += Vector2.left * m_currentDir;
        }
    }
    /// <summary>
    /// Start petting the cat
    /// </summary>
    /// <param name="value">if to pet</param>
    public void EnablePet(bool value)
    {
        m_canPet = value;
    }

    public void Jump()
    {
        m_jumping = true;
        m_jumpCounter = 0f;
        m_falling = false;
    }

    public void StopPetting()
    {
        m_input.LockInput();
        m_anim.ResetTrigger(m_HashPet);
        m_catZone.ApplyPet(false);
    }
}
