using System;
using System.Diagnostics;

namespace DreamCloud.Functions.Infrastructure;

internal struct ValueStopwatch
{
    private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;
    private long value;

    /// <summary>
    /// Starts a new instance.
    /// </summary>
    /// <returns>A new, running stopwatch.</returns>
    public static ValueStopwatch StartNew() => new ValueStopwatch(Stopwatch.GetTimestamp());

    private ValueStopwatch(long timestamp)
    {
        this.value = timestamp;
    }

    /// <summary>
    /// Returns true if this instance is running or false otherwise.
    /// </summary>
    public bool IsRunning => this.value > 0;

    /// <summary>
    /// Returns the elapsed time.
    /// </summary>
    public TimeSpan Elapsed => TimeSpan.FromTicks(this.ElapsedTicks);

    /// <summary>
    /// Returns the elapsed ticks.
    /// </summary>
    public long ElapsedTicks
    {
        get
        {
            // A positive timestamp value indicates the start time of a running stopwatch,
            // a negative value indicates the negative total duration of a stopped stopwatch.
            var timestamp = this.value;

            long delta;
            if (this.IsRunning)
            {
                // The stopwatch is still running.
                var start = timestamp;
                var end = Stopwatch.GetTimestamp();
                delta = end - start;
            }
            else
            {
                // The stopwatch has been stopped.
                delta = -timestamp;
            }

            return (long)(delta * TimestampToTicks);
        }
    }
}