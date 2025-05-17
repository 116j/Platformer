using UnityEngine;
using Zenject;

public class Shop : MonoBehaviour
{
    [Inject]
    LevelBuilder m_lvlBuilder;
    [Inject]
    FloatingCanvas m_text;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_text.ShowShopText(true, transform.position);
            collision.gameObject.GetComponent<PlayerInput>().EnableShop();
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_text.ShowShopText(false, Vector3.zero);
            collision.gameObject.GetComponent<PlayerInput>().EnableShop();
        }
    }


    private void OnDestroy()
    {
        m_lvlBuilder.ShopDestroyed();
    }
}
