using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AgentStorager
{ 
    string savePath;
    Agent agent;

    public AgentStorager(Agent agent,string modelName)
    {
        this.agent = agent;
        savePath = Path.Combine(Application.persistentDataPath, modelName);
    }
    public void Save()
    {
        using (var writer=new BinaryWriter(File.Open(savePath,FileMode.Create)))
        {
            SavePolicyFunction(new GameDataWriter(writer));
            SaveQFunction(new GameDataWriter(writer));
        }
    }
    
    public void Load()
    {
        using (var reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
        {
            LoadPolicyFunction(new GameDataReader(reader));
            LoadQFunction(new GameDataReader(reader));
        }
    }

    private void SavePolicyFunction(GameDataWriter writer)
    {
        foreach (PolicyFunction p in agent.policyFunctions.Values)
        {
            writer.Write(p.state);
            writer.Write(p.policy);
        }
    }

    private void SaveQFunction(GameDataWriter writer)
    {
        foreach (QFunction q in agent.qFunctions.Values)
        {
            writer.Write(q.state);
            writer.Write(q.action);
            writer.Write(q.value);
            writer.Write(q.counts);
        }
    }

    private void LoadPolicyFunction(GameDataReader reader)
    {
        foreach (PolicyFunction p in agent.policyFunctions.Values)
        {
            p.state = reader.ReadString();
            p.policy = reader.ReadFloatVector(2);
        }
    }

    private void LoadQFunction(GameDataReader reader)
    {
        foreach (QFunction q in agent.qFunctions.Values)
        {
            q.state = reader.ReadString();
            q.action = reader.ReadInt();
            q.value = reader.ReadFloat();
            q.counts = reader.ReadInt();
        }
    }
}
