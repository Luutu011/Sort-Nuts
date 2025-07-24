using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "NutSort/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [System.Serializable]
    public struct LevelData
    {
        public int levelNumber;
        public List<ScrewData> screws;
        public List<Vector2> screwPositions;
    }

    [System.Serializable]
    public struct ScrewData
    {
        public List<NutData> nuts;
    }

    [System.Serializable]
    public struct NutData
    {
        public int colorIndex;
        public bool isHidden;
    }

    public LevelData GetLevelData(int levelNumber)
    {
        LevelData data = new LevelData { levelNumber = levelNumber };
        int cycle = (levelNumber - 1) / 5;
        int cycleLevel = (levelNumber - 1) % 5;
        int colors = 2 + cycleLevel;
        int screws = colors + 1;
        int nutsPerColor = 4;
        int hiddenNuts = cycle * 2;

        data.screws = new List<ScrewData>();
        for (int i = 0; i < screws; i++)
        {
            data.screws.Add(new ScrewData { nuts = new List<NutData>() });
        }

        data.screwPositions = GetScrewPositions(screws);

        // Start with solved state: one screw per color, last screw empty
        int emptyScrewIndex = screws - 1;
        for (int color = 0; color < colors; color++)
        {
            for (int i = 0; i < nutsPerColor; i++)
            {
                data.screws[color].nuts.Add(new NutData { colorIndex = color, isHidden = false });
            }
        }

        // Scramble nuts with reverse moves, keeping one screw empty
        int maxMoves = colors * nutsPerColor * 2;
        List<int> sourceScrews = new List<int>();
        List<int> targetScrews = new List<int>();
        for (int i = 0; i < screws; i++)
        {
            sourceScrews.Add(i);
            targetScrews.Add(i);
        }

        for (int i = 0; i < maxMoves; i++)
        {
            sourceScrews.Shuffle();
            targetScrews.Shuffle();
            foreach (int source in sourceScrews)
            {
                if (source == emptyScrewIndex || data.screws[source].nuts.Count == 0) continue;
                int topColor = data.screws[source].nuts[data.screws[source].nuts.Count - 1].colorIndex;
                int count = 1;
                for (int j = data.screws[source].nuts.Count - 2; j >= 0; j--)
                {
                    if (data.screws[source].nuts[j].colorIndex == topColor) count++;
                    else break;
                }
                foreach (int target in targetScrews)
                {
                    if (source == target || target == emptyScrewIndex || data.screws[target].nuts.Count >= 4) continue;
                    bool isValidMove = data.screws[target].nuts.Count == 0 ||
                        data.screws[target].nuts[data.screws[target].nuts.Count - 1].colorIndex == topColor;
                    if (isValidMove && data.screws[target].nuts.Count + count <= 4)
                    {
                        var nutsToMove = data.screws[source].nuts.GetRange(data.screws[source].nuts.Count - count, count);
                        data.screws[source].nuts.RemoveRange(data.screws[source].nuts.Count - count, count);
                        data.screws[target].nuts.AddRange(nutsToMove);
                        if (data.screws[source].nuts.Count == 0)
                        {
                            emptyScrewIndex = source;
                        }
                        break;
                    }
                }
            }
        }

        // Ensure one screw is empty
        bool hasEmptyScrew = false;
        for (int i = 0; i < screws; i++)
        {
            if (data.screws[i].nuts.Count == 0)
            {
                emptyScrewIndex = i;
                hasEmptyScrew = true;
                break;
            }
        }
        if (!hasEmptyScrew)
        {
            int minNutsIndex = 0;
            int minNutsCount = data.screws[0].nuts.Count;
            for (int i = 1; i < screws; i++)
            {
                if (data.screws[i].nuts.Count < minNutsCount)
                {
                    minNutsIndex = i;
                    minNutsCount = data.screws[i].nuts.Count;
                }
            }
            if (minNutsCount > 0)
            {
                int targetIndex = -1;
                for (int i = 0; i < screws; i++)
                {
                    if (i != minNutsIndex && data.screws[i].nuts.Count + minNutsCount <= 4)
                    {
                        bool isValid = data.screws[i].nuts.Count == 0 ||
                            data.screws[i].nuts[data.screws[i].nuts.Count - 1].colorIndex == data.screws[minNutsIndex].nuts[data.screws[minNutsIndex].nuts.Count - 1].colorIndex;
                        if (isValid)
                        {
                            targetIndex = i;
                            break;
                        }
                    }
                }
                if (targetIndex >= 0)
                {
                    var nutsToMove = data.screws[minNutsIndex].nuts.GetRange(0, data.screws[minNutsIndex].nuts.Count);
                    data.screws[minNutsIndex].nuts.Clear();
                    data.screws[targetIndex].nuts.AddRange(nutsToMove);
                    emptyScrewIndex = minNutsIndex;
                }
            }
        }

        if (hiddenNuts > 0)
        {
            int screwsWithHidden = Mathf.Min(hiddenNuts, screws - 1);
            int baseHiddenPerScrew = hiddenNuts / screwsWithHidden;
            int extraHidden = hiddenNuts % screwsWithHidden;
            List<int> hiddenScrewIndices = new List<int>();
            for (int i = 0; i < screws; i++)
            {
                if (i != emptyScrewIndex) hiddenScrewIndices.Add(i);
            }
            hiddenScrewIndices.Shuffle();

            for (int i = 0; i < screwsWithHidden; i++)
            {
                int hiddenCount = baseHiddenPerScrew + (i < extraHidden ? 1 : 0);
                for (int j = 0; j < hiddenCount; j++)
                {
                    if (data.screws[hiddenScrewIndices[i]].nuts.Count < 4)
                    {
                        int colorIndex = Random.Range(0, colors);
                        data.screws[hiddenScrewIndices[i]].nuts.Insert(0, new NutData { colorIndex = colorIndex, isHidden = true });
                        for (int k = 0; k < screws; k++)
                        {
                            if (k != hiddenScrewIndices[i] && k != emptyScrewIndex && data.screws[k].nuts.Count > 0)
                            {
                                int index = data.screws[k].nuts.FindIndex(n => n.colorIndex == colorIndex && !n.isHidden);
                                if (index >= 0)
                                {
                                    data.screws[k].nuts.RemoveAt(index);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        // Prevent pre-solved screws
        bool hasSolvedScrew;
        int maxAttempts = 10;
        int attempt = 0;
        do
        {
            hasSolvedScrew = false;
            foreach (var screw in data.screws)
            {
                if (screw.nuts.Count > 1)
                {
                    int firstColor = screw.nuts[0].colorIndex;
                    bool allSame = true;
                    foreach (var nut in screw.nuts)
                    {
                        if (nut.colorIndex != firstColor)
                        {
                            allSame = false;
                            break;
                        }
                    }
                    if (allSame)
                    {
                        hasSolvedScrew = true;
                        break;
                    }
                }
            }
            if (hasSolvedScrew && attempt < maxAttempts)
            {
                List<NutData> allNuts = new List<NutData>();
                List<int> nonEmptyScrews = new List<int>();
                for (int i = 0; i < screws; i++)
                {
                    if (i != emptyScrewIndex)
                    {
                        var nonHiddenNuts = data.screws[i].nuts.FindAll(n => !n.isHidden);
                        allNuts.AddRange(nonHiddenNuts);
                        data.screws[i].nuts.RemoveAll(n => !n.isHidden);
                        if (data.screws[i].nuts.Count < 4) nonEmptyScrews.Add(i);
                    }
                }
                allNuts.Shuffle();
                int nutIndex = 0;
                int nutsPerScrew = allNuts.Count / nonEmptyScrews.Count;
                int extraNuts = allNuts.Count % nonEmptyScrews.Count;
                for (int i = 0; i < nonEmptyScrews.Count; i++)
                {
                    int nutsThisScrew = nutsPerScrew + (i < extraNuts ? 1 : 0);
                    for (int j = 0; j < nutsThisScrew && nutIndex < allNuts.Count; j++)
                    {
                        data.screws[nonEmptyScrews[i]].nuts.Add(allNuts[nutIndex]);
                        nutIndex++;
                    }
                }
                attempt++;
            }
        } while (hasSolvedScrew && attempt < maxAttempts);

        if (!hasEmptyScrew)
        {
            Debug.LogWarning("Failed to ensure an empty screw after max attempts");
        }

        return data;
    }

    private List<Vector2> GetScrewPositions(int screwCount)
    {
        List<Vector2> positions = new List<Vector2>();
        float hSpacing = 2f;
        float vSpacing = 3f;
        float offsetX = 0;
        float offsetY = 0;

        switch (screwCount)
        {
            case 3: // 1-2
                positions.Add(new Vector2(0, vSpacing));
                positions.Add(new Vector2(-hSpacing / 2, -vSpacing));
                positions.Add(new Vector2(hSpacing / 2, -vSpacing));
                break;
            case 4: // 2-2
                positions.Add(new Vector2(-hSpacing / 2, vSpacing));
                positions.Add(new Vector2(hSpacing / 2, vSpacing));
                positions.Add(new Vector2(-hSpacing / 2, -vSpacing));
                positions.Add(new Vector2(hSpacing / 2, -vSpacing));
                break;
            case 5: // 2-1-2
                positions.Add(new Vector2(-hSpacing / 2, vSpacing));
                positions.Add(new Vector2(hSpacing / 2, vSpacing));
                positions.Add(new Vector2(0, 0));
                positions.Add(new Vector2(-hSpacing / 2, -vSpacing));
                positions.Add(new Vector2(hSpacing / 2, -vSpacing));
                break;
            case 6: // 2x3
                positions.Add(new Vector2(-hSpacing, vSpacing));
                positions.Add(new Vector2(0, vSpacing));
                positions.Add(new Vector2(hSpacing, vSpacing));
                positions.Add(new Vector2(-hSpacing, -vSpacing));
                positions.Add(new Vector2(0, -vSpacing));
                positions.Add(new Vector2(hSpacing, -vSpacing));
                break;
            case 7: // 2-3-2
                positions.Add(new Vector2(-hSpacing / 2, vSpacing));
                positions.Add(new Vector2(hSpacing / 2, vSpacing));
                positions.Add(new Vector2(-hSpacing, 0));
                positions.Add(new Vector2(0, 0));
                positions.Add(new Vector2(hSpacing, 0));
                positions.Add(new Vector2(-hSpacing / 2, -vSpacing));
                positions.Add(new Vector2(hSpacing / 2, -vSpacing));
                break;
        }

        for (int i = 0; i < positions.Count; i++)
        {
            positions[i] = new Vector2(positions[i].x + offsetX, positions[i].y + offsetY);
        }

        return positions;
    }
}