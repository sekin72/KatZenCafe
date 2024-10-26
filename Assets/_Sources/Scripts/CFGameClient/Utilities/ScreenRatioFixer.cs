using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace CFGameClient.Utilities
{
    public class ScreenRatioFixer : MonoBehaviour
    {
        private readonly Dictionary<Resolution, IphoneAspectRatios> _deviceToRatioMap =
            new()
            {
                {
                    new Resolution(1920, 1080), IphoneAspectRatios.Default
                },
                {
                    new Resolution(2436, 1125), IphoneAspectRatios.IphoneXXsXrXsMax
                },
                {
                    new Resolution(1792, 828), IphoneAspectRatios.IphoneXXsXrXsMax
                },
                {
                    new Resolution(2688, 1242), IphoneAspectRatios.IphoneXXsXrXsMax
                },
                {
                    new Resolution(2048, 1536), IphoneAspectRatios.AllIpads
                },
                {
                    new Resolution(2732, 2048), IphoneAspectRatios.AllIpads
                },
                {
                    new Resolution(2234, 1668), IphoneAspectRatios.AllIpads
                }
            };

        private Camera _camera;

        private Dictionary<IphoneAspectRatios, DeviceScreenConfigures> _ratioToConfiguresMap;
        public List<DeviceScreenConfigures> DeviceScreenConfigures;

        public void Run()
        {
            _camera = GetComponent<Camera>();
            _ratioToConfiguresMap = new Dictionary<IphoneAspectRatios, DeviceScreenConfigures>();

            foreach (var config in DeviceScreenConfigures)
            {
                _ratioToConfiguresMap.Add(config.Devices, config);
            }

            var key = new Resolution(Screen.height, Screen.width);

            if (_deviceToRatioMap.ContainsKey(key))
            {
                ApplyConfigure(_deviceToRatioMap[key]);
            }
            else
            {
                ApplyConfigure();
            }
        }

        private void ApplyConfigure(IphoneAspectRatios devices = default)
        {
            if (_ratioToConfiguresMap.ContainsKey(devices))
            {
                _camera.transform.position = _ratioToConfiguresMap[devices].CameraPosition;
                _camera.fieldOfView = _ratioToConfiguresMap[devices].FieldOfView;
            }
            else
            {
                _camera.transform.position = _ratioToConfiguresMap[default].CameraPosition;
                _camera.fieldOfView = _ratioToConfiguresMap[default].FieldOfView;
            }
        }

#if UNITY_EDITOR

        [InspectorButton("InspectorButtonTagAllDevices")]
        [SerializeField]
        protected bool _tagDevices;

        // ReSharper disable once UnusedMember.Global
        protected void InspectorButtonTagAllDevices()
        {
            for (var index = 0; index < DeviceScreenConfigures.Count; index++)
            {
                var config = DeviceScreenConfigures[index];
                config.Devices = (IphoneAspectRatios)index;
            }
        }

#endif
    }

    [Serializable]
    public class DeviceScreenConfigures
    {
        public Vector3 CameraPosition;
        [ReadOnly] public IphoneAspectRatios Devices;
        public float FieldOfView;
    }

    public struct Resolution
    {
        private int H;
        private int W;

        public Resolution(int h, int w)
        {
            H = h;
            W = w;
        }
    }

    public enum IphoneAspectRatios
    {
        Default,
        IphoneXXsXrXsMax,
        AllIpads
    }
}
