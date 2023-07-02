using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollector : MonoBehaviour
{
    //A felvett log�k sz�ma.
    private int neptun = 0;
    public Text neptunText;
    public Timer script;
    public AudioSource neptunSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("neptun"))
        {
            //Hangeffekt lej�tsz�sa, a neptun log� objektum t�rl�se, a felvett log�k sz�m�n�ka n�vel�se, majd a canvasra val� ki�r�sa.
            neptunSound.Play();
            Destroy(collision.gameObject);
            neptun++;
            neptunText.text = neptun.ToString();
            script.score = script.score + 30;
        }
    }
}
