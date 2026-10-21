using Sentinela.Communication.Enums;
using Sentinela.Communication.Requests;
using System.Runtime.Versioning;

namespace Sentinela.Agent.Metrics;

[SupportedOSPlatform("linux")]
internal class LinuxMetricsCollector : IMetricsCollector
{
    private readonly DriveInfo _rootDrive;
    private long _previousIdle;
    private long _previousTotal;
    public LinuxMetricsCollector()
    {
        _rootDrive = new DriveInfo("/");
        (_previousIdle, _previousTotal) = ReadCpuTimes();
    }
    public IList<RequestSampleJson> Collect()
    {
        var now = DateTime.UtcNow;
        var samples = new List<RequestSampleJson>();

        var cpuUsage = GetCpuUsage();
        samples.Add(new RequestSampleJson
        {
            Metric = MetricType.Cpu,
            Value = cpuUsage,
            CollectedAt = now
        });
        
        var memoryUsage = GetMemoryUsage();
        samples.Add(new RequestSampleJson
        {
            Metric = MetricType.Memory,
            Value = memoryUsage,
            CollectedAt = now
        });

        var diskUsage = (double)(_rootDrive.TotalSize - _rootDrive.AvailableFreeSpace) / _rootDrive.TotalSize * 100;
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

    private static (long Idle, long Total) ReadCpuTimes()
    {
        var values = File.ReadLines("/proc/stat").First()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Take(8)
            .Select(long.Parse)
            .ToArray();

        var idle = values[3] + values[4];
        var total = values.Sum();

        return (idle, total);
    }
    private double GetCpuUsage()
    {
        var (idle, total) = ReadCpuTimes();

        var idleDelta = idle - _previousIdle;
        var totalDelta = total - _previousTotal;

        _previousIdle = idle;
        _previousTotal = total;

        if (totalDelta == 0)
            return 0;

        return (1 - (double)idleDelta / totalDelta) * 100;
    }
    private static long ReadMemInfoValue(string[] lines, string name)
    {
        var line = lines.First(line => line.StartsWith(name + ":"));
        return long.Parse(line.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);
    }
    private static double GetMemoryUsage() 
    {
        var lines = File.ReadAllLines("/proc/meminfo");

        var total = ReadMemInfoValue(lines, "MemTotal");
        var available = ReadMemInfoValue(lines, "MemAvailable");

        return (double)(total - available) / total * 100;
    }
}
