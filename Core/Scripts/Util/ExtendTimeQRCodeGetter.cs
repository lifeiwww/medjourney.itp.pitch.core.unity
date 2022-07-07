using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using dreamcube.unity.Core.Scripts.Util;
using OdinSerializer.Utilities;
using Serilog;
using UnityEngine;
using UnityEngine.UI;

namespace manutd
{
    public class ExtendTimeQRCodeGetter : MonoBehaviour
    {
        // regex pattern for image extraction
        private const string Pattern = @"(?<=<img src=""data:image/png;base64,)(.*)(?=\"")";

        [SerializeField] private RawImage QRImageExtend;
        [SerializeField] private RawImage QRImageAlmostUp;
        private readonly HttpClient _client = new HttpClient();


        private async void GetTextureFromURLAsync(string url)
        {
            Log.Debug($"Using QR from {url}");

            var response = await GetQrCodeStringAsync(url);

            if (response.IsNullOrWhitespace())
            {
                Log.Debug("Using default QR Code");
                QRImageExtend.texture =
                    Extensions.LoadTexture(Path.Combine(Application.streamingAssetsPath, Common.DEFAULT_QRCODE_FILE));
                QRImageAlmostUp.texture =
                    Extensions.LoadTexture(Path.Combine(Application.streamingAssetsPath, Common.DEFAULT_QRCODE_FILE));
            }
            else
            {
                var tex = GetQrCodeTexture(response);
                QRImageExtend.texture = tex;
                QRImageAlmostUp.texture = tex;
            }
        }

        private async Task<string> GetQrCodeStringAsync(string uri)
        {
            var response = "";

            var isUri = Uri.IsWellFormedUriString(uri, UriKind.Absolute);
            if (isUri)
                try
                {
                    // returns html with base64 as img src :/
                    response = await _client.GetStringAsync(uri);

                    // replace line breaks
                    response = Regex.Replace(response, @"\t|\n|\r", "");

                    // get base64 data
                    response = Regex.Match(response, Pattern, RegexOptions.IgnoreCase).Groups[1].Value;
                }
                catch (HttpRequestException e)
                {
                    Log.Debug("Error getting QR Code from {0}", uri);
                    Log.Debug("QR Code Error :{0} ", e.Message);
                }

            return response;
        }

        private Texture2D GetQrCodeTexture(string b64String)
        {
            var bytes = Convert.FromBase64String(b64String);
            var texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            return texture;
        }

        public void ResetTexture()
        {
            var clearTex = new Texture2D(2, 2); // Create new "empty" texture
            QRImageExtend.texture = QRImageAlmostUp.texture =
                Extensions.LoadTexture(Path.Combine(Application.streamingAssetsPath, Common.DEFAULT_QRCODE_FILE));
        }
    }
}