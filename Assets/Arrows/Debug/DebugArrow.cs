using UnityEngine;

public static class DebugArrow
{
    public static void DrawForDebug(Vector3 startingPosition, Vector3 direction, Color color, float scale = 1f, float duration = 0f,  float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Debug.DrawRay(startingPosition, direction * scale, color, duration);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Debug.DrawRay(startingPosition + direction * scale, right * arrowHeadLength, color, duration);
        Debug.DrawRay(startingPosition + direction * scale, left * arrowHeadLength, color, duration);
    }
}