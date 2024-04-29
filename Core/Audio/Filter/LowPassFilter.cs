
namespace Atem.Core.Audio.Filter
{
    internal class LowPassFilter : IAudioFilter
    {
        private float _prevLeftChannelOut = 0.0f;
        private float _prevRightChannelOut = 0.0f;
        private float _factor = 0.0f;

        public LowPassFilter(float factor)
        {
            _factor = factor;
        }

        public (float, float) Filter(float leftChannelIn, float rightChannelIn)
        {
            float leftChannelOut = (_prevLeftChannelOut * _factor) + (leftChannelIn * (1 - _factor));
            _prevLeftChannelOut = leftChannelOut;

            float rightChannelOut = (_prevRightChannelOut * _factor) + (rightChannelIn * (1 - _factor));
            _prevRightChannelOut = rightChannelOut;

            return (leftChannelOut, rightChannelOut);
        }
    }
}
