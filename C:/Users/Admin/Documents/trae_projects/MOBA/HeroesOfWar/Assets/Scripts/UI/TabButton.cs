using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TabButton : MonoBehaviour, IPointerClickHandler
{
    public Button button;
    public Text text;
    public GameObject indicator;
    
    public System.Action onClick;
    
    private void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(() => onClick?.Invoke());
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }
    
    public void SetSelected(bool selected)
    {
        if (indicator != null)
        {
            indicator.SetActive(selected);
        }
        
        if (text != null)
        {
            text.color = selected ? Color.white : Color.gray;
        }
    }
}
