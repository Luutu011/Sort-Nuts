using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    public static void GenerateLevel(LevelConfig.LevelData levelData, GameObject screwPrefab, GameObject nutPrefab)
    {
        for (int i = 0; i < levelData.screws.Count; i++)
        {
            Vector2 position = levelData.screwPositions[i];
            GameObject screwObj = Instantiate(screwPrefab, position, Quaternion.identity);
            Screw screw = screwObj.GetComponent<Screw>();
            GameManager.Instance.RegisterScrew(screw);

            var nuts = levelData.screws[i].nuts;
            for (int j = 0; j < nuts.Count; j++)
            {
                Vector2 nutPos = new Vector2(position.x, position.y + (j + 1) * 20f);
                GameObject nutObj = Instantiate(nutPrefab, nutPos, Quaternion.identity, screwObj.transform);
                Nut nut = nutObj.GetComponent<Nut>();
                nut.Initialize(nuts[j].colorIndex, nuts[j].isHidden, j == nuts.Count - 1);
                screw.AddNut(nut);
            }
        }
    }
}
