using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffects : MonoBehaviour //me, www.youtube.com/watch?v=LDeIGXwv8tc
{
    [SerializeField] private float parallaxMultiplier;

    private Transform cameraTransform;
    private Vector3 previousCameraPosition;
    private float spriteWidth, startPosition;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        previousCameraPosition = cameraTransform.position;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        startPosition = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaX = (cameraTransform.position.x - previousCameraPosition.x) * parallaxMultiplier;
        //float moveAmount = cameraTransform.position.x * (1 - parallaxMultiplier);
        transform.Translate(new Vector3(deltaX, 0, 0));
        previousCameraPosition = cameraTransform.position;

        /*if(moveAmount > startPosition + spriteWidth)
        {
            transform.Translate(new Vector3(spriteWidth, 0, 0));
            startPosition += spriteWidth;
        }
        else if (moveAmount < startPosition - spriteWidth)
        {
            transform.Translate(new Vector3(-spriteWidth, 0, 0));
            startPosition -= spriteWidth;
        }*/
    }
}
