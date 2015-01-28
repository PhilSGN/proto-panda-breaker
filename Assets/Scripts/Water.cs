using UnityEngine;
using System.Collections;

public class Water : MonoBehaviour 
{
    public static float WaterHeight = -6f;
    public static float WaterBase = -9f;

    public PhysicsMaterial2D noFriction;

    //Track node information
	private float[] _xPositions;
	private float[] _yPositions;
	private float[] _velocities;
	private float[] _accelerations;

	//Stores the nodes used for the water
	private LineRenderer _body;

	//Hold the meshes used for the water
	private GameObject[] _meshObjects;
	private Mesh[] _meshes;
	
	//Colliders allow interaction with water
	private GameObject[] _colliders;

	//Constants for water physics
	private const float SPRING_CONSTANT = 0.02f;
	private const float DAMPING = 0.04f;
	private const float SPREAD = 0.05f;
	private const float Z = -1f;

	//Dimensions of the water
	private float _baseHeight;
	private float _left;
	private float _bottom;

	//Adjust water look and feel
	private int _smoothMotion = 5;
	private float _lineWidth = 0.1f;

	//Used for the splash particle effect
	public GameObject Splash;

	//Line renderer material
	public Material Mat;

	//Mesh for the water
	public GameObject WaterMesh;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        //Spawning our water
        SpawnWater(-6,12,WaterHeight,WaterBase);
    }

	/// <summary>
	/// Spawns and manages water
	/// </summary>
	/// <param name="left">Sets left side of water.</param>
	/// <param name="width">Sets width of the water.</param>
	/// <param name="top">How high the water should be.</param>
	/// <param name="bottom">The bottom of the water.</param>
	public void SpawnWater(float left, float width, float top, float bottom)
	{
        int edgeCount = Mathf.RoundToInt (width) * _smoothMotion;
		int nodeCount = edgeCount + 1;

		//Render body of water
		_body = gameObject.AddComponent<LineRenderer> ();
		_body.sharedMaterial = Mat;
		_body.sharedMaterial.renderQueue = 1000;
		_body.SetVertexCount (nodeCount);
		_body.SetWidth (_lineWidth, _lineWidth);

		//Initialize the values based on the water created above
		_xPositions = new float[nodeCount];
		_yPositions = new float[nodeCount];
		_velocities = new float[nodeCount];
		_accelerations = new float[nodeCount];

		_meshObjects = new GameObject[edgeCount];
		_meshes = new Mesh[edgeCount];
		_colliders = new GameObject[edgeCount];

		_baseHeight = top;
		_bottom = bottom;
		_left = left;

		//Set values of the arrays
		for (int i = 0; i < nodeCount; i++) 
		{
			_yPositions[i] = top;
            _xPositions[i] = _left + ((width * i) / edgeCount);
			_accelerations[i] = 0;
			_velocities[i] = 0;
			_body.SetPosition(i, new Vector3(_xPositions[i], _yPositions[i], Z));
		}

		//Magic to create quadralaterals and their meshes?
		for(int i = 0; i < edgeCount; i++)
		{
			_meshes[i] = new Mesh();

			Vector3[] vertices = new Vector3[4];
			vertices[0] = new Vector3(_xPositions[i], _yPositions[i], Z);
			vertices[1] = new Vector3(_xPositions[i + 1], _yPositions[i + 1], Z);
			vertices[2] = new Vector3(_xPositions[i], bottom, Z);
			vertices[3] = new Vector3(_xPositions[i + 1], bottom, Z);

			Vector2[] uvs = new Vector2[4];
			uvs[0] = new Vector2(0, 1);
			uvs[1] = new Vector2(1, 1);
			uvs[2] = new Vector2(0, 0);
			uvs[3] = new Vector2(1, 0);

			int[] tris = new int[6]	{0, 1, 3, 3, 2, 0};

			_meshes[i].vertices = vertices;
			_meshes[i].uv = uvs;
			_meshes[i].triangles = tris;

			//Instantiate the meshes under the water manager gameobject
			_meshObjects[i] = Instantiate(WaterMesh, Vector3.zero, Quaternion.identity) as GameObject;
			_meshObjects[i].GetComponent<MeshFilter>().mesh = _meshes[i];
			_meshObjects[i].transform.parent = transform;

			//Create the colliders
			_colliders[i] = new GameObject();
			_colliders[i].name = "Trigger";
			_colliders[i].AddComponent<BoxCollider2D>();
			_colliders[i].transform.parent = transform;
            _colliders[i].transform.position = new Vector3(_left + width * (i + 0.5f) / edgeCount, top - 0.5f, 0);
			_colliders[i].transform.localScale = new Vector3(width / edgeCount, 1, 1);
			_colliders[i].GetComponent<BoxCollider2D>().isTrigger = true;
            _colliders[i].GetComponent<BoxCollider2D>().sharedMaterial = noFriction;
			_colliders[i].AddComponent<WaterCollision>();

		}
	}

	void FixedUpdate()
	{
        //Physics stuff - Euler method is it?
        for (int i = 0; i < _xPositions.Length; i++)
        {
            float force = SPRING_CONSTANT * (_yPositions[i] - _baseHeight) + _velocities[i] * DAMPING;
            _accelerations[i] = -force; //May need to change this to -force/mass if mass is not equal to 1
            _yPositions[i] += _velocities[i];
            _velocities[i] += _accelerations[i];
            _body.SetPosition(i, new Vector3(_xPositions[i], _yPositions[i], Z));
        }

        float[] leftDeltas = new float[_xPositions.Length];
        float[] rightDeltas = new float[_xPositions.Length];

        //More physics stuff for a spring maybe
        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < _xPositions.Length; i++)
            {
                if(i > 0)
                {
                    leftDeltas[i] = SPREAD * (_yPositions[i] - _yPositions[i-1]);
                    _velocities[i - 1] += leftDeltas[i];
                }

                if(i < _xPositions.Length -1)
                {
                    rightDeltas[i] = SPREAD * (_yPositions[i] - _yPositions[i + 1]);
                    _velocities[i + 1] += rightDeltas[i];
                }
            }

            for (int i = 0; i < _xPositions.Length; i++)
            {
                if(i > 0)
                {
                    _yPositions[i-1] += leftDeltas[i];
                }
                
                if(i < _xPositions.Length -1)
                {
                    _yPositions[i + 1] += rightDeltas[i];
                }
            }
        }



        UpdateMeshes();
	}

    /// <summary>
    /// Makes splashes based on position and velocity
    /// </summary>
    /// <param name="xpos">Xpos.</param>
    /// <param name="velocity">Velocity.</param>
    public void Splashes(float xpos, float velocity)
    {
        if(xpos >= _xPositions[0] && xpos <= _xPositions[_xPositions.Length - 1])
        {
            xpos -= _xPositions[0];

            int index = Mathf.RoundToInt((_xPositions.Length-1) * (xpos / (_xPositions[_xPositions.Length - 1] - _xPositions[0])));

            _velocities[index] = velocity;

            float lifetime = 0.93f + Mathf.Abs(velocity) * 0.07f;
            Splash.GetComponent<ParticleSystem>().startSpeed = 8 + 2 * Mathf.Pow(Mathf.Abs(velocity), 0.5f);
            Splash.GetComponent<ParticleSystem>().startSpeed = 9 + 2 * Mathf.Pow(Mathf.Abs(velocity), 0.5f);
            Splash.GetComponent<ParticleSystem>().startLifetime = lifetime;

            //Changes how the splash lands - could be removed
            Vector3 position = new Vector3(_xPositions[index],_yPositions[index]-0.35f,5);
            Quaternion rotation = Quaternion.LookRotation(new Vector3(_xPositions[Mathf.FloorToInt(_xPositions.Length / 2)], _baseHeight + 8, 5) - position);

            GameObject splish = Instantiate(Splash, position, rotation) as GameObject;
            Destroy(splish, lifetime + 0.3f);
        }
    }

	void UpdateMeshes()
	{

        for (int i = 0; i < _meshes.Length; i++) 
		{
			Vector3[] vertices = new Vector3[4];
			vertices[0] = new Vector3(_xPositions[i], _yPositions[i], Z);
			vertices[1] = new Vector3(_xPositions[i + 1], _yPositions[i + 1], Z);
			vertices[2] = new Vector3(_xPositions[i], _bottom, Z);
			vertices[3] = new Vector3(_xPositions[i + 1], _bottom, Z);

			_meshes[i].vertices = vertices;

            Vector3 temp = _colliders[i].transform.position;

            temp.y = _yPositions[i] - 0.45f;

            _colliders[i].transform.position = temp;
		} 


	}
    
}
