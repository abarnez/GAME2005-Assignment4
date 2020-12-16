using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextMeshUpdate : MonoBehaviour
{
    public CubeBehaviour cube;
    public TMP_Text floatingText;
    private RectTransform textTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        textTransform = floatingText.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        floatingText.text = cube.rigidBody.velocity.magnitude.ToString("F2") + " m/s\n" + cube.rigidBody.mass + " kg\nFriction " + cube.rigidBody.friction;
        textTransform.rotation = Quaternion.LookRotation(cube.transform.position - Camera.main.transform.position);
    }
}
