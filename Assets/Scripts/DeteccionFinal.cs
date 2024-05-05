using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeteccionFinal : MonoBehaviour //47
{
    public bool avanzando;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("DETECTA JUGADOR - DeteccionFinal");
            GameManager.instance.ActivarPanelTransicion();
            GameManager.instance.avanzandoNivel = avanzando;
            StartCoroutine(EsperarCambioPosicion());
        }
    }

    private IEnumerator EsperarCambioPosicion()
    {
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.CambiarPosicionJugador(); //cambiamos posicion del player 
        if(avanzando)
            GameManager.instance.nivelActual++; //cambiamos el nivel en el que se encuentra
        else 
            GameManager.instance.nivelActual--; //cambiamos el nivel en el que se encuentra
    }
}
