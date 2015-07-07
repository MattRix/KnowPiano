using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Teacher : MonoBehaviour 
{
	const float SPREADY = 1f;
	const float SCALEX = 10f;
	const float TIME_TO_SELECT = 0.5f;

	public Sprite barAsset;
	public Sprite noteAsset;
	public Sprite glowAsset;

	public List<SpriteRenderer> bars = new List<SpriteRenderer>();
//	public List<SpriteRenderer> notes = new List<SpriteRenderer>();

	public List<NoteGlow> glows = new List<NoteGlow>();

	internal SpriteRenderer note;

	public float MIDDLEY;

	public float noteEnergy = 0f;
	public int noteOffset = 0;
	public int baseNote = 0;

	public AudioClip middleCClip; 


	//https://github.com/keijiro/unity-midi-bridge
	// Use this for initialization
	void Start () 
	{
		this.transform.position = new Vector3(0,-5,0);

		for(int i = 0; i < 11; i++)
		{
			var barGO = new GameObject("Bar");
			barGO.transform.parent = transform;

			var bar = barGO.AddComponent<SpriteRenderer>();

			bar.sprite = barAsset;
			bar.transform.localPosition = new Vector3(0,i*SPREADY,0);
			bar.transform.localScale = new Vector3(SCALEX,1,1);

			if(i == 5) //middle C
			{
				bar.enabled = false;
				MIDDLEY = bar.transform.localPosition.y;
			}


			bars.Add(bar);
		}

		var noteGO = new GameObject("Note");
		noteGO.transform.parent = transform;

		note = noteGO.AddComponent<SpriteRenderer>();
		note.sprite = noteAsset;

		note.transform.localPosition = new Vector3(0,0,0);

		for(int i = 0; i<5; i++)
		{
			var glowGO = new GameObject("Glow");
			glowGO.transform.parent = transform;
			
			var glow = glowGO.AddComponent<NoteGlow>();

			glow.renderer = glowGO.AddComponent<SpriteRenderer>();

			glow.renderer.sprite = glowAsset;
			glow.transform.localPosition = new Vector3(0,i*SPREADY,0);
			glow.transform.localScale = new Vector3(SCALEX,1,1);

			glow.audioSource = glowGO.AddComponent<AudioSource>();
			glow.audioSource.clip = middleCClip;

			glow.renderer.enabled = false;

			glows.Add(glow);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		for(int i = 0; i<128; i++)
		{
			if(MidiInput.GetKeyUp(i))
			{
				foreach(var glow in glows)
				{
					if(glow.note == i)
					{
						glow.note = -1;
						glow.renderer.enabled = false;
					}
				}
//				Debug.Log("Releasing " + i + "!");
			}

			if(MidiInput.GetKeyDown(i))
			{
				foreach(var glow in glows)
				{
					if(glow.note == -1)
					{
						glow.note = i;
						glow.renderer.enabled = true;

						int offsetIndex = GetOffsetFromMiddleC(i); //60 is middleC

						glow.offsetFromMiddleC = offsetIndex;

						glow.transform.localPosition = new Vector3(0,MIDDLEY+(float)offsetIndex*SPREADY/2f,0);

						glow.audioSource.Stop();

						glow.audioSource.pitch = QNoteHelper.GetFrequencyForIndex(i) / QNoteHelper.GetFrequencyForIndex(60);

						glow.audioSource.Play();

						break;
					}
				}

//				Debug.Log("Pressing " + i + "!");
			}
		}

		if(noteEnergy <= 0)
		{
			noteEnergy = TIME_TO_SELECT;
			if(UnityEngine.Random.Range(0,1.0f) < 0.2f)
			{
				baseNote = UnityEngine.Random.Range(-10,10);
			}

			noteOffset = GetOffsetFromMiddleC(60 + baseNote + UnityEngine.Random.Range(-5,5));
			note.transform.localPosition = new Vector3(0,MIDDLEY+(float)noteOffset*SPREADY/2f,0);
		}

		bool wasNoteTouched = false;
		
		foreach(var glow in glows)
		{
			if(glow.note != -1 && glow.offsetFromMiddleC == noteOffset)
			{
				noteEnergy -= Time.deltaTime;
				wasNoteTouched = true;
			}
		}

		if(wasNoteTouched)
		{
			note.color = Color.green;
		}
		else
		{
			note.color = Color.white;
			noteEnergy = TIME_TO_SELECT;
		}

	}

	public int GetOffsetFromMiddleC(int noteIndex)
	{
		int middleC = GetRangeIndex(60); //60 is middle C
		return GetRangeIndex(noteIndex) - middleC;
	}

	public int GetRangeIndex(int noteIndex)
	{
		int[] range = QMusicScale.GetFullRangeNotes(0,QMusicScale.MAJOR);
		
		for(int r = 0; r<range.Length; r++)
		{
			if(range[r] >= noteIndex)
			{
				return r;
			}
		}

		return range.Length-1;
	}

}






























































