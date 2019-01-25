using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private Rigidbody2D _rigid;
    [SerializeField]
    private float _jumpForce = 5.0f;

    private bool _resetJump = false;
    [SerializeField]
    private float _speed = 2.0f;

    //get handle to rigidbody
    // Start is called before the first frame update

    //variable for jump
    //variable grounded = false
    void Start () {
        _rigid = GetComponent<Rigidbody2D> ();
    }

    // Update is called once per frame
    void Update () {

        Movement ();

    }

    void Movement () {

        //horizontal input from lef/right
        float move = Input.GetAxisRaw ("Horizontal");

        if (Input.GetKeyDown (KeyCode.Space) && IsGrounded () == true) {
            Debug.Log ("Jump");
            // _rigid.velocity = new Vector2 (_rigid.velocity.x, _jumpForce);
            _rigid.AddForce(transform.up * _jumpForce);
            Debug.Log(transform.up * _jumpForce);
            StartCoroutine (ResetJumpRoutine ());
        }

        //current velocity = new velocity (horizontal input,current velocity.y)
        // _rigid.velocity = new Vector2 (move * _speed, _rigid.velocity.y);
        _rigid.AddForce(new Vector2(move,0.0f) * _speed);
    }

    bool IsGrounded () {
        RaycastHit2D hitInfo = Physics2D.Raycast (transform.position, Vector2.down, 0.4f, 1 << 8);
        return (hitInfo.collider != null);
    }

    IEnumerator ResetJumpRoutine () {
        _resetJump = true;
        yield return new WaitForSeconds (0.1f);
        _resetJump = false;
    }
}