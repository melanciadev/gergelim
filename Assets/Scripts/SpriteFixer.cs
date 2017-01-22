using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spriter2UnityDX;

namespace Melancia.Gergelim {
	public class SpriteFixer:MonoBehaviour {
		void Awake() {
			Fix(transform);
		}

		void Fix(Transform t) {
			for (int a = 0; a < t.childCount; a++) {
				var nt = t.GetChild(a);
				var spr = nt.GetComponent<SpriteRenderer>();
				if (spr != null) {
					nt.gameObject.AddComponent<SpriteFix>();
					Fix(nt);
				} else {
					var ent = nt.GetComponent<EntityRenderer>();
					if (ent != null) {
						nt.gameObject.AddComponent<SpriteFix>();
					} else {
						Fix(nt);
					}
				}
			}
		}
	}
}