using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactuable : MonoBehaviour //38
{
    private bool puedeInteractuar;
    private BoxCollider2D bc;
    private SpriteRenderer sp;
    private GameObject indicadorInteractuable;
    private Animator anim;

    //en esta variable va a estar la variable que ejecuta la palanca 39
    public UnityEvent evento;

    public GameObject[] objetos;

    public bool esCofre;
    public bool esPalanca;
    public bool palancaAccionada;
    public bool esCheckPoint; //46

    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        //iniciamos indicadorInteractuable que sera hijo de los objetos interactuables
        //validamos si tiene objeto hijo
        if (transform.GetChild(0) != null)
            indicadorInteractuable = transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            puedeInteractuar = true;
            indicadorInteractuable.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            puedeInteractuar = false;
            indicadorInteractuable.SetActive(false);
        }
    }

    private void Cofre()
    {
        if (esCofre)
        {
            //el cofre mostrara un objeto random
            Instantiate(objetos[Random.Range(0, objetos.Length)], transform.position, Quaternion.identity);
            anim.SetBool("abrir", true);
            bc.enabled = false;
        }
    }

    //39
    private void Palanca()
    {
        if (esPalanca && !palancaAccionada)
        {
            anim.SetBool("activar", true);
            palancaAccionada = true;
            //con invoke se ejecuta lo que haya dentro del evento
            evento.Invoke();
            indicadorInteractuable.SetActive(false);
            bc.enabled = false;
            this.enabled = false;
        }
    }

    //46
    public void CheckPoint()
    {
        if (esCheckPoint)
        {
            evento.Invoke();
        }
    }

    private void Update()
    {
        if(puedeInteractuar && Input.GetKeyDown(KeyCode.C))
        {
            Cofre();
            Palanca(); //39
            CheckPoint(); //46
        }
    }
}
