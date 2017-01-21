using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Gergelim {
	[RequireComponent(typeof(AudioSource))]
	public class Music:MonoBehaviour {
		public bool autoPause = true;
		public bool circleShape = false;
		public float feather = 0;

		Transform tr;
		AudioSource aud;

		bool useRadius;
		Vector2 centre;
		float innerRadius,innerRadiusSqr,outerRadius,outerRadiusSqr;
		Vector2 innerMin,innerMax,outerMin,outerMax;
		float finalVolume;

		float expectedVolume = 0;

		float stepVelocity = .15f;

		void Awake() {
			tr = transform;
			aud = GetComponent<AudioSource>();
		}

		void Start() {
			finalVolume = aud.volume;
			stepVelocity *= finalVolume;
			centre = tr.position;
			Vector2 s = tr.lossyScale;
			float sx = Mathf.Abs(s.x)*.5f;
			float sy = Mathf.Abs(s.y)*.5f;
			float f = feather*.5f;
			if (circleShape) {
				useRadius = true;
				float r = (sx+sy)*.5f;
				innerRadius = r-feather*.5f;
				innerRadiusSqr = innerRadius*innerRadius;
				outerRadius = r+feather*.5f;
				outerRadiusSqr = outerRadius*outerRadius;
			} else {
				innerMin = new Vector2(centre.x-sx+f,centre.y-sy+f);
				innerMax = new Vector2(centre.x+sx-f,centre.y+sy-f);
				outerMin = new Vector2(centre.x-sx-f,centre.y-sy-f);
				outerMax = new Vector2(centre.x+sx+f,centre.y+sy+f);
			}
			aud.volume = 0;
			UpdateExpectedVolume();
		}

		void Update() {
			if (Cam.me != null) {
				if (Cam.me.positionUpdate) UpdateExpectedVolume();
				tr.position = Cam.me.tr.position;
			}
			if (aud.volume > expectedVolume) {
				aud.volume -= Time.deltaTime*stepVelocity;
				if (aud.volume <= expectedVolume) {
					aud.volume = expectedVolume;
					if (autoPause && Mathf.Approximately(0,aud.volume) && aud.isPlaying) aud.Pause();
				}
			} else if (aud.volume < expectedVolume) {
				aud.volume += Time.deltaTime*stepVelocity;
				if (aud.volume > expectedVolume) aud.volume = expectedVolume;
				if (autoPause && !aud.isPlaying) aud.UnPause();
			}
		}

		void UpdateExpectedVolume() {
			Vector2 pos = Cam.me.tr.position;
			if (useRadius) {
				float sd = (pos-centre).sqrMagnitude;
				if (sd < innerRadiusSqr) {
					expectedVolume = 1;
				} else if (sd < outerRadiusSqr) {
					expectedVolume = Mathf.InverseLerp(outerRadius,innerRadius,Mathf.Sqrt(sd));
				} else {
					expectedVolume = 0;
				}
			} else {
				if (pos.x >= innerMin.x && pos.x <= innerMax.x && pos.y >= innerMin.y && pos.y <= innerMax.y) {
					expectedVolume = 1;
				} else if (pos.x >= outerMin.x && pos.x <= outerMax.x && pos.y >= outerMin.y && pos.y <= outerMax.y) {
					float x,y;
					if (pos.x < centre.x) {
						x = Mathf.InverseLerp(outerMin.x,innerMin.x,pos.x);
					} else {
						x = Mathf.InverseLerp(outerMax.x,innerMax.x,pos.x);
					}
					if (pos.y < centre.y) {
						y = Mathf.InverseLerp(outerMin.y,innerMin.y,pos.y);
					} else {
						y = Mathf.InverseLerp(outerMax.y,innerMax.y,pos.y);
					}
					expectedVolume = Mathf.Min(x,y);
				} else {
					expectedVolume = 0;
				}
			}
		}

		void OnDrawGizmos() {
			if (Application.isPlaying) return;
			const int featherMarks = 3;
			if (feather < 0) feather = 0;
			Vector2 s = transform.lossyScale;
			float sx = Mathf.Abs(transform.lossyScale.x);
			float sy = Mathf.Abs(transform.lossyScale.y);
			float r = (sx+sy)*.25f;
			if (circleShape) {
				if (feather > r) feather = r;
			} else {
				float m = Mathf.Min(sx,sy);
				if (feather > m) feather = m;
			}
			for (int a = 0; a < featherMarks+2; a++) {
				float at = (float)a/(featherMarks+1);
				Gizmos.color = new Color(0,1,1,at*.75f+.25f);
				float t = feather*(at-.5f);
				if (circleShape) {
					Gizmos.DrawWireSphere(transform.position,r-t);
				} else {
					Gizmos.DrawWireCube(transform.position,new Vector3(sx-t*2,sy-t*2,1));
				}
				if (feather <= 0) break;
			}
		}
	}
}