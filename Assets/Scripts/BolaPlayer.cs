using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BolaPlayer : MonoBehaviour
{
    public float bulletSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        //PARA QUE LA BALA VAYA A DCHA O IZDA SEGUN EN EL SENTIDO DEL PJ
        if (GameObject.Find("Player").transform.localScale.x < 0)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(-bulletSpeed, 0);
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(bulletSpeed, 0);
        }
        Destroy(gameObject, 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemigo")
        {
            //para que cusndo caigamos sobre la cabeza el bat reciba daño 24
            if (collision.name == "Bat")
            {
                collision.GetComponent<Bat>().RecibirDaño();
            }
            //27
            else if (collision.name == "Skeleton")
            {
                collision.GetComponent<Skeleton>().RecibirDaño();
            }
            //31
            else if (collision.name == "Spider")
            {
                collision.GetComponent<Waypoints>().RecibirDaño();
            }
        }
        //32
        else if (collision.CompareTag("Destruible"))
        {
            collision.GetComponent<Animator>().SetBool("destruir", true);
        }
    }
}
