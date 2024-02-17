using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FondoMovimiento : MonoBehaviour //me, www.youtube.com/watch?v=7bJT6rf-Jvk
{

    [SerializeField] private Vector2 velocidadMovimiento;

    private Vector2 offset;

    private Material material;

    private Rigidbody2D jugadorRB;

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
        jugadorRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Calcular el cambio en el offset basado en la velocidad de movimiento y el tiempo transcurrido desde el último frame
        offset = (jugadorRB.velocity.x * 0.1f) * velocidadMovimiento * Time.deltaTime;
        // Sumar el offset al offset actual del material para mover el fondo
        material.mainTextureOffset += offset;
    }
}
