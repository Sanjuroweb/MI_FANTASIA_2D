using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAtaque : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemigo")
        {
            //para que cusndo caigamos sobre la cabeza el bat reciba daņo 24
            if(collision.name == "Bat")
            {
                collision.GetComponent<Bat>().RecibirDaņo();
            }
            //27
            else if(collision.name == "Skeleton")
            {
                collision.GetComponent<Skeleton>().RecibirDaņo();
            }
            //31
            else if (collision.name == "Spider")
            {
                collision.GetComponent<Waypoints>().RecibirDaņo();
            }
        }
        //32
        else if (collision.CompareTag("Destruible"))
        {
            collision.GetComponent<Animator>().SetBool("destruir", true);
        }
    }
}
