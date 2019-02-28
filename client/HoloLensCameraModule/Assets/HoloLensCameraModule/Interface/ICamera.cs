﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensCameraModule
{
    public interface ICamera : IDisposable
    {
        void Init();

        /// <summary>
        /// 撮影の最中ならtrue
        /// </summary>
        bool IsShooting { get; }

        /// <summary>
        /// cameraToWorldMatrix,projectionMatrix,textureの順で入ってくる。
        /// </summary>
        /// <param name="photoAction">
        /// 引数は
        /// cameraToWorldMatrix,projectionMatrix,texture
        /// の順
        /// </param>
        /// <returns>撮影可能であればtrue,不可能であればfalseを返す。</returns>
        bool TakePhoto(Action<Matrix4x4, Matrix4x4, Texture2D> photoAction);

        /// <summary>
        /// cameraToWorldMatrix,projectionMatrix,imgRawData,imgHeight,imgWidthの順で入ってくる。
        /// </summary>
        /// <param name="photoAction">
        /// 引数は
        /// cameraToWorldMatrix,projectionMatrix,imgRawData,imgHeight,imgWidth
        /// の順
        /// </param>
        /// <returns>撮影可能であればtrue,不可能であればfalseを返す。</returns>
        bool TakePhoto(Action<Matrix4x4, Matrix4x4, List<byte>, int, int> photoAction);
    }
}