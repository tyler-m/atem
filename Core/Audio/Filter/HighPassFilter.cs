
namespace Atem.Core.Audio.Filter
{
    /// <summary>
    /// Useful for centering waveform to 0.0
    /// </summary>
    internal class HighPassFilter : IAudioFilter
    {
        float _prevLeftChannelIn = 0.0f;
        float _prevLeftChannelOut = 0.0f;
        float _prevRightChannelIn = 0.0f;
        float _prevRightChannelOut = 0.0f;
        float _factor = 1.0f;

        public HighPassFilter(float factor)
        {
            _factor = factor;
        }

        public (float, float) Filter(float leftChannelIn, float rightChannelIn)
        {
            float leftChannelOut = _prevLeftChannelOut * _factor + leftChannelIn - _prevLeftChannelIn;
            _prevLeftChannelIn = leftChannelIn;
            _prevLeftChannelOut = leftChannelOut;

            float rightChannelOut = _prevRightChannelOut * _factor + rightChannelIn - _prevRightChannelIn;
            _prevRightChannelIn = rightChannelIn;
            _prevRightChannelOut = rightChannelOut;

            return (leftChannelOut, rightChannelOut);
        }
    }
}
