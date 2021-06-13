using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card:ReusableObject
{
    int value;
    CardColor color;
    [SerializeField]
    Text text;
    public int Value 
    {
        get
        {
            return value;
        }
        set
        {
            text.text = "�ƣ�"+Color.ToString()+" "+value.ToString();
            this.value = value;
        }
    }
    public CardColor Color 
    {
        get => color; 
        set
        {
            text.text = "�ƣ�"+value.ToString()+" "+Value.ToString();
            color = value;
        }
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        value = default;
        color = default;
    }
}
