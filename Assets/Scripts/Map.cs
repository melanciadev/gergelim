using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Gergelim {
	public class Map:MonoBehaviour {
		public static float time;
		float timeStart;
		//0: meia noite
		//32: meio dia
		//64: meia noite dnv (volta pro 0)

		const float duration = 120;
		const float usedDuration = 64;

		void Awake() {
			//
		}

		void Start() {
			//
		}

		void Update() {
			time += Time.deltaTime*(usedDuration/duration);
			while (time >= 64) time -= 64;
			// 
		}
	}
}