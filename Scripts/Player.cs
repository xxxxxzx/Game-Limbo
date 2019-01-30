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

    void Start () {
        _rigid = GetComponent<Rigidbody2D> ();
    }

    // Update is called once per frame
    void Update () {

        Movement ();

    }
    /*
    Movement() function is used to detect the input of A, D and Space Key.
    Then the character will react to the input.
     */
    void Movement () {

        //horizontal input from lef/right
        float move = Input.GetAxisRaw ("Horizontal");

        //If space is pressed, use IsGrounded() to judge whether the character should jump/is grounded
        if (Input.GetKeyDown (KeyCode.Space) && IsGrounded () == true) {
            _rigid.AddForce(transform.up * _jumpForce);
            Debug.Log("Jumpforce is "+transform.up * _jumpForce);
            StartCoroutine (ResetJumpRoutine ()); //reset the status of _resetJump to false every 0.1 second
        }

        _rigid.AddForce(new Vector2(move,0.0f) * _speed);
        Debug.Log("Speed is "+_speed+"\nHorizontal movement is "+move);
    }

    /*
    IsGrounded() function uses Raycast to find out whether the character is grounded
    so that it will tell could the character jump.
    @reuturn true iff the character is grounded
     */
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