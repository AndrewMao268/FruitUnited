using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using System.Linq;

public class HiveDrawingTools
{
    public Tilemap highlightTilemap;
    public Tile highlightTile;
    public GameObject trajectoryBrush;
    public List<GameObject> brushes;

    public HiveDrawingTools(Tilemap highlightTilemap, Tile highlightTile, GameObject trajectoryBrush, List<GameObject> brushes)
    {
        this.highlightTilemap = highlightTilemap;
        this.highlightTile = highlightTile;
        this.trajectoryBrush = trajectoryBrush;
        this.brushes = brushes;
    }
    public void ClearVisuals()
    {
        foreach (GameObject brush in brushes)
        {
            Object.Destroy(brush);
        }

        highlightTilemap.ClearAllTiles();
        HighlightTile(new Vector3Int(0, 0, 0), new Color(0.5f, 0.0f, 1.0f, 1.0f));
    }

    public void HighlightPlatform(Platform platform)
    {
        HighlightPlatform(platform, (float x) => new Color(x, x, x, 1.0f));
    }

    public void HighlightPlatform(Platform platform, System.Func<float, Color> shadeFunction)
    {
        for (int x = 0; x < platform.length; x++)
        {
            Vector3Int highlightPos = platform.start;
            highlightPos.x += x;

            Color color = shadeFunction((float)x / platform.length);

            HighlightTile(highlightPos, color);
        }
    }

    public void HighlightTile(Vector3Int highlightPos, Color color)
    {
        highlightTilemap.SetTile(highlightPos, highlightTile);
        highlightTilemap.SetTileFlags(highlightPos, TileFlags.None);

        highlightTilemap.SetColor(highlightPos, color);
    }

    public void DrawJumpTrajectory(JumpTrajectory tilemapTrajectory, JumpTrajectory worldTrajectory, AgentAttributes attributes)
    {
        Debug.Log(tilemapTrajectory.toString());

        float x1 = worldTrajectory.x1;
        float x2 = worldTrajectory.x2;

        float step = (x2 - x1) / 50.0f;
        for (int i = 0; i < 51; i++)
        {
            float x = x1 + i * step;
            float y = worldTrajectory.plugIn(x);

            GameObject gameObject = Object.Instantiate(trajectoryBrush, new Vector3(x, y, 0.0f), Quaternion.identity);
            brushes.Add(gameObject);
        }

        List<Vector3Int> tileList = CrawlTrajectory(tilemapTrajectory, attributes);

        foreach (Vector3Int pos in tileList)
        {
            HighlightTile(pos, new Color(1.0f, 0.0f, 0.0f, 0.5f));
        }
    }

    public void DrawRunTrajectory(RunTrajectory worldTrajectory)
    {
        //Debug.Log(tilemapTrajectory.toString());

        float x1 = worldTrajectory.x1;
        float x2 = worldTrajectory.x2;

        float step = (x2 - x1) / 50.0f;
        for (int i = 0; i < 51; i++)
        {
            float x = x1 + i * step;
            float y = worldTrajectory.y1;

            GameObject gameObject = Object.Instantiate(trajectoryBrush, new Vector3(x, y, 0.0f), Quaternion.identity);
            brushes.Add(gameObject);
        }
    }

    private List<Vector3Int> CrawlTrajectory(JumpTrajectory trajectory, AgentAttributes attributes)
    {
        HashSet<Vector3Int> tilePositions = new HashSet<Vector3Int>();

        float xStart = trajectory.x1;
        float xEnd = trajectory.x2;

        if (xStart > xEnd)
        {
            (xStart, xEnd) = (xEnd, xStart);
        }

        for (float i = xStart; i <= xEnd; i += Mathf.Min((xEnd - xStart) / 50.0f, 0.1f))
        {
            float fx = i;
            float fy = trajectory.plugIn(fx);

            for (float j = -1; j < 2; j++)
            {
                for (float k = -1; k < 2; k++)
                {
                    float testX = fx + attributes.width / 4.0f * j;
                    float testY = fy + attributes.height / 4.0f * k;

                    int x = Mathf.RoundToInt(testX);
                    int y = Mathf.RoundToInt(testY);

                    tilePositions.Add(new Vector3Int(x, y, 0));
                }
            }
        }

        List<Vector3Int> tileList = new List<Vector3Int>(tilePositions.ToArray());

        return tileList;
    }
}