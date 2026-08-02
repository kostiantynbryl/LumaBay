using UnityEngine;

namespace LumaBay
{
    public sealed class AudioSynth : MonoBehaviour
    {
        private AudioSource effectsSource;
        private AudioSource musicSource;
        private AudioClip click;
        private AudioClip match;
        private AudioClip win;
        private AudioClip error;
        private AudioClip ambience;
        private bool enabledAudio = true;

        public bool Enabled
        {
            get => enabledAudio;
            set
            {
                enabledAudio = value;
                if (musicSource != null)
                {
                    if (enabledAudio && !musicSource.isPlaying) musicSource.Play();
                    if (!enabledAudio && musicSource.isPlaying) musicSource.Pause();
                }
            }
        }

        private void Awake()
        {
            effectsSource = gameObject.AddComponent<AudioSource>();
            effectsSource.playOnAwake = false;
            effectsSource.spatialBlend = 0f;
            effectsSource.volume = 0.90f;

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.spatialBlend = 0f;
            musicSource.volume = 0.18f;

            click = CreateLayeredTone("click", 520f, 940f, 0.065f, 0.13f);
            match = CreateSparkle("match", 700f, 0.15f, 0.17f);
            win = CreateChord("win", new[] { 523.25f, 659.25f, 783.99f, 1046.50f }, 0.58f, 0.15f);
            error = CreateTone("error", 180f, 0.14f, 0.11f, 0.76f);
            ambience = CreateAmbience("coastal_ambience", 12f);
            musicSource.clip = ambience;
            if (enabledAudio) musicSource.Play();
        }

        public void PlayClick() => Play(click);
        public void PlayMatch() => Play(match);
        public void PlayWin() => Play(win);
        public void PlayError() => Play(error);

        private void Play(AudioClip clip)
        {
            if (enabledAudio && clip != null) effectsSource.PlayOneShot(clip);
        }

        private static AudioClip CreateLayeredTone(string name, float lowFrequency, float highFrequency, float duration, float volume)
        {
            const int sampleRate = 44100;
            int count = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[count];
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)Mathf.Max(1, count - 1);
                float envelope = Mathf.Sin(Mathf.PI * t) * (1f - t * 0.55f);
                float low = Mathf.Sin(2f * Mathf.PI * lowFrequency * i / sampleRate);
                float high = Mathf.Sin(2f * Mathf.PI * highFrequency * i / sampleRate + t * 2.1f);
                samples[i] = (low * 0.64f + high * 0.36f) * envelope * volume;
            }
            AudioClip clip = AudioClip.Create(name, count, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private static AudioClip CreateSparkle(string name, float baseFrequency, float duration, float volume)
        {
            const int sampleRate = 44100;
            int count = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[count];
            double phase = 0d;
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)Mathf.Max(1, count - 1);
                float frequency = baseFrequency * Mathf.Lerp(0.86f, 1.92f, t);
                phase += 2d * Mathf.PI * frequency / sampleRate;
                float envelope = Mathf.Pow(Mathf.Sin(Mathf.PI * t), 0.75f) * (1f - t * 0.38f);
                float overtone = Mathf.Sin((float)phase * 2.02f) * 0.22f;
                samples[i] = (Mathf.Sin((float)phase) + overtone) * envelope * volume;
            }
            AudioClip clip = AudioClip.Create(name, count, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private static AudioClip CreateTone(string name, float frequency, float duration, float volume, float endMultiplier = 1f)
        {
            const int sampleRate = 44100;
            int count = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[count];
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
            float[] samples = new float[count];
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

        private static AudioClip CreateAmbience(string name, float duration)
        {
            const int sampleRate = 22050;
            int count = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[count];
            System.Random random = new System.Random(8421);
            float filteredNoise = 0f;

            for (int i = 0; i < count; i++)
            {
                float t = i / (float)sampleRate;
                float loopPhase = i / (float)count;
                float fade = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(loopPhase * 16f)) *
                             Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((1f - loopPhase) * 16f));

                float rawNoise = (float)random.NextDouble() * 2f - 1f;
                filteredNoise = Mathf.Lerp(filteredNoise, rawNoise, 0.025f);
                float wave = filteredNoise * (0.32f + 0.12f * Mathf.Sin(t * 0.54f));
                float lowPad = Mathf.Sin(2f * Mathf.PI * 110f * t) * 0.08f +
                               Mathf.Sin(2f * Mathf.PI * 164.81f * t) * 0.045f +
                               Mathf.Sin(2f * Mathf.PI * 220f * t) * 0.025f;
                float tide = Mathf.Sin(t * 0.42f) * 0.045f;
                samples[i] = (wave * 0.12f + lowPad + tide) * fade;
            }

            AudioClip clip = AudioClip.Create(name, count, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
