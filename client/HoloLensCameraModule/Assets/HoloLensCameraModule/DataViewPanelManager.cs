using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoloLensCameraModule
{
    /// <summary>
    /// 投げ込まれた物体と世界座標の対応をとって、
    /// </summary>
    public class DataViewPanelManager : MonoBehaviour
    {
        [SerializeField, Tooltip("物体の上に表示するためのpanelのprefab")]
        public GameObject PropertyPanelObj;

        /// <summary>
        /// 描画している最中の
        /// </summary>
        private Dictionary<DetectedObjectData3D, DataViewPanel> _drawingDataObjDic =
            new Dictionary<DetectedObjectData3D, DataViewPanel>();

        public void RedrawData(List<DetectedObjectData3D> detectedObjectData3DList)
        {
            var keys = _drawingDataObjDic.Select(x => x.Key);

            foreach (var detectedObjectData3D in detectedObjectData3DList)
            {
                var worldSpaceData = keys.Where(x =>
                {
                    var vec = x.WorldPosition - detectedObjectData3D.WorldPosition;
                    if (vec.sqrMagnitude < 0.1f)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                })
                .OrderBy(x =>
                {
                    var vec = x.WorldPosition - detectedObjectData3D.WorldPosition;
                    return vec.sqrMagnitude;
                })
                .FirstOrDefault();

                //同一の物体があった場合
                if (worldSpaceData != null)
                {
                    var panel = _drawingDataObjDic[worldSpaceData];
                    panel.DetectedObjectData3D = detectedObjectData3D;
                }
                else//新しい物体の場合。
                {
                    GameObject newPropertyView = Instantiate(PropertyPanelObj);
                    DataViewPanel viewPanel = newPropertyView.GetComponent<DataViewPanel>();
                    viewPanel.DetectedObjectData3D = detectedObjectData3D;
                    _drawingDataObjDic.Add(detectedObjectData3D, viewPanel);
                }
            }
        }
    }
}