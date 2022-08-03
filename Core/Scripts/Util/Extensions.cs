using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace dreamcube.unity.Core.Scripts.Util
{
    public static class Extensions
    {

        //https://stackoverflow.com/questions/4925718/c-dynamic-runtime-cast
        public static dynamic Cast(dynamic obj, Type castTo)
        {
            return Convert.ChangeType(obj, castTo);
        }

        // Args: input min, input max, map to range min, range max, number to input
        // Inputs outside a1 to a2 will fall outside the output range
        public static float mapRange(float a1, float a2, float b1, float b2, float s)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }

        // Output is clamped to range: b1 to b2
        public static float mapRangeMinMax(float a1, float a2, float b1, float b2, float s)
        {
            var value = b1 + (s - a1) * (b2 - b1) / (a2 - a1);
            value = Mathf.Max(b1, value);
            value = Mathf.Min(value, b2);
            return value;
        }

        /// <summary>
        /// Animations
        /// </summary>
        public static IEnumerator SpriteFadeIn(SpriteRenderer target, float duration = .5f)
        {
            float elapsedTime = 0;
            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                var alpha = elapsedTime / duration;
                target.color = new Color(1, 1, 1, alpha);
                yield return new WaitForEndOfFrame();
            }
            target.color = Color.white;
        }

        public static IEnumerator SpriteFadeOut(SpriteRenderer target, float duration = .5f)
        {
            float elapsedTime = 0;

            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                var alpha = 1 - elapsedTime / duration;
                target.color = new Color(1, 1, 1, alpha);
                yield return new WaitForEndOfFrame();
            }

            target.color = new Color(1, 1, 1, 0);
        }

        public static IEnumerator TMPFadeIn(TMP_Text target, float duration = .5f, float delay = 0.0f)
        {
            float elapsedTime = 0;
            float delayTime = 0;
            target.faceColor = new Color(target.faceColor.r, target.faceColor.g, target.faceColor.b, 0);
            Color originalColor = target.faceColor;

            // delay
            while (delayTime < delay) {
                delayTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                var alpha = elapsedTime / duration;
                target.faceColor = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return new WaitForEndOfFrame();
            }

            target.faceColor = Color.white;
        }

        public static IEnumerator TMPFadeOut(TMP_Text target, float duration = .5f, float delay = 0.0f)
        {
            float elapsedTime = 0;
            float delayTime = 0;

            Color originalColor = target.faceColor;
            target.faceColor = new Color(target.faceColor.r, target.faceColor.g, target.faceColor.b, 1);

            // delay
            while (delayTime < delay) {
                delayTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                var alpha = 1 - elapsedTime / duration;
                target.faceColor = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                target.alpha = alpha;
                yield return new WaitForEndOfFrame();
            }

            //Log.Debug("target.faceColor " + target.faceColor);
            target.faceColor = new Color(1, 1, 1, 0);
        }

        public static IEnumerator fadeOutAudio(AudioSource source, float duration = 1.0f)
        {
            float elapsedTime = 0;
            source.volume = 1f;

            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                source.volume = Mathf.Lerp(source.volume, 0f, elapsedTime / duration);
                yield return null;// new WaitForEndOfFrame();
            }
        }

        public static IEnumerator fadeInAudio(AudioSource source, float duration = 1.0f)
        {
            float elapsedTime = 0;

            //source.volume = 0f;
            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                source.volume = Mathf.Lerp(source.volume, 1f, elapsedTime / duration);
                yield return null;// new WaitForEndOfFrame();
            }
        }

        public static Color AdjustAlpha(Color target, float alpha)
        {
            return new Color(target.r, target.g, target.b, alpha);
        }

        public static Color RandomColor()
        {
            return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
        }

        /// <summary>
        /// Camera/world space
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static Bounds GetOrthographicBounds(Camera camera)
        {
            var screenAspect = Screen.width / (float)Screen.height;
            var cameraHeight = camera.orthographicSize * 2;
            var bounds = new Bounds(
                camera.transform.position,
                new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
            return bounds;
        }

        public static float getCameraWidth(Camera camera)
        {
            var cameraExtent = GetOrthographicBounds(camera);
            //Log.Debug("camera extent: " + cameraExtent.extents);
            //Log.Debug("mTextMesh.GetRenderedValues(): " + mTextMesh.GetRenderedValues().x);
            var CameraWidth = cameraExtent.size.x / 2;
            return CameraWidth;
        }

        public static float getCameraHight(Camera camera)
        {
            var cameraExtent = GetOrthographicBounds(camera);
            //Log.Debug("camera extent: " + cameraExtent.extents);
            //Log.Debug("mTextMesh.GetRenderedValues(): " + mTextMesh.GetRenderedValues().x);
            var CameraHeight = cameraExtent.size.y / 2;
            return CameraHeight;
        }

        public static Vector3 ConvertMousePosToWorld(Camera cam)
        {
            var input = Input.mousePosition;
            input = new Vector3(input.x, input.y, cam.nearClipPlane);
            return cam.ScreenToWorldPoint(input);
        }

        /// <summary>
        /// string operations
        /// </summary>
        public static string removeFirstSubstring(string source, string toRemove)
        {
            var index = source.IndexOf(toRemove, StringComparison.CurrentCulture);
            var cleanPath = index < 0 ? source : source.Remove(index, toRemove.Length);
            return cleanPath.Trim();
        }

        public static string Truncate(string value, int maxLength, string replacement="..." )
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength)+replacement;
        }

        /// <summary>
        /// file system
        /// </summary>
        public static DirectoryInfo[] ListSubdirectoriesInDirectory(string filePath)
        {
            var dir = new DirectoryInfo(filePath);
            return dir.GetDirectories();
        }

        public static FileInfo[] ListFilesInDirectory(DirectoryInfo dirInfo, string extentionType = "*")
        {
            return dirInfo.GetFiles("*" + extentionType);
        }

        public static FileInfo[] ListFilesInDirectory(string filePath, string extentionType = "*")
        {
            var dir = new DirectoryInfo(filePath);
            var info = dir.GetFiles("*." + extentionType);
            return info;
        }

        public static int countSubdirectories(string filePath)
        {
            var dir = new DirectoryInfo(filePath);
            return dir.GetFiles().Length;
        }

        /// <summary>
        /// Load assets
        /// </summary>
        public static Texture2D LoadTexture(string FilePath)
        {
            // Load a PNG or JPG file from disk to a Texture2D
            // Returns null if load fails
            Texture2D Tex2D;
            byte[] FileData;

            if (File.Exists(FilePath))
                try
                {
                    FileData = File.ReadAllBytes(FilePath);
                    Tex2D = new Texture2D(2, 2); // Create new "empty" texture

                    if (Tex2D.LoadImage(FileData)) // Load the image data into the texture (size is set automatically)
                        return Tex2D; // If data is readable -> return texture
                } catch (Exception ex) {
                    Debug.LogError($"could not load texture {ex}");
                }

            return null;                     // Return null if load failed
        }

        public static List<Texture2D> loadTexturesFromDirectorty(string dirPath, string extensionType = "*")
        {
            var textures = new List<Texture2D>();
            var files = ListFilesInDirectory(dirPath, extensionType);

            foreach (var file in files) {
                var tex = LoadTexture(file.FullName);
                textures.Add(tex);
            }

            return textures;
        }


        public static Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
        {
            // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
            var SpriteTexture = LoadTexture(FilePath);
            var NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0.5f, 0.5f), PixelsPerUnit);
            return NewSprite;
        }

        public static Texture2D textureFromSprite(Sprite sprite)
        {
            if (Math.Abs(sprite.rect.width - sprite.texture.width) > Mathf.Epsilon) {
                var newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                var newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                    (int)sprite.textureRect.y,
                    (int)sprite.textureRect.width,
                    (int)sprite.textureRect.height);
                newText.SetPixels(newColors);
                newText.Apply();
                return newText;
            }

            return sprite.texture;
        }

        public static void SaveStringToFile( string input, string filename , bool useStreamingAssets = true )
        {
            var filePath = useStreamingAssets ? Path.Combine(Application.streamingAssetsPath, filename) : filename;
            File.WriteAllText(filePath, input);
        }

        public static string LoadStringFromFile(string filename, bool useStreamingAssets = true)
        {
            // Path.Combine combines strings into a file path
            var filePath = useStreamingAssets ? Path.Combine(Application.streamingAssetsPath, filename) : filename;
            if (File.Exists(filePath)) // Read the json from the file into a string
                return File.ReadAllText(filePath);

            Debug.LogError($"Cannot load file from {filePath}");
            return "";
        }

        ///
        /// date/time
        ///
        public static string getTimeSpan(float seconds, bool millies = true)
        {
            var timeSpan = TimeSpan.FromSeconds(seconds);
            if (!millies)
                return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
        }

        public static int getCurrentSecondsSinceEpoch()
        {
            var epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
            return currentEpochTime;
        }

        public static string getTimeNowFormatted(string datePatt = @"yyyy-MM-dd-HH:mm:ss:ff")
        {
            return DateTime.Now.ToString(datePatt);
        }

        public static class GuiRect
        {
            private static Texture2D _staticRectTexture;
            private static GUIStyle _staticRectStyle;

            // Note that this function is only meant to be called from OnGUI() functions.
            public static void GUIDrawRect(Rect position, Color color)
            {
                if (_staticRectTexture == null) _staticRectTexture = new Texture2D(1, 1);

                if (_staticRectStyle == null) _staticRectStyle = new GUIStyle();

                _staticRectTexture.SetPixel(0, 0, color);
                _staticRectTexture.Apply();
                _staticRectStyle.normal.background = _staticRectTexture;
                GUI.Box(position, GUIContent.none, _staticRectStyle);
            }
        }

        public static class MemberInfoGetting
        {
            public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
            {
                var expressionBody = (MemberExpression)memberExpression.Body;
                return expressionBody.Member.Name;
            }
        }
    }
}
