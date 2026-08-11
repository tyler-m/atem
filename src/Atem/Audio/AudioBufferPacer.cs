namespace Atem.Audio;

/// <summary>
/// Decides how many times a freshly filled audio buffer should be handed to the output device,
/// keeping the device's pending queue inside a target band.
/// <para>
/// The emulator and the output device run off different clocks, so their rates never match
/// exactly and the queue always drifts. Capping the queue only handles drifting upwards; if the
/// device consumes faster than the emulator produces, the queue drains to nothing and the
/// output starves, which is far more audible than a dropped buffer. Submitting a buffer twice
/// when the queue is running dry repeats a few milliseconds of audio to rebuild the cushion.
/// </para>
/// </summary>
public class AudioBufferPacer
{
    public const int DefaultTargetBuffers = 4;
    public const int DefaultMinimumBuffers = 2;
    public const int DefaultMaximumBuffers = 6;

    private readonly int _minimumBuffers;
    private readonly int _maximumBuffers;

    public int TargetBuffers { get; }

    /// <param name="targetBuffers">Depth to prime the queue to before playback starts.</param>
    /// <param name="minimumBuffers">At or below this depth the cushion is rebuilt.</param>
    /// <param name="maximumBuffers">Above this depth buffers are discarded.</param>
    public AudioBufferPacer(int targetBuffers = DefaultTargetBuffers,
        int minimumBuffers = DefaultMinimumBuffers,
        int maximumBuffers = DefaultMaximumBuffers)
    {
        TargetBuffers = targetBuffers;
        _minimumBuffers = minimumBuffers;
        _maximumBuffers = maximumBuffers;
    }

    /// <summary>
    /// How many times the next buffer should be submitted: zero to discard it because the
    /// emulator is running ahead, one in the steady state, or two to rebuild the cushion
    /// because the device is running ahead.
    /// </summary>
    public int GetSubmitCount(int pendingBuffers)
    {
        if (pendingBuffers > _maximumBuffers)
        {
            return 0;
        }

        if (pendingBuffers < _minimumBuffers)
        {
            return 2;
        }

        return 1;
    }
}
