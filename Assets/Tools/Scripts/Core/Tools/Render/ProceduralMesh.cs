using UnityEngine;

namespace Scripts.Core.Tools.Render
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public abstract class ProceduralMesh : MonoBehaviour
    {
        protected Mesh Mesh;

	    protected virtual void OnEnable()
		{
            Mesh = new Mesh();
			GetComponent<MeshFilter>().mesh = Mesh;
            
            Initialize();
		}

        protected virtual void Initialize()
        {
            Mesh.Clear();

            CalculateVertices();
            CalculateNormals();
            CalculateUv();
            CalculateTringles();
        }

        public void Update() 
		{
            if(Application.isEditor)
                Initialize();

			CalculateVertices();
		}

        public abstract void SetVertexCount(int count);

        public abstract void SetPosition(int index, Vector3 position);

        protected abstract void CalculateVertices();

        protected abstract void CalculateNormals();
		
		protected abstract void CalculateUv();
		
		protected abstract void CalculateTringles();
    }
}
