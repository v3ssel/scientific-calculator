using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ScientificCalculator.Services.Logging;

public class FileCalculatorLogger : ICalculatorLogger, ICalculatorLoggerWithRotation
{
    private RotationPeriod _rotationPeriod;
    public RotationPeriod RotationPeriod
    {
        get => _rotationPeriod;
        set => _rotationPeriod = value;
    }
    public bool Enabled { get; set; }

    public FileCalculatorLogger(RotationPeriod rotationPeriod, bool enabled = true)
    {
        RotationPeriod = rotationPeriod;
        Enabled = enabled;
    }

    public void Log(LogLevel level, string input, string? output)
    {
        if (!Enabled) return;
        
        var task = Task.Run(async () => await LogAsync(level, input, output));
        task.Wait();
    }

    public async Task LogAsync(LogLevel level, string input, string? output)
    {
        if (!Enabled) return;

        var dir = Directory.CreateDirectory("./logs");
        var last_file = dir.GetFiles($"logs_*.log").OrderBy(x => x.CreationTime).FirstOrDefault();
        var formatted_out = $"[{level}] \"{input}\" = \"{output ?? ""}\"";

        if (last_file is null || DateTime.Now >= GetLogExpireTime(last_file.CreationTime))
        {
            using StreamWriter writer = File.CreateText($"{dir.Name}/logs_{DateTime.Now:dd-MM-yy-hh-mm-ss}.log");
                await writer.WriteLineAsync(formatted_out);
        }
        else
        {
            using StreamWriter writer = last_file.AppendText();
                await writer.WriteLineAsync(formatted_out);
        }

    }

    private DateTime GetLogExpireTime(DateTime last)
    {
        return RotationPeriod switch
                {
                    RotationPeriod.Hour => last.AddHours(1),
                    RotationPeriod.Day => last.AddDays(1),
                    RotationPeriod.Month => last.AddMonths(1),
                    _ => last.AddMonths(1)
                };
    }
}
