using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	public float Force;
	public int Damage;

	private Rigidbody myRigidbody;
	private bool isFlying;
	Vector3 initialPosition;

	void Awake() {
		myRigidbody = GetComponentInChildren<Rigidbody> ();
	}

	void Start() {
		initialPosition = new Vector3 (transform.position.x, 0.0f, transform.position.z);
		isFlying = true;
		float initialZ = transform.position.z;
		Vector3 force = new Vector3(transform.forward.x * Force, transform.forward.y * Force, transform.forward.z * Force);
		myRigidbody.AddForce (force, ForceMode.VelocityChange);
		float angle = Vector3.Angle (transform.forward, Vector3.forward);
		float vel0sq = Mathf.Pow (force.magnitude, 2.0f);
		float radAngle = Mathf.Deg2Rad * angle;
		float h = transform.position.y;
		//Debug.Log (h);
			
		float secondPart = 1 + Mathf.Sqrt (1 + (2 * Physics.gravity.magnitude * h) / (vel0sq * Mathf.Sqrt(Mathf.Sin(radAngle))));
		//Debug.Log (secondPart.ToString ());
		//Debug.Log (Mathf.Sin (2 * radAngle));
		float distance = (vel0sq / (2 * Physics.gravity.magnitude)) * Mathf.Sin (2 * radAngle) * (secondPart);
		//Debug.Log (distance);
		//Debug.Log (distance + initialZ);
		//Debug.Log (transform.position.z + transform.position.x);

	}

	bool shouldStop;
	public float SlowDown;
	Vector3 collisionVelocity;
	float t = 0.0f;
	public float LerpSpeed;

	void Update() {
		if (isFlying) {
			transform.rotation = Quaternion.LookRotation (myRigidbody.velocity);
		}

		if (shouldStop) {		

			transform.Translate (transform.forward * Mathf.Lerp (SlowDown, 0.0f, t));
			t += LerpSpeed * Time.deltaTime;

			if (t >= 1.0f) {
				shouldStop = false;
			}
		}
	}

	void OnCollisionEnter(Collision other) {
		if (other.gameObject.GetComponentInParent<Arrow>() == null) {			
			isFlying = false;
			shouldStop = true;
			//myRigidbody.velocity = Vector3.zero;
			transform.SetParent (other.transform);
			//GetComponent<Collider> ().isTrigger = true;
			Destroy (GetComponent<Collider> ());
			//collisionVelocity = other.relativeVelocity;
			Destroy (myRigidbody);
			//myRigidbody.useGravity = false;
		}
		Vector3 myPosition = new Vector3 (transform.position.x, 0.0f, transform.position.z);
		if (other.gameObject.GetComponent<Enemy> () != null) {
			other.gameObject.GetComponent<Enemy> ().TakeDamage (Damage, transform.position);
		}
		// Debug.Log(Vector3.Distance(initialPosition, myPosition));
	}
}
