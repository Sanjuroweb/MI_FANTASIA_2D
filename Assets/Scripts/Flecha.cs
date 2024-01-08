using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flecha : MonoBehaviour //28
{
    private Rigidbody2D rb;
    private BoxCollider2D bc;

    public LayerMask layerPiso;
    public GameObject esqueleto;
    public Vector2 direccionFlecha;
    public float radioDeColision = 0.25f;
    public bool tocaSuelo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //aplicamos daño al player, param: direccion a donde aplicamos el daño
            collision.GetComponent<PlayerController>().RecibirDaño(-(collision.transform.position - esqueleto.transform.position).normalized);
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        //para detectar si la flecha toca piso
        tocaSuelo = Physics2D.OverlapCircle((Vector2)transform.position, radioDeColision, layerPiso);
        if (tocaSuelo)
        {
            //no queremos que se mueva
            rb.bodyType = RigidbodyType2D.Static;
            bc.enabled = false;
            this.enabled = false;
        }

        //para el angulo nos pide direccion, el resultado esta en radianes y lo mult. para obtener grados
        float angulo = Mathf.Atan2(direccionFlecha.y, direccionFlecha.x) * Mathf.Rad2Deg;

        //para modificar el angulo
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.y, transform.localEulerAngles.x, angulo);
    }
}
