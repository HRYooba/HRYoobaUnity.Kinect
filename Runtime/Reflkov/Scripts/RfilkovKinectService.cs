using System;
using UnityEngine;
using com.rfilkov.kinect;
using HRYooba.Kinect.Core;
using HRYooba.Kinect.Core.Services;

namespace HRYooba.Kinect.Rfilkov
{
    public class RfilkovKinectService : IKinectService, IDisposable
    {
        // rfilkov assets
        private readonly KinectManager _kinectManager;
        private readonly KinectInterop.SensorData _sensorData;
        private readonly DepthSensorBase _depthSensor;

        // private fields
        private readonly BodyByBodyIndexTexture _bodyByBodyIndexTexture;
        private readonly RenderTexture _pointCloudTexture;
        private ulong _lastBodyImageTime;
        private ulong _primaryUserId;

        // public properties
        public Texture ColorTexture => _sensorData?.colorImageTexture;
        public Texture DepthTexture => _sensorData?.depthImageTexture;
        public Texture DepthMappedColorTexture => _sensorData?.depthCamColorImageTexture;
        public Texture BodyIndexTexture => _sensorData?.bodyImageTexture;
        public Texture PrimaryUserTexture => _bodyByBodyIndexTexture?.Texture;
        public Texture PointCloudTexture => _depthSensor?.pointCloudVertexTexture;

        public RfilkovKinectService(KinectInterop.SensorData sensorData, BodyTrackingSensorOrientationType bodyTrackingSensorOrientation)
        {
            _kinectManager = KinectManager.Instance;
            _sensorData = sensorData;

            _depthSensor = (DepthSensorBase)_sensorData.sensorInterface;

            _bodyByBodyIndexTexture = new BodyByBodyIndexTexture(
                _sensorData.depthImageWidth,
                _sensorData.depthImageHeight,
                _depthSensor.minDepthDistance,
                _depthSensor.maxDepthDistance
            );

            _pointCloudTexture = new RenderTexture(
                _sensorData.depthImageWidth,
                _sensorData.depthImageHeight,
                0,
                RenderTextureFormat.ARGBFloat
            );

            // rfilkov.kinectの設定
            var kinectSensor = (Kinect4AzureInterface)_depthSensor;
            kinectSensor.StopBodyTracking(_sensorData);
            kinectSensor.bodyTrackingSensorOrientation = bodyTrackingSensorOrientation switch
            {
                BodyTrackingSensorOrientationType.Default => Microsoft.Azure.Kinect.Sensor.k4abt_sensor_orientation_t.K4ABT_SENSOR_ORIENTATION_DEFAULT,
                BodyTrackingSensorOrientationType.Clockwise90 => Microsoft.Azure.Kinect.Sensor.k4abt_sensor_orientation_t.K4ABT_SENSOR_ORIENTATION_CLOCKWISE90,
                BodyTrackingSensorOrientationType.CounterClockwise90 => Microsoft.Azure.Kinect.Sensor.k4abt_sensor_orientation_t.K4ABT_SENSOR_ORIENTATION_COUNTERCLOCKWISE90,
                BodyTrackingSensorOrientationType.Flip180 => Microsoft.Azure.Kinect.Sensor.k4abt_sensor_orientation_t.K4ABT_SENSOR_ORIENTATION_FLIP180,
                _ => Microsoft.Azure.Kinect.Sensor.k4abt_sensor_orientation_t.K4ABT_SENSOR_ORIENTATION_DEFAULT
            };
            kinectSensor.InitBodyTracking(kinectSensor.frameSourceFlags, _sensorData, kinectSensor.coordMapperCalib, true);
            _depthSensor.EnableDepthCameraColorFrame(_sensorData, true); // depthCamColorImageTexture, pointCloudTextureを有効にするために必要
            _depthSensor.pointCloudVertexTexture = _pointCloudTexture;
        }

        public void Dispose()
        {
            _bodyByBodyIndexTexture?.Dispose();

            _pointCloudTexture?.Release();
            UnityEngine.Object.Destroy(_pointCloudTexture);
        }

        public void PreUpdate()
        {
            _primaryUserId = 0;
            _lastBodyImageTime = _sensorData.lastBodyImageTime;
        }

        public void PreLateUpdate()
        {
            UpdateBodyByBodyIndexTexture();
        }

        public void ApplyKinectData(KinectData kinectData)
        {
            var position = kinectData.Position;
            var rotation = Quaternion.Euler(kinectData.EulerAngles);
            var minDepthDistance = kinectData.MinDepthDistance;
            var maxDepthDistance = kinectData.MaxDepthDistance;

            // _sensorData.sensorInterface.SetSensorToWorldMatrix(position, rotation, true); // BodySpinの設定するとバグるため
            _sensorData.sensorInterface.GetSensorTransform().SetPositionAndRotation(position, rotation);
            _depthSensor.minDepthDistance = minDepthDistance;
            _depthSensor.maxDepthDistance = maxDepthDistance;

            _bodyByBodyIndexTexture.SetDepthDistance(minDepthDistance, maxDepthDistance);
        }

        public void SetPrimaryUserId(ulong userId)
        {
            _primaryUserId = userId;
        }

        private void UpdateBodyByBodyIndexTexture()
        {
            if (_lastBodyImageTime != _sensorData.lastBodyImageTime) // BodyImageが更新されたら
            {
                var bodyIndex = _kinectManager.GetBodyIndexByUserId(_primaryUserId);
                _bodyByBodyIndexTexture.Update(bodyIndex, _sensorData.bodyIndexBuffer);
            }
        }

        private class BodyByBodyIndexTexture : IDisposable
        {
            private RenderTexture _texture;
            public Texture Texture => _texture;

            private Material _material;
            private float _minDepthDistance;
            private float _maxDepthDistance;

            public BodyByBodyIndexTexture(int width, int height, float minDepthDistance, float maxDepthDistance)
            {
                _minDepthDistance = minDepthDistance;
                _maxDepthDistance = maxDepthDistance;

                _texture = new RenderTexture(width, height, 0, RenderTextureFormat.Default);

                var shader = Resources.Load<Shader>("UserBodyImageByBodyIndexShader");
                // var shader = Shader.Find("HRYooba/Kinect/UserBodyImageByBodyIndexShader");
                _material = new Material(shader);
            }

            public void Dispose()
            {
                if (_texture != null)
                {
                    UnityEngine.Object.Destroy(_texture);
                    _texture = null;
                }

                if (_material != null)
                {
                    UnityEngine.Object.Destroy(_material);
                    _material = null;
                }
            }

            public void SetDepthDistance(float minDepthDistance, float maxDepthDistance)
            {
                _minDepthDistance = minDepthDistance;
                _maxDepthDistance = maxDepthDistance;
            }

            public void Update(int bodyIndex, ComputeBuffer bodyIndexBuffer)
            {
                _material.SetBuffer("_BodyIndexMap", bodyIndexBuffer);
                _material.SetInt("_BodyIndex", bodyIndex);
                _material.SetInt("_TexResX", _texture.width);
                _material.SetInt("_TexResY", _texture.height);
                _material.SetInt("_MinDepth", (int)(_minDepthDistance * 1000f));
                _material.SetInt("_MaxDepth", (int)(_maxDepthDistance * 1000f));

                Graphics.Blit(null, _texture, _material);
            }
        }
    }
}