using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuertaRompibleTunel : MonoBehaviour
{
    [Header("Estadisticas")]
    public int golpesNecesarios = 3; // Número de golpes necesarios para romper la puerta
    public int golpesRecibidos = 0; // Número de golpes recibidos
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Método para manejar los golpes recibidos
    public void RecibirGolpe()
    {
        Debug.Log("PuertaTunel recibe ostia");
        golpesRecibidos++; // Incrementar el contador de golpes recibidos

        // Verificar si se ha alcanzado el número de golpes necesarios
        if (golpesRecibidos >= golpesNecesarios)
        {
            RomperPuerta(); // Si se alcanzan los golpes necesarios, romper la puerta
        }
    }

    // Método para romper la puerta
    private void RomperPuerta()
    {
        // Aquí puedes implementar la lógica para romper la puerta, como cambiar su sprite a uno roto, reproducir un efecto de sonido, etc.
        Debug.Log("¡La puerta se ha roto!");
        Destroy(gameObject); // Destruir el GameObject de la puerta
    }
}
