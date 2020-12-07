using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[System.Serializable]
public class CollisionManager : MonoBehaviour
{
    public List<CubeBehaviour> Cubes;
    public List<BallBehaviour> Spheres;

    //sliders
    public Slider Velocity;
    public Slider Friction;
    public Slider Mass;
    public float velocity;
    public float friction;
    public float mass;
    //lables
   // public TMP_Text sMass;
   // public TMP_Text sPositon;
    public TMP_Text sVelocity;
   // public TMP_Text sAcceleration;
   // public TMP_Text sForce;

    public float FrictionCoef;
    public float MomentumCoef;

    struct Manifold
    {
        public Vector3 normal;
        public bool result;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //sVelocity.text = "Velocity: " + velocity;

        velocity = Velocity.value;
        friction = Friction.value;
        mass = Mass.value;

       
        for(int i = 0; i < Cubes.Count; i++)
        {
            for(int j = 0; j < Cubes.Count; j++)
            {
                if (i != j)
                {
                    if (AABBCheck(Cubes[i], Cubes[j]))
                    {
                        Debug.Log("Colliding Cube");
                    }
                }
            }
            for(int k = 0; k < Spheres.Count; k++)
            {
                Manifold collision = SphereAABBCheck(Spheres[k], Cubes[i]);
                if (collision.result)
                {
                    Vector3 relativeVelocity = Cubes[i].rigidBody.Velocity - Spheres[k].rigidBody.Velocity;
                    float relativeNormal = Vector3.Dot(relativeVelocity, collision.normal);
                    float e = Mathf.Min(Cubes[i].rigidBody.Restitution, Spheres[k].rigidBody.Restitution);
                    float j = (-(1 + e) * (relativeNormal)) / ((1 / Spheres[k].rigidBody.Mass) + (1 / Cubes[i].rigidBody.Mass));
                    //Cubes[i].rigidBody.Velocity = Cubes[i].rigidBody.Velocity - ((j / Cubes[i].rigidBody.Mass) * collision.normal);
                    Spheres[k].rigidBody.Velocity = Spheres[k].rigidBody.Velocity - ((j / Spheres[k].rigidBody.Mass) * collision.normal);

                    Vector3 t = relativeVelocity - (relativeNormal * collision.normal);
                    float jt = (-(1 + e) * (Vector3.Dot(relativeVelocity, t))) / ((1 / Spheres[k].rigidBody.Mass) + (1 / Cubes[i].rigidBody.Mass));
                    float friction = Mathf.Sqrt(Spheres[k].rigidBody.Friction * Cubes[i].rigidBody.Friction);
                    jt = Mathf.Min(jt, j * friction);

                    //Cubes[i].rigidBody.Velocity = Cubes[i].rigidBody.Velocity + ((jt / Cubes[i].rigidBody.Mass) * collision.normal);
                    //Spheres[k].rigidBody.Velocity = Spheres[k].rigidBody.Velocity - ((jt / Spheres[k].rigidBody.Mass) * collision.normal);
                }
            }
        }
    }

    private bool AABBCheck(CubeBehaviour Cube1, CubeBehaviour Cube2)
    {
        GameObject a = Cube1.gameObject;
        GameObject b = Cube2.gameObject;

        MeshFilter aMF = a.GetComponent<MeshFilter>();
        MeshFilter bMF = b.GetComponent<MeshFilter>();

        Bounds aB = aMF.mesh.bounds;
        Bounds bB = bMF.mesh.bounds;

        var min1 = Vector3.Scale(aB.min, a.transform.localScale) + a.transform.position;
        var max1 = Vector3.Scale(aB.max, a.transform.localScale) + a.transform.position;

        var min2 = Vector3.Scale(bB.min, b.transform.localScale) + b.transform.position;
        var max2 = Vector3.Scale(bB.max, b.transform.localScale) + b.transform.position;

        if ((min1.x <= max2.x && max1.x >= min2.x) &&
            (min1.y <= max2.y && max1.y >= min2.y) &&
            (min1.z <= max2.z && max1.z >= min2.z))
        {
            return true;
        }
        return false;
    }

    private Manifold SphereAABBCheck(BallBehaviour Sphere, CubeBehaviour Cube)
    {
        GameObject sphere = Sphere.gameObject;
        GameObject cube = Cube.gameObject;

        // Can Access Cube and Sphere's rigidbody properties here.
        //Debug.Log(Sphere.rigidBody.Velocity);

        MeshFilter aMF = sphere.GetComponent<MeshFilter>();
        MeshFilter bMF = cube.GetComponent<MeshFilter>();

        Bounds aB = aMF.mesh.bounds;
        Bounds bB = bMF.mesh.bounds;

        var min = Vector3.Scale(bB.min, cube.transform.localScale) + cube.transform.position;
        var max = Vector3.Scale(bB.max, cube.transform.localScale) + cube.transform.position;

        float aX = sphere.transform.position.x;
        float aY = sphere.transform.position.y;
        float aZ = sphere.transform.position.z;

        float x = Mathf.Max(min.x, Mathf.Min(aX, max.x));
        float y = Mathf.Max(min.y, Mathf.Min(aY, max.y));
        float z = Mathf.Max(min.z, Mathf.Min(aZ, max.z));

        float distance = Mathf.Sqrt((x - aX) * (x - aX) +
            (y - aY) * (y - aY) +
            (z - aZ) * (z - aZ));

        Vector3 relativeVelocity = Cube.rigidBody.Velocity - Sphere.rigidBody.Velocity;

        Vector3 pos1 = sphere.transform.position;
        Vector3 pos2 = cube.transform.position;
        Vector3 size1 = sphere.transform.localScale;
        Vector3 size2 = cube.transform.localScale;

        float testX = pos1.x;
        float testY = pos1.y;
        float testZ = pos1.z;

        if (pos1.x < pos2.x - size2.x / 2) testX = pos2.x - size2.x / 2; // left edge
        else if (pos1.x > pos2.x + size2.x / 2) testX = pos2.x + size2.x / 2; // right edge

        if (pos1.y < pos2.y - size2.y / 2) testY = pos2.y - size2.y / 2; // top edge
        else if (pos1.y > pos2.y + size2.y / 2) testY = pos2.y + size2.y / 2;

        if (pos1.z < pos2.z - size2.z / 2) testZ = pos2.z - size2.z / 2;
        else if (pos1.z > pos2.z + size2.z / 2) testZ = pos2.z + size2.z / 2;

        Vector3 point = new Vector3(testX, testY, testZ);
        Vector3 normal = pos1 - point;

        float radius = sphere.transform.localScale.x * 0.5f; // Vector3.Scale(aB.extents, sphere.transform.localScale).magnitude;
        Manifold result = new Manifold();
        result.normal = normal.normalized;
        result.result = distance < radius;

        //Debug.Log("Radius: " + radius);                                   

        return result;
    }
}
