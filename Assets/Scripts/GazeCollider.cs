// using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GazeCollider: MonoBehaviour
{
    public Material default_color;
    public Material triggered_color;
    
    Renderer render;
    bool is_inside = false;
    float timer = 0.0f;
    public TextMeshPro text_seconds;
    // Start is called before the first frame update
    void Start()
    {
        text_seconds.gameObject.SetActive(false);
        render = GetComponent<Renderer>();
        render.enabled = true;
        render.sharedMaterial = triggered_color;
    }

    // Update is called once per frame
    void Update()
    {
        if (is_inside == true)
        {
            text_seconds.gameObject.SetActive(true);
            timer += Time.deltaTime;
            float seconds = timer % 60;
            text_seconds.text = ""+ Math.Round(seconds, 1);
        }
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        render.material = triggered_color;
        is_inside = true;    
    }
    
    private void OnTriggerExit(Collider other) 
    {
        render.material= default_color;
        is_inside = false;    
    }
}
