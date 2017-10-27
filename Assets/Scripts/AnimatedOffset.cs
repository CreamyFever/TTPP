using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedOffset : MonoBehaviour
{
    public float scrollSpeedX = 0.1f;
    public float scrollSpeedY = 0.0f;
    public Renderer ren;
    
	void Start ()
    {
        ren = GetComponent<Renderer>();
	}
	
	void Update ()
    {
        float offsetX = Time.time * scrollSpeedX;
        float offsetY = Time.time * scrollSpeedY;
        Vector2 offset = new Vector2(offsetX, offsetY);

        ren.material.SetTextureOffset("_MainTex", offset);
	}
}
