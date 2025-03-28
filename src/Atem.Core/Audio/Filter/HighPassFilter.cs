
namespace Atem.Core.Audio.Filter
{
    /// <summary>
    /// Filters out lower frequencies from an audio channel. Has the side-effect of centering a waveform about 0.0f
    /// </summary>
    internal class HighPassFilter : IAudioFilter
    {
        private float _prevLeftChannelIn;
        private float _prevLeftChannelOut;
        private float _prevRightChannelIn;
        private float _prevRightChannelOut;
        private readonly float _factor = 1.0f;

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
