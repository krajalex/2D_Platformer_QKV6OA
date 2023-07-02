using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{
    public GameObject player; //A karakter-re hivatkozik

    void Update() //Itt történik a Main Camera mozgatása, hogy kövesse a karaktert
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
    }
}
