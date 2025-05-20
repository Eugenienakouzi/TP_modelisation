using UnityEngine;

public class TriggerParticleFX : MonoBehaviour
{
    public ParticleSystem myFX;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            myFX.Play(); // Ou myFX.Emit(30); pour émettre un nombre fixe de particules
        }
    }
}
