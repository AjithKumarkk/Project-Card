using System.Collections.Generic;
using UnityEngine;

public class CardPool : MonoBehaviour
{
    public Card cardPrefab;
    public Transform poolRoot;

    List<Card> pool = new List<Card>();

    public Card Get()
    {
        if (pool.Count > 0)
        {
            var c = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
            c.gameObject.SetActive(true);
            c.ResetState();
            return c;
        }
        var inst = Instantiate(cardPrefab, poolRoot != null ? poolRoot : transform);
        inst.ResetState();
        return inst;
    }

    public void Return(Card c)
    {
        if (c == null) return;
        c.ResetState();
        c.gameObject.SetActive(false);
        c.transform.SetParent(poolRoot != null ? poolRoot : transform, false);
        pool.Add(c);
    }

    public void ReturnAll(List<Card> list)
    {
        foreach (var c in list) Return(c);
        list.Clear();
    }
}
