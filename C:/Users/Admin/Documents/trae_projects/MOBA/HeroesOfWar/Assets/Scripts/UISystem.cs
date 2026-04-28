[System.Serializable]
public class UIWindow
{
    public string windowID;
    public string windowName;
    public string windowPrefab;
    public bool isModal;
    public bool isPersistent;
    public bool isHidden;
    public List<UIElement> elements;
    
    public UIWindow(string id, string name, string prefab, bool modal = false, bool persistent = false)
    {
        windowID = id;
        windowName = name;
        windowPrefab = prefab;
        isModal = modal;
        isPersistent = persistent;
        isHidden = true;
        elements = new List<UIElement>();
    }
    
    public void AddElement(UIElement element)
    {
        elements.Add(element);
    }
    
    public UIElement GetElement(string elementID)
    {
        return elements.Find(e => e.elementID == elementID);
    }
    
    public void Show()
    {
        isHidden = false;
    }
    
    public void Hide()
    {
        isHidden = true;
    }
}

[System.Serializable]
public class UIElement
{
    public string elementID;
    public string elementName;
    public string elementType;
    public string elementValue;
    public bool isEnabled;
    public bool isVisible;
    
    public UIElement(string id, string name, string type, string value = "")
    {
        elementID = id;
        elementName = name;
        elementType = type;
        elementValue = value;
        isEnabled = true;
        isVisible = true;
    }
    
    public void Enable()
    {
        isEnabled = true;
    }
    
    public void Disable()
    {
        isEnabled = false;
    }
    
    public void Show()
    {
        isVisible = true;
    }
    
    public void Hide()
    {
        isVisible = false;
    }
}

[System.Serializable]
public class UIManagerData
{
    public List<UIWindow> windows;
    public List<string> activeWindows;
    public string currentWindowID;
    public string previousWindowID;
    
    public UIManagerData()
    {
        windows = new List<UIWindow>();
        activeWindows = new List<string>();
        currentWindowID = "";
        previousWindowID = "";
    }
    
    public void AddWindow(UIWindow window)
    {
        windows.Add(window);
    }
    
    public UIWindow GetWindow(string windowID)
    {
        return windows.Find(w => w.windowID == windowID);
    }
    
    public void ShowWindow(string windowID)
    {
        UIWindow window = GetWindow(windowID);
        if (window != null)
        {
            window.Show();
            if (!activeWindows.Contains(windowID))
            {
                activeWindows.Add(windowID);
            }
            previousWindowID = currentWindowID;
            currentWindowID = windowID;
        }
    }
    
    public void HideWindow(string windowID)
    {
        UIWindow window = GetWindow(windowID);
        if (window != null)
        {
            window.Hide();
            activeWindows.Remove(windowID);
            if (currentWindowID == windowID)
            {
                currentWindowID = previousWindowID;
            }
        }
    }
    
    public void HideAllWindows()
    {
        foreach (UIWindow window in windows)
        {
            if (!window.isPersistent)
            {
                window.Hide();
            }
        }
        activeWindows.Clear();
        currentWindowID = "";
        previousWindowID = "";
    }
    
    public List<UIWindow> GetActiveWindows()
    {
        List<UIWindow> active = new List<UIWindow>();
        foreach (string windowID in activeWindows)
        {
            UIWindow window = GetWindow(windowID);
            if (window != null)
            {
                active.Add(window);
            }
        }
        return active;
    }
}