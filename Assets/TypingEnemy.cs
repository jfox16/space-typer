using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypingEnemy : Poolable
{
    public string targetWord;
    public bool isAlive = false;

    float currentSpeed = 0.0f;
    [SerializeField] float acceleration = 1.0f;
    [SerializeField] float maxSpeed = 1.0f;
    [SerializeField] float knockbackDecay = 0.1f;
    [SerializeField] new Collider2D collider = null;

    public int lane = -1;

    [SerializeField] TextMeshPro textMesh;
    [SerializeField] Animator animator;
    [SerializeField] AudioClip hurtClip;
    [SerializeField] AudioClip dieClip;

    public char firstLetter {
        get { return targetWord[0]; }
    }

    void Awake()
    {
        collider = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        if (currentSpeed < 0) {
            currentSpeed *= (1 - knockbackDecay);
        }

        if (isAlive)
        {

            currentSpeed += acceleration * Time.deltaTime;
            
            if (currentSpeed > maxSpeed) currentSpeed = maxSpeed;

            if (transform.position.y <= TypingGameController.ENEMY_DIE_Y)
            {
                TypingGameController.Instance.RemoveEnemy(this);
                isAlive = false;
                Die();
            }
            else if (transform.position.y > TypingGameController.ENEMY_SPAWN_Y)
            {
                transform.position = new Vector3(
                    transform.position.x,
                    TypingGameController.ENEMY_SPAWN_Y,
                    transform.position.z
                );
            }
        }

        transform.position = transform.position + Vector3.down * currentSpeed * Time.deltaTime;
    }

    public override void Spawn()
    {
        isAlive = true;
        currentSpeed = 0.0f;
    }

    public override void Die()
    {
        animator.SetTrigger("Die");
        isAlive = false;
    }

    string formatTargetWord(string targetWord, int currentIndex)
    {
        if (currentIndex < targetWord.Length)
        {
            string done = targetWord.Substring(0, currentIndex);
            string notDone = targetWord.Substring(currentIndex, targetWord.Length - currentIndex);
            // return string.Format("<color=#ffbf00>{0}</color>{1}", done, notDone);
            return string.Format("<color=#70b8ff>{0}</color>{1}", done, notDone);
        }
        else
        {
            return string.Format("");
        }
    }

    void UpdateTextMesh(string newText)
    {
        textMesh.text = newText;
    }

    public char GetLetterAt(int i)
    {
        return targetWord[i];
    }

    public void UpdateIndex(int i)
    {
        UpdateTextMesh(formatTargetWord(targetWord, i));
    }

    public void SetTargetWord(string targetWord)
    {
        this.targetWord = targetWord;
        UpdateTextMesh(targetWord);
    }

    public void Hurt(float knockbackForce)
    {
        animator.SetTrigger("Hurt");
        currentSpeed = -knockbackForce;
    }

    public void Kill(float knockbackForce)
    {
        Die();
        currentSpeed = -knockbackForce;
    }

    public void PlayHurtClip()
    {
        if (hurtClip != null) {
            AudioController.Instance.PlayOneShot(hurtClip);
        }
    }

    public void PlayDieClip()
    {
        if (dieClip != null) {
            AudioController.Instance.PlayOneShot(dieClip);
        }
    }
}
