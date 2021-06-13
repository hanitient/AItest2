using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CardFactory", menuName = "Factory/cardFactory", order = 1)]
public class CardFactory: GameObjectFactory
{
    [SerializeField] Card[] prefabs;
    List<Card>[] pools;
    List<Card> cards;
    public override void Initiate()
    {
        if(pools==null)CreatePool();
    }
    //如果还没有对象池，那么首先建立对象池
    private void CreatePool()
    {
        pools = new List<Card>[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            pools[i] = new List<Card>();
        }
    }
    public Card GetRandomCard()
    {
        int value = Random.Range(1, 11);
        CardColor[] colors = new CardColor[2] { CardColor.Red, CardColor.Black };
        CardColor color = colors[StaticData.GetRandomElement(new float[2] { 1 / 3f, 2 / 3f })];
        Card card = Get(value, color);
        return (card);
    }
    public Card Get(int value,CardColor color)
    {
        Card instance;
        List<Card> pool = pools[0];
        int lastIndex = pool.Count - 1;
        if (lastIndex >= 0)
        {
            instance = pool[lastIndex];
            instance.OnSpawn();
            pool.RemoveAt(lastIndex);
        }
        else
        {
            instance = CreateGameObjectInstance(prefabs[0]);
            cards.Add(instance);
        }
        instance.Value = value;
        instance.Color = color;
        return instance;
    }
    public void Reclaim(Card cardToRecycle)
    {
        pools[0].Add(cardToRecycle);
        cardToRecycle.UnSpawn();
    }
    public void ReclaimAll()
    {

            foreach (Card c in cards)
            {
                if(c!=null)Reclaim(c);
            }
    }
}
