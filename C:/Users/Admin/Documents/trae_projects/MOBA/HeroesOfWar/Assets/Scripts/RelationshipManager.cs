using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class RelationshipManager : MonoBehaviour
{
    public static RelationshipManager Instance { get; private set; }
    
    public RelationshipManagerData relationshipData;
    
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
        LoadRelationshipData();
        
        if (relationshipData == null)
        {
            relationshipData = new RelationshipManagerData();
        }
    }
    
    public Relationship EstablishRelationship(string targetPlayerID, string targetPlayerName, string relationshipType)
    {
        string playerID = ProfileManager.Instance.currentProfile.playerID;
        string playerName = ProfileManager.Instance.currentProfile.playerName;
        
        // 检查是否已有关系
        Relationship existingRelationship = relationshipData.GetRelationshipBetweenPlayers(playerID, targetPlayerID);
        if (existingRelationship != null)
        {
            return existingRelationship;
        }
        
        string relationshipID = "relationship_" + System.DateTime.Now.Ticks;
        Relationship relationship = new Relationship(relationshipID, playerID, playerName, targetPlayerID, targetPlayerName, relationshipType);
        relationshipData.AddRelationship(relationship);
        SaveRelationshipData();
        
        return relationship;
    }
    
    public void AddIntimacy(string relationshipID, int value)
    {
        Relationship relationship = relationshipData.GetRelationship(relationshipID);
        if (relationship != null && relationship.isActive)
        {
            relationship.AddIntimacy(value);
            SaveRelationshipData();
        }
    }
    
    public void RemoveIntimacy(string relationshipID, int value)
    {
        Relationship relationship = relationshipData.GetRelationship(relationshipID);
        if (relationship != null && relationship.isActive)
        {
            relationship.RemoveIntimacy(value);
            SaveRelationshipData();
        }
    }
    
    public void EndRelationship(string relationshipID)
    {
        Relationship relationship = relationshipData.GetRelationship(relationshipID);
        if (relationship != null)
        {
            relationship.Deactivate();
            SaveRelationshipData();
        }
    }
    
    public List<Relationship> GetPlayerRelationships()
    {
        return relationshipData.GetRelationshipsByPlayer(ProfileManager.Instance.currentProfile.playerID);
    }
    
    public Relationship GetRelationship(string relationshipID)
    {
        return relationshipData.GetRelationship(relationshipID);
    }
    
    public Relationship GetRelationshipWithPlayer(string playerID)
    {
        return relationshipData.GetRelationshipBetweenPlayers(ProfileManager.Instance.currentProfile.playerID, playerID);
    }
    
    public void SaveRelationshipData()
    {
        string path = Application.dataPath + "/Data/relationship_data.dat";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, relationshipData);
        stream.Close();
    }
    
    public void LoadRelationshipData()
    {
        string path = Application.dataPath + "/Data/relationship_data.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            relationshipData = (RelationshipManagerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            relationshipData = new RelationshipManagerData();
        }
    }
}