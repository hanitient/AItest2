using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticData
{
    public static int EpisodeLength=10;
    //��һ���б������һ�������index
    public static int GetRandomElement(float[] probabilities)
    {
        float sum = 0;
        int index = 0;
        float roll = Random.Range(0f, 1f);
        //���Ը��ʵ������Ƿ����
        for (int i = 0; i < probabilities.Length; i++)
        {
            sum += probabilities[i];
        }
        if (sum !=1)
        {
            Debug.LogAssertion("�����б��������");
            return 0;
        }
        //������������Ԫ��
        while(index<probabilities.Length)
        {
            sum = 0;
            for (int i = 0; i < index+1; i++)
            {
                sum += probabilities[i];
            }
            if (roll < sum)
            {
                return index;
            }
            index++;
        }
        return 0;
    }

    //total��������number����Ҫ���������
    public static List<int> SelectNoRepeat(int total, int number)
    {
        List<int> data = new List<int>();
        for (int i = 0; i < total; i++)
        {
            data.Add(i);
        }
        if (data.Count < number)
        {
            return data;
        }
        else
        {
            List<int> result = new List<int>();
            for (int i = 0; i < number; i++)
            {
                int index = Random.Range(0, data.Count);
                result.Add(data[index]);
                data.RemoveAt(index);
            }
            return result;
        }
    }

    //����һ���ܵȼ����������ɸ�������ķ���
    public static int[] GetSomeRandoms(int totalLevel, int number,int maxLevel)
    {
        if ((totalLevel / number) > maxLevel)
        {
            Debug.LogWarning("�䷽�ȼ�����ˣ��˼�");
            int[] errorRandom = new int[number];
            for (int i = 0; i < number; i++)
            {
                errorRandom[i] = maxLevel;
            }
            return errorRandom;
        }
        if (number < 1)
        {
            number = 1;
        }
        if (totalLevel < 1)
        {
            totalLevel = 1;
        }
        if (number > totalLevel)
        {
            totalLevel = number;
        }
        int[] result = new int[number];
        while (number > 1)
        {
            if (number == 2)
            {
                int min = 1;
                while (totalLevel - min > maxLevel)
                {
                    min++;
                }
                result[0] = Random.Range(min, totalLevel - min + 1);
                result[1] = totalLevel - result[0];
                number--;
            }
            else if (number >= 2)
            {
                int max = Mathf.Min(totalLevel - (number - 1), maxLevel);
                int min = 1;
                while (totalLevel - min > maxLevel * (number - 1))
                {
                    min++;
                }
                int a = Random.Range(min, max + 1);
                totalLevel -= a;
                result[number - 1] = a;
                number--;
            }
            else
            {
                Debug.LogWarning("ˢ����ȼ����㷨��֧�֣�");
                return null;
            }
        }
        return result;
    }
}