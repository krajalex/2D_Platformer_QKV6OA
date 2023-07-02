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
    //Felv�telkor mennyivel n�velje az �rt�ket �s mennyi ideig.
    public float multiplier = 1.5f;
    public float duration = 3f;
    public AudioSource Sound;
    PlayerMovement stats;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Ha a player felveszi a powerup-ot, akkor egy Coroutine indul el, amely a Pickup met�dust hivja meg.
        if (other.tag == "Player")
        {
            StartCoroutine(Pickup(other));
        }
    }

    //Pickup met�dus, param�terk�nt megkapja a player collider�t.
    IEnumerator Pickup(Collider2D player)
    {
        //A player statjaihoz val� hozz�f�r�s.
        stats = player.GetComponent<PlayerMovement>();

        switch (powerUpType)
        {
            case PowerUpsAvailable.Speedup:
                //A player sebess�g�nek n�vel�se a multiplierrel.
                stats.movementSpeed *= multiplier;
                //A powerup ikonj�n�k elt�ntet�se �s a collider megsz�ntet�se + hangeffekt lej�tsz�sa
                Sound.Play();
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Collider2D>().enabled = false;

                //A met�dus v�r X m�sodpercet �s ut�na folytat�dik.
                yield return new WaitForSeconds(duration);

                //A sebess�g vissza�ll�t�sa.
                stats.movementSpeed /= multiplier;

                //A powerup objektum megsemmis�t�se.
                Destroy(gameObject);
                break;

            case PowerUpsAvailable.Speeddown:
                //A player statjaihoz val� hozz�f�r�s.
                player.GetComponent<PlayerMovement>();
                float defaultSpeed = stats.movementSpeed;

                //A player sebess�g�nek cs�kkent�se a multiplierrel.
                stats.movementSpeed *= multiplier;

                //A powerup ikonj�n�k elt�ntet�se �s a collider megsz�ntet�se + hangeffekt lej�tsz�sa
                //Sound.Play();
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Collider2D>().enabled = false;

                //A met�dus v�r X m�sodpercet �s ut�na folytat�dik.
                yield return new WaitForSeconds(duration);

                //A sebess�g vissza�ll�t�sa.
                stats.movementSpeed = defaultSpeed;

                //A powerup objektum megsemmis�t�se.
                Destroy(gameObject);
                break;
            default:
                Debug.Log("Not valid type.");
                break;
        }
    }
}
