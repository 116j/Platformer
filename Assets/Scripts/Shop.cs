using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    GameObject m_player;

    [SerializeField]
    TextMeshProUGUI m_shopkeeperText;

    int m_maxHealth = 3;
    int m_dash = 2;
    int m_lDamage = 2;
    int m_hDamage = 2;
    bool m_jump=false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FloatingText.Instance.ShowShopText(true,transform.position);
            collision.gameObject.GetComponent<PlayerInput>().EnableShop();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FloatingText.Instance.ShowShopText(false,Vector3.zero);
            collision.gameObject.GetComponent<PlayerInput>().EnableShop();
        }
    }

    public void AddHealth(TextMeshProUGUI price)
    {
        if (m_maxHealth <= 0)
            return;
        UIController.Instance.AddHeart();
        m_player.GetComponent<Damagable>().IncreaseHealth();
        m_maxHealth--;
        if (m_maxHealth <= 0)
        {
            price.text = "SOLD";
        }
    }

    public void AddJump(TextMeshProUGUI price)
    {
        if (m_jump)
            return;

        m_player.GetComponent<PlayerController>().AddJump();
        price.text = "SOLD";
        m_jump = true;
    }

    public void AddLightDamage(TextMeshProUGUI price)
    {
        if (m_lDamage <= 0)
            return;

        m_player.transform.GetChild(0).GetComponent<AttackListener>().IncreaseDamage();
        m_lDamage--;
        if (m_lDamage <= 0)
        {
            price.text = "SOLD";
        }
    }

    public void AddHeavyDamage(TextMeshProUGUI price)
    {
        if (m_hDamage <= 0)
            return;

        m_player.transform.GetChild(1).GetComponent<AttackListener>().IncreaseDamage();
        m_hDamage--;
        if (m_hDamage <= 0)
        {
            price.text = "SOLD";
        }
    }

    public void AddDash(TextMeshProUGUI price)
    {
        if (m_dash <= 0)
            return;

        m_player.GetComponent<PlayerController>().DecreaseDashCooldown();
        m_dash--;
        if (m_dash <= 0)
        {
            price.text = "SOLD";
        }
    }
}
