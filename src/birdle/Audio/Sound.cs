using System;
using Pie.Audio;
using Pie.Audio.Stream;

namespace birdle.Audio;

public class Sound : IDisposable
{
    private AudioDevice _device;
    
    private AudioBuffer _buffer;

    public Sound(AudioDevice device, string path)
    {
        _device = device;
        
        Stream stream = Stream.FromFile(path);
        _buffer = device.CreateBuffer(new BufferDescription(stream.Format), stream.GetPcm());
    }

    public void Play(double speed = 1.0, double volume = 1.0)
    {
        _device.PlayBuffer(_buffer, _device.FindFreeChannel(), new PlayProperties(speed: speed, volume: volume));
    }
    
    public void Dispose()
    {
        _device.DestroyBuffer(_buffer);
    }
}