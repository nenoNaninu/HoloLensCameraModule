using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloLensCameraModule
{
    public class HandPositionPloter : MonoBehaviour, ISourcePositionHandler, ISourceStateHandler, IInputClickHandler
    {
        private Transform _children;

        public void OnInputClicked(InputClickedEventData eventData)
        {
        }

        public void OnPositionChanged(SourcePositionEventData eventData)
        {
            //Debug.Log("Position Change");
            gameObject.transform.position = eventData.GripPosition;
        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            //Debug.Log("Detect!!");
            _children.gameObject.SetActive(true);
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            //Debug.Log("Losted!!");
            _children.gameObject.SetActive(false);
        }

        // Use this for initialization
        void Start()
        {
            InputManager.Instance.AddGlobalListener(gameObject);
            _children = gameObject.transform.GetChild(0);
        }
    }
}