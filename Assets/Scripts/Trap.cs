using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField]
    AnimationClip[] m_transitions;
    [SerializeField]
    AnimationClip[] m_attacks;
    [SerializeField]
    Vector3[] m_offsets;
    [SerializeField]
    Vector3[] m_metrics;
    [SerializeField]
    Vector3[] m_attackDirections;
    [SerializeField]
    int m_trapNumber;
    [SerializeField]
    AnimationCurve[] m_spawnChances;

    protected Animator m_anim;

    Vector3 m_currentOffset;

    // Start is called before the first frame update
    void Awake()
    {
        m_anim = GetComponent<Animator>();

        SetTrap(m_trapNumber);
    }

    /// <summary>
    /// Set animatioms for trap override controller
    /// </summary>
    /// <param name="animNum"></param>
    void SetAnimations(int animNum)
    {
        AnimatorOverrideController aoc = new(m_anim.runtimeAnimatorController);
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>(2)
        {
            new(aoc.animationClips[0], m_attacks[animNum]),
            new(aoc.animationClips[1], m_transitions[animNum])
        };
        aoc.ApplyOverrides(anims);
        m_anim.runtimeAnimatorController = aoc;
    }

    int SetTrapNum()
    {
        List<float> chances = new List<float>();
        foreach (var spawnChance in m_spawnChances)
        {
            chances.Add(spawnChance.Evaluate(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().PlayedTime));
        }

        float value = Random.Range(0, chances.Sum());
        float sum = 0;
        for (int i = 0; i < chances.Count; i++)
        {
            sum += chances[i];
            if (sum < value)
            {
                return i;
            }
        }
        return chances.Count - 1;       
    }

    public void SetTrap(int trapNum)
    {
        transform.position -= m_currentOffset;
        m_currentOffset = m_offsets[trapNum];
        transform.position += m_currentOffset;
        SetAnimations(trapNum);
    }

    public void DestroyTrap()
    {
        Destroy(gameObject);
    }
}
