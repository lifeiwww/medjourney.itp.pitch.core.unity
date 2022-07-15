using UnityEngine;

public class BallHit : MonoBehaviour
{
    [SerializeField] private GameObject BallProjection;

    [SerializeField] private GameObject DiagnosticMarker;

    [SerializeField] private bool DoFeedback;

    [SerializeField] private ParticleSystem hitEffect;

    public ParticleSystem HitEffect
    {
        get => hitEffect;
        set => hitEffect = value;
    }

    private void Awake()
    {
        if (BallProjection != null)
            BallProjection.SetActive(false);
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            //Log.Debug("BallHit");
            var hitPosition = new Vector3();
            hitPosition = other.contacts[0].point;

            if (DoFeedback)
                PlayHitEffect(hitPosition, other.transform.rotation);
        }
    }

    private void PlayHitEffect(Vector3 hitPosition, Quaternion rotation)
    {
        //Log.Debug("hit position " + hitPosition);
        if (!hitEffect) return;
        var particles = Instantiate(hitEffect, hitPosition, rotation);
        particles.transform.Translate(Vector3.forward * -0.1f);
        particles.Play();
    }

    public void ActivateBallProjection( bool activate)
    {
        if (BallProjection != null)
        {
            BallProjection.SetActive(activate);
        }
    }
}