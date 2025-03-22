using System.Collections.Generic;
using Atem.Core.Audio.Filter;

namespace Atem.Core.Audio.Channel
{
    internal class AudioChannelMixer
    {
        private List<AudioChannel> _channels = new List<AudioChannel>();
        private List<IAudioFilter> _mixerFilters = new List<IAudioFilter>();

        public void AddChannel(AudioChannel channel)
        {
            if (!_channels.Contains(channel))
            {
                _channels.Add(channel);
            }
        }

        public void AddFilter(IAudioFilter filter)
        {
            if (!_mixerFilters.Contains(filter))
            {
                _mixerFilters.Add(filter);
            }
        }

        public bool RemoveFilter(IAudioFilter filter)
        {
            return _mixerFilters.Remove(filter);
        }

        public bool RemoveChannel(AudioChannel channel)
        {
            return _channels.Remove(channel);
        }

        public void Clock()
        {
            foreach (AudioChannel channel in _channels)
            {
                channel.Clock();
            }
        }

        public (float leftMix, float rightMix) Sample()
        {
            float leftMix = 0.0f;
            float rightMix = 0.0f;

            foreach (AudioChannel channel in _channels)
            {
                float sample = channel.Sample();
                leftMix += sample * (channel.LeftChannel ? 1 : 0);
                rightMix += sample * (channel.RightChannel ? 1 : 0);
            }

            foreach (IAudioFilter filter in _mixerFilters)
            {
                (leftMix, rightMix) = filter.Filter(leftMix, rightMix);
            }

            return (leftMix, rightMix);
        }
    }
}
