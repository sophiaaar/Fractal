using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour {

	// public Mesh mesh;
	public Material material;
	public int maxDepth;
	private int depth;
	public float childScale;
	public float spawnProbability;
	public float maxRotationSpeed;
	private float rotationSpeed;
	public float maxTwist;

	private static Vector3[] childDirections = {
		Vector3.up,
		Vector3.right,
		Vector3.left,
		Vector3.forward,
		Vector3.back
		};
	private static Quaternion[] childOrientations = {
		Quaternion.identity,
		Quaternion.Euler(0f, 0f, -90f),
		Quaternion.Euler(0f, 0f, 90f),
		Quaternion.Euler(90f, 0f, 0f),
		Quaternion.Euler(-90f, 0f, 0f)
		};
	private Material[,] materials;
	public Mesh[] meshes;

	// Use this for initialization
	void Start () {
		rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
		transform.Rotate(Random.Range(-maxTwist, maxTwist), 0f, 0f);
		if (materials == null)
		{
			InitializeMaterials();
		}
		gameObject.AddComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length)];
		gameObject.AddComponent<MeshRenderer>().material = materials[depth, Random.Range(0, 2)];
		//Debug.Log(materials);
		// the following line causes dynamic batching to be less effective by half
		//GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.white, Color.yellow, (float)depth/maxDepth);
		//Debug.Log(depth);
		if (depth < maxDepth)
		{
			StartCoroutine(CreateChildren());
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
		
	}

	

	private void Initialize(Fractal parent, int childIndex)
	{
		meshes = parent.meshes;
		materials = parent.materials;
		maxDepth = parent.maxDepth;
		depth = parent.depth +1;
		childScale = parent.childScale;
		transform.parent = parent.transform;
		transform.localScale = Vector3.one * childScale;
		transform.localPosition = childDirections[childIndex] * (0.5f + 0.5f * childScale);
		transform.localRotation = childOrientations[childIndex];
		spawnProbability = parent.spawnProbability;
		maxRotationSpeed = parent.maxRotationSpeed;
		maxTwist = parent.maxTwist;
	}

	private void InitializeMaterials()
	{
		materials = new Material[maxDepth +1, 2];
		for (int i = 0; i <= maxDepth; i++)
		{
			float t = i/(maxDepth - 1f);
			t *= t;
			materials[i, 0] = new Material(material);
			//materials[i, 0].color = Color.Lerp(Color.white, Color.yellow, t);
			materials[i, 0].shader = Shader.Find("graphs/Erosion");
			// grab property name from shader code
			materials[i, 0].SetColor("Color_A208B217", Color.Lerp(Color.white, Color.yellow, t));
			materials[i, 1] = new Material(material);
			//materials[i, 1].color = Color.Lerp(Color.white, Color.cyan, t);
			materials[i, 1].shader = Shader.Find("graphs/Erosion");
			materials[i, 1].SetColor("Color_A208B217", Color.Lerp(Color.white, Color.cyan, t));
		}
		//materials[maxDepth, 0].color = Color.magenta;
		materials[maxDepth, 0].shader = Shader.Find("graphs/Erosion");
		materials[maxDepth, 0].SetColor("Color_A208B217", Color.magenta);
		//materials[maxDepth, 1].color = Color.grey;
		materials[maxDepth, 1].shader = Shader.Find("graphs/Erosion");
		materials[maxDepth, 1].SetColor("Color_A208B217", Color.grey);
	}

	private IEnumerator CreateChildren()
	{
		for (int i = 0; i<childDirections.Length; i++)
		{
			if (Random.value < spawnProbability)
			{
				yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
				new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, i);
			}
		}
	}
}
