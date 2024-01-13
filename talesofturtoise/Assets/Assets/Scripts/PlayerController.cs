using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{

    //This Script is based on an arcticle of IronEqual, 04.08.2017 https://medium.com/ironequal/unity-character-controller-vs-rigidbody-a1e243591483
    // Camera Settings https://www.youtube.com/watch?v=Jh6JBpKca_k


    private float speed = 3f;
    private float rotationSpeed = 3f;

    private Rigidbody rb;

    private Vector3 _inputs = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _inputs = Vector3.zero;
        _inputs.x = UnityEngine.Input.GetAxis("Horizontal");
        _inputs.z = UnityEngine.Input.GetAxis("Vertical");
        if (_inputs != Vector3.zero)
            transform.forward = _inputs;

    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + _inputs * speed * Time.fixedDeltaTime);
    }
}
