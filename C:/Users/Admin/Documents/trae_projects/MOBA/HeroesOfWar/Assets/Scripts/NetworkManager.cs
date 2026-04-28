using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }
    
    public string serverIP = "127.0.0.1";
    public int serverPort = 8080;
    
    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private bool isConnected = false;
    
    public delegate void MessageReceivedHandler(string message);
    public event MessageReceivedHandler OnMessageReceived;
    
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
    
    public bool Connect()
    {
        try
        {
            client = new TcpClient(serverIP, serverPort);
            stream = client.GetStream();
            isConnected = true;
            
            // 启动接收线程
            receiveThread = new Thread(ReceiveMessages);
            receiveThread.Start();
            
            Debug.Log("连接到服务器成功");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("连接到服务器失败: " + e.Message);
            isConnected = false;
            return false;
        }
    }
    
    public void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            
            if (receiveThread != null && receiveThread.IsAlive)
            {
                receiveThread.Abort();
            }
            
            if (stream != null)
            {
                stream.Close();
            }
            
            if (client != null)
            {
                client.Close();
            }
            
            Debug.Log("断开与服务器的连接");
        }
    }
    
    public void SendMessage(string message)
    {
        if (isConnected && stream != null)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message + "\n");
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }
            catch (System.Exception e)
            {
                Debug.LogError("发送消息失败: " + e.Message);
                Disconnect();
            }
        }
    }
    
    private void ReceiveMessages()
    {
        while (isConnected)
        {
            try
            {
                if (stream != null && stream.DataAvailable)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    
                    if (!string.IsNullOrEmpty(message))
                    {
                        OnMessageReceived?.Invoke(message);
                    }
                }
                
                Thread.Sleep(10);
            }
            catch (System.Exception e)
            {
                Debug.LogError("接收消息失败: " + e.Message);
                Disconnect();
            }
        }
    }
    
    public bool IsConnected()
    {
        return isConnected;
    }
    
    private void OnApplicationQuit()
    {
        Disconnect();
    }
}
