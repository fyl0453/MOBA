[System.Serializable]
public class Fragment
{
    public string fragmentID;
    public string fragmentType;
    public string fragmentName;
    public int quantity;
    public int requiredQuantity;
    public string exchangeItemID;
    
    public Fragment(string id, string type, string name, int reqQty, string exchangeItem)
    {
        fragmentID = id;
        fragmentType = type;
        fragmentName = name;
        quantity = 0;
        requiredQuantity = reqQty;
        exchangeItemID = exchangeItem;
    }
    
    public void AddQuantity(int amount)
    {
        quantity += amount;
    }
    
    public void RemoveQuantity(int amount)
    {
        quantity = Mathf.Max(0, quantity - amount);
    }
    
    public bool CanExchange()
    {
        return quantity >= requiredQuantity;
    }
}

[System.Serializable]
public class FragmentManagerData
{
    public List<Fragment> fragments;
    
    public FragmentManagerData()
    {
        fragments = new List<Fragment>();
    }
    
    public void AddFragment(Fragment fragment)
    {
        fragments.Add(fragment);
    }
    
    public Fragment GetFragment(string fragmentID)
    {
        return fragments.Find(f => f.fragmentID == fragmentID);
    }
    
    public List<Fragment> GetFragmentsByType(string type)
    {
        return fragments.FindAll(f => f.fragmentType == type);
    }
    
    public List<Fragment> GetExchangeableFragments()
    {
        return fragments.FindAll(f => f.CanExchange());
    }
}