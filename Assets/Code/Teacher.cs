using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Teacher : MonoBehaviour 
{
	const float SPREADY = 1f;
	const float SCALEX = 10f;
	const float TIME_TO_SELECT = 0.1f;
	const float SPREADX = 1.5f;
	const int TOTAL_TARGETS = 8;
	const int FUTURE_TARGETS = 4;

	public Sprite barAsset;
	public Sprite noteAsset;
	public Sprite glowAsset;

	public List<SpriteRenderer> bars = new List<SpriteRenderer>();
//	public List<SpriteRenderer> notes = new List<SpriteRenderer>();

	public List<NoteGlow> glows = new List<NoteGlow>();

	public List<NoteTarget> targets = new List<NoteTarget>();

	public float MIDDLEY;

	public AudioClip middleCClip; 

	public int baseNote = 0;
	public float scrollX = 0;
	public float scrollXTarget = 0;
	public int targetIndex = 0;

	public bool hasPressedDown = false;


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

	NoteTarget CreateTarget()
	{
		var targetGO = new GameObject("NoteTarget");
		targetGO.transform.parent = transform;

		var target = targetGO.AddComponent<NoteTarget>();

		target.renderer = targetGO.AddComponent<SpriteRenderer>();
		target.renderer.sprite = noteAsset;
		
		target.transform.localPosition = new Vector3(0,0,0);

		targets.Add(target);

		if(UnityEngine.Random.Range(0,1.0f) < 0.2f)
		{
			baseNote = UnityEngine.Random.Range(-10,10);
		}

		target.offsetFromMiddleC = GetOffsetFromMiddleC(60 + baseNote + UnityEngine.Random.Range(-5,5));

		target.baseX = targetIndex * SPREADX;
		target.baseY = MIDDLEY+(float)target.offsetFromMiddleC*SPREADY/2f;

		targetIndex++;

		target.energy = TIME_TO_SELECT;

		target.renderer.color = Color.grey;

		return target;
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
				hasPressedDown = true;

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

		NoteTarget currentTarget = null;
		int futureTargets = 0;

		for(int r = 0; r<targets.Count; r++)
		{
			var target = targets[r];
			//remove far left targets here

			if(targets.Count > TOTAL_TARGETS)
			{
				targets.Remove(target);
				target.Remove();
				r--;
			}
			else 
			{
				if(!target.hasBeenPlayed)
				{
					if(currentTarget == null)
					{
						currentTarget = target;
					}
					else 
					{
						futureTargets++;
					}
				}
			}
		}

		if(currentTarget == null)
		{
			currentTarget = CreateTarget();
		}

		while(futureTargets < FUTURE_TARGETS)
		{
			CreateTarget();
			futureTargets++;
		}

		bool wasNoteTouched = false;

		if(hasPressedDown) //must press before we start to touch the note
		{
			foreach(var glow in glows)
			{
				if(glow.note != -1 && glow.offsetFromMiddleC == currentTarget.offsetFromMiddleC)
				{
					currentTarget.energy -= Time.deltaTime;
					wasNoteTouched = true;
				}
			}
		}

		if(wasNoteTouched)
		{
			currentTarget.renderer.color = Color.green;
		}
		else
		{
			currentTarget.renderer.color = Color.white;
			currentTarget.energy = TIME_TO_SELECT;
		}


		if(currentTarget.energy <= 0) //played the note!
		{
			currentTarget.hasBeenPlayed = true;
			scrollXTarget -= SPREADX; //scroll timeline
			hasPressedDown = false;
		}

		scrollX += (scrollXTarget-scrollX) * 0.1f; //ease towards scroll target

		foreach(var target in targets)
		{
			target.transform.localPosition = new Vector3(scrollX + target.baseX, target.baseY,0);
		}
	}

	static public int GetOffsetFromMiddleC(int noteIndex)
	{
		int middleC = GetRangeIndex(60); //60 is middle C
		return GetRangeIndex(noteIndex) - middleC;
	}

	static public int GetRangeIndex(int noteIndex)
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






























































