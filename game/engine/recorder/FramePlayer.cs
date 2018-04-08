using System;
using System.Collections.Generic;
using Godot;

namespace Recorder {

    public class FramePlayer<S> {
        public Recording<S> Recording;
        public delegate void StateSetter(S state);

        private StateSetter setState;
        private float currentTime = 0f;
        private bool isPlaying = true;

        public FramePlayer(Recording<S> recording, StateSetter setState) {
            this.setState = setState;
            this.Recording = recording;
        }

        public void Process(float delta) {
            if (!isPlaying) return;
            this.currentTime += delta;
            if (this.currentTime > this.Recording.Length) {
                this.StartPlayback();
            }
            var lastFrame = this.GetLastFrame(this.currentTime);
            if (lastFrame != null) {
                this.setState(lastFrame.State);
            }
        }

        public void StartPlayback() {
            this.isPlaying = true;
            this.currentTime = 0;
        }

        public void StopPlayback() {
            this.isPlaying = false;
        }

        /// <summary>
        /// Sorts in reverse order. The first element of the array will be the one
        /// with the greatest timestamp.
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        private List<Frame<S>> SortFrames(List<Frame<S>> entries) {
            entries.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));
            return entries;
        }

        private Frame<S> GetLastFrame(float time) {
            try {
                return SortFrames(this.Recording.Entries).FindAll(
                    (frame) => frame.Timestamp <= time
                )[0];
            }
            catch (ArgumentOutOfRangeException e) {
                GD.Print(e.Message);
                return null;
            }
        }
    }
}
