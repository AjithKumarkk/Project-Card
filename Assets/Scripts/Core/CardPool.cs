using System.Collections.Generic;
using UnityEngine;

public class CardPool : MonoBehaviour
{
    public Card cardPrefab;
    public Transform poolRoot; 
    List<Card> pool = new List<Card>();

    void Awake()
    {
        if (poolRoot == null) poolRoot = this.transform;
    }

    public Card Get()
    {
        Card c;
        if (pool.Count > 0)
        {
            c = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
            c.gameObject.SetActive(true);
            c.ResetState();
            return c;
        }

        
        var instGO = Instantiate(cardPrefab.gameObject, poolRoot);
        instGO.name = cardPrefab.gameObject.name + "_clone";
        instGO.SetActive(false);          
        c = instGO.GetComponent<Card>();
        c.ResetState();
        return c;
    }

    public void Return(Card c)
    {
        if (c == null) return;
        c.ResetState();
        c.gameObject.SetActive(false);
        c.transform.SetParent(poolRoot, false);
        pool.Add(c);
    }

    public void ReturnAll(IEnumerable<Card> list)
    {
        foreach (var c in list) Return(c);
    }

    public void ClearPool(bool destroy = false)
    {
        if (destroy)
        {
            foreach (var c in pool) if (c) DestroyImmediate(c.gameObject);
        }
        pool.Clear();
    }
}
