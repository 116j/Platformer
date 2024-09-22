using UnityEngine;
using UnityEngine.Events;

public class Cat : MonoBehaviour
{
    [SerializeField]
    DetectZone m_groundZone;

    public Transform PetPlayerLocation;
    public bool CanPet { get; private set; } = true;

    Animator m_anim;
    TouchingCheck m_touchings;
    Rigidbody2D m_rb;
    readonly UnityEvent<int> m_addHeart = new();

    readonly int m_HashWalk = Animator.StringToHash("Walk");
    readonly int m_HashSleep = Animator.StringToHash("Sleep");
    readonly int m_HashCanMove = Animator.StringToHash("CanMove");
    readonly int[] m_triggers = new int[]
    {
      Animator.StringToHash("Turn"),
      Animator.StringToHash("Jump"),
      Animator.StringToHash("CleanFace"),
      Animator.StringToHash("Lick")
    };

    readonly float m_walkRecoverTimeMax = 10f;
    readonly float m_walkRecoverTimeMin = 5f;
    readonly float m_triggerRecoverTime = 2f;
    readonly float m_walkSpeed = 2f;

    bool m_petting = false;
    bool m_walking = false;
    bool m_triggered = false;

    float m_walkTimer;
    float m_triggerTimer;
    int m_currentDir = 1;
    float m_speed;
    float m_walkTime;
    // Start is called before the first frame update
    void Start()
    {
        m_anim = GetComponent<Animator>();
        m_touchings = GetComponent<TouchingCheck>();
        m_rb = GetComponent<Rigidbody2D>();
        m_walkTime = Random.Range(m_walkRecoverTimeMin, m_walkRecoverTimeMax);
        m_addHeart.AddListener(GameObject.FindGameObjectWithTag("Player").GetComponent<Damagable>().ApplyHealth);
    }

    // Update is called once per frame
    void Update()
    {
        // stop and turn around if cant go further
        if (!m_groundZone.TargetDetected || m_touchings.IsWalls())
        {
            m_walkTimer = 0f;
            m_walking = false;
            m_speed = 0f;
            TurnAround();
        }
        //if is not petting - walk
        if (!m_petting && CanPet)
            Walk();
        m_anim.SetBool(m_HashWalk, m_walking && !m_petting);
        // is not triggered - trigger
        if (!m_petting && !m_triggered)
        {
            m_anim.SetTrigger(m_triggers[Random.Range(0, m_triggers.Length)]);
            m_triggered = true;
        }
        // if triggered - set trigger cooldown
        if (m_triggered)
        {
            m_triggerTimer += Time.deltaTime;
            if (m_triggerTimer >= m_triggerRecoverTime)
            {
                m_triggerTimer = 0f;
                m_triggered = false;
            }
        }
    }

    private void FixedUpdate()
    {
        m_rb.velocity = transform.right * (m_anim.GetBool(m_HashCanMove) ? m_speed : 0f);
    }

    /// <summary>
    /// Walk for walktime, than stop
    /// </summary>
    void Walk()
    {
        m_walkTimer += Time.deltaTime;
        if (m_walkTimer >= m_walkTime)
        {
            m_walkTime = Random.Range(m_walkRecoverTimeMin, m_walkRecoverTimeMax);
            m_walking = !m_walking;
            m_walkTimer = 0;
            m_speed = m_walking ? m_walkSpeed : 0f;
        }
    }

    public void TurnAround()
    {
        m_currentDir *= -1;
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + m_currentDir * 180f, 0f);
    }
    /// <summary>
    /// Stop when start pettng and sleep when end
    /// </summary>
    /// <param name="pet"></param>
    public void Pet(bool pet)
    {
        if (pet)
        {
            m_speed = 0f;
            m_petting = true;
            m_walking = false;
        }
        else
        {
            m_addHeart.Invoke(1);
            m_anim.SetBool(m_HashSleep, true);
            CanPet = false;
        }
    }
}
