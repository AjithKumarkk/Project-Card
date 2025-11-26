using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Simple grid spawner for UI cards inside a RectTransform container.
public class GridLayoutManager : MonoBehaviour
{
    public RectTransform container; // must be under Canvas
    public CardPool pool;
    public Sprite backSprite;
    public List<Sprite> faceSprites = new List<Sprite>();
    public float padding = 8f;

    List<Card> active = new List<Card>();
    public List<Card> ActiveCards => active;

    // Make grid: rows x cols
    public void MakeGrid(int rows, int cols, int seed = 0)
    {
        // return old cards
        if (pool != null) pool.ReturnAll(active);
        active.Clear();

        int total = rows * cols;
        if (total % 2 != 0) total -= 1;

        List<int> ids = new List<int>();
        int pairs = total / 2;
        for (int i = 0; i < pairs; i++) { ids.Add(i); ids.Add(i); }

        // shuffle
        System.Random r = seed == 0 ? new System.Random() : new System.Random(seed);
        for (int i = ids.Count - 1; i > 0; i--)
        {
            int j = r.Next(i + 1);
            int tmp = ids[i]; ids[i] = ids[j]; ids[j] = tmp;
        }

        Canvas.ForceUpdateCanvases();
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(container);

        float w = container.rect.width;
        float h = container.rect.height;
        float cardW = (w - (cols + 1) * padding) / cols;
        float cardH = (h - (rows + 1) * padding) / rows;
        float size = Mathf.Max(8, Mathf.Min(cardW, cardH));

        float left = -w * container.pivot.x;
        float top = h * (1 - container.pivot.y);

        int idx = 0;
        for (int rRow = 0; rRow < rows; rRow++)
        {
            for (int cCol = 0; cCol < cols; cCol++)
            {
                if (idx >= ids.Count) break;
                var card = pool.Get();
                card.transform.SetParent(container, false);
                card.gameObject.SetActive(true); // newly spawned cards from pool may still be inactive

                RectTransform rt = card.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(size, size);

                float x = left + padding + size * 0.5f + cCol * (size + padding);
                float y = top - padding - size * 0.5f - rRow * (size + padding);
                rt.anchoredPosition = new Vector2(x, y);

                int id = ids[idx];
                card.id = id;
                if (faceSprites.Count > 0)
                    card.frontImage.sprite = faceSprites[id % faceSprites.Count];
                card.backImage.sprite = backSprite;

                active.Add(card);
                idx++;
            }
        }
    }

    public void ClearGrid()
    {
        if (ActiveCards == null || ActiveCards.Count == 0)
            return;

        foreach (var card in ActiveCards)
        {
            if (card != null)
            {
                pool.Return(card);
            }
        }
        ActiveCards.Clear();
        Canvas.ForceUpdateCanvases();
    }

}
