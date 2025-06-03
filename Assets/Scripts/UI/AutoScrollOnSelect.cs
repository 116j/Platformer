using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class AutoScrollOnSelect : MonoBehaviour
{
    ScrollRect m_scroll;
    RectTransform m_content;
    RectTransform m_viewport;

    void Awake()
    {
        m_scroll = GetComponent<ScrollRect>();
        m_content = m_scroll.content;
        m_viewport = m_scroll.viewport;
    }

    // Подпишитесь на событие Select у каждого дочернего Selectable
    public void OnElementSelected(RectTransform selected)
    {
        Canvas.ForceUpdateCanvases();

        // мировые углы элемента и вьюпорта
        Vector3[] itemCorners = new Vector3[4];
        Vector3[] viewCorners = new Vector3[4];
        selected.GetWorldCorners(itemCorners);
        m_scroll.viewport.GetWorldCorners(viewCorners);

        float viewTop = viewCorners[1].y;
        float viewBottom = viewCorners[0].y;

        float itemTop = itemCorners[1].y;
        float itemBottom = itemCorners[0].y;

        // текущее normalized
        float norm = m_scroll.verticalNormalizedPosition;

        // если элемент «вылез» сверху — прокручиваем вверх
        if (itemTop > viewTop)
        {
            float delta = itemTop - viewTop;
            norm += delta / (m_scroll.content.rect.height - m_scroll.viewport.rect.height);
        }
        // если «вылез» снизу — прокручиваем вниз
        else if (itemBottom < viewBottom)
        {
            float delta = viewBottom - itemBottom;
            norm -= delta / (m_scroll.content.rect.height - m_scroll.viewport.rect.height);
        }

        m_scroll.verticalNormalizedPosition = Mathf.Clamp01(norm);
    }
}
