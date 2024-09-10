using UnityEngine;

namespace HRYooba.Kinect
{
    public class ProvideTextures
    {
        public Texture ColorTexture { get; }
        public Texture DepthTexture { get; }
        public Texture DepthMappedColorTexture { get; }
        public Texture BodyIndexTexture { get; }
        public Texture PrimaryUserTexture { get; }
        public Texture PointCloudTexture { get; }

        public ProvideTextures(
            Texture colorTexture,
            Texture depthTexture,
            Texture depthMappedColorTexture,
            Texture bodyIndexTexture,
            Texture primaryUserTexture,
            Texture pointCloudTexture)
        {
            ColorTexture = colorTexture;
            DepthTexture = depthTexture;
            DepthMappedColorTexture = depthMappedColorTexture;
            BodyIndexTexture = bodyIndexTexture;
            PrimaryUserTexture = primaryUserTexture;
            PointCloudTexture = pointCloudTexture;
        }
    }
}
