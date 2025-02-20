using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plane_controller : MonoBehaviour
{
    public float max_force = 1000f; // Realistic 300
    public float torque = 1f;
    float accel_factor = 1f;
    float k = 0.0062f;
    float drag_forward = 0.00062f; // 0.0062
    float drag_up = 1.0f;
    float drag_side = 10.0f;
    float C = 25.8f;
    float roll_factor = 0.01f;
    float pitch_factor = 0.01f;
    float yaw_factor = 0.01f;

    Vector3 init_pos;
    Vector3 init_vel;
    Vector3 init_angularVel;
    Quaternion init_rot;


    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();;
        rb.velocity = rb.rotation * Vector3.forward * 10.2f;

        init_pos = rb.position;
        init_vel = rb.velocity;
        init_angularVel = rb.angularVelocity; 
        init_rot = rb.rotation;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("space"))
        {
            rb.position = init_pos;
            rb.velocity = init_vel;
            rb.angularVelocity = new Vector3(0f,0f,0f); 
            rb.rotation = Quaternion.Euler(0f,180f,0f);
        }

        float dt = Time.deltaTime;
        Debug.Log("Delta time: " + dt);

        Vector3 diagonalDirection = new Vector3(1f, 1f, 0f);

        Vector3 normalizedDiagonalDirection = diagonalDirection.normalized;

        float roll = Input.GetAxis("Roll");
        float pitch = Input.GetAxis("Pitch");
        float accel_input = Input.GetAxis("Acceleration");

        Debug.Log("Input:" + accel_input);

         // We assume that the propellers can't change direction
        if(accel_input < 0) {
            accel_input = 0;
        }

        Debug.Log("Roll: " + roll * 100 + " Pitch: " + pitch * 100 + " Acceleration: " + accel_input * 100 + " Velocity: " + rb.velocity);
        
        Vector3 local_vel = transform.InverseTransformDirection(rb.velocity);
        float forward_vel_body = Vector3.Dot(Vector3.forward, local_vel);
        float left_vel_body = Vector3.Dot(Vector3.left, local_vel);
        float up_vel_body = Vector3.Dot(Vector3.up, local_vel);
        Debug.Log("Local vel " + local_vel);
        Debug.Log("Forward vel: " + forward_vel_body);
        Debug.Log("Up vel: " + up_vel_body);

       // Acceleration
        Vector3 acceleration = ((max_force * accel_factor * accel_input) / rb.mass * Vector3.forward);
        
        // Gravity & Lift
        Vector3 gravity = Vector3.down * 9.81f;
        Vector3 lift =  (Vector3.up * Mathf.Sign(forward_vel_body) * forward_vel_body * forward_vel_body * C / rb.mass);

        // Pitch & Yaw
        Vector3 pitch_vel = (Vector3.left * forward_vel_body * pitch * pitch_factor);
        Vector3 roll_yaw_vel =  (Vector3.forward * roll_factor * roll + Vector3.down * yaw_factor * roll) * forward_vel_body;

        // Drag
        Vector3 drag = Vector3.back * drag_forward * Mathf.Sign(forward_vel_body) * forward_vel_body * forward_vel_body 
                        + Vector3.down * drag_up * Mathf.Sign(up_vel_body) * up_vel_body * up_vel_body 
                        + Vector3.right * drag_side * Mathf.Sign(left_vel_body) * left_vel_body * left_vel_body;

        Vector3 local_angular_vel = rb.rotation * rb.angularVelocity;
        Vector3 angular_drag = 10f * local_angular_vel.sqrMagnitude * -local_angular_vel.normalized;



        //Debug.Log("Gravity: " + gravity);
        Debug.Log("Velocity: " + rb.velocity);
        Debug.Log("Lift " + lift);
        //Debug.Log("pitch_rotation " + 100 * pitch_vel);
        //Debug.Log("roll_yaw_rotation " + 100 * roll_yaw_vel);
        Debug.Log("drag" + drag);

        // rb.velocity += Quaternion.Inverse(rb.rotation) * (acceleration + lift + drag + gravity) * dt;
        // rb.angularVelocity += Quaternion.Inverse(rb.rotation) * (pitch_vel + roll_yaw_vel + angular_drag) * dt;

        rb.velocity += transform.TransformDirection((acceleration + lift + drag + gravity) * dt);
        rb.angularVelocity += transform.TransformDirection((pitch_vel + roll_yaw_vel) * dt);


        // rb.AddRelativeForce(acceleration + lift + drag + gravity);
        // rb.AddRelativeTorque(pitch_vel + roll_yaw_vel);

        //rb.AddRelativeTorque(Vector3.forward * roll * roll_factor * forward_vel_body);
        //rb.AddRelativeTorque(Vector3.left * pitch * pitch_factor * forward_vel_body);

     
    }
}
