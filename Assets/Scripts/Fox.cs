using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Gergelim {
	public class Fox:MonoBehaviour {
		public Transform tr { get; private set; }
		public Rigidbody2D rb { get; private set; }
		public Collider2D col { get; private set; }

		public static Fox me { get; private set; }

		Dictionary<Collider2D,Collision2D> cols;
		bool grounded = false;

		const float velocity = 3;
		const float jump = 5;
		const float zVelocity = 2;

		float jumpTimeout = 0;

		float zPosition = 0;
		bool zTransitioning = false;
		bool zBack = false;

		bool teleport = false,leaf = false;
		Vector2 teleportFrom,teleportTo;
		float teleportVel;
		float teleportProgress;

		static int frontMask = -1;
		static int backMask = -1;

		AudioClip clip;

		void Awake() {
			me = this;
			tr = transform;
			rb = GetComponent<Rigidbody2D>();
			col = tr.Find("collider").GetComponent<Collider2D>();
			cols = new Dictionary<Collider2D,Collision2D>();
		}
		
		void Start() {
			if (frontMask < 0) {
				frontMask = LayerMask.NameToLayer("Front");
				backMask = LayerMask.NameToLayer("Back");
			}
			col.gameObject.layer = frontMask;
			clip = Resources.Load<AudioClip>("p");
		}

		void Update() {
			if (teleport) {
				teleportProgress += Time.deltaTime*teleportVel;
				if (teleportProgress >= 1) {
					teleportProgress = 1;
					teleport = false;
					SetRigidbody(true);
					if (leaf) rb.AddForce(Vector2.up*jump,ForceMode2D.Impulse);
				} else if (leaf && Input.GetButton("jump")) {
					teleport = false;
					SetRigidbody(true);
					rb.AddForce(Vector2.up*jump,ForceMode2D.Impulse);
				}
				tr.position = new Vector3(
					Mathf.Lerp(teleportFrom.x,teleportTo.x,teleportProgress),
					Mathf.Lerp(teleportFrom.y,teleportTo.y,teleportProgress),
					tr.position.z
				);
			} else if (zTransitioning) {
				if (zBack) {
					zPosition += Time.deltaTime*zVelocity;
					if (zPosition >= 1) {
						zPosition = 1;
						zTransitioning = false;
						SetRigidbody(true);
						col.gameObject.layer = backMask;
					}
				} else {
					zPosition -= Time.deltaTime*zVelocity;
					if (zPosition <= 0) {
						zPosition = 0;
						zTransitioning = false;
						SetRigidbody(true);
						col.gameObject.layer = frontMask;
					}
				}
				tr.position = new Vector3(tr.position.x,tr.position.y,Utils.Ease(zPosition));
			} else {
				float x = Input.GetAxisRaw("x");
				if (Mathf.Abs(x) >= .25f) tr.position += new Vector3(x*velocity*Time.deltaTime,0,0);
				if (jumpTimeout > 0) {
					jumpTimeout -= Time.deltaTime;
					if (jumpTimeout < 0) jumpTimeout = 0;
				}
				if (grounded && Mathf.Approximately(0,jumpTimeout) && Input.GetButton("jump")) {
					rb.AddForce(Vector2.up*jump,ForceMode2D.Impulse);
					jumpTimeout = .5f;
				}
				if (Input.GetButtonDown("action")) {
					AreaInfo info;
					if (Area.GetArea(tr.position,out info)) {
						if (info.tel) {
							TeleportTo(new Vector2(info.telX,info.telY),info.leaf);
						} else {
							SwitchDepth();
						}
					}
				}
			}
			if (Input.GetButtonDown("alt")) tr.PlaySound(clip,1,1);
		}
		
		void SwitchDepth() {
			zTransitioning = true;
			zBack = !zBack;
			SetRigidbody(false);
		}
		
		void TeleportTo(Vector2 pos,bool leaf) {
			teleport = true;
			teleportFrom = tr.position;
			teleportTo = pos;
			teleportVel = 4f/Vector2.Distance(teleportFrom,pos);
			teleportProgress = 0;
			if (leaf) {
				this.leaf = true;
				teleportVel *= .5f;
			} else {
				this.leaf = false;
			}
			SetRigidbody(false);
		}

		void SetRigidbody(bool on) {
			col.enabled = on;
			rb.isKinematic = !on;
			if (!on) {
				rb.velocity = Vector2.zero;
				cols.Clear();
			}
		}

		void FixedUpdate() {
			grounded = false;
			foreach (var col in cols) {
				for (int a = 0; a < col.Value.contacts.Length; a++) {
					if (col.Value.contacts[a].normal.y >= 0) {
						grounded = true;
						return;
					}
				}
			}
		}

		void OnCollisionEnter2D(Collision2D col) {
			cols[col.collider] = col;
		}

		void OnCollisionStay2D(Collision2D col) {
			cols[col.collider] = col;
		}

		void OnCollisionExit2D(Collision2D col) {
			cols.Remove(col.collider);
		}
	}
}