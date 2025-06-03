using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class ScrollSelectHandler : MonoBehaviour, ISelectHandler
{
    [SerializeField]
    int ind;
    [SerializeField] 
    AutoScrollOnSelect m_autoScroller;

    [Inject]
    ShopLayout m_shop;

    public void OnSelect(BaseEventData eventData)
    {
        m_shop.ShowItemText(ind);
        m_autoScroller.OnElementSelected(GetComponent<RectTransform>());
    }
}
