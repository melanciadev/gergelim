using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Gergelim {
	public class Cam:MonoBehaviour {
		public static Cam me;

		public Transform tr { get; private set; }
		public Renderer multiply { get; private set; }
		public Renderer screen { get; private set; }

		public static Color multiplyColour = Color.blue;
		public static Color screenColour = Color.yellow;

		Vector2 expectedPos = Vector2.zero;

		void Awake() {
			me = this;
			tr = transform;
			multiply = tr.Find("multiply").GetComponent<Renderer>();
			screen = tr.Find("screen").GetComponent<Renderer>();
		}

		void Start() {
			if (Fox.me != null) {
				expectedPos = Fox.me.tr.position;
				Move(expectedPos.x,expectedPos.y);
			}
		}
		
		void LateUpdate() {
			if (Fox.me != null) expectedPos = Fox.me.tr.position;
			Move(expectedPos.x,expectedPos.y,Time.deltaTime*4);
		}

		void Move(float x,float y,float t = 1) {
			var pos = tr.position;
			tr.position = new Vector3(Mathf.Lerp(pos.x,x,t),Mathf.Lerp(pos.y,y+.5f,t),pos.z);
		}

		public static void Multiply(float a = 0) {
			if (me == null) return;
			if (Mathf.Approximately(a,0)) {
				me.multiply.enabled = false;
			} else {
				me.multiply.material.color = new Color(
					multiplyColour.r*a,
					multiplyColour.g*a,
					multiplyColour.b*a,
				1);
			}
		}

		public static void Screen(float a = 0) {
			if (me == null) return;
			if (Mathf.Approximately(a,0)) {
				me.screen.enabled = false;
			} else {
				me.screen.material.color = new Color(
					1-(1-screenColour.r)*a,
					1-(1-screenColour.g)*a,
					1-(1-screenColour.b)*a,
				1);
			}
		}
	}
}