using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Gergelim {
	public class Area:MonoBehaviour {
		public bool teleport;
		public Vector2 teleportTo;
		public bool leaf;

		public static List<AreaInfo> areas = new List<AreaInfo>();
		
		void Awake() {
			var tr = transform;
			var p = (Vector2)tr.position;
			var s = tr.lossyScale*.5f;
			teleportTo += p;
			areas.Add(new AreaInfo {
				minX = p.x-s.x,
				maxX = p.x+s.x,
				minY = p.y-s.y,
				maxY = p.y+s.y,
				tel = teleport,
				telX = teleportTo.x,
				telY = teleportTo.y,
				leaf = leaf,
			});
			Destroy(gameObject);
		}
		
		void OnDrawGizmos() {
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(transform.position,transform.lossyScale);
			if (teleport) {
				Gizmos.color = Color.yellow;
				var p = transform.position+(Vector3)teleportTo;
				Gizmos.DrawWireCube(p,Vector3.one);
				Gizmos.DrawLine(transform.position,p);
				Gizmos.DrawIcon(transform.position,"teleport.png");
			} else {
				Gizmos.DrawIcon(transform.position,"shift.png");
			}
		}

		public static bool GetArea(Vector2 pos,out AreaInfo info) {
			for (int a = 0; a < areas.Count; a++) {
				if (areas[a].minX <= pos.x && areas[a].maxX >= pos.x && areas[a].minY <= pos.y && areas[a].maxY >= pos.y) {
					info = areas[a];
					return true;
				}
			}
			info = new AreaInfo();
			return false;
		}
	}

	public struct AreaInfo {
		public float minX,maxX,minY,maxY,telX,telY;
		public bool tel,leaf;
	}
}