﻿
namespace Atem.Views.Audio
{
    public interface ISoundService
    {
        public void Play();
        public void SubmitBuffer(byte[] buffer);
    }
}
