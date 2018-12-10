﻿/*
 * Audio.cs - Music and sound effects playback base.
 *
 * Copyright (C) 2015-2017  Wicked_Digger<wicked_digger@mail.ru>
 * Copyright (C) 2018       Robert Schneckenhaus<robert.schneckenhaus@web.de>
 *
 * This file is part of freeserf.net.freeserf.net is based on freeserf.
 *
 * freeserf.net is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * freeserf.net is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with freeserf.net. If not, see<http://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;

namespace Freeserf
{
    public class ExceptionAudio : ExceptionFreeserf
    {
        public ExceptionAudio(string description)
            : base(description)
        {

        }

        public virtual string Platform => "Abstract";

        public override string System => "audio";

        public override string Message => "[" + System + ":" + Platform + "]" + Description;
    }

    public abstract class Audio
    {
        public enum TypeSfx
        {
            Message = 1,
            Accepted = 2,
            NotAccepted = 4,
            Undo = 6,
            Click = 8,
            Fight01 = 10,
            Fight02 = 14,
            Fight03 = 18,
            Fight04 = 22,
            ResourceFound = 26,
            PickBlow = 28,
            MetalHammering = 30,
            AxBlow = 32,
            TreeFall = 34,
            WoodHammering = 36,
            Elevator = 38,
            HammerBlow = 40,
            Sawing = 42,
            MillGrinding = 43,
            BackswordBlow = 44,
            GeologistSampling = 46,
            Planting = 48,
            Digging = 50,
            Mowing = 52,
            FishingRodReel = 54,
            Unknown21 = 58,
            PigOink = 60,
            GoldBoils = 62,
            Rowing = 64,
            Unknown25 = 66,
            SerfDying = 69,
            BirdChirp0 = 70,
            BirdChirp1 = 74,
            Ahhh = 76,
            BirdChirp2 = 78,
            BirdChirp3 = 82,
            Burning = 84,
            Unknown28 = 86,
            Unknown29 = 88
        }

        public enum TypeMidi
        {
            None = -1,
            Track0 = 0,
            Track1 = 1,
            Track2 = 2,
            Track3 = 4,
            TrackLast = Track3,
        }

        public abstract class VolumeController
        {
            public abstract float GetVolume();
            public abstract void SetVolume(float volume);
            public abstract void VolumeUp();
            public abstract void VolumeDown();
        }

        public abstract class Track
        {
            public abstract void Play();
        }

        public abstract class Player
        {
            protected Dictionary<int, Track> trackCache = new Dictionary<int, Track>();
            protected bool enabled = true;

            public virtual Track PlayTrack(int trackID)
            {
                if (!IsEnabled)
                {
                    return null;
                }

                Track track;

                if (!trackCache.ContainsKey(trackID))
                {
                    track = CreateTrack(trackID);

                    if (track != null)
                    {
                        trackCache[trackID] = track;
                    }
                }
                else
                {
                    track = trackCache[trackID];
                }

                if (track != null)
                {
                    track.Play();
                }

                return track;
            }

            public abstract void Enable(bool enable);

            public virtual bool IsEnabled => enabled;

            public abstract VolumeController GetVolumeController();

            protected abstract Track CreateTrack(int trackID);

            protected abstract void Stop();
        }

        protected float volume = 0.75f;

        /* Common audio. */

        public abstract VolumeController GetVolumeController();
        public abstract Player GetSoundPlayer();
        public abstract Player GetMusicPlayer();
    }

    public interface IAudioFactory
    {
        Audio GetAudio();
    }
}
