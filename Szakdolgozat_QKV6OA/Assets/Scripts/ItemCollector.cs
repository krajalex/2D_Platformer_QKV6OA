using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollector : MonoBehaviour
{
    //A felvett logók száma.
    private int neptun = 0;
    public Text neptunText;
    public Timer script;
    public AudioSource neptunSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("neptun"))
        {
            //Hangeffekt lejátszása, a neptun logó objektum törlése, a felvett logók számánáka növelése, majd a canvasra való kiírása.
            neptunSound.Play();
            Destroy(collision.gameObject);
            neptun++;
            neptunText.text = neptun.ToString();
            script.score = script.score + 30;
        }
    }
}
