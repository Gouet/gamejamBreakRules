using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Animator))]
public class PlayerAnimation : MonoBehaviour {

	private Animator	anim;

	void Start () {
		anim = this.GetComponent<Animator> ();
	}

	void Update () {
		if (Input.GetButton ("Jump"))
			anim.SetTrigger ("Jump");
		else
			anim.ResetTrigger ("Jump");
		if (Input.GetButton ("Horizontal")) {
			Vector3 vec = this.transform.localScale;
			if ((Input.GetAxis ("Horizontal") > 0 && vec.x > 0) || (Input.GetAxis("Horizontal") < 0 && vec.x < 0))
				vec.x *= -1;
		this.transform.localScale = vec;
		anim.SetBool ("IsRunning", true);
		}
		else
			anim.SetBool ("IsRunning", false);
	}
}
