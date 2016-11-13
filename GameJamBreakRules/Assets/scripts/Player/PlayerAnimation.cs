﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Text.RegularExpressions;

[RequireComponent (typeof (Animator))]
[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (AudioSource))]
public class PlayerAnimation : MonoBehaviour {

	private Animator	anim;
	private Rigidbody2D rigid;
	private Vector3 force;
	private bool jumping = false;
	private bool isDead = false;
	private AudioSource	audioSrc;
	private GameObject timer;

	public float forceJump = 2;
	public float forceRun = 0.5f;
	public float maxSpeed = 10.0f;
	public AudioClip[] deathSounds;

	private void editScore(float chrono) {
		if (chrono <= 0)
			return;
		string levelname = "level_" + Regex.Match (SceneManager.GetActiveScene ().name, @"-?\d+").Value;
		float score1 = PlayerPrefs.GetFloat (levelname + "_" + 1);
		float score2 = PlayerPrefs.GetFloat (levelname + "_" + 2);
		float score3 = PlayerPrefs.GetFloat (levelname + "_" + 3);
		score3 = score3 > 0 ? score3 : (score2 > 0 ? chrono : 0);
		score2 = score2 > 0 ? score2 : (score1 > 0 ? chrono : 0);
		score1 = score1 > 0 ? score1 : chrono;
		if (score1 >= chrono) {
			PlayerPrefs.SetFloat (levelname + "_" + 3, score2);
			PlayerPrefs.SetFloat (levelname + "_" + 2, score1);
			PlayerPrefs.SetFloat (levelname + "_" + 1, chrono);
		} else if (score2 >= chrono) {
			PlayerPrefs.SetFloat (levelname + "_" + 3, score2);
			PlayerPrefs.SetFloat (levelname + "_" + 2, chrono);
		} else if (score3 >= chrono) {
			PlayerPrefs.SetFloat (levelname + "_" + 3, chrono);
		}
		for (int i = 1; i <= 3; ++i)
			Debug.Log ("Time " + i + " on " + levelname + ": " + PlayerPrefs.GetFloat(levelname + "_" + i));
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "DeathObject" && !anim.GetBool("IsDead")) {
			editScore (timer.GetComponent<timer> ().getTimer ());
			isDead = true;
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
		timer = GameObject.Find ("Timer");;
	}

	void Update () {
		if (isDead && anim.GetCurrentAnimatorStateInfo (0).IsName ("Stickman_dab"))
			SceneManager.LoadScene("Levels/level_" + (int.Parse(Regex.Match(SceneManager.GetActiveScene().name, @"-?\d+").Value) + 1));
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
		if (!isDead && timer.GetComponent<timer> ().getTimer () <= 0.0f)
			SceneManager.LoadScene (SceneManager.GetActiveScene().name);
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
