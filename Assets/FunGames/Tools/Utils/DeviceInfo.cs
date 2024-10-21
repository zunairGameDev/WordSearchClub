using UnityEngine;

namespace FunGames.Tools.Utils
{
    public class DeviceInfo
    {
        public string DeviceModel { get; }
        public string DeviceType { get; }
        public string DeviceName { get; }
        public int SystemMemorySize { get; }
        public int GraphicsMemorySize { get; }
        public int ProcessorCount { get; }
        public int ProcessorFrequency { get; }

        public DeviceInfo()
        {
            DeviceModel = SystemInfo.deviceModel;
            DeviceType = SystemInfo.deviceType.ToString();
            DeviceName = SystemInfo.deviceName;
            SystemMemorySize = SystemInfo.systemMemorySize;
            GraphicsMemorySize = SystemInfo.graphicsMemorySize;
            ProcessorCount = SystemInfo.processorCount;
            ProcessorFrequency = SystemInfo.processorFrequency;
        }
    }
}