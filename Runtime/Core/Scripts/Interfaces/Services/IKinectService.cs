using UnityEngine;

namespace HRYooba.Kinect.Core.Services
{
    public interface IKinectService : IKinectTextureProvider
    {
        
    }

    public interface IKinectTextureProvider
    {
        /// <summary>
        /// Colorテクスチャ
        /// </summary>
        public Texture ColorTexture { get; }

        /// <summary>
        /// Depthテクスチャ
        /// </summary>
        public Texture DepthTexture { get; }

        /// <summary>
        /// DepthCameraの座標系にマップされたColorテクスチャ
        /// </summary>
        public Texture DepthMappedColorTexture { get; }

        /// <summary>
        /// BodyIndexテクスチャ
        /// </summary>
        public Texture BodyIndexTexture { get; }

        /// <summary>
        /// 優先されるユーザーのテクスチャ
        /// </summary>
        public Texture PrimaryUserTexture { get; }

        /// <summary>
        /// PointCloud用のテクスチャ (RGBAFloat)
        /// </summary>
        public Texture PointCloudTexture { get; }
    }
}