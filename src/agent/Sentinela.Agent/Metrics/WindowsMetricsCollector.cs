using Sentinela.Communication.Enums;
using Sentinela.Communication.Requests;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace Sentinela.Agent.Metrics;

[SupportedOSPlatform("windows")]
internal class WindowsMetricsCollector : IMetricsCollector
{
    private readonly PerformanceCounter _cpuCounter;
    private readonly PerformanceCounter _memoryCounter;
    private readonly DriveInfo _systemDrive;

    public WindowsMetricsCollector()
    {
        _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        _cpuCounter.NextValue();

        _memoryCounter = new PerformanceCounter("Memory", "Available Bytes");
        _memoryCounter.NextValue();

        _systemDrive = new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory)!);
    }
    public IList<RequestSampleJson> Collect()
    {
        var now = DateTime.UtcNow;
        var samples = new List<RequestSampleJson>();

        var cpuUsage = _cpuCounter.NextValue();
        samples.Add(new RequestSampleJson
        {
            Metric = MetricType.Cpu,
            Value = cpuUsage,
            CollectedAt = now
        });

        var totalMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
        var availableMemory = _memoryCounter.NextValue();
        var memoryUsage = (totalMemory - availableMemory) / totalMemory * 100;
        samples.Add(new RequestSampleJson
        {
            Metric = MetricType.Memory,
            Value = memoryUsage,
            CollectedAt = now
        });

        var diskUsage = (double)(_systemDrive.TotalSize - _systemDrive.AvailableFreeSpace) / _systemDrive.TotalSize * 100;
        samples.Add(new RequestSampleJson
        {
            Metric = MetricType.Disk,
            Value = diskUsage,
            CollectedAt = now
        });

        var uptime = Environment.TickCount64 / 1000.0;    
        samples.Add(new RequestSampleJson
        {
            Metric = MetricType.Uptime,
            Value = uptime,
            CollectedAt = now
        });
        return samples;
    }
}
