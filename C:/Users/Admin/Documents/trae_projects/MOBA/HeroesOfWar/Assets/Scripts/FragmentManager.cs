using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class FragmentManager : MonoBehaviour
{
    public static FragmentManager Instance { get; private set; }
    
    public FragmentManagerData fragmentData;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        LoadFragmentData();
        
        if (fragmentData == null)
        {
            fragmentData = new FragmentManagerData();
            InitializeDefaultFragments();
        }
    }
    
    private void InitializeDefaultFragments()
    {
        Fragment guanyuFragment = new Fragment("fragment_guanyu", "Hero", "关羽碎片", 50, "hero_guanyu");
        fragmentData.AddFragment(guanyuFragment);
        
        Fragment zhangfeiFragment = new Fragment("fragment_zhangfei", "Hero", "张飞碎片", 50, "hero_zhangfei");
        fragmentData.AddFragment(zhangfeiFragment);
        
        Fragment liubeiFragment = new Fragment("fragment_liubei", "Hero", "刘备碎片", 50, "hero_liubei");
        fragmentData.AddFragment(liubeiFragment);
        
        Fragment zhaoyunFragment = new Fragment("fragment_zhaoyun", "Hero", "赵云碎片", 50, "hero_zhaoyun");
        fragmentData.AddFragment(zhaoyunFragment);
        
        Fragment zhugeliangFragment = new Fragment("fragment_zhugeliang", "Hero", "诸葛亮碎片", 50, "hero_zhugeliang");
        fragmentData.AddFragment(zhugeliangFragment);
        
        Fragment guanyuSkinFragment = new Fragment("fragment_guanyu_skin", "Skin", "关羽皮肤碎片", 20, "skin_guanyu_spring");
        fragmentData.AddFragment(guanyuSkinFragment);
        
        Fragment zhaoyunSkinFragment = new Fragment("fragment_zhaoyun_skin", "Skin", "赵云皮肤碎片", 20, "skin_zhaoyun_dragon");
        fragmentData.AddFragment(zhaoyunSkinFragment);
        
        SaveFragmentData();
    }
    
    public void AddFragment(string fragmentID, int quantity)
    {
        Fragment fragment = fragmentData.GetFragment(fragmentID);
        if (fragment != null)
        {
            fragment.AddQuantity(quantity);
            SaveFragmentData();
        }
    }
    
    public void ExchangeFragment(string fragmentID)
    {
        Fragment fragment = fragmentData.GetFragment(fragmentID);
        if (fragment != null && fragment.CanExchange())
        {
            fragment.RemoveQuantity(fragment.requiredQuantity);
            
            // 根据碎片类型兑换物品
            if (fragment.fragmentType == "Hero")
            {
                HeroMasteryManager.Instance.UnlockHero(fragment.exchangeItemID, fragment.fragmentName.Replace("碎片", ""));
            }
            else if (fragment.fragmentType == "Skin")
            {
                SkinManager.Instance.PurchaseSkin(fragment.exchangeItemID);
            }
            
            SaveFragmentData();
        }
    }
    
    public Fragment GetFragment(string fragmentID)
    {
        return fragmentData.GetFragment(fragmentID);
    }
    
    public List<Fragment> GetFragmentsByType(string type)
    {
        return fragmentData.GetFragmentsByType(type);
    }
    
    public List<Fragment> GetExchangeableFragments()
    {
        return fragmentData.GetExchangeableFragments();
    }
    
    public void SaveFragmentData()
    {
        string path = Application.dataPath + "/Data/fragment_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, fragmentData);
        stream.Close();
    }
    
    public void LoadFragmentData()
    {
        string path = Application.dataPath + "/Data/fragment_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            fragmentData = (FragmentManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            fragmentData = new FragmentManagerData();
        }
    }
}