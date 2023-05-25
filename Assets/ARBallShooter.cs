using System.Collections;
using UnityEngine;

public class ARBallShooter : MonoBehaviour
{
    public float force = 100.0f;

    [SerializeField]
    private float frequency = 0.5f;

    [SerializeField]
    private Vector3 initialOffset = new Vector3(0, 0, 1);

    [SerializeField]
    private GameObject ballPrefab;

    [SerializeField]
    private float lifeInSeconds = 15.0f;

    public bool enableShooting = true;

    private Coroutine shootingCoroutine;

    private void Awake()
    {
        shootingCoroutine = StartCoroutine(ShootingRoutine());
    }

    private void OnDisable()
    {
        StopCoroutine(shootingCoroutine);
    }

    private IEnumerator ShootingRoutine()
    {
        while(true)
        {
            if (enableShooting)
            {
                var ball = Instantiate(ballPrefab, transform.forward + initialOffset, Quaternion.identity, transform.parent);
                ball.GetComponent<Rigidbody>().AddForce(transform.forward * force);
                ball.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                Destroy(ball, lifeInSeconds);
            }
            yield return new WaitForSeconds(frequency);
        }
    }
}