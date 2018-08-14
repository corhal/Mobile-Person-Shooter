using UnityEngine;
using System.Collections;

public class Shooter : MonoBehaviour {

	public GameObject MarkerPrefab;

	public GameObject ShootPoint;
	public GameObject ArrowPrefab;
	public GameObject Target;

	public int Damage;
	public float ShootForce;
	public float HorizontalSpread;
	public float VerticalSpread;

	float g = Physics.gravity.magnitude;
	float v0sq;
	float l;
	float h;

	int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
	float camRayLength = 100f;          // The length of the ray from the camera into the scene.

	Rigidbody myRigidbody;

	void Awake () {
		myRigidbody = GetComponent<Rigidbody> ();
		floorMask = LayerMask.GetMask ("Floor");
	}

	// Use this for initialization
	void Start () {
		v0sq = Mathf.Pow (ShootForce, 2.0f);
	}

	float[] RandomizeCirclePoint() {
		float[] xz = new float[2];
		float radius = Target.transform.localScale.x / 2;
		float t = 2 * Mathf.PI * Random.Range (0, radius);
		float u = Random.Range (0, radius) + Random.Range (0, radius);
		float r;

		if (u > radius) {
			r = 2 - u;
		} else {
			r = u;
		}

		Debug.Log (t);

		// t = t * Mathf.Rad2Deg;

		float x = r * Mathf.Cos (t);
		float z = r * Mathf.Sin (t);
		xz[0] = Target.transform.position.x + x;
		xz[1] = Target.transform.position.z + z;

		return xz;
	}

	void Shoot(float x, float y, float z) {		
		float dx = Random.Range (-HorizontalSpread, HorizontalSpread);

		transform.rotation = Quaternion.LookRotation (new Vector3(x, 0.0f, z));

		Vector3 distance = new Vector3 (x, ShootPoint.transform.position.y, z);
		l = Vector3.Distance (ShootPoint.transform.position, distance);
		h = ShootPoint.transform.position.y - y;
		Debug.Log ("l: " + l + ", h: " + h);

		float dAlphaX = Random.Range (-VerticalSpread, VerticalSpread);
		float dAlphaY = Random.Range (-VerticalSpread, VerticalSpread);
		float alpha = FindAngle () + dAlphaX;
		ShootPoint.transform.eulerAngles = new Vector3 (-alpha, transform.eulerAngles.y + dAlphaY, transform.eulerAngles.z);
		//Debug.Log (ShootPoint.transform.eulerAngles);
		GameObject ArrowObject = Instantiate (ArrowPrefab, ShootPoint.transform.position, ShootPoint.transform.rotation) as GameObject;
		Arrow arrow = ArrowObject.GetComponent<Arrow> ();
		arrow.Damage = Damage;
		arrow.Force = ShootForce;

		// y = Ax - Bx2
		float A = Mathf.Tan (alpha * Mathf.Deg2Rad);
		// B = g / ( 2 v0sq cos^2 alpha)
		float B = g / (2 * v0sq * Mathf.Pow(Mathf.Cos(alpha * Mathf.Deg2Rad), 2.0f));
	}

	void ShootStraight () {	

		Invoke ("ReleaseArrow", 0.1f);
	}

	void ReleaseArrow () {
		GameObject ArrowObject = Instantiate (ArrowPrefab, ShootPoint.transform.position, ShootPoint.transform.rotation) as GameObject;
		Arrow arrow = ArrowObject.GetComponent<Arrow> ();
		arrow.Damage = Damage;
		arrow.Force = ShootForce;
	}

	Vector3 firstPoint;
	Vector3 secondPoint;

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit)) {
				firstPoint = hit.point;
				// Vector3 coords = hit.point;
				// ShootStraight (coords.x, coords.y, coords.z);
				// Shoot (coords.x, coords.y, coords.z);
			}
		}
		/*if (Input.GetMouseButtonUp(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit)) {
				secondPoint = hit.point;
				Vector3 direction = firstPoint - secondPoint;
				direction.y = 0.0f;
				//GameObject marker = Instantiate (MarkerPrefab, firstPoint, transform.rotation);
				//GameObject marker2 = Instantiate (MarkerPrefab, secondPoint, transform.rotation);

				Quaternion newRotation = Quaternion.LookRotation (direction);

				// Set the player's rotation to this new rotation.
				myRigidbody.MoveRotation (newRotation);

				//Vector3 clickedPoint = Vector3.Lerp (secondPoint, firstPoint, 1.5f);
				//ShootStraight (clickedPoint);
			}
		}*/

		if (Input.GetMouseButton(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit)) {
				secondPoint = hit.point;
				Vector3 direction = firstPoint - secondPoint;
				direction.y = 0.0f;

				if (direction.magnitude < 1.0f) {
					return;
				}

				Quaternion newRotation = Quaternion.LookRotation (direction);
				// newRotation.eulerAngles = new Vector3 (newRotation.eulerAngles.x, newRotation.eulerAngles.y - 45.0f, newRotation.eulerAngles.z);
				// Set the player's rotation to this new rotation.
				myRigidbody.MoveRotation (newRotation);

				//Vector3 clickedPoint = Vector3.Lerp (secondPoint, firstPoint, 1.5f);
				//ShootStraight (clickedPoint);
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			ShootStraight ();
		}
	}

	float FindAngle() {
		float a = (g * Mathf.Pow (l, 2)) / (2 * v0sq);
		float b = -l;
		float c = a - h;
		Debug.Log ("a, b, c: " + a + ":" + b + ":" + c);
		float alpha = SquareSolve (a, b, c);

		Debug.Log ("alpha: " + alpha.ToString ());
		return alpha;
	}

	float SquareSolve(float a, float b, float c) {
		float D = Mathf.Pow (b, 2.0f) - 4 * a * c;
		float x1 = 0;
		float x2 = 0;
		if (D >= 0) {
			x1 = (-b + Mathf.Sqrt (Mathf.Pow (b, 2) - 4 * a * c)) / (2 * a);
			x2 = (-b - Mathf.Sqrt (Mathf.Pow (b, 2) - 4 * a * c)) / (2 * a);
		}
		Debug.Log ("x: " + x1 + ":" + x2);
		float alpha1 = Mathf.Atan (x1);
		float alpha2 = Mathf.Atan (x2);
		Debug.Log ("alphas: " + alpha1 * Mathf.Rad2Deg  + ":" + alpha2 * Mathf.Rad2Deg);
		alpha1 = alpha1 * Mathf.Rad2Deg;
		alpha2 = alpha2 * Mathf.Rad2Deg;
		// Debug.Log ("alpha1: " + alpha1 + ", alpha2: " + alpha2);
		return alpha2;
		/*if (Mathf.Min(alpha1, alpha2) > 0) {
			return Mathf.Min (alpha1, alpha2);
		} else {
			return Mathf.Max (alpha1, alpha2);
		}*/

	}
}
