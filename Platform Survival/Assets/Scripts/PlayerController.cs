using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;





public class PlayerController : MonoBehaviour, IPausable
{

    public float playerSpeed;
    public Camera followCamera;

    public float number1, number2;
    public UnityEvent OnPlayerLost;

    private Rigidbody m_Rb;
    private GameObject m_Elevator;
    private float m_ElevatorOffsetY;
    private Vector3 m_CameraPosition;
    private float m_SpeedModifier;

    void Awake()
    {
        m_Rb = GetComponent<Rigidbody>();
        m_ElevatorOffsetY = 0f;
        m_SpeedModifier = 1;
        m_CameraPosition = followCamera.transform.position - m_Rb.transform.position;
        enabled = false;
    }

    // Update is called once per frame
    // Use Fixed Update for rb
    void FixedUpdate()
    {

        if (transform.position.y < -15.0f)
        {
            OnPlayerLost.Invoke();
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 playerPos = m_Rb.position;
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (movement == Vector3.zero)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(movement);

        if (m_Elevator != null)
        {
            playerPos.y = m_Elevator.transform.position.y + m_ElevatorOffsetY;
        }

        // Rotate from the player's current position, to the target rotation, in X degrees per frame
        targetRotation = Quaternion.RotateTowards(
        transform.rotation, 
        targetRotation, 
        360 * Time.fixedDeltaTime);

        m_Rb.MovePosition(playerPos + movement * Time.fixedDeltaTime * playerSpeed * m_SpeedModifier);
        m_Rb.MoveRotation(targetRotation);
        
    }

    private void LateUpdate()
    {
        followCamera.transform.position = m_Rb.position + m_CameraPosition;
    }

    public void OnGameStart()
    {
        enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Elevator"))
        {
            m_Elevator = other.gameObject;
            m_ElevatorOffsetY = transform.position.y - m_Elevator.transform.position.y;
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PowerUp"))
        {
            Destroy(collision.gameObject);
            m_SpeedModifier = 2;
            StartCoroutine(BonusSpeedCountdown());
        }

        if (collision.gameObject.CompareTag("Enemy") && m_SpeedModifier > 1)
        {
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = collision.transform.position - transform.position;
            enemyRb.AddForce(awayFromPlayer * 20f, ForceMode.Impulse);
        }
    }

    private IEnumerator BonusSpeedCountdown()
    {
        yield return new WaitForSeconds(3.0f);
        m_SpeedModifier = 1;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Elevator"))
        {
            m_Elevator = null;
            m_ElevatorOffsetY = 0f;
        }
    }
}




















/*
        
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float newHorizontalPosition = transform.position.x + horizontalInput * Time.deltaTime * playerSpeed;
        float newVerticalPosition = transform.position.z + verticalInput * Time.deltaTime * playerSpeed;

        transform.position = new Vector3(
            newHorizontalPosition,
            transform.position.y,
            newVerticalPosition
        );

*/

// transform.Translate(movement * Time.deltaTime * playerSpeed);



/*
        // check if we are going backwards
        if (Mathf.Approximately(Vector3.Dot(movement, Vector3.forward), -1.0f))
        {
            // look rotation on Y axis so target will rotate into the direction.
            targetRotation = Quaternion.LookRotation(-Vector3.forward);
        }
*/