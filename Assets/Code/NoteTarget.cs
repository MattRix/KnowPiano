using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class NoteTarget : MonoBehaviour {

	public SpriteRenderer renderer;
	public int offsetFromMiddleC = 0;
	public float energy = 0f;
	public bool hasBeenPlayed = false;
	public float baseX = 0;
	public float baseY = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void MoveX (float deltaX)
	{
		var pos = transform.localPosition;
	}

	public void Remove ()
	{
		GameObject.Destroy(gameObject);
	}
}
