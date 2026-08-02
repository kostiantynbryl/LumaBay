using UnityEngine;

namespace LumaBay
{
    public sealed class AudioSynth : MonoBehaviour
    {
        private AudioSource source;
        private AudioClip click;
        private AudioClip match;
        private AudioClip win;
        private AudioClip error;

        public bool Enabled { get; set; } = true;

        private void Awake()
        {
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            click = CreateTone("click", 520f, 0.055f, 0.16f);
            match = CreateTone("match", 760f, 0.11f, 0.18f, 1.35f);
            win = CreateChord("win", new[] { 523.25f, 659.25f, 783.99f }, 0.42f, 0.16f);
            error = CreateTone("error", 180f, 0.12f, 0.12f, 0.82f);
        }

        public void PlayClick() => Play(click);
        public void PlayMatch() => Play(match);
        public void PlayWin() => Play(win);
        public void PlayError() => Play(error);

        private void Play(AudioClip clip)
        {
            if (Enabled && clip != null) source.PlayOneShot(clip);
        }

        private static AudioClip CreateTone(string name, float frequency, float duration, float volume, float endMultiplier = 1f)
        {
            const int sampleRate = 44100;
            int count = Mathf.CeilToInt(sampleRate * duration);
            var samples = new float[count];
            double phase = 0d;
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)Mathf.Max(1, count - 1);
                float envelope = Mathf.Sin(Mathf.PI * t) * (1f - t * 0.35f);
                float currentFrequency = Mathf.Lerp(frequency, frequency * endMultiplier, t);
                phase += 2d * Mathf.PI * currentFrequency / sampleRate;
                samples[i] = Mathf.Sin((float)phase) * envelope * volume;
            }
            AudioClip clip = AudioClip.Create(name, count, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private static AudioClip CreateChord(string name, float[] frequencies, float duration, float volume)
        {
            const int sampleRate = 44100;
            int count = Mathf.CeilToInt(sampleRate * duration);
            var samples = new float[count];
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)Mathf.Max(1, count - 1);
                float envelope = Mathf.Sin(Mathf.PI * t) * (1f - t * 0.25f);
                float value = 0f;
                foreach (float frequency in frequencies)
                {
                    value += Mathf.Sin(2f * Mathf.PI * frequency * i / sampleRate);
                }
                samples[i] = value / frequencies.Length * envelope * volume;
            }
            AudioClip clip = AudioClip.Create(name, count, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
