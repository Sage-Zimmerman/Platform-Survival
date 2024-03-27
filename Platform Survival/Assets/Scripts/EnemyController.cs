using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    private Rigidbody m_Rb;
    private GameObject m_FollowTarget;
    private bool m_isRecharged = true;

    public float speed;
    public float pushRadius;

    //for rigid body
    void Awake()
    {
        AddCircle();

        m_Rb = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        m_FollowTarget = GameObject.Find("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 moveTowards = m_FollowTarget.transform.position - transform.position;
        moveTowards.y = 0;
        m_Rb.AddForce(moveTowards.normalized * speed);

        if ((Mathf.Abs(moveTowards.magnitude) <= pushRadius) && m_isRecharged)
        {
            m_isRecharged = false;
            m_Rb.AddForce(moveTowards.normalized * speed * 1.1f, ForceMode.Impulse);
            Invoke(nameof(Recharge), 2.0f);
        }

        if (transform.position.y <= -15.0f)
        {
            Destroy(gameObject);
        }

    }

    void Recharge()
    {
        m_isRecharged = true;
    }

    private void AddCircle()
    {
        GameObject go = new GameObject() {
            name = "Circle"
        };

        Vector3 circlePosition = Vector3.zero;
        circlePosition.y = -0.49f;

        go.transform.parent = transform;
        go.transform.localPosition = circlePosition;

        go.DrawCircle(pushRadius, 0.02f);
    }
}
