using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spriter2UnityDX;

namespace Melancia.Gergelim {
	public class SpriteFix:MonoBehaviour {
		Transform tr;
		SpriteRenderer spriteRenderer;
		EntityRenderer entityRenderer;
		float z;
		
		void Awake() {
			tr = transform;
			spriteRenderer = GetComponent<SpriteRenderer>();
			entityRenderer = GetComponent<EntityRenderer>();
			UpdateDepth();
		}

		void FixedUpdate() {
			if (z != tr.position.z) UpdateDepth();
		}

		void UpdateDepth() {
			z = tr.position.z;
			int order = -(int)((z+6)*100);
			if (spriteRenderer != null) spriteRenderer.sortingOrder = order;
			if (entityRenderer != null) entityRenderer.SortingOrder = order;
		}
	}
}