using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LumaBay.Editor
{
    [InitializeOnLoad]
    public static class LumaBayAudioPackGenerator
    {
        private const int SampleRate = 44100;
        private const string Root = "Assets/LumaBay/Resources/Audio";
        private const string VersionKey = "LumaBay.AudioPack.0.1.2.v1";

        static LumaBayAudioPackGenerator()
        {
            EditorApplication.delayCall += GenerateIfNeeded;
        }

        [MenuItem("Luma Bay/Generate Game Audio Pack", priority = 4)]
        public static void GenerateAll()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            Directory.CreateDirectory(Root);

            Write("ui_click", 0.12f, UiClick);
            Write("swap", 0.20f, Swap);
            Write("invalid", 0.28f, Invalid);
            Write("match", 0.34f, Match);
            Write("cascade", 0.66f, Cascade);
            Write("booster", 0.78f, Booster);
            Write("coin", 0.42f, Coin);
            Write("star", 0.78f, Star);
            Write("win", 1.65f, Win);
            Write("lose", 1.05f, Lose);
            Write("restore", 2.10f, Restore);
            Write("ambience", 18f, Ambience, true);
            Write("music", 32f, Music, true);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorPrefs.SetBool(VersionKey, true);
            Debug.Log("Luma Bay original game audio pack generated.");
        }

        private static void GenerateIfNeeded()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            if (!EditorPrefs.GetBool(VersionKey, false) || !File.Exists($"{Root}/music.wav")) GenerateAll();
        }

        private static void Write(string name, float duration, Func<float, float> generator, bool loop = false)
        {
            int count = Mathf.CeilToInt(duration * SampleRate);
            var samples = new float[count];
            float peak = 0.0001f;
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)SampleRate;
                float value = Mathf.Clamp(generator(t), -1f, 1f);
                samples[i] = value;
                peak = Mathf.Max(peak, Mathf.Abs(value));
            }

            float gain = peak > 0.86f ? 0.86f / peak : 1f;
            for (int i = 0; i < samples.Length; i++) samples[i] *= gain;

            string path = $"{Root}/{name}.wav";
            File.WriteAllBytes(path, EncodeWav(samples, SampleRate));
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            if (AssetImporter.GetAtPath(path) is AudioImporter importer)
            {
                var settings = importer.defaultSampleSettings;
                settings.loadType = loop ? AudioClipLoadType.Streaming : AudioClipLoadType.DecompressOnLoad;
                settings.compressionFormat = AudioCompressionFormat.Vorbis;
                settings.quality = loop ? 0.52f : 0.78f;
                importer.defaultSampleSettings = settings;
                importer.forceToMono = true;
                importer.preloadAudioData = !loop;
                importer.loadInBackground = loop;
                importer.SaveAndReimport();
            }
        }

        private static float UiClick(float t)
        {
            float env = ExpDecay(t, 0.055f) * FadeIn(t, 0.004f);
            float chime = Sine(920f - t * 1050f, t) * 0.48f + Triangle(1380f, t) * 0.18f;
            return chime * env;
        }

        private static float Swap(float t)
        {
            float env = Bell(t, 0.20f, 0.015f, 0.12f);
            float sweep = Sine(Mathf.Lerp(430f, 820f, Mathf.Clamp01(t / 0.18f)), t);
            float sparkle = Sine(1280f, t) * ExpDecay(t, 0.06f) * 0.20f;
            return sweep * env * 0.52f + sparkle;
        }

        private static float Invalid(float t)
        {
            float env = ExpDecay(t, 0.16f) * FadeIn(t, 0.008f);
            float pitch = Mathf.Lerp(270f, 155f, Mathf.Clamp01(t / 0.24f));
            return (Triangle(pitch, t) * 0.42f + Sine(pitch * 0.5f, t) * 0.24f) * env;
        }

        private static float Match(float t)
        {
            float a = Mallet(t, 0.00f, 659.25f, 0.25f, 0.45f);
            float b = Mallet(t, 0.065f, 783.99f, 0.23f, 0.38f);
            float c = Mallet(t, 0.13f, 987.77f, 0.20f, 0.32f);
            return a + b + c;
        }

        private static float Cascade(float t)
        {
            float value = 0f;
            float[] notes = { 523.25f, 659.25f, 783.99f, 1046.50f, 1318.51f };
            for (int i = 0; i < notes.Length; i++) value += Mallet(t, i * 0.09f, notes[i], 0.27f, 0.30f);
            return value;
        }

        private static float Booster(float t)
        {
            float rise = Sine(Mathf.Lerp(180f, 920f, Mathf.Clamp01(t / 0.48f)), t) * Bell(t, 0.58f, 0.02f, 0.30f) * 0.34f;
            float shimmer = 0f;
            for (int i = 0; i < 5; i++) shimmer += Mallet(t, 0.24f + i * 0.065f, 880f * Mathf.Pow(1.12246f, i), 0.24f, 0.16f);
            return rise + shimmer;
        }

        private static float Coin(float t)
        {
            return Mallet(t, 0f, 1174.66f, 0.22f, 0.47f) + Mallet(t, 0.09f, 1567.98f, 0.28f, 0.40f);
        }

        private static float Star(float t)
        {
            return Mallet(t, 0f, 783.99f, 0.34f, 0.34f) +
                   Mallet(t, 0.12f, 987.77f, 0.40f, 0.36f) +
                   Mallet(t, 0.25f, 1318.51f, 0.44f, 0.33f);
        }

        private static float Win(float t)
        {
            float value = 0f;
            float[] notes = { 523.25f, 659.25f, 783.99f, 1046.50f };
            for (int i = 0; i < notes.Length; i++) value += Mallet(t, i * 0.11f, notes[i], 0.62f, 0.28f);
            float chordStart = 0.52f;
            float chordTime = t - chordStart;
            if (chordTime >= 0f)
            {
                float env = Bell(chordTime, 1.05f, 0.08f, 0.65f);
                value += (Sine(523.25f, chordTime) + Sine(659.25f, chordTime) + Sine(783.99f, chordTime)) * env * 0.10f;
            }
            return value;
        }

        private static float Lose(float t)
        {
            float[] notes = { 392.00f, 329.63f, 261.63f };
            float value = 0f;
            for (int i = 0; i < notes.Length; i++) value += Mallet(t, i * 0.22f, notes[i], 0.48f, 0.27f);
            return value;
        }

        private static float Restore(float t)
        {
            float rumble = Sine(78f, t) * Bell(t, 1.2f, 0.02f, 0.42f) * 0.10f;
            float shimmer = 0f;
            float[] notes = { 392f, 523.25f, 659.25f, 783.99f, 1046.5f, 1318.51f };
            for (int i = 0; i < notes.Length; i++) shimmer += Mallet(t, 0.20f + i * 0.16f, notes[i], 0.58f, 0.19f);
            float light = t > 1.05f
                ? (Sine(523.25f, t) + Sine(659.25f, t) + Sine(783.99f, t)) * Bell(t - 1.05f, 0.95f, 0.08f, 0.55f) * 0.09f
                : 0f;
            return rumble + shimmer + light;
        }

        private static float Ambience(float t)
        {
            float cycle = t / 18f * Mathf.PI * 2f;
            float waves = Sine(0.09f, t) * 0.18f + Sine(0.13f, t + 1.7f) * 0.13f;
            float surfEnvelope = Mathf.Pow((Mathf.Sin(cycle * 3f) + 1f) * 0.5f, 3f);
            float surf = FilteredNoise(t, 113, 2.2f) * surfEnvelope * 0.10f;
            float wind = FilteredNoise(t, 911, 0.32f) * 0.055f;
            float distantTone = (Sine(110f, t) + Sine(164.81f, t)) * 0.007f;
            return waves * 0.035f + surf + wind + distantTone;
        }

        private static float Music(float t)
        {
            const float beat = 0.75f;
            int beatIndex = Mathf.FloorToInt(t / beat);
            int bar = (beatIndex / 4) % 8;
            int step = beatIndex % 4;
            float localBeat = t - beatIndex * beat;

            float[][] chords =
            {
                new[] { 261.63f, 329.63f, 392.00f, 493.88f }, // Cmaj7
                new[] { 220.00f, 261.63f, 329.63f, 392.00f }, // Am7
                new[] { 174.61f, 220.00f, 261.63f, 329.63f }, // Fmaj7
                new[] { 196.00f, 261.63f, 293.66f, 392.00f }, // Gsus
            };
            float[] chord = chords[(bar / 2) % chords.Length];
            float pad = 0f;
            for (int i = 0; i < chord.Length; i++)
            {
                pad += Sine(chord[i] * 0.5f, t) * 0.018f + Triangle(chord[i], t) * 0.010f;
            }

            float[] melody = { 659.25f, 783.99f, 587.33f, 523.25f, 659.25f, 880.00f, 783.99f, 587.33f };
            float note = melody[(bar + step * 2) % melody.Length];
            float melodyEnv = Bell(localBeat, beat * 0.90f, 0.04f, beat * 0.42f);
            float lead = (Sine(note, localBeat) * 0.032f + Triangle(note * 0.5f, localBeat) * 0.018f) * melodyEnv;

            float pluck = Mallet(localBeat, 0f, chord[step], 0.42f, 0.075f);
            float breathe = 0.82f + Mathf.Sin(t * Mathf.PI * 2f / 8f) * 0.18f;
            return (pad * breathe + lead + pluck) * 0.78f;
        }

        private static float Mallet(float t, float start, float frequency, float duration, float volume)
        {
            float local = t - start;
            if (local < 0f || local > duration) return 0f;
            float env = ExpDecay(local, duration * 0.31f) * FadeIn(local, 0.006f) * Mathf.Clamp01((duration - local) / 0.045f);
            float body = Sine(frequency, local) * 0.70f + Sine(frequency * 2.01f, local) * 0.20f + Sine(frequency * 3.98f, local) * 0.10f;
            return body * env * volume;
        }

        private static float Sine(float frequency, float t) => Mathf.Sin(2f * Mathf.PI * frequency * t);
        private static float Triangle(float frequency, float t) => Mathf.Asin(Mathf.Sin(2f * Mathf.PI * frequency * t)) * (2f / Mathf.PI);
        private static float FadeIn(float t, float duration) => Mathf.Clamp01(t / Mathf.Max(0.0001f, duration));
        private static float ExpDecay(float t, float timeConstant) => Mathf.Exp(-t / Mathf.Max(0.0001f, timeConstant));

        private static float Bell(float t, float duration, float attack, float release)
        {
            if (t < 0f || t > duration) return 0f;
            return Mathf.Clamp01(t / Mathf.Max(0.001f, attack)) * Mathf.Clamp01((duration - t) / Mathf.Max(0.001f, release));
        }

        private static float FilteredNoise(float t, int seed, float frequency)
        {
            float position = t * frequency;
            int index = Mathf.FloorToInt(position);
            float fraction = position - index;
            float a = HashNoise(index, seed);
            float b = HashNoise(index + 1, seed);
            float smooth = fraction * fraction * (3f - 2f * fraction);
            return Mathf.Lerp(a, b, smooth);
        }

        private static float HashNoise(int value, int seed)
        {
            unchecked
            {
                uint n = (uint)(value * 374761393 + seed * 668265263);
                n = (n ^ (n >> 13)) * 1274126177u;
                n ^= n >> 16;
                return n / (float)uint.MaxValue * 2f - 1f;
            }
        }

        private static byte[] EncodeWav(float[] samples, int sampleRate)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            int dataSize = samples.Length * sizeof(short);

            writer.Write(new[] { 'R', 'I', 'F', 'F' });
            writer.Write(36 + dataSize);
            writer.Write(new[] { 'W', 'A', 'V', 'E' });
            writer.Write(new[] { 'f', 'm', 't', ' ' });
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)1);
            writer.Write(sampleRate);
            writer.Write(sampleRate * sizeof(short));
            writer.Write((short)sizeof(short));
            writer.Write((short)16);
            writer.Write(new[] { 'd', 'a', 't', 'a' });
            writer.Write(dataSize);

            foreach (float sample in samples)
            {
                short value = (short)Mathf.RoundToInt(Mathf.Clamp(sample, -1f, 1f) * short.MaxValue);
                writer.Write(value);
            }
            return stream.ToArray();
        }
    }
}
