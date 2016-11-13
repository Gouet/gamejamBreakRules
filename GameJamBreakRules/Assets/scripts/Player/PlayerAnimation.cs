using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Animator))]
[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (AudioSource))]
public class PlayerAnimation : MonoBehaviour {

	private Animator	anim;
	private Rigidbody2D rigid;
	private Vector3 force;
	private bool jumping = false;
	private AudioSource	audioSrc;

	public float forceJump = 2;
	public float forceRun = 0.5f;
	public float maxSpeed = 10.0f;
	public AudioClip[] deathSounds;

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "DeathObject" && !anim.GetBool("IsDead")) {
			anim.SetBool("IsDead", true);
			anim.SetTrigger ("Dies");
			if (deathSounds.Length > 1) {
				audioSrc.clip = deathSounds [coll.gameObject.name == "toaster" ? 1 : 0];
				audioSrc.Play ();
			}
		}
	}

	void Start () {
		anim = this.GetComponent<Animator> ();
		rigid = this.GetComponent<Rigidbody2D> ();
		audioSrc = this.GetComponent<AudioSource> ();
	}

	void Update () {
		if (!anim.GetBool ("IsDead")) {
			if (Input.GetButton ("Jump") && !jumping) {
				anim.SetTrigger ("Jump");
				force = (transform.up * forceJump) / Time.fixedDeltaTime;
				rigid.AddForce (force);
				jumping = true;
			}
			else
				anim.ResetTrigger ("Jump");

			if (Input.GetButton ("Horizontal")) {
				Vector3 vec = this.transform.localScale;
				if ((Input.GetAxis ("Horizontal") > 0 && vec.x > 0) || (Input.GetAxis ("Horizontal") < 0 && vec.x < 0))
					vec.x *= -1;
				this.transform.localScale = vec;
				anim.SetBool ("IsRunning", true);
				force = (transform.right * forceRun * vec.x * -1) / Time.deltaTime;
				rigid.AddForce (force);
			} else
				anim.SetBool ("IsRunning", false);

			if (jumping && rigid.velocity.y == 0)
				jumping = false;
		}
	}

	void FixedUpdate() {
		if (rigid.velocity.x > maxSpeed) {
			force = rigid.velocity;
			force.x = maxSpeed;
			rigid.velocity = force;
		} else if (rigid.velocity.x < -maxSpeed) {
			force = rigid.velocity;
			force.x = -maxSpeed;
			rigid.velocity = force;
		}
	}
}
