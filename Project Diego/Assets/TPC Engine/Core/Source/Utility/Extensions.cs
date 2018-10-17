using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TPCEngine.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrayInitial"></param>
        /// <param name="arrayToAppend"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] Append<T>(this T[] arrayInitial, T[] arrayToAppend)
        {
            if (arrayToAppend == null)
            {
                throw new ArgumentNullException("The appended object cannot be null");
            }
            if ((arrayInitial is string) || (arrayToAppend is string))
            {
                throw new ArgumentException("The argument must be an enumerable");
            }
            T[] ret = new T[arrayInitial.Length + arrayToAppend.Length];
            arrayInitial.CopyTo(ret, 0);
            arrayToAppend.CopyTo(ret, arrayInitial.Length);

            return ret;
        }

        /// <summary>
        /// Normalized the angle. between -180 and 180 degrees
        /// </summary>
        /// <param Name="eulerAngle">Euler angle.</param>
        public static Vector3 NormalizeAngle(this Vector3 eulerAngle)
        {
            var delta = eulerAngle;

            if (delta.x > 180) delta.x -= 360;
            else if (delta.x < -180) delta.x += 360;

            if (delta.y > 180) delta.y -= 360;
            else if (delta.y < -180) delta.y += 360;

            if (delta.z > 180) delta.z -= 360;
            else if (delta.z < -180) delta.z += 360;

            return new Vector3(delta.x, delta.y, delta.z); //round values to angle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="otherVector"></param>
        /// <returns></returns>
        public static Vector3 Difference(this Vector3 vector, Vector3 otherVector)
        {
            return otherVector - vector;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameObjet"></param>
        /// <param name="value"></param>
        public static void SetActiveChildren(this GameObject gameObjet, bool value)
        {
            foreach (Transform child in gameObjet.transform)
                child.gameObject.SetActive(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="layer"></param>
        public static void SetLayerRecursively(this GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float ClampAngle(float angle, float min, float max)
        {
            do
            {
                if (angle < -360)
                    angle += 360;
                if (angle > 360)
                    angle -= 360;
            } while (angle < -360 || angle > 360);

            return Mathf.Clamp(angle, min, max);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="pos"></param>
        /// <param name="clipPlaneMargin"></param>
        /// <returns></returns>
        public static ClipPlanePoints NearClipPlanePoints(this Camera camera, Vector3 pos, float clipPlaneMargin)
        {
            var clipPlanePoints = new ClipPlanePoints();

            var transform = camera.transform;
            var halfFOV = (camera.fieldOfView / 2) * Mathf.Deg2Rad;
            var aspect = camera.aspect;
            var distance = camera.nearClipPlane;
            var height = distance * Mathf.Tan(halfFOV);
            var width = height * aspect;
            height *= 1 + clipPlaneMargin;
            width *= 1 + clipPlaneMargin;
            clipPlanePoints.LowerRight = pos + transform.right * width;
            clipPlanePoints.LowerRight -= transform.up * height;
            clipPlanePoints.LowerRight += transform.forward * distance;

            clipPlanePoints.LowerLeft = pos - transform.right * width;
            clipPlanePoints.LowerLeft -= transform.up * height;
            clipPlanePoints.LowerLeft += transform.forward * distance;

            clipPlanePoints.UpperRight = pos + transform.right * width;
            clipPlanePoints.UpperRight += transform.up * height;
            clipPlanePoints.UpperRight += transform.forward * distance;

            clipPlanePoints.UpperLeft = pos - transform.right * width;
            clipPlanePoints.UpperLeft += transform.up * height;
            clipPlanePoints.UpperLeft += transform.forward * distance;

            return clipPlanePoints;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boxCollider"></param>
        /// <param name="torso"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static HitBarPoints GetBoundPoint(this BoxCollider boxCollider, Transform torso, LayerMask mask)
        {
            HitBarPoints bp = new HitBarPoints();
            var boxPoint = boxCollider.GetBoxPoint();
            Ray toTop = new Ray(boxPoint.top, boxPoint.top - torso.position);
            Ray toCenter = new Ray(torso.position, boxPoint.center - torso.position);
            Ray toBottom = new Ray(torso.position, boxPoint.bottom - torso.position);
            Debug.DrawRay(toTop.origin, toTop.direction, Color.red, 2);
            Debug.DrawRay(toCenter.origin, toCenter.direction, Color.green, 2);
            Debug.DrawRay(toBottom.origin, toBottom.direction, Color.blue, 2);
            RaycastHit hit;
            var dist = Vector3.Distance(torso.position, boxPoint.top);
            if (Physics.Raycast(toTop, out hit, dist, mask))
            {
                bp |= HitBarPoints.Top;
                Debug.Log(hit.transform.name);
            }
            dist = Vector3.Distance(torso.position, boxPoint.center);
            if (Physics.Raycast(toCenter, out hit, dist, mask))
            {
                bp |= HitBarPoints.Center;
                Debug.Log(hit.transform.name);
            }
            dist = Vector3.Distance(torso.position, boxPoint.bottom);
            if (Physics.Raycast(toBottom, out hit, dist, mask))
            {
                bp |= HitBarPoints.Bottom;
                Debug.Log(hit.transform.name);
            }

            return bp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boxCollider"></param>
        /// <returns></returns>
        public static BoxPoint GetBoxPoint(this BoxCollider boxCollider)
        {
            BoxPoint bp = new BoxPoint();
            bp.center = boxCollider.transform.TransformPoint(boxCollider.center);
            var height = boxCollider.transform.lossyScale.y * boxCollider.size.y;
            var ray = new Ray(bp.center, boxCollider.transform.up);

            bp.top = ray.GetPoint((height * 0.5f));
            bp.bottom = ray.GetPoint(-(height * 0.5f));

            return bp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boxCollider"></param>
        /// <returns></returns>
        public static Vector3 BoxSize(this BoxCollider boxCollider)
        {
            var length = boxCollider.transform.lossyScale.x * boxCollider.size.x;
            var width = boxCollider.transform.lossyScale.z * boxCollider.size.z;
            var height = boxCollider.transform.lossyScale.y * boxCollider.size.y;
            return new Vector3(length, height, width);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool Contains(this Enum keys, Enum flag)
        {
            if (keys.GetType() != flag.GetType())
                throw new ArgumentException("Type Mismatch");
            return (Convert.ToUInt64(keys) & Convert.ToUInt64(flag)) != 0;
        }

        /// <summary>
        /// Generate random position in the circle with specific radius
        /// </summary>
        /// <param name="radius">Circle radius</param>
        /// <returns>Return Vector3</returns>
        public static Vector3 RandomPositionInCircle(this Vector3 center, float radius)
        {
            Vector2 randomPos = Random.insideUnitCircle * radius;
            return new Vector3(center.x + randomPos.x, center.y, center.z + randomPos.y);
        }

        /// <summary>
        /// Generate random position in the rectangle
        /// </summary>
        /// <param name="lenght">Rectangle lenght</param>
        /// <param name="weight">Rectangle weight</param>
        /// <returns>Return Vector3</returns>
        public static Vector3 RandomPositionInRectangle(this Vector3 center, float lenght, float weight)
        {
            Vector3 position;
            position.x = Random.Range(center.x - weight / 2, center.x + weight / 2);
            position.y = center.y;
            position.z = Random.Range(center.z - lenght / 2, center.z + lenght / 2);
            return position;
        }

        /// <summary>
        /// Add spaces to this line
        /// 
        ///     Note: Spaces are added only between  capital letters.
        /// </summary>
        /// <returns>Return string</returns>
        public static string AddSpaces(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        /// <summary>
        /// Change all rigidbodys kinematic state of current transfrom
        /// </summary>
        /// <param name="transform">Transform whose change kinematic state</param>
        /// <param name="value">True/False</param>
        public static void SetKinematic(this Transform transform, bool value)
        {
            Rigidbody[] rigidbodies = transform.GetComponentsInChildren<Rigidbody>();
            Collider[] colliders = transform.GetComponentsInChildren<Collider>();
            for (int i = 1; i < rigidbodies.Length; i++)
            {
                rigidbodies[i].isKinematic = value;
                colliders[i].enabled = !value;
            }

        }

        /// <summary>
        /// Calculate center of the vectors
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static Vector3 CenterOfVectors(params Vector3[] vectors)
        {
            Vector3 sum = Vector3.zero;
            if (vectors == null && vectors.Length == 0)
                return sum;
                
            for (int i = 0; i < vectors.Length; i++)
                sum += vectors[i];
            return sum / vectors.Length;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public struct BoxPoint
    {
        public Vector3 top;
        public Vector3 center;
        public Vector3 bottom;

    }

    /// <summary>
    /// 
    /// </summary>
    public struct ClipPlanePoints
    {
        public Vector3 UpperLeft;
        public Vector3 UpperRight;
        public Vector3 LowerLeft;
        public Vector3 LowerRight;
    }
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum HitBarPoints
    {
        None = 0,
        Top = 1,
        Center = 2,
        Bottom = 4
    }
}