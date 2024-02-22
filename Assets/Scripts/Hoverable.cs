using UnityEngine;
using TMPro;
using System;

public class Hoverable : MonoBehaviour
{
    public Material CustomHighlightMaterial;

    private Renderer _renderer;
    private Material _initialMaterial;
    
    float timer = 0.0f;
    public TextMeshProUGUI textSeconds;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        //textSeconds.gameObject.SetActive(false);
        _renderer = this.GetComponent<Renderer>();
        _initialMaterial = _renderer.material;
    }
    
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        if (IsSelected)
        {
            textSeconds.gameObject.SetActive(true);
            timer += Time.deltaTime;
            float seconds = timer%60;
            textSeconds.text = ""+Math.Round(seconds, 1);
        }
    }
    
    public bool IsSelected {get; private set;}
    
    public void OnEnter()
    {
        this.IsSelected = true;
        _renderer.material = CustomHighlightMaterial;
    }
    
    public void onExit()
    {
        this.IsSelected = false;
        _renderer.material = _initialMaterial;
    }
}