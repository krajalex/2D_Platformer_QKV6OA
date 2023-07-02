using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform player; //Player objektum
    public Transform downPos; //Egyik pozició
    public Transform upPos; //Másik pozició

    public float speed; //Platform mozgási sebesség
    bool isPlatformdown; //Lent van-e a platform


    void Update()
    {
        //Ha a platform a downposition-ön van akkor true értéket kap az isplatformdown.
        if(transform.position.y <= downPos.position.y)
        {
            isPlatformdown = true;
        }
        //Ha a platform a upposition-ön van akkor false értéket kap az isplatformdown.
        else if (transform.position.y >= upPos.position.y)
        {
            isPlatformdown = false;
        }
        //Ha lent van a platform, akkor az upposition felé mozgatjuk a platformot a megadott sebességgel.
        if (isPlatformdown)
        {
            transform.position = Vector2.MoveTowards(transform.position, upPos.position, speed * Time.deltaTime);
        }
        //Ellenkezõ esetben a downposition felé mozgatjuk.
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, downPos.position, speed * Time.deltaTime);
        }
    }
}
