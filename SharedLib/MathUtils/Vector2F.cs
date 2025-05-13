/* ----- ----- ----- ----- */
// Vector2F.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/11
// Update Date: 2025/05/11
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

namespace SharedLib.MathUtils
{
    /// <summary>
    /// Represents a 2D vector with X and Y coordinates.
    /// </summary>
    public class Vector2F
    {
        public float X { get; set; }
        public float Y { get; set; }

        /// <summary>
        /// Sets the X and Y using default values `(0f, 0f)`.
        /// </summary>
        public Vector2F()
        {
            X = 0.0f;
            Y = 0.0f;
        }

        /// <summary>
        /// Sets the X and Y using given values.
        /// </summary>
        public Vector2F(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector2F Zero => new Vector2F(0, 0);

        /// <summary>
        /// Convert to Size type
        /// </summary>
        public Size ToSize()
        {
            return new Size((int)X, (int)Y);
        }
        public static Size ToSize(Vector2F vector)
        {
            return new Size((int)vector.X, (int)vector.Y);
        }

        /// <summary>
        /// Convert to Point type
        /// </summary>
        public Point ToPoint()
        {
            return new Point((int)X, (int)Y);
        }
        public static Point ToPoint(Vector2F vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
        }

        /// <summary>
        /// Convert to PointF type
        /// </summary>
        public PointF ToPointF()
        {
            return new PointF(X, Y);
        }
        public static PointF ToPointF(Vector2F vector)
        {
            return new PointF(vector.X, vector.Y);
        }

        /// <summary>
        /// Convert position and size to RectangleF type
        /// </summary>
        public static RectangleF ToRectangleF(Vector2F position, Vector2F size)
        {
            return new RectangleF(position.X, position.Y, size.X, size.Y);
        }

        /// <summary>
        /// Convert position and size to RectangleF type
        /// </summary>
        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        // Subtraction (Vector2F - Vector2F)
        public static Vector2F operator -(Vector2F v)
        {
            return new Vector2F(-v.X, -v.Y);
        }

        // Addition (Vector2F + Vector2F)
        public static Vector2F operator +(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.X + b.X, a.Y + b.Y);
        }
        
        // Addition (Vector2F + float)
        public static Vector2F operator +(Vector2F v, float scalar)
        {
            return new Vector2F(v.X + scalar, v.Y + scalar);
        }

        // Addition (float + Vector2F)
        public static Vector2F operator +(float scalar, Vector2F v)
        {
            return new Vector2F(v.X + scalar, v.Y + scalar);
        }

        // Subtraction (Vector2F - Vector2F)
        public static Vector2F operator -(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.X - b.X, a.Y - b.Y);
        }
        
        // Subtraction (Vector2F + float)
        public static Vector2F operator -(Vector2F v, float scalar)
        {
            return new Vector2F(v.X - scalar, v.Y - scalar);
        }

        // Subtraction (float + Vector2F)
        public static Vector2F operator -(float scalar, Vector2F v)
        {
            return new Vector2F(scalar - v.X, scalar - v.Y);
        }

        // Multiplication (Vector2F * Vector2F)
        public static Vector2F operator *(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.X * b.X, a.Y * b.Y);
        }

        // Multiplication (Vector2F * float)
        public static Vector2F operator *(Vector2F v, float scalar)
        {
            return new Vector2F(v.X * scalar, v.Y * scalar);
        }

        // Multiplication (float * Vector2F)
        public static Vector2F operator *(float scalar, Vector2F v)
        {
            return new Vector2F(v.X * scalar, v.Y * scalar);
        }

        // Division (Vector2F / Vector2F)
        public static Vector2F operator /(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.X / b.X, a.Y / b.Y);
        }

        // Division (Vector2F / float)
        public static Vector2F operator /(Vector2F v, float scalar)
        {
            return new Vector2F(v.X / scalar, v.Y / scalar);
        }

        // Division (float / Vector2F)
        public static Vector2F operator /(float scalar, Vector2F v)
        {
            return new Vector2F(v.X / scalar, v.Y / scalar);
        }

        // DotProduct (Vector2F * Vector2F)
        public static float DotProduct(Vector2F a, Vector2F b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        /// <summary>
        /// Returns the magnitude (length) of the vector.
        /// </summary>
        public float Length()
        {
            return MathF.Sqrt(X * X + Y * Y);
        }

        /// <summary>
        /// Returns a normalized version of this vector.
        /// </summary>
        public Vector2F Normalize()
        {
            float length = Length();
            return length > 0.001f ?
                new Vector2F(X / length, Y / length) :
                new Vector2F(0, 0);
        }
        
        /// <summary>
        /// Returns a lerp of this vector.
        /// </summary>
        public static Vector2F Lerp(Vector2F a, Vector2F b, float t)
        {
            return new Vector2F(
                MathUtil.Lerp(a.X, b.X, t),
                MathUtil.Lerp(a.Y, b.Y, t)
            );
        }

        /// <summary>
        /// Returns a angle between two vectors (unit: radians). 
        /// </summary>
        public static float AngleBetween(Vector2F a, Vector2F b)
        {
            float dot = a.X * b.X + a.Y * b.Y;
            float magProduct = a.Length() * b.Length();
            if (magProduct == 0f) return 0f;

            float cosTheta = MathUtil.ClampF(dot / magProduct, -1f, 1f);
            return MathF.Acos(cosTheta);  // Unit: radians
        }

        /// <summary>
        /// Returns a new Vector2F that is this vector rotated by the given angle (in radians).
        /// </summary>
        /// <param name="angleRadians">The angle to rotate by, in radians.</param>
        /// <returns>The rotated vector.</returns>
        public Vector2F Rotate(float angleRadians)
        {
            float cos = MathF.Cos(angleRadians);
            float sin = MathF.Sin(angleRadians);

            return new Vector2F(
                X * cos - Y * sin,
                X * sin + Y * cos
            );
        }
    }
}