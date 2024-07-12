using System.Collections.Generic;
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
    int m_trapNumber;

    Animator m_anim;

    Vector3 m_currentOffset;

    // Start is called before the first frame update
    void Start()
    {
        m_anim = GetComponent<Animator>();
        m_currentOffset = m_offsets[m_trapNumber];
        transform.position += m_offsets[m_trapNumber];

        SetAnimations(m_trapNumber);
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

    public void SetTrap(int trapNum)
    {
        transform.position -= m_currentOffset;
        transform.position += m_offsets[trapNum];
        SetAnimations(trapNum);
    }
}
