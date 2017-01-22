using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Gergelim {
	public class Animal : MonoBehaviour {

		Animator anim;

		void Start(){
			anim = GetComponent<Animator> ();
		}

		// Update is called once per frame
		void Update () {
			if (Map.night && !anim.GetBool("NightTime")) {
				anim.SetBool ("NightTime", true);
			}
			else if (Map.day) {
				anim.SetBool ("NightTime", false);
			}
		}
	}
}
