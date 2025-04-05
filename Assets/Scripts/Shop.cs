using UnityEngine;

public class Shop : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FloatingText.Instance.ShowShopText(true, transform.position);
            collision.gameObject.GetComponent<PlayerInput>().EnableShop();
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FloatingText.Instance.ShowShopText(false, Vector3.zero);
            collision.gameObject.GetComponent<PlayerInput>().EnableShop();
        }
    }


    private void OnDestroy()
    {
        LevelBuilder.Instance.ShopDestroyed();
    }
}
