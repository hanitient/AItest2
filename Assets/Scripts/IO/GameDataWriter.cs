using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameDataWriter
{
    BinaryWriter writer;
    public GameDataWriter(BinaryWriter writer)
    {
        this.writer = writer;
    }

    public void Write(int value)
    {
        writer.Write(value);
    }

    public void Write(float value)
    {
        writer.Write(value);
    }

    public void Write(string value)
    {
        writer.Write(value);
    }

    public void Write(float[] value)
    {
        for (int i = 0; i < value.Length; i++)
        {
            writer.Write(value[i]);
        }
    }
}

