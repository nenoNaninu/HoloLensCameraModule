using System;
using UnityEngine;
using UnityEngine.UI;

namespace HoloLensCameraModule
{
    public class DataViewPanel : MonoBehaviour
    {
        private DetectedObjectData3D _detectedObjectData3D;
        private Text _text;

        public DetectedObjectData3D DetectedObjectData3D
        {
            get { return _detectedObjectData3D; }
            set
            {
                _detectedObjectData3D = value;
                if (_text == null)
                {
                    _text = gameObject.GetComponentInChildren<Text>();
                }
                _text.text = $"クラス: {value.ClassLabel}{Environment.NewLine}";
                transform.position = value.WorldPosition;
            }
        }

        // Use this for initialization
        void Start()
        {
            _text = gameObject.GetComponentInChildren<Text>();
        }
    }
}
