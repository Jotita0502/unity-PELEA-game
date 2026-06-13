using UnityEngine;

public class Personaje : MonoBehaviour
{
    private float velMovimiento = 5.0f;
    private float velRotacion = 200.0f;

    private float ejeX, ejeY; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ejeX = Input.GetAxis("Horizontal");
        ejeY = Input.GetAxis("Vertical");

        transform.Rotate(0, ejeX * Time.deltaTime * velRotacion, 0);
        transform.Translate(0, 0, ejeY * Time.deltaTime * velMovimiento);
    }
}
