using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Gergelim {
	public class Map:MonoBehaviour {
		public static float time;
		//0: meia noite
		//pi: meio dia
		//2*pi: meia noite (volta pro 0)

		float timeStart;

		const float duration = 120;

		void Awake() {
			time = 0;
			timeStart = Time.time;
		}

		void Update() {
			time = ((Time.time-timeStart)*Mathf.PI*2/duration)%(Mathf.PI*2);
			Cam.multiplyColour = new Color32(46,70,129,255);
			Cam.screenColour = new Color32(77,(byte)(12+65*Mathf.Clamp01(-Mathf.Cos(time))),12,255);
			Cam.Multiply(Mathf.Clamp01(Mathf.Cos(time)+.35f)*.8f);
			Cam.Screen(-Mathf.Cos(time)*.3f+.2f);
		}
	}
}