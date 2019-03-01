using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloLensCameraModule
{

    /// <summary>
    /// 今回のアプリケーションの一番でかいManagerクラス。
    /// </summary>
    [RequireComponent(typeof(DataViewPanelManager))]
    public class CameraModuleSample : MonoBehaviour, IInputClickHandler
    {
        /// <summary>
        /// falseだとAir-tapしたタイミングで撮影。
        /// trueだと(いまの実装だと)5秒おきに撮影。
        /// </summary>
        public bool RealTimeDetection = false;

        [SerializeField] private GameObject _handPlotObj;

        private ICamera _colorCameraObject;
        private ICamera _depthCameraObject;
        private IObjectDetector<DetectedObjectData2D> _objectDetector;

        /// <summary>
        /// オブジェクトの上にプロパティを表示する。
        /// そのプロパティが表示されているGameObjectの参照を握っておく。
        /// </summary>
        private List<GameObject> _propertyViewList = new List<GameObject>();

        private DataViewPanelManager _dataViewPanelManager;


        private bool _canTakePhoto = true;

        // Use this for initialization
        void Start()
        {
            _colorCameraObject = ColorCameraInject();
            _depthCameraObject = DepthCameraInject();
            _objectDetector = ObjectDetectorInject();
            _dataViewPanelManager = gameObject.GetComponent<DataViewPanelManager>();

            if (RealTimeDetection)
            {
                StartCoroutine(TakePhotoCoroutine());
            }
            else
            {
                InputManager.Instance.AddGlobalListener(gameObject);
            }

        }

        /// <summary>
        /// 一定間隔で発火
        /// </summary>
        public void TakePhoto()
        {
            _colorCameraObject?.TakePhoto(async (camera2WorkdMatrix, projectionMatrix, imageRawdata, height, width) =>
            {
                //ここでカメラから実際のオブジェクトの対応を取る。

                List<DetectedObjectData2D> objectData2DList = await _objectDetector.ObjectDetecte(imageRawdata, height, width);
                var currentDetectedObjectData3D = new List<DetectedObjectData3D>();
                int x = 0, y = 0;
                if (CoordinateTransfer.WorldPos2ImagePos(_handPlotObj.transform.position, projectionMatrix,
                    camera2WorkdMatrix, height, width, ref x, ref y))
                {
                    Debug.Log($"handPosition on image : {x}, {y}");
                }

                //今画面に映っている物体の位置が取得できたやつらを保存。
                foreach (var objectData2D in objectData2DList)
                {
                    Vector3 worldPos;
                    if (CoordinateTransfer.ImagePos2WorldPos(objectData2D.X, objectData2D.Y, height, width, projectionMatrix,
                        camera2WorkdMatrix, out worldPos))
                    {
                        currentDetectedObjectData3D.Add(new DetectedObjectData3D(worldPos, objectData2D));
                    }
                }
                _dataViewPanelManager.RedrawData(currentDetectedObjectData3D);
            });

            _depthCameraObject?.TakePhoto(async (camera2WorkdMatrix, projectionMatrix, imageRawdata, height, width) =>
            {
                await _objectDetector.ObjectDetecte(imageRawdata, height, width);
            });
        }

        /// <summary>
        /// デバッグの時と実機の時でカメラのオブジェクトを切り替える。
        /// </summary>
        /// <returns></returns>
        ICamera ColorCameraInject()
        {
#if WINDOWS_UWP
            return new HoloLensColorCamera();
#else
            return null;
#endif
        }

        ICamera DepthCameraInject()
        {
#if WINDOWS_UWP
            return new HoloLensDepthCamera();
#else
            return null;
#endif
        }

        IObjectDetector<DetectedObjectData2D> ObjectDetectorInject()
        {
#if WINDOWS_UWP
            return new ObjectDetectorUsingServer();
#else
            return null;
#endif
        }

        IEnumerator TakePhotoCoroutine()
        {
            yield return new WaitForSeconds(10);

            while (_canTakePhoto)
            {
                TakePhoto();
                yield return new WaitForSeconds(5);
            }
        }

        void OnApplicationQuit()
        {
            _objectDetector?.Dispose();
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            TakePhoto();
        }
    }
}


