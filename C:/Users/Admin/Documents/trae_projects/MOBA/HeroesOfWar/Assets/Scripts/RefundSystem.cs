[System.Serializable]
public class RefundSystem
{
    public string systemID;
    public string systemName;
    public string systemDescription;
    public bool isEnabled;
    public List<RefundRequest> refundRequests;
    public List<RechargeRecord> rechargeRecords;
    
    public RefundSystem(string id, string name, string desc)
    {
        systemID = id;
        systemName = name;
        systemDescription = desc;
        isEnabled = true;
        refundRequests = new List<RefundRequest>();
        rechargeRecords = new List<RechargeRecord>();
    }
    
    public void AddRefundRequest(RefundRequest refundRequest)
    {
        refundRequests.Add(refundRequest);
    }
    
    public void AddRechargeRecord(RechargeRecord rechargeRecord)
    {
        rechargeRecords.Add(rechargeRecord);
    }
    
    public RefundRequest GetRefundRequest(string requestID)
    {
        return refundRequests.Find(r => r.requestID == requestID);
    }
    
    public RechargeRecord GetRechargeRecord(string recordID)
    {
        return rechargeRecords.Find(r => r.recordID == recordID);
    }
    
    public List<RefundRequest> GetRefundRequestsByPlayer(string playerID)
    {
        return refundRequests.FindAll(r => r.playerID == playerID);
    }
    
    public List<RechargeRecord> GetRechargeRecordsByPlayer(string playerID)
    {
        return rechargeRecords.FindAll(r => r.playerID == playerID);
    }
    
    public List<RefundRequest> GetRefundRequestsByStatus(string status)
    {
        return refundRequests.FindAll(r => r.status == status);
    }
}

[System.Serializable]
public class RefundRequest
{
    public string requestID;
    public string playerID;
    public string playerName;
    public string rechargeRecordID;
    public int rechargeAmount;
    public string refundReason;
    public string status;
    public string requestTime;
    public string processTime;
    public string handlerID;
    public string handlerName;
    public string remark;
    
    public RefundRequest(string id, string playerID, string playerName, string rechargeRecordID, int rechargeAmount, string refundReason)
    {
        requestID = id;
        this.playerID = playerID;
        this.playerName = playerName;
        this.rechargeRecordID = rechargeRecordID;
        this.rechargeAmount = rechargeAmount;
        this.refundReason = refundReason;
        status = "Pending";
        requestTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        processTime = "";
        handlerID = "";
        handlerName = "";
        remark = "";
    }
    
    public void Approve(string handlerID, string handlerName, string remark)
    {
        status = "Approved";
        processTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.handlerID = handlerID;
        this.handlerName = handlerName;
        this.remark = remark;
    }
    
    public void Reject(string handlerID, string handlerName, string remark)
    {
        status = "Rejected";
        processTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.handlerID = handlerID;
        this.handlerName = handlerName;
        this.remark = remark;
    }
    
    public void UpdateStatus(string status)
    {
        this.status = status;
    }
}

[System.Serializable]
public class RechargeRecord
{
    public string recordID;
    public string playerID;
    public string playerName;
    public int amount;
    public string currency;
    public string paymentMethod;
    public string transactionID;
    public string status;
    public string rechargeTime;
    public string refundStatus;
    
    public RechargeRecord(string id, string playerID, string playerName, int amount, string currency, string paymentMethod, string transactionID)
    {
        recordID = id;
        this.playerID = playerID;
        this.playerName = playerName;
        this.amount = amount;
        this.currency = currency;
        this.paymentMethod = paymentMethod;
        this.transactionID = transactionID;
        status = "Completed";
        rechargeTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        refundStatus = "None";
    }
    
    public void UpdateStatus(string status)
    {
        this.status = status;
    }
    
    public void UpdateRefundStatus(string refundStatus)
    {
        this.refundStatus = refundStatus;
    }
}

[System.Serializable]
public class RefundManagerData
{
    public RefundSystem system;
    
    public RefundManagerData()
    {
        system = new RefundSystem("refund_system", "退款系统", "管理游戏内充值退款");
    }
}