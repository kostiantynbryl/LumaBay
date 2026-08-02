using UnityEngine;

namespace LumaBay
{
    public sealed class AudioSynth : MonoBehaviour
    {
        private AudioSource effectsSource;
        private AudioSource musicSource;
        private AudioSource ambienceSource;
        private bool effectsEnabled = true;
        private bool musicEnabled = true;

        private AudioClip click;
        private AudioClip swap;
        private AudioClip invalid;
        private AudioClip match;
        private AudioClip cascade;
        private AudioClip booster;
        private AudioClip coin;
        private AudioClip star;
        private AudioClip win;
        private AudioClip lose;
        private AudioClip restore;

        public bool Enabled
        {
            get => effectsEnabled;
            set => effectsEnabled = value;
        }

        public bool MusicEnabled
        {
            get => musicEnabled;
            set
            {
                musicEnabled = value;
                ApplyMusicState();
            }
        }

        private void Awake()
        {
            effectsSource = CreateSource("Effects", false, 0.86f);
            musicSource = CreateSource("Music", true, 0.22f);
            ambienceSource = CreateSource("Coastal Ambience", true, 0.13f);

            click = LumaBayArtPack.Audio("ui_click");
            swap = LumaBayArtPack.Audio("swap");
            invalid = LumaBayArtPack.Audio("invalid");
            match = LumaBayArtPack.Audio("match");
            cascade = LumaBayArtPack.Audio("cascade");
            booster = LumaBayArtPack.Audio("booster");
            coin = LumaBayArtPack.Audio("coin");
            star = LumaBayArtPack.Audio("star");
            win = LumaBayArtPack.Audio("win");
            lose = LumaBayArtPack.Audio("lose");
            restore = LumaBayArtPack.Audio("restore");

            musicSource.clip = LumaBayArtPack.Audio("music");
            ambienceSource.clip = LumaBayArtPack.Audio("ambience");
            ApplyMusicState();
        }

        public void PlayClick() => Play(click, 0.88f);
        public void PlaySwap() => Play(swap, 0.78f);
        public void PlayMatch() => Play(match, 0.90f);
        public void PlayCascade() => Play(cascade, 0.92f);
        public void PlayBooster() => Play(booster, 0.94f);
        public void PlayCoin() => Play(coin, 0.88f);
        public void PlayStar() => Play(star, 0.92f);
        public void PlayWin() => Play(win, 0.96f);
        public void PlayLose() => Play(lose, 0.90f);
        public void PlayRestore() => Play(restore, 1f);
        public void PlayError() => Play(invalid, 0.82f);

        private AudioSource CreateSource(string sourceName, bool loop, float volume)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.name = sourceName;
            source.playOnAwake = false;
            source.loop = loop;
            source.spatialBlend = 0f;
            source.volume = volume;
            source.ignoreListenerPause = true;
            return source;
        }

        private void ApplyMusicState()
        {
            if (musicSource == null || ambienceSource == null) return;
            SetLoopState(musicSource, musicEnabled);
            SetLoopState(ambienceSource, musicEnabled);
        }

        private static void SetLoopState(AudioSource source, bool shouldPlay)
        {
            if (source == null || source.clip == null) return;
            if (shouldPlay)
            {
                if (!source.isPlaying) source.Play();
            }
            else if (source.isPlaying)
            {
                source.Pause();
            }
        }

        private void Play(AudioClip clip, float volume)
        {
            if (!effectsEnabled || clip == null || effectsSource == null) return;
            effectsSource.PlayOneShot(clip, Mathf.Clamp01(volume));
        }
    }
}
