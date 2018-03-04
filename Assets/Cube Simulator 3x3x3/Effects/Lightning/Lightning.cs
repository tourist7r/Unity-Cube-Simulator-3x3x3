using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class Lightning : MonoBehaviour {
	
	//Public inspector properties
	public Transform EndPoint;	    		//Where the lightning effect terminates
	public int numberOfSegments = 12;  		//Number of line segments to use. Higher numbers make a smoother line (but may impact performance)
	public float randomness = 0.01f;		//randomness (in world units) of each segment of the lightning chain
	public float radius = 10f;				//Arc radius.
	public float duration = 1f;				//duration of the effect. Set this to 0 to make it stay permanently
	public float frameRate = 10f;			//Number of frames per second at which it should animate. Set to 0 to not animate at all.
	public bool fadeOut = true;				//Should it fade out over time?
	public Color startColor = Color.white;	//Color of the line at the start
	public Color endColor = Color.white;	//Color of the line at the end

	//private members
	private Vector2 midPoint;
	private float maxZ;
	private float timer = 0f;
	private LineRenderer lineRenderer;
	private GameObject endGameObject;
	private int vertCount = 0;

	void Start () {
		//set up the line renderer
		lineRenderer = GetComponent<LineRenderer> ();
		lineRenderer.useWorldSpace = false;
		lineRenderer.SetColors (startColor, endColor);

		//If there is no EndPoint set, create one at 0,0,0
		if (EndPoint == null) {
			endGameObject = new GameObject ("EndPoint");
			EndPoint = endGameObject.transform;
		}
		transform.LookAt (EndPoint);

		//If we have a frame rate greater than 0, we will animate the line over time.
		// Otherwise, just render it once.
		if (frameRate > 0) {
			StartCoroutine (AnimateLightning ());
		} else {
			RenderLightning ();
		}
	}
	
	public void RenderLightning () {
		//If the number of segments has changed, update the line renderer
		if (vertCount != numberOfSegments) {
			lineRenderer.SetVertexCount (numberOfSegments);
			vertCount = numberOfSegments;
		}

		//grab a random point inside a circle for the midpoint our arc.
		midPoint = Random.insideUnitCircle * radius;

		//get the total length of the line
		maxZ = (EndPoint.position - transform.position).magnitude;

		//interpolate each point of the line over an arc.
		for (int i=1; i < numberOfSegments-1; i++) {
			float z = ((float)i) * (maxZ) / (float)(numberOfSegments - 1);
			
			float x = -midPoint.x * z * z / (2 * maxZ) + z * midPoint.x / 2f;
			
			float y = -midPoint.y * z * z / (2 * maxZ) + z * midPoint.y / 2f;

			//set the position, and add some randomness for a nice jaggy lightning effect.
			lineRenderer.SetPosition (i, new Vector3 (x + Random.Range (randomness, -randomness), y + Random.Range (randomness, -randomness), z));
		}

		//set the first and last position
		lineRenderer.SetPosition (0, Vector3.zero);
		lineRenderer.SetPosition (numberOfSegments - 1, new Vector3 (0, 0, maxZ));
	}

	public IEnumerator AnimateLightning () {
		while (frameRate > 0) {
			RenderLightning ();

			yield return new WaitForSeconds (1f / frameRate);
		}
	}

	void Update () {
		//Keep it pointed at the EndPoint
		transform.LookAt (EndPoint);

		//Update the color in case we are fading (or you change it in the inspector)
		lineRenderer.SetColors (startColor, endColor);

		if (timer < duration) {
			timer += Time.deltaTime;	//keep out timer ticking up

			if (fadeOut) {
				//smoothly lerp the colors alpha value toward 0
				startColor.a = Mathf.Lerp (startColor.a, 0f, timer / duration);
				endColor.a = Mathf.Lerp (endColor.a, 0f, timer / duration);
			}

			//if the duration has run out, kill the effect.
			if (timer >= duration) {
				//if we had spawned our own end point, clean it up here.
				if (endGameObject != null) {
					Destroy (endGameObject);
				}
				Destroy (gameObject);
			}
		}
	}
}