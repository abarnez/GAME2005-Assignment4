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


    public GameObject player;

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
                        if (Cubes[i].CompareTag("Player") || Cubes[j].CompareTag("Player"))
                        {
                            Debug.Log("Player Colliding Cube");
                            player.transform.position  -= new Vector3(0.08f,0.0f,0.08f);
                        } 
                        Debug.Log("Colliding Cube");
                    }
                }
            }
            for(int k = 0; k < Spheres.Count; k++)
            {
                if (SphereAABBCheck(Spheres[k], Cubes[i]))
                {
                    Debug.Log("Colliding Sphere");
                    Destroy(Spheres[k].gameObject);
                    Spheres.RemoveAt(k);
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

    private bool SphereAABBCheck(BallBehaviour Sphere, CubeBehaviour Cube)
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

        var x = Mathf.Max(min.x, Mathf.Min(aX, max.x));
        var y = Mathf.Max(min.y, Mathf.Min(aY, max.y));
        var z = Mathf.Max(min.z, Mathf.Min(aZ, max.z));

        var distance = Mathf.Sqrt((x - aX) * (x - aX) +
            (y - aY) * (y - aY) +
            (z - aZ) * (z - aZ));

        float radius = Vector3.Scale(aB.extents, sphere.transform.localScale).magnitude;

        return distance < Vector3.Scale(aB.extents, sphere.transform.localScale).magnitude;
    }
}
