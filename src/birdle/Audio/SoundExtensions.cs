using Pie.Audio;

namespace birdle.Audio;

public static class SoundExtensions
{
    public static ushort FindFreeChannel(this AudioSystem system)
    {
        for (ushort i = 0; i < system.NumVoices; i++)
        {
            if (system.GetVoiceState(i) == PlayState.Stopped)
                return i;
        }

        return 1;
    }
}