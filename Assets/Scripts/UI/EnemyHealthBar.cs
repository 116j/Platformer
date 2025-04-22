using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public static EnemyHealthBar Instance { get; private set; }
    [SerializeField]
    Image m_healthBar;
    [SerializeField]
    Sprite[] m_healthSprites;


    Vector3 m_offset = new(1.8f, 0);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    public void ShowBar(Transform enemy)
    {
        transform.SetParent(enemy);
        transform.localPosition = m_offset;
        SetHealthSprite(1);
        m_healthBar.gameObject.SetActive(true);
    }

    public void HideBar()
    {
        transform.SetParent(null);
        m_healthBar.gameObject.SetActive(false);
        ChangeRotation(1);
    }

    public void SetHealthSprite(float fill)
    {
        m_healthBar.sprite = m_healthSprites[Mathf.FloorToInt(fill * (m_healthSprites.Length - 1))];
    }


    public void ChangeRotation(float dir)
    {
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + dir * 180f, 0f);
    }

}
