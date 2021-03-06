using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypingGamePlayer : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] TypingWeapon typingWeapon;
    [SerializeField] Keyboard keyboard;

    // For MoveToPosition
    bool isMoving = false;
    Vector3 start;
    Vector3 end;
    float t = 0.0f;
    float t_step = 1.0f;

    void Update()
    {
        if (isMoving)
        {
            t += t_step * Time.deltaTime;
            Vector3 newPosition = Vector3.Lerp( start, end, Mathf.SmoothStep(0.0f, 1.0f, t) );
            float xDiff = newPosition.x - transform.position.x;
            transform.position = newPosition;

            animator.SetFloat("Horizontal Movement", xDiff);
        }
        else
        {
            animator.SetFloat("Horizontal Movement", 0);
        }
    }

    public void MoveToPosition(Vector3 destination, float time)
    {
        start = transform.position;
        end = destination;
        t = 0;
        t_step = 1 / time;
        isMoving = true;
    }

    public void StopMoving()
    {
        this.isMoving = false;
    }

    public void Shoot(TypingEnemy target)
    {
        typingWeapon.Shoot(target);
    }

    public void ShootKillShot(TypingEnemy target)
    {
        typingWeapon.ShootKillShot(target);
    }

    public void PlayKeyPress()
    {
        keyboard.PlayKeyPress();
    }
}
