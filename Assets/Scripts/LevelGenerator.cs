using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject dotPrefab;
    public GameObject powerDotPrefab;
    public GameObject floorPrefab;

    [TextArea]
    public string levelText;

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        string[] rows = levelText.Split('\n');
        for (int z = 0; z < rows.Length; z++)
        {
            string row = rows[z];
            for (int x = 0; x < row.Length; x++)
            {
                char c = row[x];
                Vector3 basePos = new Vector3(x, 0, -z);

                // Boden bei y = -0.5
                Vector3 floorPos = new Vector3(x, -0.5f, -z);
                Instantiate(floorPrefab, floorPos, Quaternion.identity, transform);

                switch (c)
                {
                    case 'W':
                        Instantiate(wallPrefab, basePos + Vector3.up * 0.5f, Quaternion.identity, transform);
                        break;
                    case '.':
                        Instantiate(dotPrefab, basePos + Vector3.up * 0.2f, Quaternion.identity, transform);
                        break;
                    case '*':
                        Instantiate(powerDotPrefab, basePos + Vector3.up * 0.2f, Quaternion.identity, transform);
                        break;
                    case 'G':
                        // Ghost Spawn Position – optional verarbeiten
                        break;
                }
            }
        }
    }
}