using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{
    public GameObject player; //A karakter-re hivatkozik

    void Update() //Itt t�rt�nik a Main Camera mozgat�sa, hogy k�vesse a karaktert
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
    }
}
