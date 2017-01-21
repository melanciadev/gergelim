using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Gergelim {
	public static class Utils {
		public static float EaseIn(float t) {
			if (t <= 0) return 0;
			if (t >= 1) return 1;
			return t*t;
		}

		public static float EaseOut(float t) {
			if (t <= 0) return 0;
			if (t >= 1) return 1;
			return (2-t)*t;
		}

		public static float Ease(float t) {
			if (t <= 0) return 0;
			if (t >= 1) return 1;
			return (3-2*t)*t*t;
		}
	}
}