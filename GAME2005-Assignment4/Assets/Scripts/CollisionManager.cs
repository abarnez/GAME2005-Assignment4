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
    public Slider cubeFrictionSlider;
    public Slider restitutionSlider;
    public Slider ballFrictionSlider;
    public Slider cubeMassSlider;
    public Slider ballMassSlider;

    private float velocityScalar = 0.9999f;

    //labels
    // public TMP_Text sMass;
    // public TMP_Text sPositon;
    public TMP_Text sVelocity;
   // public TMP_Text sAcceleration;
   // public TMP_Text sForce;

    private static CollisionManager _instance;

    public AudioSource bounceSound;
    public static CollisionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<CollisionManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("CollisionManager");
                    _instance = container.AddComponent<CollisionManager>();
                }
            }

            return _instance;
        }
    }

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

        for (int i = 0; i < Spheres.Count; i++)
        {
            Spheres[i].rigidBody.friction = ballFrictionSlider.value;
            Spheres[i].rigidBody.restitution = restitutionSlider.value;
            Spheres[i].rigidBody.mass = ballMassSlider.value;
            for (int j = 1; j < Spheres.Count; j++)
            {
                if (i != j && Spheres[i] != Spheres[j])
                {
                    Manifold collision = SphereSphereCheck(Spheres[i], Spheres[j]);
                    if (collision.result)
                    {
                        CalculateImpulse(collision, Spheres[i].rigidBody, Spheres[j].rigidBody, out Vector3 velocity1, out Vector3 velocity2);
                        if (!Spheres[i].contacts.balls.Contains(Spheres[j]))
                        {
                            Spheres[i].contacts.balls.Add(Spheres[j]);
                            Spheres[i].rigidBody.velocity = velocity1;
                            Spheres[j].rigidBody.velocity = velocity2;

                            bounceSound.Play();
                        }
                        else
                        {
                            // resolve collision instead of impulse again.
                            Spheres[i].rigidBody.velocity = velocity1 * velocityScalar;
                            Spheres[j].rigidBody.velocity = velocity2 * velocityScalar;
                        }
                    }
                    else
                    {
                        if (Spheres[i].contacts.balls.Contains(Spheres[j]))
                        {
                            Spheres[i].contacts.balls.Remove(Spheres[j]);
                        }
                    }
                }
            }
        }

        for (int i = 0; i < Cubes.Count; i++)
        {
            Cubes[i].rigidBody.friction = cubeFrictionSlider.value;
            Cubes[i].rigidBody.restitution = restitutionSlider.value;
            Cubes[i].rigidBody.mass = cubeMassSlider.value;
            for (int j = i + 1; j < Cubes.Count; j++)
            {
                if (i != j)
                {
                    if (Cubes[i].CompareTag("Player") || Cubes[j].CompareTag("Player"))
                    {
                        //player.transform.position -= new Vector3(0.08f, 0.0f, 0.08f);
                    }
                    else
                    {
                        Manifold collision = AABBCheck(Cubes[i], Cubes[j]);
                        if (collision.result)
                        {
                            CalculateImpulse(collision, Cubes[i].rigidBody, Cubes[j].rigidBody, out Vector3 velocity1, out Vector3 velocity2);
                            if (!Cubes[i].contacts.cubes.Contains(Cubes[j]))
                            {
                                //CalculateImpulse(collision, Cubes[i].rigidBody, Cubes[j].rigidBody, out Vector3 velocity1, out Vector3 velocity2);
                                Cubes[i].contacts.cubes.Add(Cubes[j]);

                                // Add collision impulse
                                if (!Cubes[i].rigidBody.anchored)
                                    Cubes[i].rigidBody.velocity = velocity1;
                                if (!Cubes[j].rigidBody.anchored)
                                    Cubes[j].rigidBody.velocity = velocity2;
                            }
                            else
                            {
                                // resolve collision instead of impulse again.
                                if (!Cubes[i].rigidBody.anchored)
                                    Cubes[i].rigidBody.velocity = velocity1 * velocityScalar;
                                if (!Cubes[j].rigidBody.anchored)
                                    Cubes[j].rigidBody.velocity = velocity2 * velocityScalar;
                            }
                        }
                        else
                        {
                            if (Cubes[i].contacts.cubes.Contains(Cubes[j]))
                            {
                                Cubes[i].contacts.cubes.Remove(Cubes[j]);
                            }
                        }
                    }
                }
            }
            for (int k = 0; k < Spheres.Count; k++)
            {
                if (Cubes[i].CompareTag("Player")) continue;
                Manifold collision = SphereAABBCheck(Spheres[k], Cubes[i]);
                if (collision.result)
                {
                    CalculateImpulse(collision, Spheres[k].rigidBody, Cubes[i].rigidBody, out Vector3 velocity1, out Vector3 velocity2);
                    if (!Spheres[k].contacts.cubes.Contains(Cubes[i]))
                    {
                        Spheres[k].contacts.cubes.Add(Cubes[i]);

                        // Add collision impulse
                        Spheres[k].rigidBody.velocity = velocity1;
                        if (!Cubes[i].rigidBody.anchored)
                            Cubes[i].rigidBody.velocity = velocity2;

                        bounceSound.Play();
                    }
                    else
                    {
                        // resolve collision instead of impulse again.
                        Spheres[k].rigidBody.velocity = velocity1 * velocityScalar;
                        if (!Cubes[i].rigidBody.anchored)
                            Cubes[i].rigidBody.velocity = velocity2 * velocityScalar;
                    }
                }
                else
                {
                    if (Spheres[k].contacts.cubes.Contains(Cubes[i]))
                    {
                        Spheres[k].contacts.cubes.Remove(Cubes[i]);
                    }
                }
            }
        }
    }

    private Manifold AABBCheck(CubeBehaviour Cube1, CubeBehaviour Cube2)
    {
        Manifold result = new Manifold();
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

        if (!((min1.x <= max2.x && max1.x >= min2.x) &&
            (min1.y <= max2.y && max1.y >= min2.y) &&
            (min1.z <= max2.z && max1.z >= min2.z)))
        {
            result.result = false;
            return result;
        }

        Vector3 pos1 = a.transform.position;
        Vector3 pos2 = b.transform.position;
        Vector3 size1 = a.transform.localScale;
        Vector3 size2 = b.transform.localScale;

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

        result.normal = normal;
        result.result = true;

        return result;
    }

    private Manifold SphereAABBCheck(BallBehaviour Sphere, CubeBehaviour Cube)
    {
        GameObject sphere = Sphere.gameObject;
        GameObject cube = Cube.gameObject;

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

        Vector3 relativeVelocity = Cube.rigidBody.velocity - Sphere.rigidBody.velocity;

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

        float radius = sphere.transform.localScale.x * 0.5f;
        Manifold result = new Manifold();
        result.normal = normal.normalized;
        result.result = distance < radius;

        return result;
    }
    Manifold SphereSphereCheck(BallBehaviour ball1, BallBehaviour ball2)
    {
        Manifold result = new Manifold();
        var pos1 = ball1.transform.position;
        var pos2 = ball2.transform.position;
        var distance = Mathf.Sqrt((pos1.x - pos2.x) * (pos1.x - pos2.x) +
            (pos1.y - pos2.y) * (pos1.y - pos2.y) +
            (pos1.z - pos2.z) * (pos1.z - pos2.z));
        var combinedRadius = ball1.rigidBody.radius + ball2.rigidBody.radius;
        if (distance > combinedRadius)
        {
            result.result = false;
            return result;
        }
        var normal = (pos1 - pos2).normalized;
        var penetrationDepth = ((distance * 0.5f) - combinedRadius);
        var contactPoint = (pos1 - pos2).magnitude - (combinedRadius);
        result.normal = normal;
        result.result = true;

        return result;
    }
    Manifold SweptRectRect(CubeBehaviour Cube1, CubeBehaviour Cube2)
    {
        GameObject obj1 = Cube1.gameObject;
        GameObject obj2 = Cube2.gameObject;
        Manifold result = new Manifold();

        Vector3 vel1 = Cube1.rigidBody.velocity;
        Vector3 vel2 = Cube1.rigidBody.velocity;

        Vector3 pos1 = obj1.transform.position;
        Vector3 pos2 = obj2.transform.position;

        Vector3 movedPos1 = pos1 + vel1;
        Vector3 movedPos2 = pos2 + vel2;

        Vector3 size1 = obj1.transform.localScale;
        Vector3 size2 = obj2.transform.localScale;

        // If no collision
        if (!(movedPos1.x - size1.x / 2 < movedPos2.x + size2.x / 2 &&
            movedPos1.x + size1.x / 2 > movedPos2.x - size2.x / 2 &&
            movedPos1.y - size1.y / 2 < movedPos2.y + size2.y / 2 &&
            movedPos1.y + size1.y / 2 > movedPos2.y - size2.y / 2 &&
            movedPos1.z - size1.z / 2 < movedPos2.z + size2.z / 2 &&
            movedPos1.z + size1.z / 2 > movedPos2.z - size2.z / 2))
        {
            result.normal.x = 0.0f;
            result.normal.y = 0.0f;
            result.result = false;
            return result;
        }

        result.result = true;

        float testX = movedPos1.x;
        float testY = movedPos1.y;
        float testZ = movedPos1.z;

        if (pos1.x < pos2.x - size2.x / 2) testX = pos2.x - size2.x / 2; // left edge
        else if (pos1.x > pos2.x + size2.x / 2) testX = pos2.x + size2.x / 2; // right edge

        if (pos1.y < pos2.y - size2.y / 2) testY = pos2.y - size2.y / 2; // top edge
        else if (pos1.y > pos2.y + size2.y / 2) testY = pos2.y + size2.y / 2; // bottom edge

        if (pos1.z < pos2.z - size2.z / 2) testZ = pos2.z - size2.z / 2; // front edge
        else if (pos1.z > pos2.z + size2.z / 2) testZ = pos2.z + size2.z / 2; // back edge

        float distX = testX - pos1.x;
        float distY = testY - pos1.y;
        float distZ = testZ - pos1.z;

        float difX = size1.x / 2 + size2.x / 2 - Mathf.Abs(movedPos2.x - movedPos1.x);
        float difY = size1.y / 2 + size2.y / 2 - Mathf.Abs(movedPos2.y - movedPos1.y);
        float difZ = size1.z / 2 + size2.z / 2 - Mathf.Abs(movedPos2.z - movedPos1.z);

        Vector3 diffMove1 = new Vector3();
        Vector3 diffMove2 = new Vector3();

        // Determine the normal
        float max_penetration = Mathf.Max(Mathf.Abs(distZ), Mathf.Max(Mathf.Abs(distX), Mathf.Abs(distY)));
        if (max_penetration == Mathf.Abs(distX))
        {
            result.normal.x = (distX > 0.0f ? -1.0f : 1.0f);
            result.normal.y = 0.0f;
            result.normal.z = 0.0f;

            float velsum = Mathf.Abs(Cube1.rigidBody.velocity.x) + Mathf.Abs(Cube2.rigidBody.velocity.x);
            float propK1 = Mathf.Abs(Cube1.rigidBody.velocity.x) / velsum;
            float propK2 = Mathf.Abs(Cube2.rigidBody.velocity.x) / velsum;

            diffMove1 = new Vector3(-result.normal.x * (difX + Mathf.Abs(vel1.x) + 0.0f) * propK1, 0.0f, 0.0f);
            diffMove2 = new Vector3(result.normal.x * (difX + Mathf.Abs(vel2.x) + 0.0f) * propK2, 0.0f, 0.0f);
        }
        else if (max_penetration == Mathf.Abs(distY))
        {
            result.normal.x = 0.0f;
            result.normal.y = (distY > 0.0f ? -1.0f : 1.0f);
            result.normal.z = 0.0f;

            float velsum = Mathf.Abs(Cube1.rigidBody.velocity.y) + Mathf.Abs(Cube2.rigidBody.velocity.y);
            float propK1 = Mathf.Abs(Cube1.rigidBody.velocity.y) / velsum;
            float propK2 = Mathf.Abs(Cube2.rigidBody.velocity.y) / velsum;

            diffMove1 = new Vector3(0.0f, -result.normal.y * (difY + Mathf.Abs(vel1.y) + 0.0f) * propK1, 0.0f);
            diffMove2 = new Vector3(0.0f, result.normal.y * (difY + Mathf.Abs(vel2.y) + 0.0f) * propK2, 0.0f);
        }
        else if (max_penetration == Mathf.Abs(distZ))
        {
            result.normal.x = 0.0f;
            result.normal.y = 0.0f;
            result.normal.z = (distZ > 0.0f ? -1.0f : 1.0f);

            float velsum = Mathf.Abs(Cube1.rigidBody.velocity.z) + Mathf.Abs(Cube2.rigidBody.velocity.z);
            float propK1 = Mathf.Abs(Cube1.rigidBody.velocity.z) / velsum;
            float propK2 = Mathf.Abs(Cube2.rigidBody.velocity.z) / velsum;

            diffMove1 = new Vector3(0.0f, 0.0f, -result.normal.z * (difZ + Mathf.Abs(vel1.z) + 0.0f) * propK1);
            diffMove2 = new Vector3(0.0f, 0.0f, result.normal.z * (difZ + Mathf.Abs(vel2.z) + 0.0f) * propK2);
        }

        return result;
    }

    Manifold SweptSphereCheck(BallBehaviour P0, BallBehaviour P1, BallBehaviour Q0, BallBehaviour Q1, float outT)
    {
        Manifold result = new Manifold();
        // Compute X, Y, a, b, and c
        Vector3 X = P0.transform.position - Q0.transform.position;
        Vector3 Y = P1.transform.position - P0.transform.position - (Q1.transform.position - Q0.transform.position);
        float a = Vector3.Dot(Y, Y);
        float b = 2 * Vector3.Dot(X, Y);
        float sumRadii = P0.rigidBody.radius + Q0.rigidBody.radius;
        float c = Vector3.Dot(X, X) - sumRadii * sumRadii;
        // Solve discriminant
        float disc = b * b - 4 * a * c;
        if(disc < 0.0f)
        {
            result.result = false;
            return result;
        }
        else
        {
            disc = Mathf.Sqrt(disc);
            outT = (-b - disc) / (2 * a);
            if (outT >= 0 && outT <= 0)
            {
                result.result = true;
                return result;
            }
            else
            {
                result.result = false;
                return result;
            }
        }
    }
    void CalculateImpulse(Manifold collision, RigidBody rigidBody1, RigidBody rigidBody2, out Vector3 velocity1, out Vector3 velocity2)
    {
        Vector3 relativeVelocity = rigidBody2.velocity - rigidBody1.velocity;
        float relativeNormal = Vector3.Dot(relativeVelocity, collision.normal);
        float e = Mathf.Min(rigidBody2.restitution, rigidBody1.restitution);
        float inverseMass1 = 1 / rigidBody1.mass;
        float inverseMass2 = 1 / rigidBody2.mass;
        float j = (-(1 + e) * (relativeNormal)) / (inverseMass1 + inverseMass2);

        Vector3 t = relativeVelocity - (relativeNormal * collision.normal);
        float jt = (-(1 + e) * (Vector3.Dot(relativeVelocity, t))) / (inverseMass1 + inverseMass2);
        float friction = Mathf.Sqrt(rigidBody1.friction * rigidBody2.friction);
        jt = Mathf.Max(jt, -j * friction);
        jt = Mathf.Min(jt, j * friction);

        velocity2 = rigidBody2.velocity + ((jt / rigidBody2.mass) * collision.normal);
        velocity1 = rigidBody1.velocity - ((jt / rigidBody1.mass) * collision.normal);
    }
}
