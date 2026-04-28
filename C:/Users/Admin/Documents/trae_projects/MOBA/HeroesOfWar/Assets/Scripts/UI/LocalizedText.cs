using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour
{
    public string localizationKey;
    public string[] formatArgs;
    
    private Text textComponent;
    
    private void Start()
    {
        textComponent = GetComponent<Text>();
        UpdateText();
        
        // 监听语言变化事件
        // 这里可以添加语言变化的事件监听
    }
    
    public void UpdateText()
    {
        if (LocalizationManager.Instance != null && textComponent != null)
        {
            if (formatArgs != null && formatArgs.Length > 0)
            {
                textComponent.text = LocalizationManager.Instance.GetLocalizedString(localizationKey, formatArgs);
            }
            else
            {
                textComponent.text = LocalizationManager.Instance.GetLocalizedString(localizationKey);
            }
        }
    }
    
    public void SetLocalizationKey(string key)
    {
        localizationKey = key;
        UpdateText();
    }
    
    public void SetFormatArgs(params string[] args)
    {
        formatArgs = args;
        UpdateText();
    }
}
