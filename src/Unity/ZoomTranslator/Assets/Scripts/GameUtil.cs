using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace HoneycombUtility
{
    public class GameUtil
    {
        public static int Limit(int src, int minInclude, int maxInclude)
        {
            return Math.Min(maxInclude, Math.Max(minInclude, src));
        }
        public static float Limit(float src, float min, float max)
        {
            return Mathf.Min(max, Mathf.Max(min, src));
        }
        public static Vector3 AddVector(Vector3 src, Vector3 add)
        {
            return new Vector3(src.x + add.x, src.y + add.y, src.z + add.z);
        }
        public static Vector2 AddVector(Vector2 src, Vector2 add)
        {
            return new Vector2(src.x + add.x, src.y + add.y);
        }
        public static Vector2 DivVector(Vector2 src, float div)
        {
            return new Vector2(src.x / div, src.y / div);
        }
        public static bool IsInRange(int n, int minInclude, int maxExclude)
        {
            return minInclude <= n && n < maxExclude;
        }
        public static bool IsInRange(float n, float minInclude, float maxExclude)
        {
            return minInclude <= n && n < maxExclude;
        }

        /* 255という文字列をColorで使う0.0f-1.0fに変換 */
        public static float ConvertColor(string s)
        {
            float r = 1.0f;
            int tmp;
            if (int.TryParse(s, out tmp))
            {
                tmp = GameUtil.Limit(tmp, 0, 255);
                r = (float)tmp / 255.0f;
            }
            else
            {
                Debug.LogError("MonumentAttributeModel color : " + s);
            }

            return r;
        }
        public static Color ConvertColorFromHex8(string ffffffff)
        {
            if (ffffffff.Length < 8)
            {
                while (ffffffff.Length < 8)
                {
                    ffffffff += "F";
                }
            }

            ffffffff = ffffffff.ToLowerInvariant();

            byte[] b = new byte[4];
            byte tmp = 0;
            b[0] = byte.TryParse(ffffffff.Substring(0, 2), out tmp) ? tmp : (byte)1;
            b[1] = byte.TryParse(ffffffff.Substring(2, 2), out tmp) ? tmp : (byte)1;
            b[2] = byte.TryParse(ffffffff.Substring(4, 2), out tmp) ? tmp : (byte)1;
            b[3] = byte.TryParse(ffffffff.Substring(6, 2), out tmp) ? tmp : (byte)1;
            return new Color(b[0], b[1], b[2], b[3]);
        }



        public static void ResetTransform(Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }
        public static void ResetTransform(GameObject g)
        {
            ResetTransform(g.transform);
        }
        public static RectTransform ResetRectTransform(GameObject o)
        {
            RectTransform r = o.GetComponent<RectTransform>();
            if (r == null) return null;
            ResetTransform(o.transform);

            r.anchoredPosition = Vector2.zero;
            r.anchoredPosition3D = Vector3.zero;

            return r;
        }
        public static void ResetSizeDelta(GameObject o)
        {
            RectTransform r = o.GetComponent<RectTransform>();
            if (r == null) return;
            r.sizeDelta = Vector2.zero;
        }
        public static void SetExpandRectTransform(RectTransform t)
        {
            if (t == null) return;
            t.anchorMin = Vector2.zero;
            t.anchorMax = Vector2.one;
            t.offsetMin = Vector3.zero;
            t.offsetMax = Vector3.zero;
            t.sizeDelta = Vector2.zero;
            //t.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, 1000f);
            //t.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0f, 0f);
        }
        public static void ResetTransformStretchXY(GameObject o)
        {
            ResetRectTransform(o);
            SetExpandRectTransform(o.GetComponent<RectTransform>());
        }


        public static DateTime GetTimeNow()
        {
            DateTime ret = System.DateTime.Now;
            //ret = ret.AddDays(1); ログイン報酬デバッグ用
            //Debug.Log("DateTime(Now)     : " + System.DateTime.Now);
            return ret;

        }

        public static Vector2 MousePosition2Ortho(float orthographicSize)
        {
            float w = Screen.width;
            float h = Screen.height;
            Vector3 mp = Input.mousePosition;
            mp = new Vector3(
                (mp.x - 0.5f * w) / w,
                (mp.y - 0.5f * h) / h, 0f);
            return 2f * new Vector2(mp.x * orthographicSize * w / h, mp.y * orthographicSize);
        }

        /* 文字列の前後の「"」を削除 エクセル対応 */
        public static string DataTextDoubleQuoteFormatter(string t)
        {
            while (t.StartsWith("\""))
            {
                t = t.Remove(0, 1);
            }
            while (t.EndsWith("\""))
            {
                t = t.Remove(t.Length - 1, 1);
            }
            return t;
        }

        public static class Easing
        {
            // Adapted from source : http://www.robertpenner.com/easing/

            public static float Ease(float linearStep, float acceleration, EasingType type)
            {
                float easedStep = acceleration > 0 ? EaseIn(linearStep, type) :
                                  acceleration < 0 ? EaseOut(linearStep, type) :
                                  (float)linearStep;

                return MathHelper.Lerp(linearStep, easedStep, Math.Abs(acceleration));
            }

            public static float EaseIn(float linearStep, EasingType type)
            {
                switch (type)
                {
                    case EasingType.Step: return linearStep < 0.5 ? 0 : 1;
                    case EasingType.Linear: return (float)linearStep;
                    case EasingType.Sine: return Sine.EaseIn(linearStep);
                    case EasingType.Quadratic: return Power.EaseIn(linearStep, 2);
                    case EasingType.Cubic: return Power.EaseIn(linearStep, 3);
                    case EasingType.Quartic: return Power.EaseIn(linearStep, 4);
                    case EasingType.Quintic: return Power.EaseIn(linearStep, 5);
                }
                throw new NotImplementedException();
            }

            public static float EaseOut(float linearStep, EasingType type)
            {
                switch (type)
                {
                    case EasingType.Step: return linearStep < 0.5 ? 0 : 1;
                    case EasingType.Linear: return (float)linearStep;
                    case EasingType.Sine: return Sine.EaseOut(linearStep);
                    case EasingType.Quadratic: return Power.EaseOut(linearStep, 2);
                    case EasingType.Cubic: return Power.EaseOut(linearStep, 3);
                    case EasingType.Quartic: return Power.EaseOut(linearStep, 4);
                    case EasingType.Quintic: return Power.EaseOut(linearStep, 5);
                }
                throw new NotImplementedException();
            }

            public static float EaseInOut(float linearStep, EasingType easeInType, EasingType easeOutType)
            {
                return linearStep < 0.5 ? EaseInOut(linearStep, easeInType) : EaseInOut(linearStep, easeOutType);
            }
            public static float EaseInOut(float linearStep, EasingType type)
            {
                switch (type)
                {
                    case EasingType.Step: return linearStep < 0.5 ? 0 : 1;
                    case EasingType.Linear: return (float)linearStep;
                    case EasingType.Sine: return Sine.EaseInOut(linearStep);
                    case EasingType.Quadratic: return Power.EaseInOut(linearStep, 2);
                    case EasingType.Cubic: return Power.EaseInOut(linearStep, 3);
                    case EasingType.Quartic: return Power.EaseInOut(linearStep, 4);
                    case EasingType.Quintic: return Power.EaseInOut(linearStep, 5);
                }
                throw new NotImplementedException();
            }

            static class Sine
            {
                public static float EaseIn(float s)
                {
                    return (float)Math.Sin(s * MathHelper.HalfPi - MathHelper.HalfPi) + 1;
                }
                public static float EaseOut(float s)
                {
                    return (float)Math.Sin(s * MathHelper.HalfPi);
                }
                public static float EaseInOut(float s)
                {
                    return (float)(Math.Sin(s * MathHelper.Pi - MathHelper.HalfPi) + 1) / 2;
                }
            }

            static class Power
            {
                public static float EaseIn(float s, int power)
                {
                    return (float)Math.Pow(s, power);
                }
                public static float EaseOut(float s, int power)
                {
                    var sign = power % 2 == 0 ? -1 : 1;
                    return (float)(sign * (Math.Pow(s - 1, power) + sign));
                }
                public static float EaseInOut(float s, int power)
                {
                    s *= 2;
                    if (s < 1) return EaseIn(s, power) / 2;
                    var sign = power % 2 == 0 ? -1 : 1;
                    return (float)(sign / 2.0 * (Math.Pow(s - 2, power) + sign * 2));
                }
            }
        }

        public enum EasingType
        {
            Step,
            Linear,
            Sine,
            Quadratic,
            Cubic,
            Quartic,
            Quintic
        }

        public static class MathHelper
        {
            public const float Pi = (float)Math.PI;
            public const float HalfPi = (float)(Math.PI / 2);

            public static float Lerp(float from, float to, float step)
            {
                return (float)((to - from) * step + from);
            }
        }


        public static float EaseOutBounce(float t)
        {
            if ((t) < (1f / 2.75f))
            {
                return (7.5625f * t * t);
            }
            else if (t < (2f / 2.75f))
            {
                return (7.5625f * (t -= (1.5f / 2.75f)) * t + .75f);
            }
            else if (t < (2.5f / 2.75f))
            {
                return (7.5625f * (t -= (2.25f / 2.75f)) * t + .9375f);
            }
            else
            {
                return (7.5625f * (t -= (2.625f / 2.75f)) * t + .984375f);
            }
        }
        public static float EaseOutQuart(float t)
        {
            t = t - 1f;
            return -1.0f * (t * t * t * t - 1f);
        }
        public static float EaseOutBack(float t, float s)
        {
            t = t - 1;
            return (t * t * ((s + 1) * t * s) + 1);
        }



    }

    public class FileUtil
    {
        public static byte[] LoadLocalFile(string path)
        {
            byte[] bytes = null;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                bytes = new byte[fs.Length];
                int numBytesToRead = (int)fs.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    // Read may return anything from 0 to numBytesToRead.
                    int n = fs.Read(bytes, numBytesRead, numBytesToRead);

                    // Break when the end of the file is reached.
                    if (n == 0)
                        break;

                    numBytesRead += n;
                    numBytesToRead -= n;
                }
                numBytesToRead = bytes.Length;
            }
            return bytes;
        }
        public static void SaveLocalFileCore(string path, byte[] data)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
            // iCloudの保存対象から外す
#if UNITY_IOS
        UnityEngine.iOS.Device.SetNoBackupFlag(path);
#endif

        }

    }



    /******************************************************************************* 
     * color 
     */

    /// <summary>
    /// HSV色空間を扱うクラス
    /// </summary>
    public static class ColorHSV
    {
        /// <summary>
        /// HSV色空間による指定でUnityEngine.Colorを作成します。
        /// </summary>
        /// <param name="h">色相(Hue) 0-360</param>
        /// <param name="s">彩度(Saturation) 0-255</param>
        /// <param name="v">明度(Value) 0-255</param>
        /// <returns></returns>
        public static Color FromHsv(int h, int s, int v)
        {
            while (h >= 360) h -= 360;
            while (h < 0) h += 360;
            if (s > 255) s = 255;
            if (s < 0) s = 0;
            if (v > 255) v = 255;
            if (v < 0) v = 0;

            return FromHsv((float)h, (float)s / 255.0f, (float)v / 255.0f);
        }




        /// <summary>
        /// HSV色空間による指定でUnityEngine.Colorを作成します。
        /// </summary>
        /// <param name="h">色相(Hue) 0.0-360.0</param>
        /// <param name="s">彩度(Saturation) 0.0-1.0</param>
        /// <param name="v">明度(Value) 0.0-1.0</param>
        /// <returns></returns>
        private static Color FromHsv(float h, float s, float v)
        {
            if (h > 360.0) throw new ArgumentOutOfRangeException("h", h, "0～360で指定してください。");
            if (h < 0.0) throw new ArgumentOutOfRangeException("h", h, "0～360で指定してください。");
            if (s > 1.0) throw new ArgumentOutOfRangeException("s", s, "0.0～1.0で指定してください。");
            if (s < 0.0) throw new ArgumentOutOfRangeException("s", s, "0.0～1.0で指定してください。");
            if (v > 1.0) throw new ArgumentOutOfRangeException("v", v, "0.0～1.0で指定してください。");
            if (v < 0.0) throw new ArgumentOutOfRangeException("v", v, "0.0～1.0で指定してください。");

            Color resColor = Color.clear;

            if (s == 0.0) //Gray
            {
                int rgb = Convert.ToInt16((float)(v * 255));
                resColor = new Color(rgb, rgb, rgb);
            }
            else
            {
                int Hi = (int)(Mathf.Floor(h / 60.0f) % 6.0f);
                float f = (h / 60.0f) - Hi;

                float p = v * (1 - s);
                float q = v * (1 - f * s);
                float t = v * (1 - (1 - f) * s);

                float r = 0.0f;
                float g = 0.0f;
                float b = 0.0f;

                switch (Hi)
                {
                    case 0: r = v; g = t; b = p; break;
                    case 1: r = q; g = v; b = p; break;
                    case 2: r = p; g = v; b = t; break;
                    case 3: r = p; g = q; b = v; break;
                    case 4: r = t; g = p; b = v; break;
                    case 5: r = v; g = p; b = q; break;
                    default: break;
                }

                resColor = new Color(r, g, b);
            }

            return resColor;
        }
    }




    /// <summary>
    /// UnityEngine.ColorのHSV色空間への拡張
    /// </summary>
    public static class ColorExtension
    {
        /// <summary>
        /// 色相(Hue)
        /// 0-360
        /// </summary>
        public static int h(this Color c)
        {
            float min = Mathf.Min(new float[] { c.r, c.g, c.b });
            float max = Mathf.Max(new float[] { c.r, c.g, c.b });

            if (max == 0) return 0;

            float h = 0;

            if (max == c.r) h = 60 * (c.g - c.b) / (max - min) + 0;
            else if (max == c.g) h = 60 * (c.b - c.r) / (max - min) + 120;
            else if (max == c.b) h = 60 * (c.r - c.g) / (max - min) + 240;

            if (h < 0) h += 360;

            return (int)Mathf.Round(h);
        }




        /// <summary>
        /// 彩度(Saturation)
        /// 0-255
        /// </summary>
        public static int s(this Color c)
        {
            float min = Mathf.Min(new float[] { c.r, c.g, c.b });
            float max = Mathf.Max(new float[] { c.r, c.g, c.b });

            if (max == 0) return 0;
            return (int)(255 * (max - min) / max);
        }


        /// <summary>
        /// 明度(Value)
        /// 0-255
        /// </summary>
        public static int v(this Color c)
        {
            return (int)(255.0f * Mathf.Max(new float[] { c.r, c.g, c.b }));
        }




        /// <summary>
        /// 現在の色を基準にHSV色空間を移動します。
        /// </summary>
        /// <param name="c"></param>
        /// <param name="offsetH">色相(Hue)オフセット値</param>
        /// <param name="offsetS">彩度(Saturation)オフセット値</param>
        /// <param name="offsetV">明度(Value)オフセット値</param>
        /// <returns></returns>
        public static Color Offset(this Color c, int offsetH, int offsetS, int offsetV)
        {
            int newH = (int)(c.h() + offsetH);
            int newS = (int)(c.s() + offsetS);
            int newV = (int)(c.v() + offsetV);

            return ColorHSV.FromHsv(newH, newS, newV);
        }


        /// <summary>
        /// 現在の色を文字列として返します。
        /// </summary>
        /// <returns></returns>
        public static string ToString2(this Color c)
        {
            return string.Format("R={0}, G={1}, B={2}, H={3}, S={4}, V={5}",
            new object[]{c.r,c.g,c.b,  c.h(),c.s(),c.v() });
        }



    }
}