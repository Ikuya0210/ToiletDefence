using UnityEngine;

namespace GGGameOver
{
    public static class Vector2Extentions
    {
        public static bool IsZero(this Vector2 vector)
        {
            return Mathf.Approximately(vector.x, 0f) && Mathf.Approximately(vector.y, 0f);
        }

        public static Vector2 ToVector2(this Vector3 vector3)
        {
            return new(vector3.x, vector3.y);
        }

        public static Vector3 ToVector3(this Vector2 vector2, float z = 0f)
        {
            return new(vector2.x, vector2.y, z);
        }

        public static Vector2 ClampMagnitude(this Vector2 vector, float maxLength)
        {
            if (vector.sqrMagnitude > maxLength * maxLength)
            {
                return vector.normalized * maxLength;
            }
            return vector;
        }
        public static Vector2 Rotate(this Vector2 vector, float angle)
        {
            float rad = angle * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            return new(
                vector.x * cos - vector.y * sin,
                vector.x * sin + vector.y * cos
            );
        }
        public static Vector2 Lerp(this Vector2 from, Vector2 to, float t)
        {
            return new(
                Mathf.Lerp(from.x, to.x, t),
                Mathf.Lerp(from.y, to.y, t)
            );
        }
    }
}