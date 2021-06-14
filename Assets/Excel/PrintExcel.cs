using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrintExcel : MonoBehaviour
{

    public List<DepenceTableData> listdata;

    void Start()
    {

        Text T = this.GetComponent<Text>();

        T.text = "";//清空一开始的文本

        listdata = DoExcel.Load(Application.dataPath + "\\Excel\\" + "test.xlsx");



        foreach (var listing in listdata)

        {

            print(listing.instruct + "     " + listing.word);

            T.text += (listing.instruct + "     " + listing.word + 
                "      "+listing.another+"\n").ToString();

        }

    }

}