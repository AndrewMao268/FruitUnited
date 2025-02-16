using System.Collections.Generic;
using UnityEngine;

public class HiveBin
{
    private float crawlProgress(float start, float finish, float value)
    {
        if (start == finish)
            return 1.0f;
        else if (Mathf.Sign(finish - start) != Mathf.Sign(finish - value))
            return 1.0f;
        else
            return (value - start) / (finish - start);
    }

    private void crawlLine(ref HashSet<Vector3Int> tilePositions, float fx1, float fy1, float fx2, float fy2)
    {
        int x1 = Mathf.RoundToInt(fx1);
        int y1 = Mathf.RoundToInt(fy1);
        int x2 = Mathf.RoundToInt(fx2);
        int y2 = Mathf.RoundToInt(fy2);

        int xDir = x2 > x1 ? 1 : -1;
        int xCurrent = x1;
        float xProgress = crawlProgress(x1, x2, xCurrent);

        int yDir = y2 > y1 ? 1 : -1;
        int yCurrent = y1;
        float yProgress = crawlProgress(y1, y2, yCurrent);

        tilePositions.Add(new Vector3Int(xCurrent, yCurrent, 0));

        while (xProgress < 1.0f || yProgress < 1.0f)
        {
            bool shouldMoveX = xProgress <= yProgress;
            bool shouldMoveY = yProgress <= xProgress;

            if (shouldMoveX && shouldMoveY)
            {
                float tempXProgress = crawlProgress(x1, x2, xCurrent + 0.1f * xDir);
                float tempYProgress = crawlProgress(y1, y2, yCurrent + 0.1f * yDir);
                shouldMoveX = tempXProgress <= tempYProgress;
                shouldMoveY = tempYProgress <= tempXProgress;

            }

            if (shouldMoveX)
            {
                xCurrent += xDir;
                xProgress = crawlProgress(x1, x2, xCurrent);
            }
            if (shouldMoveY)
            {
                yCurrent += yDir;
                yProgress = crawlProgress(y1, y2, yCurrent);
            }

            tilePositions.Add(new Vector3Int(xCurrent, yCurrent, 0));
        }
    }

    private float mapRange(float input, float inputStart, float inputEnd, float outputStart, float outputEnd)
    {
        return outputStart + ((outputEnd - outputStart) / (inputEnd - inputStart)) * (input - inputStart);
    }
}