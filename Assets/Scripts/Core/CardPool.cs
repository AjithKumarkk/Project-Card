using System.Collections.Generic;
using UnityEngine;

public class CardPool : MonoBehaviour
{
    public Card cardPrefab;

    public Transform poolRoot;
    private readonly Stack<Card> pool = new Stack<Card>();
    public Card Get()
    {
        Card c;
        if (pool.Count > 0)
        {
            c = pool.Pop();
            c.gameObject.SetActive(true);
        }
        else
        {
            // Instantiate under poolRoot 
            Transform parent = poolRoot != null ? poolRoot : transform;
            c = Instantiate(cardPrefab, parent);
        }

        c.ResetState();
        return c;
    }
    public void Return(Card c)
    {
        if (c == null) return;
        c.ResetState();
        c.gameObject.SetActive(false);
        c.transform.SetParent(poolRoot != null ? poolRoot : transform, false);
        pool.Push(c);
    }

    public void ReturnAll(IEnumerable<Card> cards)
    {
        foreach (var c in cards)
            Return(c);
    }

    public void ClearPool(bool destroyExisting = false)
    {
        if (destroyExisting)
        {
            while (pool.Count > 0)
            {
                var c = pool.Pop();
                if (c != null) DestroyImmediate(c.gameObject);
            }
        }
        else
        {
            pool.Clear();
        }
    }
}
