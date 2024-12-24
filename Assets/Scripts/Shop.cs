using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    GameObject m_player;

    [SerializeField]
    TextMeshProUGUI m_shopkeeperText;


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

    public void AddHealth()
    {

    }

    public void AddJump()
    {

    }

    public void AddStangth()
    {

    }
}
