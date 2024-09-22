using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    [SerializeField]
    Transform m_parent;
    [SerializeField]
    Text m_text;

    bool m_showText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_showText)
        {
            transform.position = m_parent.position + Vector3.up * 2f;
        }
    }

    public void ShowText(bool show)
    {
        m_text.gameObject.SetActive(show);
        m_showText = show;
    }
}
