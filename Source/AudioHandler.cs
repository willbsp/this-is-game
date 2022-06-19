using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace computing_project
{
    public static class AudioHandler
    {

        private static List<KeyValuePair<string, SoundEffect>> _soundEffects; // Stores the sound effects with a string to identify each one.
        private static Song _music; // The current song playing.
        public static float SoundEffectVolume = 1f; // Volumes, can be set from anywhere.
        public static float MusicVolume = 0.25f;

        public static void Initialize()
        {
            MusicVolume = 0.25f;
            _soundEffects = new List<KeyValuePair<string, SoundEffect>>();
        }

        // Add a sound effect to the list along with a string for identification.
        public static void AddSoundEffect(string soundEffect, Game game)
        {
            _soundEffects.Add(new KeyValuePair<string, SoundEffect>(soundEffect, game.Content.Load<SoundEffect>(soundEffect)));
        }

        // Play a specified sound effect by name.
        public static void PlaySoundEffect(string soundEffect)
        {
            for (int i = 0; i < _soundEffects.Count; i++)
            {
                if (_soundEffects.ElementAt(i).Key == soundEffect)
                {
                    _soundEffects.ElementAt(i).Value.Play();
                    var instance = _soundEffects.ElementAt(i).Value.CreateInstance();
                    instance.Volume = SoundEffectVolume;
                }
            }
        }

        // Get the duration of a specific sound effect.
        public static float GetSoundEffectDuration(string soundEffect)
        {
            for (int i = 0; i < _soundEffects.Count; i++)
            {
                if (_soundEffects.ElementAt(i).Key == soundEffect)
                {
                    return (float)_soundEffects.ElementAt(i).Value.Duration.TotalSeconds;
                }
            }
            return 0f;
        }

        // Load a song to play.
        public static void LoadSong(string song, Game game)
        {
            _music = game.Content.Load<Song>(song);
        }

        // Play the loaded song, only one song can be loaded at a time.
        public static void PlaySong(bool repeat)
        {
            MediaPlayer.Volume = MusicVolume;
            MediaPlayer.IsRepeating = repeat;
            MediaPlayer.Play(_music);
        }

        // Stop the song.
        public static void StopSong()
        {
            MediaPlayer.Stop();
        }

        // Get the duration of a song.
        public static float GetSongDuration()
        {
            return (float)_music.Duration.TotalSeconds;
        }

    }
}
