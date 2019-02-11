using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private const float PI = 3.141592f;

    public float maxSpeed = 10;
    public float acceleration = 36;
    public float jumpSpeed = 8;
    public float jumpDuration = 150;
    public float slowDown = 0.5f;
    public float airControl = 0.4f;
    public bool enableDoubleJump = true;
    public bool wallHitJumpReset = true;

    public float testAccel = 0.3f;
    public float testSlowDown = 0.6f;


    public float fallMultiplyer = 2.5f;
    public float lowJumpMultiplyer = 2.0f;

    bool canDoubleJump = true;
    float currentJumpDuration;
    bool jumpKeyDown = false;
    bool canVariableJump = false;


    private float accelScale = 0;



    // Update is called once per frame
    void Update()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        float horizontal = Input.GetAxis("Horizontal");
        float xVel = rb.velocity.x;
        float yVel = rb.velocity.y;

        float newXVel = 0.0f;

        float delta = Time.deltaTime;

        bool onTheGround = isOnGround();

        bool leftWallHit = isOnWallLeft();
        bool rightWallHit = isOnWallRight();

        if (horizontal > 0.01f || horizontal < -0.01f)
        {
            float accelChange = horizontal * testAccel * delta;
            accelScale += (onTheGround) ? accelChange : accelChange * airControl;

            if (accelScale > 0.5f * PI)
            {
                accelScale = 0.5f * PI;
            }
            else if (accelScale < -0.5f * PI)
            {
                accelScale = -0.5f * PI;
            }
        }
        else
        {
            if (onTheGround)
            {
                if (accelScale > -0.005f && accelScale < 0.005f)
                {
                    accelScale = 0.0f;
                }

                accelScale *= testSlowDown * delta;
            }
        }

        if ((leftWallHit && accelScale < 0.0f) || (rightWallHit && accelScale > 0.0f))
        {
            accelScale = 0.0f;
        }

        newXVel = Mathf.Sin(accelScale) * maxSpeed;
        rb.velocity = new Vector2(newXVel, yVel);

        float vertical = Input.GetAxis("Vertical");

        if (onTheGround)
        {
            canDoubleJump = true;
        }


        if (vertical > 0.1f)
        {
            if (!jumpKeyDown) // First Frame
            {
                jumpKeyDown = true;

                bool wallHit = false;
                int wallHitDirection = 0;

                if (horizontal != 0)
                {
                    if (leftWallHit)
                    {
                        if (accelScale < 0.0f)
                        {
                            accelScale = 0.25f * PI;
                        }
                        wallHit = true;
                        wallHitDirection = 1;
                    }
                    else if (rightWallHit)
                    {
                        if (accelScale > 0.0f)
                        {
                            accelScale = -0.25f * PI;
                        }
                        wallHit = true;
                        wallHitDirection = -1;
                    }
                  

                    if (wallHit && wallHitJumpReset)
                    {
                        canDoubleJump = true;
                    }

                }

                if (!wallHit)
                {
                    if (onTheGround || (canDoubleJump && enableDoubleJump))
                    {
                        rb.velocity = new Vector2(newXVel, this.jumpSpeed);

                        currentJumpDuration = 0.0f;
                        canVariableJump = true;
                    }
                }
                else
                {
                    rb.velocity = new Vector2(this.jumpSpeed * wallHitDirection, this.jumpSpeed);
                    accelScale = wallHitDirection * 0.25f * PI;

                    currentJumpDuration = 0.0f;
                    canVariableJump = true;
                }

                if (!onTheGround && !wallHit)
                {
                    canDoubleJump = false;
                }

            } // Second Frame
            else if (canVariableJump)
            {
                currentJumpDuration += delta;

                if (currentJumpDuration < this.jumpDuration / 1000)
                {
                    rb.velocity = new Vector2(newXVel, this.jumpSpeed);
                }
            }
        }
        else
        {
            //rb.velocity = new Vector2(newXVel, yVel * Physics2D.gravity.y * (lowJumpMultiplyer - 1) * delta);
            
            jumpKeyDown = false;
            canVariableJump = false;
        }

    }

    private bool isOnGround()
    {
        float lengthToSearch = 0.1f;
        float colliderThreshold = 0.001f;

        Vector2 lineStart = new Vector2(this.transform.position.x, this.transform.position.y - this.GetComponent<Renderer>().bounds.extents.y - colliderThreshold);
        Vector2 vectorToSearch = new Vector2(this.transform.position.x, lineStart.y - lengthToSearch);

        RaycastHit2D hit = Physics2D.Linecast(lineStart, vectorToSearch);

        return hit;
    }

    private bool isOnWallLeft()
    {
        bool retVal = false;

        float lengthToSearch = 0.1f;
        float colliderThreshold = 0.01f;

        Vector2 lineStart = new Vector2(this.transform.position.x - this.GetComponent<Renderer>().bounds.extents.x - colliderThreshold, this.transform.position.y);
        Vector2 vectorToSearch = new Vector2(lineStart.x - lengthToSearch, this.transform.position.y);

        RaycastHit2D hitLeft = Physics2D.Linecast(lineStart, vectorToSearch);
        retVal = hitLeft;

        if (retVal)
        {
            if (hitLeft.collider.GetComponent<NoSlideJump>())
            {
                return false;
            }
        }

        return retVal;
    }


    private bool isOnWallRight()
    {
        bool retVal = false;

        float lengthToSearch = 0.1f;
        float colliderThreshold = 0.01f;

        Vector2 lineStart = new Vector2(this.transform.position.x + this.GetComponent<Renderer>().bounds.extents.x + colliderThreshold, this.transform.position.y);
        Vector2 vectorToSearch = new Vector2(lineStart.x + lengthToSearch, this.transform.position.y);

        RaycastHit2D hitRight = Physics2D.Linecast(lineStart, vectorToSearch);
        retVal = hitRight;

        if (retVal)
        {
            if (hitRight.collider.GetComponent<NoSlideJump>())
            {
                return false;
            }
        }

        return retVal;
    }
}
