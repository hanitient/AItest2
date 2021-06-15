using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameDataReader 
{
    BinaryReader reader;

    public GameDataReader(BinaryReader reader)
    {
        this.reader = reader;
    }

    public int ReadInt()
    {
        return reader.ReadInt32();
    }

    public float ReadFloat()
    {
        return reader.ReadSingle();
    }

    public string ReadString()
    {
        return reader.ReadString();
    }

    public float[] ReadFloatVector(int length)
    {
        float[] value = new float[length];
        for (int i = 0; i < length; i++)
        {
            value[i] = reader.ReadSingle();
        }
        return value;
    }
}
