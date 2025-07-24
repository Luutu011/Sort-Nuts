using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class Screw : MonoBehaviour
{
    private List<Nut> nuts = new List<Nut>();
    private const int MaxNuts = 4;

    private void Awake()
    {
        transform.localScale = Vector3.one;
        foreach (Transform child in transform)
        {
            child.localScale = Vector3.one;
        }
    }

    public List<Nut> GetNuts() => nuts;

    public bool CanAddNuts(int count) => nuts.Count + count <= MaxNuts;

    public void AddNut(Nut nut)
    {
        if (nuts.Count < MaxNuts)
        {
            nuts.Add(nut);
            UpdateNutAppearances();
            UpdateNutPositions();
        }
    }

    public (List<Nut>, int) GetMovableSequence()
    {
        if (nuts.Count == 0) return (new List<Nut>(), 0);
        int topColor = nuts[nuts.Count - 1].ColorIndex;
        int count = 1;
        for (int i = nuts.Count - 2; i >= 0; i--)
        {
            if (nuts[i].ColorIndex == topColor) count++;
            else break;
        }
        List<Nut> sequence = nuts.GetRange(nuts.Count - count, count);
        return (sequence, count);
    }

    public List<Nut> RemoveTopSequence(int count)
    {
        List<Nut> sequence = nuts.GetRange(nuts.Count - count, count);
        nuts.RemoveRange(nuts.Count - count, count);
        UpdateNutAppearances();
        UpdateNutPositions();
        return sequence;
    }

    public void AddNuts(List<Nut> newNuts)
    {
        if (nuts.Count + newNuts.Count <= MaxNuts)
        {
            foreach (var nut in newNuts)
            {
                nut.transform.SetParent(transform);
                nut.transform.localScale = Vector3.one;
                nuts.Add(nut);
            }
            UpdateNutAppearances();
            UpdateNutPositions();
        }
    }

    public void UpdateNutAppearances()
    {
        for (int i = 0; i < nuts.Count; i++)
        {
            nuts[i].SetTop(i == nuts.Count - 1);
            nuts[i].transform.localScale = Vector3.one;
        }
    }

    private void UpdateNutPositions()
    {
        for (int i = 0; i < nuts.Count; i++)
        {
            float spacing = 0.3f;
            Vector2 targetPos = new Vector2(0, -0.4f + i * spacing);
            nuts[i].transform.DOKill();
            nuts[i].transform.DOLocalMove(targetPos, 0.5f);
            nuts[i].transform.localScale = Vector3.one;
            SpriteRenderer nutRenderer = nuts[i].GetComponent<SpriteRenderer>();
            if (nutRenderer != null)
            {
                nutRenderer.sortingOrder = MaxNuts + i;
            }
        }
    }

    public void Highlight(bool isHighlighted)
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
        if (isHighlighted)
        {
            transform.DOScale(1.1f, 0.2f).SetEase(Ease.InOutQuad);
        }
        foreach (var nut in nuts)
        {
            nut.transform.localScale = Vector3.one;
        }
    }
}