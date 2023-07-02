using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PowerUpsAvailable
{
    Speedup,
    Speeddown,
    JumpForceDown,
    JumpForceUp,
    InstantDeath
}
public class PowerUp : MonoBehaviour
{
    public PowerUpsAvailable powerUpType;
    //Felvételkor mennyivel növelje az értéket és mennyi ideig.
    public float multiplier = 1.5f;
    public float duration = 3f;
    public AudioSource Sound;
    PlayerMovement stats;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Ha a player felveszi a powerup-ot, akkor egy Coroutine indul el, amely a Pickup metódust hivja meg.
        if (other.tag == "Player")
        {
            StartCoroutine(Pickup(other));
        }
    }

    //Pickup metódus, paraméterként megkapja a player colliderét.
    IEnumerator Pickup(Collider2D player)
    {
        //A player statjaihoz való hozzáférés.
        stats = player.GetComponent<PlayerMovement>();

        switch (powerUpType)
        {
            case PowerUpsAvailable.Speedup:
                //A player sebességének növelése a multiplierrel.
                stats.movementSpeed *= multiplier;
                //A powerup ikonjánák eltüntetése és a collider megszüntetése + hangeffekt lejátszása
                Sound.Play();
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Collider2D>().enabled = false;

                //A metódus vár X másodpercet és utána folytatódik.
                yield return new WaitForSeconds(duration);

                //A sebesség visszaállítása.
                stats.movementSpeed /= multiplier;

                //A powerup objektum megsemmisítése.
                Destroy(gameObject);
                break;

            case PowerUpsAvailable.Speeddown:
                //A player statjaihoz való hozzáférés.
                player.GetComponent<PlayerMovement>();
                float defaultSpeed = stats.movementSpeed;

                //A player sebességének csökkentése a multiplierrel.
                stats.movementSpeed *= multiplier;

                //A powerup ikonjánák eltüntetése és a collider megszüntetése + hangeffekt lejátszása
                //Sound.Play();
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Collider2D>().enabled = false;

                //A metódus vár X másodpercet és utána folytatódik.
                yield return new WaitForSeconds(duration);

                //A sebesség visszaállítása.
                stats.movementSpeed = defaultSpeed;

                //A powerup objektum megsemmisítése.
                Destroy(gameObject);
                break;
            default:
                Debug.Log("Not valid type.");
                break;
        }
    }
}
