using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Melancia.Gergelim {
	public class Sound:MonoBehaviour {
		public static Sound me { get; private set; }

		public AudioMixerGroup master;
		AudioMixerSnapshot lowpass0,lowpass1;
		
		AudioSource[] aud;
		const int poolSize = 16;
		int audIndex;

		const float maxDistance = 20;

		void Awake() {
			me = this;
			aud = new AudioSource[poolSize];
			for (int a = 0; a < poolSize; a++) {
				var s = aud[a] = gameObject.AddComponent<AudioSource>();
				s.loop = false;
				s.playOnAwake = false;
				s.outputAudioMixerGroup = master;
			}
			audIndex = 0;
			lowpass0 = master.audioMixer.FindSnapshot("0");
			lowpass1 = master.audioMixer.FindSnapshot("1");
		}

		public static void Play(AudioClip clip,float volume = 1,float pitch = 1) {
			Play(clip,volume,pitch,Cam.me.tr.position);
		}

		public static void Play(AudioClip clip,float volume,float pitch,Vector2 at) {
			if (me != null) me.PlaySound(clip,volume,pitch,at);
		}

		public void PlaySound(AudioClip clip,float volume,float pitch,Vector2 at) {
			var cam = Cam.me.tr.position;
			float sd = (at-(Vector2)cam).sqrMagnitude;
			if (sd >= maxDistance*maxDistance) return;
			int n = 0;
			while (n < poolSize && aud[audIndex].isPlaying) {
				audIndex++;
				if (audIndex >= poolSize) audIndex = 0;
				n++;
			}
			aud[audIndex].Stop();
			aud[audIndex].clip = clip;
			aud[audIndex].volume = volume*(1-Utils.Ease(Mathf.Sqrt(sd)/maxDistance));
			aud[audIndex].pitch = pitch;
			aud[audIndex].panStereo = Mathf.Atan(at.x-cam.x)/Mathf.PI;
			aud[audIndex].time = 0;
			aud[audIndex].Play();
			audIndex++;
			if (audIndex >= poolSize) audIndex = 0;
		}

		public static void SetLowpass(bool enable) {
			if (me == null) return;
			(enable ? me.lowpass1 : me.lowpass0).TransitionTo(.5f);
		}
	}

	public static class SoundExtension {
		public static void PlaySound(this Transform bh,AudioClip clip,float volume = 1,float pitch = 1) {
			Sound.Play(clip,volume,pitch,bh.transform.position);
		}
	}
}