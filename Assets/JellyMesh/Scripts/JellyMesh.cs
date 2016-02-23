using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif 

using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Jelly Mesh/Jelly Mesh")]
[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
public class JellyMesh : MonoBehaviour 
{
#region PUBLIC_VARIABLES
	// Arrangement of the physics bodies
	public enum PhysicsStyle
	{
		Circle,
		Sphere,
		Rectangle,
		Cube,
		Triangle,
		Pyramid,
		Free,
	}

	public PhysicsStyle m_Style = PhysicsStyle.Cube;

	/// <summary>
	/// Controls body positions in free mode
	/// </summary>
	public List<Vector3> m_FreeModeBodyPositions;
	public List<float> m_FreeModeBodyRadii;
	public List<bool> m_FreeModeBodyKinematic;

	// Whether the chosen mass value is applied per rigid body or
	// to the soft body as a whole
	public enum MassStyle
	{
		Global,
		PerBody
	}

	public MassStyle m_MassStyle = MassStyle.PerBody;
	public Mesh m_SourceMesh;

	// Physics materials for 2D/3D modes
	public PhysicMaterial m_PhysicsMaterial;
	public PhysicsMaterial2D m_PhysicsMaterial2D;
	
	// How many vertices make up the rendered physics mesh
	public int m_VertexDensity = 10;

	// Radius of the rigid body colliders (given as a % of mesh size)
	public float m_SphereRadius = 0.25f;

	// How strongly we map reference points to mesh vertices. Higher values will make the mesh vertex distortion
	// more accurately correspond to the rigid body movement, but may cause visual artefacts, especially on
	// soft bodies made up of only a few rigid bodies.
	public float m_DistanceExponent = 2.0f;

	// How stiff the soft body physics springs are
	public float m_Stiffness = 2.5f;

	// The mass of the entire mesh (each of the n reference point is 1/n times this value)
	public float m_Mass = 1.0f;

	// Whether or not to lock rotation
	public bool m_LockRotationX = true;
	public bool m_LockRotationY = true;
	public bool m_LockRotationZ = true;

	// Gravity scale (in 2D mode)
	public float m_GravityScale = 1.0f;

	// Whether child bodies should collider with one another
	public bool m_CollideConnected = false;

	// Whether to make the central body kinematic (ie. not move)
	public bool m_CentralBodyKinematic = false;

	// Drag (in 3D mode)
	public float m_Drag = 0.0f;
	public float m_AngularDrag = 0.05f;

	// Circle-configuration only - how many rigid bodies are placed around the circle radius
	public int m_RadiusPoints = 4;
	
	// The amount by which the spring force is reduced in proportion to the movement speed. The spring will oscillate
	// with a certain frequency as it attempts to reestablish the desired distance between the objects. The higher
	// the damping ratio, the quicker the oscillation will die down to zero.
	public float m_DampingRatio = 0.0f;

	/// <summary>
	/// Used to scale the collider up/down without adjusting the actual mesh size
	/// </summary>
	public Vector3 m_SoftBodyScale = Vector3.one;

	/// <summary>
	/// Used to adjust the position of the soft body relative to the mesh
	/// </summary>
	public Vector3 m_SoftBodyOffset = Vector3.zero;

	/// <summary>
	/// Used to adjust the position of the soft body relative to the mesh
	/// </summary>
	public Vector3 m_SoftBodyPivotOffset = Vector3.zero;

	/// <summary>
	/// Used to adjust the rotation of the soft body relative to the mesh
	/// </summary>
	public Vector3 m_SoftBodyRotation = Vector3.zero;

	/// <summary>
	/// Used to scale the mesh size
	/// </summary>
	public Vector3 m_MeshScale = Vector3.one;

	// Whether to use 2D or 3D rigid bodies/colliders
	public bool m_2DMode = false;

	// Whether or not to recalculate the normals each frame
	public bool m_RecalculateNormals = true;

	// Controls whether bodies are attached to their neighboring bodies as well as to
	// the central point
	public bool m_AttachNeighbors = false;
	
	// Array of attach points - used to attach child objects to this jelly mesh
	public int m_NumAttachPoints = 0;
	public Transform[] m_AttachPoints = new Transform[0];

	public List<ReferencePoint> ReferencePoints { get { return m_ReferencePoints; } }
	public ReferencePoint CentralPoint { get { return m_CentralPoint; } }
#endregion

#region PRIVATE_VARIABLES
	// Internal rendering data
	Vector3[] 	m_Vertices;
	Vector3[] 	m_InitialVertexPositions;
	Vector3[] 	m_Normals;
	Color[] 	m_Colors;
	Vector2[] 	m_TexCoords;
	Vector2[] 	m_UV1;
	Vector2[] 	m_UV2;
	int[] 		m_Triangles;
	Mesh 		m_Mesh;

	// Physics reference points
	public List<ReferencePoint> m_ReferencePoints;

	// Reference point->vertex weighting values
	float[,] m_ReferencePointWeightings;

	// Reference point->attach point weighting valuse
	float[,] m_AttachPointWeightings;

	// Initial attach point positions
	Vector3[] m_InitialAttachPointPositions = new Vector3[0];

	// Saves us checking components every frame to see if an
	// attached object is actually another Jelly Mesh
	bool[] m_IsAttachPointJellyMesh = new bool[0];

	// Parent object for rigidbodies
	GameObject m_ReferencePointParent;

	// Central body point
	ReferencePoint m_CentralPoint;

	// List of reference point offset
	Vector3[] m_ReferencePointOffsets;

	// Mesh renderer
	MeshRenderer m_MeshRenderer;

	// If this is the first update
	bool m_FirstUpdate;
#endregion

#region PUBLIC_CLASSES
	/// <summary>
	/// The ReferencePoint class encapsulates a rigid body (2D or 3D) and information about
	/// the bodies initial position. From there, we can work out how much the body has moved 
	/// from its initial position and then map the movement to the visible mesh.
	/// </summary>
	public class ReferencePoint
	{
		public Vector3 InitialOffset { get { return m_InitialOffset; } set { m_InitialOffset = value; } }
		public Rigidbody2D Body2D { get { return m_RigidBody2D; } }
		public Rigidbody Body3D { get { return m_RigidBody3D; } }
		public bool IsDummy { get { return m_RigidBody2D == null && m_RigidBody3D == null; } }

		Vector3 m_InitialOffset;
		Rigidbody2D m_RigidBody2D;
		Rigidbody m_RigidBody3D;

		/// <summary>
		/// ReferencePoint 2D Constructor
		/// </summary>
		public ReferencePoint(Rigidbody2D body)
		{
			m_RigidBody2D = body;
		}

		/// <summary>
		/// ReferencePoint 3D Constructor
		/// </summary>
		public ReferencePoint(Rigidbody body)
		{
			m_RigidBody3D = body;
		}

		/// <summary>
		/// Get the radius of the rigid body
		/// </summary>
		public float Radius 
		{ 
			get 
			{ 
				if(m_RigidBody2D != null)
				{
					if(m_RigidBody2D.GetComponent<Collider2D>())
					{
						CircleCollider2D circleCollider = m_RigidBody2D.GetComponent<Collider2D>() as CircleCollider2D;
						return circleCollider.radius;
					}
				}
				else if(m_RigidBody3D != null)
				{
					if(m_RigidBody3D.GetComponent<Collider>())
					{
						SphereCollider circleCollider = m_RigidBody3D.GetComponent<Collider>() as SphereCollider;
						return circleCollider.radius;
					}
				}
				
				return 0.0f;
			} 
		}

		/// <summary>
		/// Gets the game object.
		/// </summary>
		public GameObject GameObject 
		{ 
			get 
			{
				if(m_RigidBody2D != null)
				{
					return m_RigidBody2D.gameObject;
				}
				else if(m_RigidBody3D != null)
				{
					return m_RigidBody3D.gameObject;
				}
				
				return null;
			}
		}

		/// <summary>
		/// Set the kinematic flag on this object
		/// </summary>
		public void SetKinematic(bool kinematic)
		{
			if(m_RigidBody2D != null)
			{
				m_RigidBody2D.isKinematic = kinematic;
			}
			else if(m_RigidBody3D != null)
			{
				m_RigidBody3D.isKinematic = kinematic;
			}
		}
	}

	/// <summary>
	/// Helper class for passing information about collisions
	/// </summary>
	public class JellyCollision
	{
		public Collision Collision { get; set; }
		public JellyMeshReferencePoint ReferencePoint { get; set; }
	}

	/// <summary>
	/// Helper class for passing information about 2D collisions
	/// </summary>
	public class JellyCollision2D
	{
		public Collision2D Collision2D { get; set; }
		public JellyMeshReferencePoint ReferencePoint { get; set; }
	}

	/// <summary>
	/// Helper class for passing information about triggers
	/// </summary>
	public class JellyCollider
	{
		public Collider Collider { get; set; }
		public JellyMeshReferencePoint ReferencePoint { get; set; }
	}
	
	/// <summary>
	/// Helper class for passing information about 2D collisions
	/// </summary>
	public class JellyCollider2D
	{
		public Collider2D Collider2D { get; set; }
		public JellyMeshReferencePoint ReferencePoint { get; set; }
	}
#endregion

	/// <summary>
	/// Raises the validate event.
	/// </summary>
	void OnValidate()
	{
		if(IsMeshValid() && m_Mesh == null)
		{
			RefreshMesh();
		}
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Awake()
	{
		if(IsMeshValid())
		{
			if(m_FreeModeBodyPositions == null)
			{
				m_FreeModeBodyPositions = new List<Vector3>();
				m_FreeModeBodyPositions.Add(Vector3.zero);

				m_FreeModeBodyRadii = new List<float>();
				m_FreeModeBodyRadii.Add(1.0f);

				m_FreeModeBodyKinematic = new List<bool>();
				m_FreeModeBodyKinematic.Add(false);
			}

			// Maintaining support for users upgrading from 1.07 to 1.08
			if(m_FreeModeBodyKinematic.Count != m_FreeModeBodyPositions.Count)
			{
				m_FreeModeBodyKinematic = new List<bool>();

				for(int loop = 0; loop < m_FreeModeBodyPositions.Count; loop++)
				{
					m_FreeModeBodyKinematic.Add(false);
				}
			}

			Bounds meshBounds = m_SourceMesh.bounds;
			InitVertices();
			InitMesh();

			m_InitialAttachPointPositions = new Vector3[m_AttachPoints.Length];
			m_IsAttachPointJellyMesh = new bool[m_AttachPoints.Length];

			if(Application.isPlaying)
			{
				Vector3 meshAngle = this.transform.eulerAngles;
				this.transform.eulerAngles = Vector3.zero;

				if(m_2DMode && !Physics2D.GetIgnoreLayerCollision(this.gameObject.layer, this.gameObject.layer))
				{
					Debug.LogError("Layer '" + LayerMask.LayerToName(this.gameObject.layer) + "' is set to collide with itself - soft body physics will not work as intended. Please disable collisions between this layer and itself (Edit->Project Settings->Physics 2D)");
					return;
				}

				m_ReferencePointParent = new GameObject();
				m_ReferencePointParent.name = this.name + " Reference Points";

				m_ReferencePoints = new List<ReferencePoint>();

				switch(m_Style)
				{
				case PhysicsStyle.Circle:
					CreateRigidBodiesCircle(meshBounds);
					break;
				case PhysicsStyle.Sphere:
					CreateRigidBodiesSphere(meshBounds);
					break;
				case PhysicsStyle.Triangle:
					CreateRigidBodiesTriangle(meshBounds);
					break;
				case PhysicsStyle.Pyramid:
					CreateRigidBodiesPyramid(meshBounds);
					break;
				case PhysicsStyle.Rectangle:
					CreateRigidBodiesRectangle(meshBounds);
					break;
				case PhysicsStyle.Cube:
					CreateRigidBodiesCube(meshBounds);
					break;				
				case PhysicsStyle.Free:
					CreateRigidBodiesFree(meshBounds);
					break;
				}

				if(m_CentralPoint != null)
				{
					m_CentralPoint.GameObject.name = this.name + " Central Ref Point";
				}

				if(m_Style != PhysicsStyle.Free)
				{
					UpdateRotationLock();
				}

				CalculateInitialOffsets();
				InitMass();
				CalculateWeightingValues();
				SetupCollisions();

				m_ReferencePointOffsets = new Vector3[m_ReferencePoints.Count];

				foreach(ReferencePoint referencePoint in m_ReferencePoints)
				{
					if(!referencePoint.IsDummy)
					{
						Vector3 referencePointPosition = referencePoint.GameObject.transform.position;
						Vector3 centralPointPosition = this.transform.position;
						referencePoint.GameObject.transform.position = centralPointPosition + (Quaternion.Euler(meshAngle) * (referencePointPosition - centralPointPosition));
					}
				}

				m_CentralPoint.GameObject.transform.eulerAngles = meshAngle;
				UpdateRotationLock();
				m_FirstUpdate = true;
			}
		}
		else if(Application.isPlaying)
		{
			Debug.LogWarning("JellyMesh: " + this.name + " does not have a valid mesh");
		}
	}

	/// <summary>
	/// Calculates the initial offsets of each reference point
	/// </summary>
	void CalculateInitialOffsets()
	{
		foreach(ReferencePoint referencePoint in m_ReferencePoints)
		{
			if(referencePoint.GameObject && referencePoint != m_CentralPoint)
			{
				referencePoint.InitialOffset = m_CentralPoint.GameObject.transform.InverseTransformPoint(referencePoint.GameObject.transform.position);
			}
		}
	
		int index = 0;

		foreach(Transform attachPointTransform in m_AttachPoints)
		{
			JellyMesh attachedJellyMesh = attachPointTransform.GetComponent<JellyMesh>();
			
			if(attachedJellyMesh)
			{
				m_IsAttachPointJellyMesh[index] = true;
				m_InitialAttachPointPositions[index++] = m_CentralPoint.GameObject.transform.InverseTransformPoint(attachPointTransform.position);
			}
			else
			{
				m_IsAttachPointJellyMesh[index] = false;
				m_InitialAttachPointPositions[index++] = m_CentralPoint.GameObject.transform.InverseTransformPoint(attachPointTransform.position);
				attachPointTransform.parent = this.transform;
			}
		}

		for(int loop = 0; loop < m_Vertices.Length; loop++)
		{
			m_InitialVertexPositions[loop] -= this.transform.InverseTransformPoint(m_CentralPoint.GameObject.transform.position);
			m_Vertices[loop] -= this.transform.InverseTransformPoint(m_CentralPoint.GameObject.transform.position);
		}
	}

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	void OnEnable()
	{
		// Collisions need to be set up again each time the object is activated
		SetupCollisions();
	}

	/// <summary>
	/// Check if the mesh is valid
	/// </summary>
	protected bool IsMeshValid()
	{
		return m_SourceMesh != null;
	}
	
	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	void OnDestroy()
	{
		if(m_ReferencePointParent != null)
		{
			Destroy(m_ReferencePointParent);
		}
	}

	/// <summary>
	/// Create reference points in a circular formation around the central body. Each point is linked to
	/// its neighbors and to the center
	/// </summary>
	void CreateRigidBodiesCircle(Bounds meshBounds)
	{
		int numPoints = m_RadiusPoints;
		float width = meshBounds.size.x * m_MeshScale.x;
		float radius = width * 0.5f;

		m_CentralPoint = AddReferencePoint(m_SoftBodyPivotOffset, width * m_SphereRadius, false);

		// Add nodes in a circle around the centre
		for(int loop = 0; loop < numPoints; loop++)
		{
			// Work out the correct offset to place the node
			float angle = ((Mathf.PI * 2)/numPoints) * loop;
			Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0.0f);
			offset *= radius;
			offset.x *= m_SoftBodyScale.x;
			offset.y *= m_SoftBodyScale.y;
			ReferencePoint referencePoint = AddReferencePoint(offset * (1.0f - (2 * m_SphereRadius)) + m_SoftBodyOffset, width * m_SphereRadius, true);
			AttachPoint(referencePoint, m_CentralPoint);
		}

		if(m_AttachNeighbors)
		{
			for(int loop = 2; loop < m_ReferencePoints.Count; loop++)
			{
				AttachPoint(m_ReferencePoints[loop], m_ReferencePoints[loop - 1]);
			}

			AttachPoint(m_ReferencePoints[m_ReferencePoints.Count - 1], m_ReferencePoints[1]);
		}
	}

	/// <summary>
	/// Create reference points in a sphere formation. Each point is connected to the central point
	/// and to its neighbors
	/// </summary>
	void CreateRigidBodiesSphere(Bounds meshBounds)
	{
		float width = meshBounds.size.x * m_MeshScale.x;
		float radius = width * 0.5f;		
		m_CentralPoint = AddReferencePoint(m_SoftBodyPivotOffset, width * m_SphereRadius, false);

		int latitudes = m_RadiusPoints;
		int longitudes = m_RadiusPoints;

		float latitudeIncrement = 360.0f / latitudes;
		float longitudeIncrement = 180.0f / longitudes;
		int lattitudeIndex = 0;
		int maxLongitudes = 0;

		for (float u = 0; u <= 360.0f; u += latitudeIncrement) 
		{
			ReferencePoint previousPoint = null;
			int longitudeIndex = 0;

			for (float t = 0; t <= 180.0f; t += longitudeIncrement) 
			{			
				float rad = radius;				
				float x = (float) (rad * Mathf.Sin(Mathf.Deg2Rad * t) * Mathf.Sin(Mathf.Deg2Rad * u)) * m_SoftBodyScale.x;
				float y = (float) (rad * Mathf.Cos(Mathf.Deg2Rad * t)) * m_SoftBodyScale.y;
				float z = (float) (rad * Mathf.Sin(Mathf.Deg2Rad * t) * Mathf.Cos(Mathf.Deg2Rad * u)) * m_SoftBodyScale.z;

				Vector3 bodyPosition = new Vector3(x, y, z) * (1.0f - (2 * m_SphereRadius));
				bodyPosition += m_SoftBodyOffset;

				ReferencePoint referencePoint = AddReferencePoint(bodyPosition, width * m_SphereRadius, true);
				AttachPoint(referencePoint, m_CentralPoint);

				if(m_AttachNeighbors)
				{
					// Attach points horizontally
					if(previousPoint != null)
					{
						AttachPoint(previousPoint, referencePoint);
					}

					if(lattitudeIndex > 0)
					{
						ReferencePoint prevVerticalPoint = m_ReferencePoints[m_ReferencePoints.Count - maxLongitudes - 1];
						AttachPoint(prevVerticalPoint, referencePoint);
					}
				}

				previousPoint = referencePoint;
              	longitudeIndex++;
				maxLongitudes = Mathf.Max(longitudeIndex, maxLongitudes);
			}

			lattitudeIndex++;
		}
	}

	/// <summary>
	/// Create reference points in a triangle formation. Each point is connected to the central point
	/// and to its neighbors
	/// </summary>
	void CreateRigidBodiesTriangle(Bounds meshBounds)
	{
		float width = meshBounds.size.x * m_SoftBodyScale.x * m_MeshScale.x;
		float height = meshBounds.size.y * m_SoftBodyScale.y * m_MeshScale.y;
		float radius = meshBounds.size.y * m_SphereRadius * m_MeshScale.y;
		float offsetFactor = 0.5f - m_SphereRadius;

		m_CentralPoint = AddReferencePoint(m_SoftBodyPivotOffset, radius, false);

		ReferencePoint bottomLeftPoint = AddReferencePoint(new Vector3(-width * offsetFactor, -height * offsetFactor) + m_SoftBodyOffset, radius, true);
		AttachPoint(bottomLeftPoint, m_CentralPoint);

		ReferencePoint bottomRightPoint = AddReferencePoint(new Vector3(width * offsetFactor, -height * offsetFactor) + m_SoftBodyOffset, radius, true);
		AttachPoint(bottomRightPoint, m_CentralPoint);

		ReferencePoint topCentrePoint = AddReferencePoint(new Vector3(0.0f, height * offsetFactor) + m_SoftBodyOffset, radius, true);
		AttachPoint(topCentrePoint, m_CentralPoint);

		if(m_AttachNeighbors)
		{
			AttachPoint(m_ReferencePoints[1], m_ReferencePoints[2]);
			AttachPoint(m_ReferencePoints[2], m_ReferencePoints[3]);
			AttachPoint(m_ReferencePoints[3], m_ReferencePoints[1]);
		}
	}

	void CreateRigidBodiesPyramid(Bounds meshBounds)
	{
		float width = meshBounds.size.x * m_SoftBodyScale.x * m_MeshScale.x;
		float height = meshBounds.size.y * m_SoftBodyScale.y * m_MeshScale.y;
		float depth = meshBounds.size.z * m_SoftBodyScale.z * m_MeshScale.z;
		float radius = meshBounds.size.y * m_SphereRadius * m_MeshScale.y;
		float offsetFactor = 0.5f - m_SphereRadius;
		
		m_CentralPoint = AddReferencePoint(m_SoftBodyPivotOffset, radius, false);
		
		ReferencePoint bottomLeftFarPoint = AddReferencePoint(new Vector3(-width * offsetFactor, -height * offsetFactor, -depth * offsetFactor) + m_SoftBodyOffset, radius, true);
		AttachPoint(bottomLeftFarPoint, m_CentralPoint);
		
		ReferencePoint bottomRightFarPoint = AddReferencePoint(new Vector3(width * offsetFactor, -height * offsetFactor, -depth * offsetFactor) + m_SoftBodyOffset, radius, true);
		AttachPoint(bottomRightFarPoint, m_CentralPoint);

		ReferencePoint bottomRightNearPoint = AddReferencePoint(new Vector3(width * offsetFactor, -height * offsetFactor, depth * offsetFactor) + m_SoftBodyOffset, radius, true);
		AttachPoint(bottomRightNearPoint, m_CentralPoint);

		ReferencePoint bottomLeftNearPoint = AddReferencePoint(new Vector3(-width * offsetFactor, -height * offsetFactor, depth * offsetFactor) + m_SoftBodyOffset, radius, true);
		AttachPoint(bottomLeftNearPoint, m_CentralPoint);
		
		ReferencePoint topCentrePoint = AddReferencePoint(new Vector2(0.0f, height * offsetFactor), radius, true);
		AttachPoint(topCentrePoint, m_CentralPoint);
		
		if(m_AttachNeighbors)
		{
			AttachPoint(m_ReferencePoints[1], m_ReferencePoints[2]);
			AttachPoint(m_ReferencePoints[2], m_ReferencePoints[3]);
			AttachPoint(m_ReferencePoints[3], m_ReferencePoints[4]);
			AttachPoint(m_ReferencePoints[4], m_ReferencePoints[1]);
		}
	}

	/// <summary>
	/// Create reference points in a rectangular formation with each point connected to the central
	/// point and to its neighbors
	/// </summary>
	void CreateRigidBodiesRectangle(Bounds meshBounds)
	{
		float width = meshBounds.size.x * m_SoftBodyScale.x * m_MeshScale.x;
		float height = meshBounds.size.y * m_SoftBodyScale.y * m_MeshScale.y;
		float radius = meshBounds.size.y * m_SphereRadius * m_MeshScale.y;
		float offsetFactor = 0.5f - m_SphereRadius;

		m_CentralPoint = AddReferencePoint(m_SoftBodyPivotOffset, radius, false);

		ReferencePoint bottomLeftPoint = AddReferencePoint(new Vector3(-width * offsetFactor, -height * offsetFactor) + m_SoftBodyOffset, radius, true);
		AttachPoint(bottomLeftPoint, m_CentralPoint);
		
		ReferencePoint bottomRightPoint = AddReferencePoint(new Vector3(width * offsetFactor, -height * offsetFactor) + m_SoftBodyOffset, radius, true);
		AttachPoint(bottomRightPoint, m_CentralPoint);
		
		ReferencePoint topRightPoint = AddReferencePoint(new Vector3(width * offsetFactor, height * offsetFactor) + m_SoftBodyOffset, radius, true);
		AttachPoint(topRightPoint, m_CentralPoint);

		ReferencePoint topLeftPoint = AddReferencePoint(new Vector3(-width * offsetFactor, height * offsetFactor) + m_SoftBodyOffset, radius, true);
		AttachPoint(topLeftPoint, m_CentralPoint);

		if(m_AttachNeighbors)
		{
			AttachPoint(m_ReferencePoints[1], m_ReferencePoints[2]);
			AttachPoint(m_ReferencePoints[2], m_ReferencePoints[3]);
			AttachPoint(m_ReferencePoints[3], m_ReferencePoints[4]);
			AttachPoint(m_ReferencePoints[4], m_ReferencePoints[1]);
		}
	}

	/// <summary>
	/// Create reference points in a cube formation with each point connected to the central
	/// point and to its neighbors
	/// </summary>
	void CreateRigidBodiesCube(Bounds meshBounds)
	{
		float width = meshBounds.size.x * m_SoftBodyScale.x * m_MeshScale.x;
		float height = meshBounds.size.y * m_SoftBodyScale.y * m_MeshScale.y;
		float depth = meshBounds.size.z * m_SoftBodyScale.z * m_MeshScale.z;
		float radius = meshBounds.size.y * m_SphereRadius * m_MeshScale.y;
		float offsetFactor = 0.5f - m_SphereRadius;
		
		m_CentralPoint = AddReferencePoint(m_SoftBodyPivotOffset, radius, false);
		
		ReferencePoint bottomLeftFarPoint = AddReferencePoint(new Vector3(-width * offsetFactor, -height * offsetFactor, -depth * offsetFactor) + m_SoftBodyOffset, radius, true);
		AttachPoint(bottomLeftFarPoint, m_CentralPoint);
		
		ReferencePoint bottomRightFarPoint = AddReferencePoint(new Vector3(width * offsetFactor, -height * offsetFactor, -depth * offsetFactor) + m_SoftBodyOffset, radius, true);
		AttachPoint(bottomRightFarPoint, m_CentralPoint);
		
		ReferencePoint topRightFarPoint = AddReferencePoint(new Vector3(width * offsetFactor, height * offsetFactor, -depth * offsetFactor) + m_SoftBodyOffset, radius, true);
		AttachPoint(topRightFarPoint, m_CentralPoint);
		
		ReferencePoint topLeftFarPoint = AddReferencePoint(new Vector3(-width * offsetFactor, height * offsetFactor, -depth * offsetFactor) + m_SoftBodyOffset, radius, true);
		AttachPoint(topLeftFarPoint, m_CentralPoint);

		ReferencePoint bottomLeftNearPoint = AddReferencePoint(new Vector3(-width * offsetFactor, -height * offsetFactor, depth * offsetFactor) + m_SoftBodyOffset, radius, true);
		AttachPoint(bottomLeftNearPoint, m_CentralPoint);
		
		ReferencePoint bottomRightNearPoint = AddReferencePoint(new Vector3(width * offsetFactor, -height * offsetFactor, depth * offsetFactor) + m_SoftBodyOffset, radius, true);
		AttachPoint(bottomRightNearPoint, m_CentralPoint);
		
		ReferencePoint topRightNearPoint = AddReferencePoint(new Vector3(width * offsetFactor, height * offsetFactor, depth * offsetFactor) + m_SoftBodyOffset, radius, true);
		AttachPoint(topRightNearPoint, m_CentralPoint);
		
		ReferencePoint topLeftNearPoint = AddReferencePoint(new Vector3(-width * offsetFactor, height * offsetFactor, depth * offsetFactor) + m_SoftBodyOffset, radius, true);
		AttachPoint(topLeftNearPoint, m_CentralPoint);

		if(m_AttachNeighbors)
		{
			AttachPoint(m_ReferencePoints[1], m_ReferencePoints[2]);
			AttachPoint(m_ReferencePoints[2], m_ReferencePoints[3]);
			AttachPoint(m_ReferencePoints[3], m_ReferencePoints[4]);
			AttachPoint(m_ReferencePoints[4], m_ReferencePoints[1]);

			AttachPoint(m_ReferencePoints[5], m_ReferencePoints[6]);
			AttachPoint(m_ReferencePoints[6], m_ReferencePoints[7]);
			AttachPoint(m_ReferencePoints[7], m_ReferencePoints[8]);
			AttachPoint(m_ReferencePoints[8], m_ReferencePoints[5]);

			AttachPoint(m_ReferencePoints[1], m_ReferencePoints[5]);
			AttachPoint(m_ReferencePoints[2], m_ReferencePoints[6]);
			AttachPoint(m_ReferencePoints[3], m_ReferencePoints[7]);
			AttachPoint(m_ReferencePoints[4], m_ReferencePoints[8]);
		}
	}

	/// <summary>
	/// Creates the rigid bodies in a free configuration based around a central point
	/// </summary>
	/// <param name="">.</param>
	void CreateRigidBodiesFree(Bounds meshBounds)
	{
		m_CentralPoint = AddReferencePoint(m_FreeModeBodyPositions[0], m_FreeModeBodyRadii[0], false);
		m_CentralPoint.SetKinematic(m_FreeModeBodyKinematic[0]);

		for(int loop = 1; loop < m_FreeModeBodyPositions.Count; loop++)
		{
			ReferencePoint referencePoint = AddReferencePoint(m_FreeModeBodyPositions[loop], m_FreeModeBodyRadii[loop], true);
			AttachPoint(referencePoint, m_CentralPoint);
			referencePoint.SetKinematic(m_FreeModeBodyKinematic[loop]);
		}

		if(m_AttachNeighbors)
		{
			for(int loop = 2; loop < m_ReferencePoints.Count; loop++)
			{
				AttachPoint(m_ReferencePoints[loop], m_ReferencePoints[loop - 1]);
			}
			
			AttachPoint(m_ReferencePoints[m_ReferencePoints.Count - 1], m_ReferencePoints[1]);
		}
	}

	/// <summary>
	/// Update the mesh after changing the rotation lock flag
	/// </summary>
	public void UpdateRotationLock()
	{
		if(m_CentralPoint != null)
		{
			if(m_2DMode)
			{
				Rigidbody2D centreRigidBody = m_CentralPoint.GameObject.GetComponent<Rigidbody2D>();
				centreRigidBody.fixedAngle = m_LockRotationZ;
				centreRigidBody.isKinematic = m_CentralBodyKinematic;
			}
			else
			{
				Rigidbody centreRigidBody = m_CentralPoint.GameObject.GetComponent<Rigidbody>();
				
				// Fix the body to the 2D plane
				RigidbodyConstraints constraints = centreRigidBody.constraints;

				if(m_LockRotationX)
				{
					constraints |= RigidbodyConstraints.FreezeRotationX;
				}
				else
				{
					constraints &= ~RigidbodyConstraints.FreezeRotationX;
				}

				if(m_LockRotationY)
				{
					constraints |= RigidbodyConstraints.FreezeRotationY;
				}
				else
				{
					constraints &= ~RigidbodyConstraints.FreezeRotationY;
				}

				if(m_LockRotationZ)
				{
					constraints |= RigidbodyConstraints.FreezeRotationZ;
				}
				else
				{
					constraints &= ~RigidbodyConstraints.FreezeRotationZ;
				}
				
				centreRigidBody.constraints = constraints;
				centreRigidBody.isKinematic = m_CentralBodyKinematic;
			}
		}
	}

	/// <summary>
	/// Add a reference point - essentially just a rigid body + circle collider - at the given
	/// position and with the given properties
	/// </summary>
	ReferencePoint AddReferencePoint(Vector3 position, float radius, bool addCollider)
	{
		position = Quaternion.Euler(m_SoftBodyRotation) * position;
		GameObject referencePointObject = new GameObject();
		referencePointObject.name = this.name + " Ref Point " + m_ReferencePoints.Count.ToString();
		referencePointObject.transform.parent = m_ReferencePointParent.transform;
		referencePointObject.transform.position = this.transform.TransformPoint(position);
		referencePointObject.layer = gameObject.layer;
		referencePointObject.tag = gameObject.tag;

		JellyMeshReferencePoint refPointBehaviour = referencePointObject.AddComponent<JellyMeshReferencePoint>();
		refPointBehaviour.ParentJellyMesh = this.gameObject;
		refPointBehaviour.Index = m_ReferencePoints.Count;

		ReferencePoint referencePoint = null;

		if(m_2DMode)
		{
			if(addCollider)
			{
				CircleCollider2D circleCollider = referencePointObject.AddComponent<CircleCollider2D>();
				circleCollider.radius = radius;
				circleCollider.sharedMaterial = m_PhysicsMaterial2D;
			}

			Rigidbody2D newRigidBody = referencePointObject.AddComponent<Rigidbody2D>();
			newRigidBody.fixedAngle = true;

			referencePoint = new ReferencePoint(newRigidBody);
		}
		else
		{
			if(addCollider)
			{
				SphereCollider circleCollider = referencePointObject.AddComponent<SphereCollider>();
				circleCollider.radius = radius;
				circleCollider.sharedMaterial = m_PhysicsMaterial;
			}
			
			Rigidbody newRigidBody = referencePointObject.AddComponent<Rigidbody>();

			// Fix the body to the 2D plane
			RigidbodyConstraints constraints = newRigidBody.constraints;
			constraints |= RigidbodyConstraints.FreezeRotationX;
			constraints |= RigidbodyConstraints.FreezeRotationY;
			constraints |= RigidbodyConstraints.FreezeRotationZ;

			newRigidBody.constraints = constraints;
			referencePoint = new ReferencePoint(newRigidBody);
		}

		m_ReferencePoints.Add(referencePoint);
		return referencePoint;
	}

	/// <summary>
	/// Attach two reference points together using a spring joint
	/// </summary>
	void AttachPoint(ReferencePoint point1, ReferencePoint point2)
	{
		if(m_2DMode)
		{
			SpringJoint2D joint = point1.Body2D.gameObject.AddComponent<SpringJoint2D>();
			joint.connectedBody = point2.Body2D;
			joint.connectedAnchor = point1.Body2D.transform.localPosition - point2.Body2D.transform.localPosition;
			joint.distance = 0.0f;
			
			joint.enableCollision = m_CollideConnected;
			joint.frequency = m_Stiffness;
			joint.dampingRatio = m_DampingRatio;
		}
		else
		{
			SpringJoint joint = point1.Body3D.gameObject.AddComponent<SpringJoint>();
			joint.connectedBody = point2.Body3D;
			joint.connectedAnchor = point1.Body3D.transform.localPosition - point2.Body3D.transform.localPosition;
			joint.minDistance = 0.0f;
			joint.maxDistance = 0.0f;

			joint.spring = m_Stiffness;
			joint.damper = m_DampingRatio;
		}
	}

	/// <summary>
	/// Each vertex takes its position from the movement of the reference points, with closer reference
	/// points contributing more to the final position than those far away. We can pre-calculate these weighting
	/// values as they remain constant.
	/// </summary>
	public void CalculateWeightingValues()
	{
		if(m_ReferencePoints != null)
		{
			m_ReferencePointWeightings = new float[m_Vertices.Length, m_ReferencePoints.Count];
			
			for(int vertexIndex = 0; vertexIndex < m_Vertices.Length; vertexIndex++)
			{
				float distanceSum = 0.0f;
				
				for(int referencePointIndex = 1; referencePointIndex < m_ReferencePoints.Count; referencePointIndex++)
				{
					if(!m_ReferencePoints[referencePointIndex].IsDummy)
					{
						float distance = Vector3.Distance(m_ReferencePoints[referencePointIndex].InitialOffset, m_Vertices[vertexIndex]);
						distance = Mathf.Pow(distance, m_DistanceExponent);
						float invDistance = float.MaxValue;

						if(distance > 0.0f)
						{
							invDistance = 1.0f/distance;
						}

						distanceSum += invDistance;
					}
				}
				
				for(int referencePointIndex = 1; referencePointIndex < m_ReferencePoints.Count; referencePointIndex++)
				{
					if(!m_ReferencePoints[referencePointIndex].IsDummy)
					{
						float distance = Vector3.Distance(m_ReferencePoints[referencePointIndex].InitialOffset, m_Vertices[vertexIndex]);
						distance = Mathf.Pow(distance, m_DistanceExponent);
						float invDistance = float.MaxValue;

						if(distance > 0.0f)
						{
							invDistance = 1.0f/distance;
						}

						m_ReferencePointWeightings[vertexIndex, referencePointIndex] = invDistance/distanceSum;
					}
				}
			}
		}

		if(m_AttachPoints != null && m_ReferencePoints != null)
		{
			m_AttachPointWeightings = new float[m_AttachPoints.Length, m_ReferencePoints.Count];

			for(int attachPointIndex = 0; attachPointIndex < m_AttachPoints.Length; attachPointIndex++)
			{
				float distanceSum = 0.0f;
				
				for(int referencePointIndex = 1; referencePointIndex < m_ReferencePoints.Count; referencePointIndex++)
				{
					if(!m_ReferencePoints[referencePointIndex].IsDummy)
					{
						float distance = Vector3.Distance(m_ReferencePoints[referencePointIndex].InitialOffset, m_AttachPoints[attachPointIndex].localPosition);
						distance = Mathf.Pow(distance, m_DistanceExponent);
						float invDistance = float.MaxValue;

						if(distance > 0.0f)
						{
							invDistance = 1.0f/distance;
						}

						distanceSum += invDistance;
					}
				}
				
				for(int referencePointIndex = 1; referencePointIndex < m_ReferencePoints.Count; referencePointIndex++)
				{
					if(!m_ReferencePoints[referencePointIndex].IsDummy)
					{
						float distance = Vector3.Distance(m_ReferencePoints[referencePointIndex].InitialOffset, m_AttachPoints[attachPointIndex].localPosition);
						distance = Mathf.Pow(distance, m_DistanceExponent);
						float invDistance = float.MaxValue;

						if(distance > 0.0f)
						{
							invDistance = 1.0f/distance;
						}

						m_AttachPointWeightings[attachPointIndex, referencePointIndex] = invDistance/distanceSum;
					}
				}
			}
		}
	}

	/// <summary>
	/// Disable reference points from colliding with one another
	/// </summary>
	void SetupCollisions()
	{
		if(m_ReferencePoints != null)
		{
			for(int referencePointIndex = 0; referencePointIndex < m_ReferencePoints.Count; referencePointIndex++)
			{
				for(int comparisonPointIndex = 0; comparisonPointIndex < m_ReferencePoints.Count; comparisonPointIndex++)
				{
					if(!m_ReferencePoints[referencePointIndex].IsDummy && !m_ReferencePoints[comparisonPointIndex].IsDummy)
					{
						if(m_2DMode)
						{
							if(referencePointIndex != comparisonPointIndex)
							{
								// TODO - If/when Unity supports disabling 2D collisions on a per object basis, we can do this
								// set this up automatically as with the 3D version. Until then, all we can do is prompt the user 
								// to set up their layers correctly.
								if(!Physics2D.GetIgnoreLayerCollision(this.gameObject.layer, this.gameObject.layer))
								{
									Debug.LogError("Layer '" + LayerMask.LayerToName(this.gameObject.layer) + "' is set to collide with itself - soft body physics will not work as intended. Please disable collisions between this layer and itself (Edit->Project Settings->Physics 2D)");
									return;
								}
							}

						}
						else
						{
							if(referencePointIndex != comparisonPointIndex)
							{
								if(m_ReferencePoints[referencePointIndex].Body3D.GetComponent<Collider>() && m_ReferencePoints[comparisonPointIndex].Body3D.GetComponent<Collider>())
								{
									Physics.IgnoreCollision(m_ReferencePoints[referencePointIndex].Body3D.GetComponent<Collider>(), m_ReferencePoints[comparisonPointIndex].Body3D.GetComponent<Collider>());
								}
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Initialise the grid of vertices that will be used to render this object.
	/// </summary>
	void InitVertices()
	{
		if(m_SourceMesh)
		{
			m_Vertices = new Vector3[m_SourceMesh.vertices.Length];
			m_InitialVertexPositions = new Vector3[m_SourceMesh.vertices.Length];
			m_Colors = new Color[m_SourceMesh.colors.Length];
			m_TexCoords = new Vector2[m_SourceMesh.uv.Length];
			m_Triangles = new int[m_SourceMesh.triangles.Length];
			m_UV1 = new Vector2[m_SourceMesh.uv2.Length];
			m_UV2 = new Vector2[m_SourceMesh.uv2.Length];
			m_Normals = new Vector3[m_SourceMesh.normals.Length];

			m_SourceMesh.vertices.CopyTo(m_Vertices, 0);
			m_SourceMesh.vertices.CopyTo(m_InitialVertexPositions, 0);
			m_SourceMesh.colors.CopyTo(m_Colors, 0);
			m_SourceMesh.uv.CopyTo(m_TexCoords, 0);
			m_SourceMesh.uv2.CopyTo(m_UV1, 0);
			m_SourceMesh.uv2.CopyTo(m_UV2, 0);
			m_SourceMesh.triangles.CopyTo(m_Triangles, 0);
			m_SourceMesh.normals.CopyTo(m_Normals, 0);

			for(int loop = 0; loop < m_Vertices.Length; loop++)
			{
				m_Vertices[loop].Scale(m_MeshScale);
				m_InitialVertexPositions[loop].Scale(m_MeshScale);
			}
		}
	}

	/// <summary>
	/// Sets the position of the Jelly Mesh
	/// </summary>
	public void SetPosition(Vector3 position, bool resetVelocity)		
	{		
		Vector3 offset = position - CentralPoint.GameObject.transform.position;
		
		foreach(JellyMesh.ReferencePoint referencePoint in ReferencePoints)		
		{			
			if(!referencePoint.IsDummy)			
			{				
				referencePoint.GameObject.transform.position = referencePoint.GameObject.transform.position + offset;
				
				if(resetVelocity)					
				{					
					if(referencePoint.Body2D)						
					{						
						referencePoint.Body2D.angularVelocity = 0.0f;					
						referencePoint.Body2D.velocity = Vector2.zero;						
					}					
					else if(referencePoint.Body3D)						
					{						
						referencePoint.Body3D.angularVelocity = Vector3.zero;
						referencePoint.Body3D.velocity = Vector3.zero;
					}
				}
			}
		}
	}

	/// <summary>
	/// Sets whether or not the Jelly Mesh is kinematic
	/// </summary>
	public void SetKinematic(bool isKinematic, bool centralPointOnly)		
	{		
		foreach(JellyMesh.ReferencePoint referencePoint in ReferencePoints)		
		{			
			if(!referencePoint.IsDummy)			
			{				
				if(referencePoint == m_CentralPoint || !centralPointOnly)
				{
					if(referencePoint.Body2D)						
					{						
						referencePoint.Body2D.isKinematic = isKinematic;
					}					
					else if(referencePoint.Body3D)						
					{						
						referencePoint.Body3D.isKinematic = isKinematic;
					}
				}
			}
		}
	}
	
	/// <summary>
	/// Rotate the whole Jelly Mesh by the given angle
	/// </summary>
	public void Rotate(Vector3 eulerAngleChange)
	{
		// Rotate the central body by the required amount	
		CentralPoint.GameObject.transform.localEulerAngles = CentralPoint.GameObject.transform.localEulerAngles + eulerAngleChange;
		
		// Now go through all the reference points and orbit them around the central body by the required amount
		foreach(ReferencePoint referencePoint in ReferencePoints)			
		{			
			if(!referencePoint.IsDummy)			
			{				
				Vector3 referencePointPosition = referencePoint.GameObject.transform.position;				
				Vector3 centralPointPosition = this.transform.position;				
				referencePoint.GameObject.transform.position = centralPointPosition + (Quaternion.Euler(eulerAngleChange) * (referencePointPosition - centralPointPosition));				
			}			
		}
	}

	/// <summary>
	/// Check if the Jelly Mesh is touching the given layer. You can specify how many physics bodies need to be touching for the
	/// whole Jelly Mesh to be classes as grounded
	/// </summary>
	public bool IsGrounded(LayerMask groundLayer, int minGroundedBodies)
	{
		int numGroundedBodies = 0;
		
		foreach(JellyMesh.ReferencePoint referencePoint in ReferencePoints)
		{
			if(!referencePoint.IsDummy)
			{
				if(referencePoint.Body3D)
				{
					if(referencePoint.Body3D.GetComponent<Collider>())
					{
						SphereCollider sphereCollider = referencePoint.Body3D.GetComponent<Collider>() as SphereCollider;
						
						if(Physics.CheckSphere(sphereCollider.bounds.center + new Vector3(0, -sphereCollider.radius * 0.1f, 0), sphereCollider.radius, groundLayer))
						{
							numGroundedBodies++;
							
							if(numGroundedBodies >= minGroundedBodies)
							{
								return true;
							}
						}
					}
				}
				else if(referencePoint.Body2D)
				{		
					if(referencePoint.Body2D.GetComponent<Collider2D>())
					{
						CircleCollider2D circleCollider = referencePoint.Body2D.GetComponent<Collider2D>() as CircleCollider2D;
						Vector2 bodyPosition = referencePoint.GameObject.transform.position;
						
						if(Physics2D.OverlapCircle(bodyPosition + new Vector2(0, -circleCollider.radius * 0.1f), circleCollider.radius, groundLayer))
						{
							numGroundedBodies++;
							
							if(numGroundedBodies >= minGroundedBodies)
							{
								return true;
							}
						}
					}
				}
			}
		}
		
		return false;
	}

	/// <summary>
	/// Resizes the attach point array
	/// </summary>
	public void ResizeAttachPoints()
	{
		Transform[] oldAttachPoints = new Transform[m_AttachPoints.Length];
		bool[] oldIsAttachPointJellyMesh = new bool[m_AttachPoints.Length];
		Vector3[] oldInitialAttachPointPositions = new Vector3[m_AttachPoints.Length];

		m_AttachPoints.CopyTo(oldAttachPoints, 0);
		m_IsAttachPointJellyMesh.CopyTo(oldIsAttachPointJellyMesh, 0);
		m_InitialAttachPointPositions.CopyTo(oldInitialAttachPointPositions, 0);

		m_AttachPoints = new Transform[m_NumAttachPoints];
		m_IsAttachPointJellyMesh = new bool[m_NumAttachPoints];
		m_InitialAttachPointPositions = new Vector3[m_NumAttachPoints];

		for(int loop = 0; loop < m_NumAttachPoints && loop < oldAttachPoints.Length; loop++)
		{
			m_AttachPoints[loop] = oldAttachPoints[loop];
			m_IsAttachPointJellyMesh[loop] = oldIsAttachPointJellyMesh[loop];
			m_InitialAttachPointPositions[loop] = oldInitialAttachPointPositions[loop];
		}

		if(m_AttachPointWeightings != null)
		{
			float[,] oldAttachPointWeightings = new float[m_AttachPointWeightings.GetLength(0),m_AttachPointWeightings.GetLength(1)];

			for(int x = 0; x < m_AttachPointWeightings.GetLength(0); x++)
			{
				for(int y = 0; y < m_AttachPointWeightings.GetLength(1); y++)
				{
					oldAttachPointWeightings[x, y] = m_AttachPointWeightings[x, y];
				}
			}

			m_AttachPointWeightings = new float[m_AttachPoints.Length, m_ReferencePoints.Count];

			for(int x = 0; x < oldAttachPointWeightings.GetLength(0); x++)
			{
				for(int y = 0; y < oldAttachPointWeightings.GetLength(1); y++)
				{
					m_AttachPointWeightings[x, y] = oldAttachPointWeightings[x, y];
				}
			}
		}
	}

	/// <summary>
	/// Called when free mode is selected for the first time - copies all existing points to 
	/// the free mode configuration
	/// </summary>
	public void OnCopyToFreeModeSelected()
	{
		if(IsMeshValid())
		{
			m_FreeModeBodyPositions.Clear();
			m_FreeModeBodyRadii.Clear();
			m_FreeModeBodyKinematic.Clear();

			Bounds meshBounds = m_SourceMesh.bounds;
			float width = meshBounds.size.x * m_SoftBodyScale.x * m_MeshScale.x;
			float height = meshBounds.size.y * m_SoftBodyScale.y * m_MeshScale.y;
			float depth = meshBounds.size.z * m_SoftBodyScale.z * m_MeshScale.z;
							
			switch(m_Style)
			{
				case PhysicsStyle.Circle:
				{
					width = meshBounds.size.x * m_MeshScale.x;
					height = meshBounds.size.y * m_MeshScale.x;
					
					int numPoints = m_RadiusPoints;
					float radius = width * 0.5f;

					AddFreeModeBodyDefinition(m_SoftBodyPivotOffset, width * m_SphereRadius, m_CentralBodyKinematic);

					for(int loop = 0; loop < numPoints; loop++)
					{
						float angle = ((Mathf.PI * 2)/numPoints) * loop;
						Vector3 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
						offset *= radius;
						offset.x *= m_SoftBodyScale.x;
						offset.y *= m_SoftBodyScale.y;
						AddFreeModeBodyDefinition(offset * (1.0f - (2 * m_SphereRadius)) + m_SoftBodyOffset, width * m_SphereRadius, false);
					}
				}
				break;

			case PhysicsStyle.Sphere:
				{
					width = meshBounds.size.x * m_MeshScale.x;
					float radius = width * 0.5f;		
					AddFreeModeBodyDefinition(m_SoftBodyPivotOffset, width * m_SphereRadius, m_CentralBodyKinematic);
					
					int latitudes = m_RadiusPoints;
					int longitudes = m_RadiusPoints;
					
					float latitudeIncrement = 360.0f / latitudes;
					float longitudeIncrement = 180.0f / longitudes;
					
					for (float u = 0; u <= 360.0f; u += latitudeIncrement) 
					{
						for (float t = 0; t <= 180.0f; t += longitudeIncrement) 
						{			
							float rad = radius;				
							float x = (float) (rad * Mathf.Sin(Mathf.Deg2Rad * t) * Mathf.Sin(Mathf.Deg2Rad * u)) * m_SoftBodyScale.x;
							float y = (float) (rad * Mathf.Cos(Mathf.Deg2Rad * t)) * m_SoftBodyScale.y;
							float z = (float) (rad * Mathf.Sin(Mathf.Deg2Rad * t) * Mathf.Cos(Mathf.Deg2Rad * u)) * m_SoftBodyScale.z;
							AddFreeModeBodyDefinition((new Vector3(x, y, z) * (1.0f - (2 * m_SphereRadius))) + m_SoftBodyOffset, width * m_SphereRadius, false);
						}
					}
				}
				break;
					
				case PhysicsStyle.Triangle:
				{
					float radius = meshBounds.size.y * m_SphereRadius * m_MeshScale.y;
					float offsetFactor = 0.5f - m_SphereRadius;
					AddFreeModeBodyDefinition(m_SoftBodyPivotOffset, radius, m_CentralBodyKinematic);
					AddFreeModeBodyDefinition(new Vector3(-width * offsetFactor, -height * offsetFactor) + m_SoftBodyOffset, radius, false);
					AddFreeModeBodyDefinition(new Vector3(width * offsetFactor, -height * offsetFactor) + m_SoftBodyOffset, radius, false);
					AddFreeModeBodyDefinition(new Vector3(0.0f, height * offsetFactor) + m_SoftBodyOffset, radius, false);
				}
				break;

			case PhysicsStyle.Pyramid:
				{
					float radius = meshBounds.size.y * m_SphereRadius * m_MeshScale.y;
					float offsetFactor = 0.5f - m_SphereRadius;
					AddFreeModeBodyDefinition(m_SoftBodyPivotOffset, radius, m_CentralBodyKinematic);
					AddFreeModeBodyDefinition(new Vector3(-width * offsetFactor, -height * offsetFactor, -depth * offsetFactor) + m_SoftBodyOffset, radius, false);
					AddFreeModeBodyDefinition(new Vector3(width * offsetFactor, -height * offsetFactor, -depth * offsetFactor) + m_SoftBodyOffset, radius, false);
					AddFreeModeBodyDefinition(new Vector3(width * offsetFactor, -height * offsetFactor, depth * offsetFactor) + m_SoftBodyOffset, radius, false);
					AddFreeModeBodyDefinition(new Vector3(-width * offsetFactor, -height * offsetFactor, depth * offsetFactor) + m_SoftBodyOffset, radius, false);					
					AddFreeModeBodyDefinition(new Vector3(0.0f, height * offsetFactor), radius, false);
				}
				break;
					
				case PhysicsStyle.Rectangle:
				{
					float radius = meshBounds.size.y * m_SphereRadius * m_MeshScale.y;
					float offsetFactor = 0.5f - m_SphereRadius;
					AddFreeModeBodyDefinition(m_SoftBodyPivotOffset, radius, m_CentralBodyKinematic);
					AddFreeModeBodyDefinition(new Vector3(-width * offsetFactor, -height * offsetFactor) + m_SoftBodyOffset, radius, false);
					AddFreeModeBodyDefinition(new Vector3(width * offsetFactor, -height * offsetFactor) + m_SoftBodyOffset, radius, false);
					AddFreeModeBodyDefinition(new Vector3(-width * offsetFactor, height * offsetFactor) + m_SoftBodyOffset, radius, false);
					AddFreeModeBodyDefinition(new Vector3(width * offsetFactor, height * offsetFactor) + m_SoftBodyOffset, radius, false);
				}
				break;		

				case PhysicsStyle.Cube:
				{
					float radius = meshBounds.size.y * m_SphereRadius * m_MeshScale.y;
					float offsetFactor = 0.5f - m_SphereRadius;
					AddFreeModeBodyDefinition(m_SoftBodyPivotOffset, radius, m_CentralBodyKinematic);

					AddFreeModeBodyDefinition(new Vector3(-width * offsetFactor, -height * offsetFactor, -depth * offsetFactor) + m_SoftBodyOffset, radius, false);
					AddFreeModeBodyDefinition(new Vector3(width * offsetFactor, -height * offsetFactor, -depth * offsetFactor) + m_SoftBodyOffset, radius, false);
					AddFreeModeBodyDefinition(new Vector3(-width * offsetFactor, height * offsetFactor, -depth * offsetFactor) + m_SoftBodyOffset, radius, false);
					AddFreeModeBodyDefinition(new Vector3(width * offsetFactor, height * offsetFactor, -depth * offsetFactor) + m_SoftBodyOffset, radius, false);

					AddFreeModeBodyDefinition(new Vector3(-width * offsetFactor, -height * offsetFactor, depth * offsetFactor) + m_SoftBodyOffset, radius, false);
					AddFreeModeBodyDefinition(new Vector3(width * offsetFactor, -height * offsetFactor, depth * offsetFactor) + m_SoftBodyOffset, radius, false);
					AddFreeModeBodyDefinition(new Vector3(-width * offsetFactor, height * offsetFactor, depth * offsetFactor) + m_SoftBodyOffset, radius, false);
					AddFreeModeBodyDefinition(new Vector3(width * offsetFactor, height * offsetFactor, depth * offsetFactor) + m_SoftBodyOffset, radius, false);
				}
				break;

				case PhysicsStyle.Free:
				break;					
			}

			m_SoftBodyPivotOffset = Vector3.zero;
			m_SoftBodyRotation = Vector3.zero;
			m_SoftBodyRotation = Vector3.zero;
			m_Style = PhysicsStyle.Free;
		}
	}

	/// <summary>
	/// First time setup of the mesh
	/// </summary>
	void InitMesh()
	{
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		m_Mesh = new Mesh();
		m_Mesh.name = this.name + " JellyMesh Mesh";
		m_Mesh.MarkDynamic();
		meshFilter.sharedMesh = m_Mesh;

		m_Mesh.Clear();
		m_Mesh.vertices = m_Vertices;
		m_Mesh.uv = m_TexCoords;
		m_Mesh.uv2 = m_UV1;
		m_Mesh.uv2 = m_UV2;
		m_Mesh.triangles = m_Triangles;
		m_Mesh.colors = m_Colors;
		m_Mesh.normals = m_Normals;
		m_Mesh.RecalculateBounds();
	}

	/// <summary>
	/// Update the vertex positions of the mesh
	/// </summary>
	void UpdateMesh()
	{
		// For each vertex, look at the offset values of each reference point and apply the same offset
		// (scaled by the weighting value) to the vertex's position
		if(Application.isPlaying)
		{
			// Calculate reference point offsets
			bool haveAnyPointsMoved = m_FirstUpdate;

			if(m_ReferencePoints != null)
			{
				for(int referencePointIndex = 0; referencePointIndex < m_ReferencePoints.Count; referencePointIndex++)				
				{				
					if(!m_ReferencePoints[referencePointIndex].IsDummy && m_ReferencePoints[referencePointIndex] != m_CentralPoint)					
					{					
						ReferencePoint referencePoint = m_ReferencePoints[referencePointIndex];					
						Vector3 offset = m_CentralPoint.GameObject.transform.InverseTransformPoint(referencePoint.GameObject.transform.position);
						offset -= referencePoint.InitialOffset;
						
						if(haveAnyPointsMoved || m_ReferencePointOffsets[referencePointIndex] != offset)
						{
							m_ReferencePointOffsets[referencePointIndex] = offset;
							haveAnyPointsMoved = true;
						}
					}
					else
					{					
						m_ReferencePointOffsets[referencePointIndex] = Vector3.zero;					
					}
				}
			}

			if(!haveAnyPointsMoved)
			{
				return;
			}

			for(int vertexIndex = 0; vertexIndex < m_Vertices.Length; vertexIndex++)
			{
				Vector3 totalOffset = Vector3.zero;
				
				for(int referencePointIndex = 0; referencePointIndex < m_ReferencePoints.Count; referencePointIndex++)
				{
					if(!m_ReferencePoints[referencePointIndex].IsDummy && m_ReferencePoints[referencePointIndex] != m_CentralPoint)
					{
						totalOffset += m_ReferencePointOffsets[referencePointIndex] * m_ReferencePointWeightings[vertexIndex, referencePointIndex];
					}
				}
				
				m_Vertices[vertexIndex] = m_InitialVertexPositions[vertexIndex] + totalOffset;
			}
			
			// Update the mesh
			m_Mesh.vertices = m_Vertices;
			m_Mesh.RecalculateBounds();

			if(m_RecalculateNormals)
			{
				m_Mesh.RecalculateNormals();
			}

			UpdateAttachPoints();
		}
	}

	/// <summary>
	/// Update the attach point positions
	/// </summary>
	void UpdateAttachPoints()
	{
		// For each vertex, look at the offset values of each reference point and apply the same offset
		// (scaled by the weighting value) to the vertex's position
		if(Application.isPlaying)
		{
			for(int attachPointIndex = 0; attachPointIndex < m_AttachPoints.Length; attachPointIndex++)
			{
				Vector3 totalOffset = Vector3.zero;
				
				for(int referencePointIndex = 0; referencePointIndex < m_ReferencePoints.Count; referencePointIndex++)
				{
					if(!m_ReferencePoints[referencePointIndex].IsDummy && m_ReferencePoints[referencePointIndex] != m_CentralPoint)
					{
						totalOffset += m_ReferencePointOffsets[referencePointIndex] * m_AttachPointWeightings[attachPointIndex, referencePointIndex];
					}
				}

				JellyMesh attachedJellyMesh = m_AttachPoints[attachPointIndex].GetComponent<JellyMesh>();

				// Attached Jelly Meshs need to behave slightly differently from regular objects - we set the central
				// body to be kinematic and then adjust the position of this, which allows the Jelly Mesh to track the
				// attach point position while still being able to wobble around
				if(m_IsAttachPointJellyMesh[attachPointIndex])
				{
					attachedJellyMesh.CentralPoint.GameObject.transform.parent = this.transform;
					attachedJellyMesh.CentralPoint.SetKinematic(true);
					attachedJellyMesh.CentralPoint.GameObject.transform.localPosition = m_InitialAttachPointPositions[attachPointIndex] + totalOffset;
				}
				else
				{ 
					m_AttachPoints[attachPointIndex].transform.localPosition = m_InitialAttachPointPositions[attachPointIndex] + totalOffset;
				}
			}
		}
	}

	/// <summary>
	/// Add a force to every reference point
	/// </summary>
	public void AddForce(Vector3 force, bool centralPointOnly)
	{
		if(m_ReferencePoints != null)
		{
			foreach(ReferencePoint referencePoint in m_ReferencePoints)
			{
				if(!centralPointOnly || referencePoint == m_CentralPoint)
				{
					if(referencePoint.Body2D)
					{
						referencePoint.Body2D.AddForce(force);
					}
					
					if(referencePoint.Body3D)
					{
						referencePoint.Body3D.AddForce(force);
					}
				}
			}
		}
	}

	/// <summary>
	/// Add a force to every reference point
	/// </summary>
	public void AddTorque(Vector3 torque, bool centreOnly)
	{
		if(m_ReferencePoints != null)
		{
			foreach(ReferencePoint referencePoint in m_ReferencePoints)
			{
				if(!centreOnly || referencePoint == m_CentralPoint)
				{
					if(referencePoint.Body2D)
					{
						referencePoint.Body2D.AddTorque(torque.x);
					}
					
					if(referencePoint.Body3D)
					{
						referencePoint.Body3D.AddTorque(torque);
					}
				}
			}
		}
	}

	/// <summary>
	/// Add a force at a given position to every reference point
	/// </summary>
	public void AddForceAtPosition(Vector2 force, Vector2 position)
	{
		if(m_ReferencePoints != null)
		{
			foreach(ReferencePoint referencePoint in m_ReferencePoints)
			{
				if(referencePoint.Body2D)
				{
					referencePoint.Body2D.AddForceAtPosition(force, position);
				}
				
				if(referencePoint.Body3D)
				{
					referencePoint.Body3D.AddForceAtPosition(force, position);
				}
			}
		}
	}

	/// <summary>
	/// Called when the editor wants to update the visible mesh
	/// </summary>
	public void RefreshMesh()
	{
		if(IsMeshValid())
		{
			InitVertices();
			InitMesh();

			if(m_ReferencePoints != null)
			{
				CalculateInitialOffsets();
				CalculateWeightingValues();
			}

			UpdateMesh();
		}
	}

	/// <summary>
	/// Set up the mass of each rigidbody
	/// </summary>
	public void InitMass()
	{
		if(m_ReferencePoints != null)
		{
			float mass = m_Mass;

			// If the mass is being defined on a global scale, then for n rigid
			// bodies, each one has 1/n of the total mass.
			if(m_MassStyle == MassStyle.Global)
			{
				int numNonDummyReferencePoints = 0;
				
				foreach(ReferencePoint referencePoint in m_ReferencePoints)
				{
					if(!referencePoint.IsDummy)
					{
						numNonDummyReferencePoints++;
					}
				}
				
				mass /= numNonDummyReferencePoints;
			}
			
			
			foreach(ReferencePoint referencePoint in m_ReferencePoints)
			{
				if(!referencePoint.IsDummy)
				{
					if(referencePoint.Body2D)
					{
						referencePoint.Body2D.mass = mass;
						referencePoint.Body2D.gravityScale = m_GravityScale;
					}
					
					if(referencePoint.Body3D)
					{
						referencePoint.Body3D.mass = mass;
						referencePoint.Body3D.angularDrag = m_AngularDrag;
						referencePoint.Body3D.drag = m_Drag;
					}
				}
			}
		}
	}

	/// <summary>
	/// Reapply our spring/damping values to each joint
	/// </summary>
	public void UpdateJoints()
	{
		if(m_ReferencePoints != null)
		{
			foreach(ReferencePoint referencePoint in m_ReferencePoints)
			{
				if(!referencePoint.IsDummy)
				{
					if(referencePoint.Body2D != null)
					{
						SpringJoint2D[] joints = referencePoint.Body2D.gameObject.GetComponents<SpringJoint2D>();

						if(joints != null)
						{
							for(int jointIndex = 0; jointIndex < joints.Length; jointIndex++)
							{
								joints[jointIndex].frequency = m_Stiffness;
								joints[jointIndex].dampingRatio = m_DampingRatio;
							}
						}
					}
					
					if(referencePoint.Body3D != null)
					{
						SpringJoint[] joints = referencePoint.Body3D.gameObject.GetComponents<SpringJoint>();

						if(joints != null)
						{
							for(int jointIndex = 0; jointIndex < joints.Length; jointIndex++)
							{
								joints[jointIndex].spring = m_Stiffness;
								joints[jointIndex].damper = m_DampingRatio;
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Use this function to scale the Jelly Mesh at runtime. Scales the rigid bodies and
	/// rendered mesh by the given amount
	/// </summary>
	public void Scale(float scaleRatio)
	{
		int index = 0;
		Vector3[] refPointPositions = new Vector3[m_ReferencePoints.Count];

		foreach(ReferencePoint refPoint in m_ReferencePoints)
		{
			if(refPoint.GameObject)
			{
				refPointPositions[index] = refPoint.GameObject.transform.position;
			}

			index++;
		}

		this.transform.localScale = this.transform.localScale * scaleRatio;
		m_CentralPoint.GameObject.transform.parent.localScale = m_CentralPoint.GameObject.transform.parent.localScale * scaleRatio;

		index = 0;

		foreach(ReferencePoint refPoint in m_ReferencePoints)
		{
			if(refPoint.GameObject)
			{
				refPoint.GameObject.transform.position = refPointPositions[0] + ((refPointPositions[index] - refPointPositions[0]) * scaleRatio);

				if(m_2DMode)
				{
					SpringJoint2D[] springJoints = refPoint.GameObject.GetComponents<SpringJoint2D>();

					for(int jointLoop = 0; jointLoop < springJoints.Length; jointLoop++)
					{
						springJoints[jointLoop].connectedAnchor = springJoints[jointLoop].connectedAnchor * scaleRatio;
					}
				}
				else
				{
					SpringJoint[] springJoints = refPoint.GameObject.GetComponents<SpringJoint>();
					
					for(int jointLoop = 0; jointLoop < springJoints.Length; jointLoop++)
					{
						springJoints[jointLoop].connectedAnchor = springJoints[jointLoop].connectedAnchor * scaleRatio;
					}
				}
			}
			
			index++;
		}
	}

	/// <summary>
	/// Attaches a new object to the Jelly Mesh at runtime
	/// </summary>
	public void AddAttachPoint(Transform newAttachedObject)
	{
		m_NumAttachPoints++;
		ResizeAttachPoints();

		m_AttachPoints[m_NumAttachPoints - 1] = newAttachedObject;

		JellyMesh attachedJellyMesh = newAttachedObject.GetComponent<JellyMesh>();
			
		if(attachedJellyMesh)
		{
			m_IsAttachPointJellyMesh[m_NumAttachPoints - 1] = true;
			m_InitialAttachPointPositions[m_NumAttachPoints - 1] = m_CentralPoint.GameObject.transform.InverseTransformPoint(newAttachedObject.position);
		}
		else
		{
			m_IsAttachPointJellyMesh[m_NumAttachPoints - 1] = false;
			m_InitialAttachPointPositions[m_NumAttachPoints - 1] = m_CentralPoint.GameObject.transform.InverseTransformPoint(newAttachedObject.position);
			newAttachedObject.parent = this.transform;
		}

		float distanceSum = 0.0f;
		
		for(int referencePointIndex = 0; referencePointIndex < m_ReferencePoints.Count; referencePointIndex++)
		{
			if(!m_ReferencePoints[referencePointIndex].IsDummy)
			{
				float distance = Vector3.Distance(m_ReferencePoints[referencePointIndex].InitialOffset, m_AttachPoints[m_NumAttachPoints - 1].localPosition);
				distance = Mathf.Pow(distance, m_DistanceExponent);
				float invDistance = float.MaxValue;
				
				if(distance > 0.0f)
				{
					invDistance = 1.0f/distance;
				}
				
				distanceSum += invDistance;
			}
		}
		
		for(int referencePointIndex = 0; referencePointIndex < m_ReferencePoints.Count; referencePointIndex++)
		{
			if(!m_ReferencePoints[referencePointIndex].IsDummy) 
			{
				float distance = Vector3.Distance(m_ReferencePoints[referencePointIndex].InitialOffset, m_AttachPoints[m_NumAttachPoints - 1].localPosition);
				distance = Mathf.Pow(distance, m_DistanceExponent);
				float invDistance = float.MaxValue;
				
				if(distance > 0.0f)
				{
					invDistance = 1.0f/distance;
				}
				
				m_AttachPointWeightings[m_NumAttachPoints - 1, referencePointIndex] = invDistance/distanceSum;
			}
		}
	}

	/// <summary>
	/// Wake up the whole body - useful for editor controls when they update a value
	/// </summary>
	public void WakeUp()
	{
		if(m_ReferencePoints != null)
		{
			foreach(ReferencePoint referencePoint in m_ReferencePoints)
			{
				if(referencePoint.Body2D != null)
				{
					referencePoint.Body2D.WakeUp();
				}

				if(referencePoint.Body3D != null)
				{
					referencePoint.Body3D.WakeUp();
				}
			}
		}
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{
		if(m_ReferencePoints != null)
		{
#if UNITY_EDITOR
			// Debug draw the joints that connect each node
			if(Selection.activeGameObject == this.gameObject)
			{
				foreach(ReferencePoint referencePoint in m_ReferencePoints)
				{
					if(!referencePoint.IsDummy)
					{
						if(m_2DMode)
						{
							SpringJoint2D[] springJoints = referencePoint.Body2D.GetComponents<SpringJoint2D>();
							
							for(int jointIndex = 0; jointIndex < springJoints.Length; jointIndex++)
							{
								Debug.DrawLine(springJoints[jointIndex].transform.position, springJoints[jointIndex].connectedBody.transform.position, Color.green);
							}
						}
						else
						{
							SpringJoint[] springJoints = referencePoint.Body3D.GetComponents<SpringJoint>();
							
							for(int jointIndex = 0; jointIndex < springJoints.Length; jointIndex++)
							{
								Debug.DrawLine(springJoints[jointIndex].transform.position, springJoints[jointIndex].connectedBody.transform.position, Color.green);
							}
						}
					}
				}
			}
#endif

			this.transform.position = m_CentralPoint.GameObject.transform.position;
			this.transform.rotation = m_CentralPoint.GameObject.transform.rotation;

			// Apply our rigid body movements to the rendered mesh
			UpdateMesh();
			m_FirstUpdate = false;
		}
	}

	/// <summary>
	/// Add a position/radius pair to the free mode bodies
	/// </summary>
	void AddFreeModeBodyDefinition(Vector3 position, float radius, bool kinematic)
	{
		position = Quaternion.Euler(m_SoftBodyRotation) * position;
		m_FreeModeBodyPositions.Add(position);
		m_FreeModeBodyRadii.Add(radius);
		m_FreeModeBodyKinematic.Add(kinematic);
	}

	/// <summary>
	/// Helper function to draw a sphere with a line connecting to it from the object's origin
	/// </summary>
	void DrawSphereWithCentreConnection(Vector3 position, float radius, Color color)
	{
		position = Quaternion.Euler(m_SoftBodyRotation) * position;
		Vector3 worldPoint = this.transform.localToWorldMatrix.MultiplyPoint(position);
		Vector3 originPoint = this.transform.localToWorldMatrix.MultiplyPoint(m_SoftBodyPivotOffset);

		Gizmos.color = color;
		Gizmos.DrawWireSphere(worldPoint, radius);
		
		Gizmos.color = Color.white;
		Gizmos.DrawLine(worldPoint, originPoint);
	}

	/// <summary>
	/// Helper function to draw a sphere with a line connecting to it from the object's origin
	/// </summary>
	void DrawCentreConnection(Vector3 position, Vector3 centre)
	{
		position = Quaternion.Euler(m_SoftBodyRotation) * position;
		Vector3 worldPoint = this.transform.localToWorldMatrix.MultiplyPoint(position);
		Vector3 originPoint = this.transform.localToWorldMatrix.MultiplyPoint(centre);
		
		Gizmos.color = Color.white;
		Gizmos.DrawLine(worldPoint, originPoint);
	}

	/// <summary>
	/// Helper function to draw a sphere with a line connecting to it from the object's origin
	/// </summary>
	void DrawCentreConnectionUnrotated(Vector3 position, Vector3 centre)
	{
		Vector3 worldPoint = this.transform.localToWorldMatrix.MultiplyPoint(position);
		Vector3 originPoint = this.transform.localToWorldMatrix.MultiplyPoint(centre);
		
		Gizmos.color = Color.white;
		Gizmos.DrawLine(worldPoint, originPoint);
	}

	/// <summary>
	/// Draw the positions of the colliders when we select objects in the hierarchy
	/// </summary>
	void OnDrawGizmosSelected () 
	{
		if(!Application.isPlaying && IsMeshValid())
		{
			Bounds meshBounds = m_SourceMesh.bounds;
			float width = meshBounds.size.x * m_SoftBodyScale.x * m_MeshScale.x;
			float height = meshBounds.size.y * m_SoftBodyScale.y * m_MeshScale.y;
			float depth = meshBounds.size.z * m_SoftBodyScale.z * m_MeshScale.z;

			if(m_Style != PhysicsStyle.Free)
			{
				DrawSphereWithCentreConnection(m_SoftBodyPivotOffset, Mathf.Min(width, Mathf.Min(depth, height)) * 0.1f, Color.blue);
			}
			
			switch(m_Style)
			{
			case PhysicsStyle.Circle:
				{
					width = meshBounds.size.x * m_MeshScale.x;
					int numPoints = m_RadiusPoints;
					float radius = width * 0.5f;
					Vector3 prevPosition = Vector3.zero;
					Vector3 startPosition = Vector3.zero;					
					
					for(int loop = 0; loop < numPoints; loop++)
					{
						float angle = ((Mathf.PI * 2)/numPoints) * loop;
						Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
						offset *= radius;
						offset.x *= m_SoftBodyScale.x;
						offset.y *= m_SoftBodyScale.y;

						Vector3 bodyPosition = offset * (1.0f - (2 * m_SphereRadius));
						bodyPosition += m_SoftBodyOffset;

						DrawSphereWithCentreConnection(bodyPosition, width * m_SphereRadius, Color.green);
						
						if(m_AttachNeighbors)
						{
							if(loop == 0)
							{
								startPosition = bodyPosition;
							}
							else
							{
								DrawCentreConnection(bodyPosition, prevPosition);
							}
						}
						
						prevPosition = bodyPosition;
					}

					if(m_AttachNeighbors)
					{
						DrawCentreConnection(prevPosition, startPosition);
					}
				}
				break;

			case PhysicsStyle.Sphere:
				{
					width = meshBounds.size.x * m_MeshScale.x;
					float radius = width * 0.5f;

					int latitudes = m_RadiusPoints;
					int longitudes = m_RadiusPoints;
					
					float latitudeIncrement = 360.0f / latitudes;
					float longitudeIncrement = 180.0f / longitudes;

					Vector3 previousLongitudePosition = Vector3.zero;					
					
					for (float u = 0; u <= 360.0f; u += latitudeIncrement) 
					{
						for (float t = 0; t <= 180.0f; t += longitudeIncrement) 
						{			
							float rad = radius;				
							float x = (float) (rad * Mathf.Sin(Mathf.Deg2Rad * t) * Mathf.Sin(Mathf.Deg2Rad * u)) * m_SoftBodyScale.x;
							float y = (float) (rad * Mathf.Cos(Mathf.Deg2Rad * t)) * m_SoftBodyScale.y;
							float z = (float) (rad * Mathf.Sin(Mathf.Deg2Rad * t) * Mathf.Cos(Mathf.Deg2Rad * u))  * m_SoftBodyScale.z;		

							Vector3 bodyPosition = new Vector3(x, y, z) * (1.0f - (2 * m_SphereRadius));
							bodyPosition += m_SoftBodyOffset;
							DrawSphereWithCentreConnection(bodyPosition, width * m_SphereRadius, Color.green);

							if(m_AttachNeighbors)
							{
								DrawCentreConnection(bodyPosition, previousLongitudePosition);																
								float prevX = (float) (rad * Mathf.Sin(Mathf.Deg2Rad * t) * Mathf.Sin(Mathf.Deg2Rad * (u-latitudeIncrement))) * m_SoftBodyScale.x;
								float prevY = (float) (rad * Mathf.Cos(Mathf.Deg2Rad * t)) * m_SoftBodyScale.y;
								float prevZ = (float) (rad * Mathf.Sin(Mathf.Deg2Rad * t) * Mathf.Cos(Mathf.Deg2Rad * (u-latitudeIncrement)))  * m_SoftBodyScale.z;				

								Vector3 prevLatitudePosition = new Vector3(prevX, prevY, prevZ) * (1.0f - (2 * m_SphereRadius));
								prevLatitudePosition += m_SoftBodyOffset;
								DrawCentreConnection(bodyPosition, prevLatitudePosition);								
							}					

							previousLongitudePosition = bodyPosition;
						}
					}
				}
				break;
				
			case PhysicsStyle.Triangle:
				{
					float radius = meshBounds.size.y * m_SphereRadius * m_MeshScale.y;
					float offsetFactor = 0.5f - m_SphereRadius;
					
					Vector3 point1 = new Vector3(-width * offsetFactor, -height * offsetFactor) + m_SoftBodyOffset;
					Vector3 point2 = new Vector3(width * offsetFactor, -height * offsetFactor) + m_SoftBodyOffset;
					Vector3 point3 = new Vector3(0.0f, height * offsetFactor) + m_SoftBodyOffset;

					DrawSphereWithCentreConnection(point1, radius, Color.green);
					DrawSphereWithCentreConnection(point2, radius, Color.green);
					DrawSphereWithCentreConnection(point3, radius, Color.green);

					if(m_AttachNeighbors)
					{
						DrawCentreConnection(point1, point2);
						DrawCentreConnection(point2, point3);
						DrawCentreConnection(point3, point1);
					}
				}
				break;

			case PhysicsStyle.Pyramid:
				{
					float radius = meshBounds.size.y * m_SphereRadius * m_MeshScale.y;
					float offsetFactor = 0.5f - m_SphereRadius;
					
					Vector3 point1 = new Vector3(-width * offsetFactor, -height * offsetFactor, -depth * offsetFactor) + m_SoftBodyOffset;
					Vector3 point2 = new Vector3(width * offsetFactor, -height * offsetFactor, -depth * offsetFactor) + m_SoftBodyOffset;
					Vector3 point3 = new Vector3(width * offsetFactor, -height * offsetFactor, depth * offsetFactor) + m_SoftBodyOffset;
					Vector3 point4 = new Vector3(-width * offsetFactor, -height * offsetFactor, depth * offsetFactor) + m_SoftBodyOffset;					
					Vector3 point5 = new Vector3(0.0f, height * offsetFactor) + m_SoftBodyOffset;
					
					DrawSphereWithCentreConnection(point1, radius, Color.green);
					DrawSphereWithCentreConnection(point2, radius, Color.green);
					DrawSphereWithCentreConnection(point3, radius, Color.green);
					DrawSphereWithCentreConnection(point4, radius, Color.green);
					DrawSphereWithCentreConnection(point5, radius, Color.green);
					
					if(m_AttachNeighbors)
					{
						DrawCentreConnection(point1, point2);
						DrawCentreConnection(point2, point3);
						DrawCentreConnection(point3, point4);
						DrawCentreConnection(point4, point1);
					}
				}
				break;
				
			case PhysicsStyle.Rectangle:
				{
					float radius = meshBounds.size.y * m_SphereRadius * m_MeshScale.y;
					float offsetFactor = 0.5f - m_SphereRadius;

					Vector3 point1 = new Vector3(-width * offsetFactor, -height * offsetFactor) + m_SoftBodyOffset;
					Vector3 point2 = new Vector3(width * offsetFactor, -height * offsetFactor) + m_SoftBodyOffset;
					Vector3 point3 = new Vector3(width * offsetFactor, height * offsetFactor) + m_SoftBodyOffset;
					Vector3 point4 = new Vector3(-width * offsetFactor, height * offsetFactor) + m_SoftBodyOffset;

					DrawSphereWithCentreConnection(point1, radius, Color.green);
					DrawSphereWithCentreConnection(point2, radius, Color.green);
					DrawSphereWithCentreConnection(point3, radius, Color.green);
					DrawSphereWithCentreConnection(point4, radius, Color.green);

					if(m_AttachNeighbors)
					{
						DrawCentreConnection(point1, point2);
						DrawCentreConnection(point2, point3);
						DrawCentreConnection(point3, point4);
						DrawCentreConnection(point4, point1);
					}
				}
				break;

			case PhysicsStyle.Cube:
				{
					float radius = meshBounds.size.y * m_SphereRadius * m_MeshScale.y;
					float offsetFactor = 0.5f - m_SphereRadius;
					
					Vector3 point1 = new Vector3(-width * offsetFactor, -height * offsetFactor, -depth * offsetFactor) + m_SoftBodyOffset;
					Vector3 point2 = new Vector3(width * offsetFactor, -height * offsetFactor, -depth * offsetFactor) + m_SoftBodyOffset;
					Vector3 point3 = new Vector3(width * offsetFactor, height * offsetFactor, -depth * offsetFactor) + m_SoftBodyOffset;
					Vector3 point4 = new Vector3(-width * offsetFactor, height * offsetFactor, -depth * offsetFactor) + m_SoftBodyOffset;
					Vector3 point5 = new Vector3(-width * offsetFactor, -height * offsetFactor, depth * offsetFactor) + m_SoftBodyOffset;
					Vector3 point6 = new Vector3(width * offsetFactor, -height * offsetFactor, depth * offsetFactor) + m_SoftBodyOffset;
					Vector3 point7 = new Vector3(width * offsetFactor, height * offsetFactor, depth * offsetFactor) + m_SoftBodyOffset;
					Vector3 point8 = new Vector3(-width * offsetFactor, height * offsetFactor, depth * offsetFactor) + m_SoftBodyOffset;
					
					DrawSphereWithCentreConnection(point1, radius, Color.green);
					DrawSphereWithCentreConnection(point2, radius, Color.green);
					DrawSphereWithCentreConnection(point3, radius, Color.green);
					DrawSphereWithCentreConnection(point4, radius, Color.green);
					DrawSphereWithCentreConnection(point5, radius, Color.green);
					DrawSphereWithCentreConnection(point6, radius, Color.green);
					DrawSphereWithCentreConnection(point7, radius, Color.green);
					DrawSphereWithCentreConnection(point8, radius, Color.green);
					
					if(m_AttachNeighbors)
					{
						DrawCentreConnection(point1, point2);
						DrawCentreConnection(point2, point3);
						DrawCentreConnection(point3, point4);
						DrawCentreConnection(point4, point1);

						DrawCentreConnection(point5, point6);
						DrawCentreConnection(point6, point7);
						DrawCentreConnection(point7, point8);
						DrawCentreConnection(point8, point5);

						DrawCentreConnection(point1, point5);
						DrawCentreConnection(point2, point6);
						DrawCentreConnection(point3, point7);
						DrawCentreConnection(point4, point8);
					}
				}
				break;

			case PhysicsStyle.Free:
				{
					if(m_FreeModeBodyPositions != null)
					{
						for(int loop = 1; loop < m_FreeModeBodyPositions.Count; loop++)
						{
							DrawCentreConnectionUnrotated(m_FreeModeBodyPositions[loop], m_FreeModeBodyPositions[0]);
						}
					}
				}
				break;						
			}
		}
	}

#if UNITY_EDITOR
	[MenuItem("GameObject/Create Other/Jelly Mesh/Jelly Mesh", false, 12951)]
	static void DoCreateJellyMeshObject()
	{
		GameObject gameObject = new GameObject("Jelly Mesh");
		gameObject.AddComponent<JellyMesh>();
		MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
		meshRenderer.material = new Material(Shader.Find("Diffuse"));
		Selection.activeGameObject = gameObject;
		Undo.RegisterCreatedObjectUndo(gameObject, "Create Jelly Mesh");
	}
#endif
}
