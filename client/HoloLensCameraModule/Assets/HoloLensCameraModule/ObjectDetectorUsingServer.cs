using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

#if WINDOWS_UWP
using Windows.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
#endif

namespace HoloLensCameraModule
{
    /// <summary>
    /// サーバーに送信
    /// </summary>
    public class ObjectDetectorUsingServer : IObjectDetector<DetectedObjectData2D>
    {
        public bool Calclating { get; } = false;

        private string _serverURL = "";

#if WINDOWS_UWP
        private static HttpClient _client;
#endif

        static ObjectDetectorUsingServer()
        {
#if WINDOWS_UWP
            _client = new HttpClient();
#endif
        }

        public ObjectDetectorUsingServer()
        {
#if WINDOWS_UWP

            Task.Run(async () =>
            {
                using (var stream = await KnownFolders.Objects3D.OpenStreamForReadAsync("DetectionServerUrl.txt"))
                {
                    var strRaw = new byte[stream.Length];
                    await stream.ReadAsync(strRaw, 0, strRaw.Length);
                    this._serverURL = System.Text.Encoding.UTF8.GetString(strRaw);
                }
            });
#endif
        }

        public async Task<List<DetectedObjectData2D>> ObjectDetecte(Texture2D cameraPhotoImage)
        {
#if WINDOWS_UWP
            string endpoint = _serverURL;
            var bytes = cameraPhotoImage.EncodeToPNG();
            var content = new ByteArrayContent(bytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            HttpResponseMessage response = await _client.PostAsync(endpoint, content);

            string jsonData = await response.Content.ReadAsStringAsync();
            List<DetectedObjectData2D> deserializeDatas =
                await Task.Run(() => JsonConvert.DeserializeObject<List<DetectedObjectData2D>>(jsonData));

            return deserializeDatas;
#else
            return null;
#endif
        }

        public void Dispose()
        {
#if WINDOWS_UWP
            _client?.Dispose();
#endif
        }

        public async Task<List<DetectedObjectData2D>> ObjectDetecte(List<byte> cameraPhotoRawImg, int imageHeight, int imageWidth)
        {
#if WINDOWS_UWP
            try
            {
                string endpoint = _serverURL;
                var content = new ByteArrayContent(cameraPhotoRawImg.ToArray());
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                content.Headers.Add("width", imageWidth.ToString());
                content.Headers.Add("height", imageHeight.ToString());
                HttpResponseMessage response = await _client.PostAsync(endpoint, content);

                string jsonData = await response.Content.ReadAsStringAsync();
                List<DetectedObjectData2D> deserializeDatas =
                    await Task.Run(() => JsonConvert.DeserializeObject<List<DetectedObjectData2D>>(jsonData));

                return deserializeDatas;
            }
            catch (Exception e)
            {
                Debug.LogError("ObjectDetecteでエラー発生" + e.Message);
                return new List<DetectedObjectData2D>();
            }
#else
            return null;
#endif
        }
    }
}

