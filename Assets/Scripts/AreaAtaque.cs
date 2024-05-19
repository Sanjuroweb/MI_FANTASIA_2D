using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAtaque : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemigo")
        {
            //para que cusndo caigamos sobre la cabeza el bat reciba daño 24
            if(collision.name == "Bat")
            {
                collision.GetComponent<Bat>().RecibirDaño();
            }
            //27
            else if(collision.name == "Skeleton")
            {
                collision.GetComponent<Skeleton>().RecibirDaño();
            }
            //31
            else if (collision.name == "Spider")
            {
                collision.GetComponent<Waypoints>().RecibirDaño();
            }
            //enemB
            else if (collision.name == "SkeletonMelee")
            {
                collision.GetComponent<SkeletonMelee>().RecibirDaño();
            }
            //enemB
            else if (collision.name == "SkeMage")
            {
                collision.GetComponent<SkeletonMage>().RecibirDaño();
            }
        }
        //32
        else if (collision.CompareTag("Destruible"))
        {
            collision.GetComponent<Animator>().SetBool("destruir", true);
        }

        //me
        else if (collision.CompareTag("PuertaRompibleTunel"))
        {
            collision.GetComponent<PuertaRompibleTunel>().RecibirGolpe();
        }
        //comentario Manuel
    }
}
