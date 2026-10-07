using System.Collections.Generic;
using UnityEngine;

namespace Dopeboyz
{
    public enum SoundType
    {
        SprayHiss,
        Shoot,
        WallClaim,
        Dash,
        Siren,
        LowriderHop,
        Pickup,
        CashChime,
        HitDeflect,
        SlowmoWarp,
        BoomboxBass,
        PaintSplat,
        UpdraftSteam,
        TurntableScratch,
        GeyserBlast,
        CrateCrack,
        EngineRev,
        CompressorHiss,
        ShootPaint,
        Explosion,
        SprayCan,
        WallComplete,
        LevelUp,
        Footstep,
        BubbleShield,
        SlowMo,
        SprayCapSelect,
        Whoosh,
        CrewOrder,
        ShieldHit,
        HealthPickup,
        PaintPickup,
        AmmoPickup
    }

    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        private AudioSource sfxSource;
        private AudioSource musicSource;
        private Dictionary<SoundType, AudioClip> clips = new Dictionary<SoundType, AudioClip>();

        [Range(0f, 1f)] public float sfxVolume = 0.8f;
        [Range(0f, 1f)] public float musicVolume = 0.4f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            sfxSource = gameObject.AddComponent<AudioSource>();
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;

            GenerateProceduralAudio();
            StartMusic();
        }

        private void GenerateProceduralAudio()
        {
            clips[SoundType.SprayHiss] = CreateNoiseClip(0.3f, 0.4f);
            clips[SoundType.Shoot] = CreateFrequencySweepClip(500f, 90f, 0.12f);
            clips[SoundType.WallClaim] = CreateArpeggioClip(new float[] { 440f, 554f, 659f, 880f }, 0.08f);
            clips[SoundType.Dash] = CreateNoiseSweepClip(0.18f);
            clips[SoundType.Siren] = CreateDualToneClip(750f, 950f, 0.6f);
            clips[SoundType.LowriderHop] = CreateSubBassClip(65f, 30f, 0.4f);
            clips[SoundType.Pickup] = CreateArpeggioClip(new float[] { 523f, 659f, 784f }, 0.06f);
            clips[SoundType.CashChime] = CreateArpeggioClip(new float[] { 880f, 1320f }, 0.09f);
            clips[SoundType.HitDeflect] = CreateFrequencySweepClip(1200f, 300f, 0.08f);
            clips[SoundType.SlowmoWarp] = CreateFrequencySweepClip(400f, 120f, 0.5f);
            clips[SoundType.BoomboxBass] = CreateSubBassClip(55f, 28f, 0.65f);
            clips[SoundType.PaintSplat] = CreateSplatClip(0.15f);
            clips[SoundType.UpdraftSteam] = CreateNoiseSweepClip(0.45f);
            clips[SoundType.TurntableScratch] = CreateScratchClip(0.4f);
            clips[SoundType.GeyserBlast] = CreateGeyserClip(0.6f);
            clips[SoundType.CrateCrack] = CreateFrequencySweepClip(350f, 80f, 0.25f);
            clips[SoundType.EngineRev] = CreateEngineRevClip(0.55f);
            clips[SoundType.CompressorHiss] = CreateCompressorClip(0.42f);
            clips[SoundType.ShootPaint] = clips[SoundType.Shoot];
            clips[SoundType.Explosion] = CreateNoiseSweepClip(0.45f);
            clips[SoundType.SprayCan] = clips[SoundType.SprayHiss];
            clips[SoundType.WallComplete] = clips[SoundType.WallClaim];
            clips[SoundType.LevelUp] = clips[SoundType.CashChime];
            clips[SoundType.Footstep] = clips[SoundType.Dash];
            clips[SoundType.BubbleShield] = clips[SoundType.HitDeflect];
            clips[SoundType.SlowMo] = clips[SoundType.SlowmoWarp];
            clips[SoundType.SprayCapSelect] = clips[SoundType.Dash];
            clips[SoundType.Whoosh] = clips[SoundType.Dash];
            clips[SoundType.CrewOrder] = clips[SoundType.CashChime];
            clips[SoundType.ShieldHit] = clips[SoundType.HitDeflect];

            // Crash-proof fallback: ensure all enum values exist in clips
            foreach (SoundType st in System.Enum.GetValues(typeof(SoundType)))
            {
                if (!clips.ContainsKey(st) || clips[st] == null)
                {
                    clips[st] = clips[SoundType.Dash];
                }
            }
        }

        public void PlaySound(SoundType type, float volumeMultiplier = 1.0f)
        {
            if (sfxSource != null && clips.TryGetValue(type, out var clip) && clip != null)
            {
                sfxSource.PlayOneShot(clip, sfxVolume * volumeMultiplier);
            }
        }

        public void SetVolumes(float music, float effects)
        {
            musicVolume = music; sfxVolume = effects;
            if (musicSource != null) musicSource.volume = music;
        }
        public void PlayAt(SoundType type, Vector3 position)
        {
            if (clips.TryGetValue(type, out var clip)) AudioSource.PlayClipAtPoint(clip, position, sfxVolume);
        }

        private void StartMusic()
        {
            // Procedural Lo-Fi Electro Bassline Loop
            int sampleRate = 44100;
            float loopDuration = 4.0f; // 4 second bar
            int numSamples = (int)(sampleRate * loopDuration);
            float[] samples = new float[numSamples];

            float[] bassNotes = new float[] { 110f, 110f, 130.81f, 98f }; // A2, C3, G2
            int noteLength = numSamples / 8;

            for (int i = 0; i < numSamples; i++)
            {
                int beatIdx = (i / noteLength) % bassNotes.Length;
                float freq = bassNotes[beatIdx];
                float t = (float)i / sampleRate;

                // Bass tone
                float bass = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.25f;

                // Subtle percussion click on quarter beats
                float perc = 0f;
                int quarter = (int)(sampleRate * 0.5f);
                int sub = i % quarter;
                if (sub < 800)
                {
                    perc = (Random.value * 2f - 1f) * (1f - (float)sub / 800f) * 0.15f;
                }

                samples[i] = Mathf.Clamp(bass + perc, -1f, 1f);
            }

            AudioClip musicClip = AudioClip.Create("StreetShooters_Theme", numSamples, 1, sampleRate, false);
            musicClip.SetData(samples, 0);

            musicSource.clip = musicClip;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }

        private AudioClip CreateNoiseClip(float duration, float amplitude)
        {
            int sampleRate = 44100;
            int numSamples = (int)(sampleRate * duration);
            float[] samples = new float[numSamples];
            for (int i = 0; i < numSamples; i++)
            {
                samples[i] = (Random.value * 2f - 1f) * amplitude * (1f - (float)i / numSamples);
            }
            AudioClip clip = AudioClip.Create("SprayNoise", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateFrequencySweepClip(float startFreq, float endFreq, float duration)
        {
            int sampleRate = 44100;
            int numSamples = (int)(sampleRate * duration);
            float[] samples = new float[numSamples];
            float phase = 0f;
            for (int i = 0; i < numSamples; i++)
            {
                float frac = (float)i / numSamples;
                float currentFreq = Mathf.Lerp(startFreq, endFreq, frac);
                phase += 2f * Mathf.PI * currentFreq / sampleRate;
                samples[i] = Mathf.Sin(phase) * (1f - frac);
            }
            AudioClip clip = AudioClip.Create("SweepClip", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateArpeggioClip(float[] frequencies, float noteDuration)
        {
            int sampleRate = 44100;
            int noteSamples = (int)(sampleRate * noteDuration);
            int totalSamples = noteSamples * frequencies.Length;
            float[] samples = new float[totalSamples];

            for (int n = 0; n < frequencies.Length; n++)
            {
                float freq = frequencies[n];
                for (int i = 0; i < noteSamples; i++)
                {
                    float t = (float)i / sampleRate;
                    float frac = (float)i / noteSamples;
                    samples[n * noteSamples + i] = Mathf.Sin(2f * Mathf.PI * freq * t) * (1f - frac * 0.7f) * 0.5f;
                }
            }
            AudioClip clip = AudioClip.Create("ArpClip", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateNoiseSweepClip(float duration)
        {
            int sampleRate = 44100;
            int numSamples = (int)(sampleRate * duration);
            float[] samples = new float[numSamples];
            for (int i = 0; i < numSamples; i++)
            {
                float frac = (float)i / numSamples;
                samples[i] = (Random.value * 2f - 1f) * Mathf.Sin(frac * Mathf.PI) * 0.4f;
            }
            AudioClip clip = AudioClip.Create("DashNoise", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateDualToneClip(float freq1, float freq2, float duration)
        {
            int sampleRate = 44100;
            int numSamples = (int)(sampleRate * duration);
            float[] samples = new float[numSamples];
            int half = numSamples / 2;
            for (int i = 0; i < numSamples; i++)
            {
                float freq = (i < half) ? freq1 : freq2;
                float t = (float)i / sampleRate;
                samples[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.4f;
            }
            AudioClip clip = AudioClip.Create("SirenClip", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateSubBassClip(float startFreq, float endFreq, float duration)
        {
            int sampleRate = 44100;
            int numSamples = (int)(sampleRate * duration);
            float[] samples = new float[numSamples];
            float phase = 0f;
            for (int i = 0; i < numSamples; i++)
            {
                float frac = (float)i / numSamples;
                float currentFreq = Mathf.Lerp(startFreq, endFreq, frac);
                phase += 2f * Mathf.PI * currentFreq / sampleRate;
                samples[i] = Mathf.Sin(phase) * Mathf.Pow(1f - frac, 0.5f) * 0.7f;
            }
            AudioClip clip = AudioClip.Create("SubBassClip", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateSplatClip(float duration)
        {
            int sampleRate = 44100;
            int numSamples = (int)(sampleRate * duration);
            float[] samples = new float[numSamples];
            float phase = 0f;
            for (int i = 0; i < numSamples; i++)
            {
                float frac = (float)i / numSamples;
                float currentFreq = Mathf.Lerp(160f, 40f, frac);
                phase += 2f * Mathf.PI * currentFreq / sampleRate;
                float thud = Mathf.Sin(phase) * (1f - frac);
                float squelch = (Random.value * 2f - 1f) * Mathf.Pow(1f - frac, 2f) * 0.4f;
                samples[i] = Mathf.Clamp(thud + squelch, -1f, 1f);
            }
            AudioClip clip = AudioClip.Create("PaintSplatClip", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateScratchClip(float duration)
        {
            int sampleRate = 44100;
            int numSamples = (int)(sampleRate * duration);
            float[] samples = new float[numSamples];
            float phase = 0f;
            for (int i = 0; i < numSamples; i++)
            {
                float frac = (float)i / numSamples;
                // Wobbling vinyl scratch frequency modulation
                float mod = Mathf.Sin(frac * Mathf.PI * 8f);
                float freq = Mathf.Lerp(400f, 1600f, Mathf.Abs(mod));
                phase += 2f * Mathf.PI * freq / sampleRate;
                float vinylSine = Mathf.Sin(phase) * 0.5f;
                float frictionNoise = (Random.value * 2f - 1f) * 0.35f * Mathf.Abs(mod);
                samples[i] = Mathf.Clamp((vinylSine + frictionNoise) * (1f - frac * 0.3f), -1f, 1f);
            }
            AudioClip clip = AudioClip.Create("ScratchClip", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateGeyserClip(float duration)
        {
            int sampleRate = 44100;
            int numSamples = (int)(sampleRate * duration);
            float[] samples = new float[numSamples];
            float rumblePhase = 0f;
            for (int i = 0; i < numSamples; i++)
            {
                float frac = (float)i / numSamples;
                rumblePhase += 2f * Mathf.PI * 85f / sampleRate;
                float rumble = Mathf.Sin(rumblePhase) * 0.4f;
                float waterRush = (Random.value * 2f - 1f) * 0.6f;
                float env = Mathf.Sin(frac * Mathf.PI);
                samples[i] = Mathf.Clamp((rumble + waterRush) * env, -1f, 1f);
            }
            AudioClip clip = AudioClip.Create("GeyserClip", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateCrackClip(float duration)
        {
            int sampleRate = 44100;
            int numSamples = (int)(sampleRate * duration);
            float[] samples = new float[numSamples];
            float phase = 0f;
            for (int i = 0; i < numSamples; i++)
            {
                float frac = (float)i / numSamples;
                phase += 2f * Mathf.PI * Mathf.Lerp(320f, 90f, frac) / sampleRate;
                float snap = Mathf.Sin(phase) * Mathf.Pow(1f - frac, 4f);
                float splinter = (Random.value * 2f - 1f) * Mathf.Pow(1f - frac, 3f) * 0.5f;
                samples[i] = Mathf.Clamp(snap + splinter, -1f, 1f);
            }
            AudioClip clip = AudioClip.Create("CrackClip", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateEngineRevClip(float duration)
        {
            int sampleRate = 44100;
            int numSamples = (int)(sampleRate * duration);
            float[] samples = new float[numSamples];
            float phase = 0f;
            for (int i = 0; i < numSamples; i++)
            {
                float frac = (float)i / numSamples;
                float freq = Mathf.Lerp(90f, 260f, Mathf.Pow(frac, 1.4f));
                phase += 2f * Mathf.PI * freq / sampleRate;
                // Distorted sawtooth-like harmonics
                float engineHarmonic = Mathf.Sin(phase) + 0.4f * Mathf.Sin(phase * 2f) + 0.2f * Mathf.Sin(phase * 3f);
                float throttleNoise = (Random.value * 2f - 1f) * 0.2f;
                float env = Mathf.Sin(frac * Mathf.PI * 0.95f);
                samples[i] = Mathf.Clamp((engineHarmonic + throttleNoise) * env * 0.7f, -1f, 1f);
            }
            AudioClip clip = AudioClip.Create("EngineRevClip", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateCompressorClip(float duration)
        {
            int sampleRate = 44100;
            int numSamples = (int)(sampleRate * duration);
            float[] samples = new float[numSamples];
            float popPhase = 0f;
            for (int i = 0; i < numSamples; i++)
            {
                float frac = (float)i / numSamples;
                float hiss = (Random.value * 2f - 1f) * Mathf.Pow(1f - frac, 0.7f) * 0.5f;
                // Pop valve discharge right at end
                float pop = 0f;
                if (frac > 0.85f)
                {
                    popPhase += 2f * Mathf.PI * 180f / sampleRate;
                    pop = Mathf.Sin(popPhase) * Mathf.Sin((frac - 0.85f) / 0.15f * Mathf.PI) * 0.6f;
                }
                samples[i] = Mathf.Clamp(hiss + pop, -1f, 1f);
            }
            AudioClip clip = AudioClip.Create("CompressorClip", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
