using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public int HP;
	ParticleSystem hitParticles;

	void Awake () {
		hitParticles = GetComponentInChildren<ParticleSystem> ();
	}

	public void TakeDamage (int amount, Vector3 hitPoint) {
		hitParticles.transform.position = hitPoint;

		// And play the particles.
		hitParticles.Play();

		HP -= amount;
		if (HP <= 0) {
			Destroy (gameObject);
		}
	}
}
