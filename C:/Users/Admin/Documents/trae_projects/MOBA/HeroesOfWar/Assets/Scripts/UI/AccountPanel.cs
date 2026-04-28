using UnityEngine;
using UnityEngine.UI;

public class AccountPanel : UIPanel
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;
    
    // 登录面板
    [SerializeField] private InputField loginUsernameInput;
    [SerializeField] private InputField loginPasswordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button switchToRegisterButton;
    
    // 注册面板
    [SerializeField] private InputField registerUsernameInput;
    [SerializeField] private InputField registerPasswordInput;
    [SerializeField] private InputField registerEmailInput;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button switchToLoginButton;
    
    [SerializeField] private Button backButton;
    [SerializeField] private Text messageText;
    
    private void Start()
    {
        loginButton.onClick.AddListener(OnLogin);
        switchToRegisterButton.onClick.AddListener(() => SwitchPanel("Register"));
        registerButton.onClick.AddListener(OnRegister);
        switchToLoginButton.onClick.AddListener(() => SwitchPanel("Login"));
        backButton.onClick.AddListener(OnBack);
        
        SwitchPanel("Login");
    }
    
    private void SwitchPanel(string panelName)
    {
        loginPanel.SetActive(panelName == "Login");
        registerPanel.SetActive(panelName == "Register");
        messageText.text = "";
    }
    
    private void OnLogin()
    {
        string username = loginUsernameInput.text;
        string password = loginPasswordInput.text;
        
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            messageText.text = "请输入用户名和密码";
            return;
        }
        
        bool success = AccountManager.Instance.Login(username, password);
        if (success)
        {
            messageText.text = "登录成功";
            // 登录成功后显示服务器选择面板
            MainUIManager.Instance.ShowPanel("ServerSelect");
        }
        else
        {
            messageText.text = "用户名或密码错误";
        }
    }
    
    private void OnRegister()
    {
        string username = registerUsernameInput.text;
        string password = registerPasswordInput.text;
        string email = registerEmailInput.text;
        
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
        {
            messageText.text = "请填写所有字段";
            return;
        }
        
        bool success = AccountManager.Instance.Register(username, password, email);
        if (success)
        {
            messageText.text = "注册成功，请登录";
            SwitchPanel("Login");
        }
        else
        {
            messageText.text = "注册失败，用户名已存在";
        }
    }
    
    private void OnBack()
    {
        MainUIManager.Instance.ShowPanel("MainMenu");
    }
}
