using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.IO;

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

    [HideInInspector]
    public bool facingRight = false;
    [HideInInspector]
    public bool jump = false;
    public float moveForce = 365f;
    public float jumpForce = 1000f;
    public Transform groundCheck;


    private bool grounded = false;
	public float maxSpeed = 10.0f;
	public AudioClip[] deathSounds;

	private void editScore(float chrono) {
		if (chrono <= 0 || isDead)
			return;
        Debug.Log("Editing score");
		string levelname = "level_" + Regex.Match (SceneManager.GetActiveScene ().name, @"-?\d+").Value;

        List<float> scores = new List<float>();

        scores.Add(PlayerPrefs.GetFloat(levelname + "_" + 1));
        scores.Add(PlayerPrefs.GetFloat(levelname + "_" + 2));
        scores.Add(PlayerPrefs.GetFloat(levelname + "_" + 3));
        scores.Add(chrono);

        scores.Sort();

        PlayerPrefs.SetFloat(levelname + "_" + 3, scores[1]);
        PlayerPrefs.SetFloat(levelname + "_" + 2, scores[2]);
        PlayerPrefs.SetFloat(levelname + "_" + 1, scores[3]);
        
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
                StartCoroutine(calllvl(anim.GetCurrentAnimatorClipInfo(0).Length));
			}
		}
	}

    IEnumerator calllvl(int length)
    {
        yield return new WaitForSeconds(length);

        string lvl = "Levels/level_" + (int.Parse(Regex.Match(SceneManager.GetActiveScene().name, @"-?\d+").Value) + 1);
        
        if (Application.CanStreamedLevelBeLoaded(lvl))
            SceneManager.LoadScene(lvl);
        else
            SceneManager.LoadScene("Highscores");
    }

	void Start () {
		anim = this.GetComponent<Animator> ();
		rigid = this.GetComponent<Rigidbody2D> ();
		audioSrc = this.GetComponent<AudioSource> ();
		timer = GameObject.Find ("Timer");;
	}

	void Update () {
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

        if (Input.GetButtonDown("Jump") && grounded)
            jump = true;
        if (Input.GetButton("Cancel"))
            SceneManager.LoadScene("Menu");
		if (!isDead && timer.GetComponent<timer> ().getTimer () <= 0.0f)
			SceneManager.LoadScene (SceneManager.GetActiveScene().name);
	}

	void FixedUpdate() {
        if (isDead)
            return;

        float h = Input.GetAxis("Horizontal");

        //anim.SetFloat("Speed", Mathf.Abs(h));

        if (h * rigid.velocity.x < maxSpeed)
            rigid.AddForce(Vector2.right * h * moveForce);

        if (Mathf.Abs(rigid.velocity.x) > maxSpeed)
            rigid.velocity = new Vector2(Mathf.Sign(rigid.velocity.x) * maxSpeed, rigid.velocity.y);

        if (h > 0 && facingRight)
            Flip();
        else if (h < 0 && !facingRight)
            Flip();

        anim.SetBool("IsRunning", h != 0);

        if (jump)
        {
            anim.SetTrigger("Jump");
            rigid.AddForce(new Vector2(0f, jumpForce));
            jump = false;
        }
	}

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
