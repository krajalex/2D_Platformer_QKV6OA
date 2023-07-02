using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform player; //Player objektum
    public Transform downPos; //Egyik pozici�
    public Transform upPos; //M�sik pozici�

    public float speed; //Platform mozg�si sebess�g
    bool isPlatformdown; //Lent van-e a platform


    void Update()
    {
        //Ha a platform a downposition-�n van akkor true �rt�ket kap az isplatformdown.
        if(transform.position.y <= downPos.position.y)
        {
            isPlatformdown = true;
        }
        //Ha a platform a upposition-�n van akkor false �rt�ket kap az isplatformdown.
        else if (transform.position.y >= upPos.position.y)
        {
            isPlatformdown = false;
        }
        //Ha lent van a platform, akkor az upposition fel� mozgatjuk a platformot a megadott sebess�ggel.
        if (isPlatformdown)
        {
            transform.position = Vector2.MoveTowards(transform.position, upPos.position, speed * Time.deltaTime);
        }
        //Ellenkez� esetben a downposition fel� mozgatjuk.
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, downPos.position, speed * Time.deltaTime);
        }
    }
}
