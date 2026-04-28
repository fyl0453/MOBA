using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class RefundManager : MonoBehaviour
{
    public static RefundManager Instance { get; private set; }
    
    public RefundManagerData refundData;
    
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
        LoadRefundData();
        
        if (refundData == null)
        {
            refundData = new RefundManagerData();
        }
    }
    
    public void RecordRecharge(string playerID, string playerName, int amount, string currency, string paymentMethod, string transactionID)
    {
        string recordID = System.Guid.NewGuid().ToString();
        RechargeRecord newRecord = new RechargeRecord(recordID, playerID, playerName, amount, currency, paymentMethod, transactionID);
        refundData.system.AddRechargeRecord(newRecord);
        SaveRefundData();
        Debug.Log($"成功记录充值: {amount} {currency}");
    }
    
    public string SubmitRefundRequest(string playerID, string playerName, string rechargeRecordID, string refundReason)
    {
        RechargeRecord rechargeRecord = refundData.system.GetRechargeRecord(rechargeRecordID);
        if (rechargeRecord != null && rechargeRecord.playerID == playerID)
        {
            string requestID = System.Guid.NewGuid().ToString();
            RefundRequest newRequest = new RefundRequest(requestID, playerID, playerName, rechargeRecordID, rechargeRecord.amount, refundReason);
            refundData.system.AddRefundRequest(newRequest);
            SaveRefundData();
            Debug.Log($"成功提交退款申请: {rechargeRecord.amount} {rechargeRecord.currency}");
            return requestID;
        }
        else
        {
            Debug.Log("充值记录不存在或不属于该玩家");
            return "";
        }
    }
    
    public void ApproveRefundRequest(string requestID, string handlerID, string handlerName, string remark)
    {
        RefundRequest request = refundData.system.GetRefundRequest(requestID);
        if (request != null)
        {
            request.Approve(handlerID, handlerName, remark);
            
            // 更新充值记录的退款状态
            RechargeRecord rechargeRecord = refundData.system.GetRechargeRecord(request.rechargeRecordID);
            if (rechargeRecord != null)
            {
                rechargeRecord.UpdateRefundStatus("Refunded");
            }
            
            // 扣除玩家的钻石
            ProfileManager.Instance.currentProfile.gems -= request.rechargeAmount * 10;
            ProfileManager.Instance.SaveProfile();
            
            SaveRefundData();
            Debug.Log($"成功批准退款申请: {request.rechargeAmount}");
        }
    }
    
    public void RejectRefundRequest(string requestID, string handlerID, string handlerName, string remark)
    {
        RefundRequest request = refundData.system.GetRefundRequest(requestID);
        if (request != null)
        {
            request.Reject(handlerID, handlerName, remark);
            
            // 更新充值记录的退款状态
            RechargeRecord rechargeRecord = refundData.system.GetRechargeRecord(request.rechargeRecordID);
            if (rechargeRecord != null)
            {
                rechargeRecord.UpdateRefundStatus("Rejected");
            }
            
            SaveRefundData();
            Debug.Log($"成功拒绝退款申请: {request.rechargeAmount}");
        }
    }
    
    public List<RefundRequest> GetPlayerRefundRequests(string playerID, int limit = 50)
    {
        List<RefundRequest> requests = refundData.system.GetRefundRequestsByPlayer(playerID);
        requests.Sort((a, b) => b.requestTime.CompareTo(a.requestTime));
        return requests.GetRange(0, Mathf.Min(limit, requests.Count));
    }
    
    public List<RechargeRecord> GetPlayerRechargeRecords(string playerID, int limit = 50)
    {
        List<RechargeRecord> records = refundData.system.GetRechargeRecordsByPlayer(playerID);
        records.Sort((a, b) => b.rechargeTime.CompareTo(a.rechargeTime));
        return records.GetRange(0, Mathf.Min(limit, records.Count));
    }
    
    public List<RefundRequest> GetRefundRequestsByStatus(string status, int limit = 100)
    {
        List<RefundRequest> requests = refundData.system.GetRefundRequestsByStatus(status);
        requests.Sort((a, b) => b.requestTime.CompareTo(a.requestTime));
        return requests.GetRange(0, Mathf.Min(limit, requests.Count));
    }
    
    public RefundRequest GetRefundRequest(string requestID)
    {
        return refundData.system.GetRefundRequest(requestID);
    }
    
    public RechargeRecord GetRechargeRecord(string recordID)
    {
        return refundData.system.GetRechargeRecord(recordID);
    }
    
    public void SaveRefundData()
    {
        string path = Application.dataPath + "/Data/refund_system_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, refundData);
        stream.Close();
    }
    
    public void LoadRefundData()
    {
        string path = Application.dataPath + "/Data/refund_system_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            refundData = (RefundManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            refundData = new RefundManagerData();
        }
    }
}