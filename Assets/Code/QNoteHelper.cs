using UnityEngine;
using System;

public class QNoteHelper
{
	private static float LOG2E = 1.442695040888963387f;
	
	public static String[] LETTERS = {"C","C#","D","D#","E","F","F#","G","G#","A","A#","B"};
	
	public static int GetLetterIndexForNoteName(String noteName)
	{
		for(int i = 0; i<LETTERS.Length; i++)
		{
			if(LETTERS[i] == noteName)
			{
				return i;	
			}
		}
		
		return -1;
	}
	
	public static int GetIndexForFrequency(float frequency)
	{
		return (int) Mathf.Round(69.0f + 12.0f * Mathf.Log(frequency/440.0f) * LOG2E);
	}

	//TODO: make this use chars instead of strings everywhere (I was very new to C# when I wrote this)
	public static int GetIndexForName(String noteName)  //A#0 or A0, goes as low as C0-1 (the -1th octave)
	{
		String lastChar = (String) noteName[noteName.Length-1].ToString();
		int parseResult = 0;
		Boolean hasOctave = int.TryParse(lastChar,out parseResult);
		int octave = hasOctave ? 0 : -1;
		int letterIndex = GetLetterIndexForNoteName(noteName[0].ToString());
		
		int hasNoOctaveInt = hasOctave ? 0 : 1;
		
		if(noteName.Length >= (3-hasNoOctaveInt))
		{
			String char2 = noteName[1].ToString ();
			if(char2 == "#")
			{
				letterIndex++;
			}
			else if(char2 == "b")
			{
				letterIndex--;
			}
			else if(hasOctave && char2 == "-") //-1th octave
			{
				octave = -1;
			}
			
			if (hasOctave && noteName[2].ToString() == "-")
			{
				octave = -1;
			}
		}
		
		if(octave == 0) //if it's not -1, figure out what it is
		{
			octave = int.Parse(lastChar);
		}
		
		return (octave+1)*12 + letterIndex;
	} 
	
	//FREQUENCY
	
	public static float GetFrequencyForIndex(int index)
	{
		return 440.0f * (float)Mathf.Pow(2.0f,(index-69.0f)/12.0f);
	}
	
	public static float GetFrequencyForName(String noteName)
	{
		return GetFrequencyForIndex(GetIndexForName(noteName));
	}
	
	//NAME

	public static String GetNameForIndex(int index)
	{
		int octave = (int) (Mathf.Floor(index/12.0f)-1.0f);
		
		int letterIndex = (index+12000)%12;
		return LETTERS[letterIndex] + octave.ToString(); 
	}
	
	
	public static String GetNameForFrequency(float frequency)
	{
		return GetNameForIndex(GetIndexForFrequency(frequency));
	}
	
	public static String Romanize(int value)
	{
		value = value % 10;
		switch(value)
		{
			case 0: return "0"; 
			case 1: return "I"; 
			case 2: return "II"; 
			case 3: return "III"; 
			case 4: return "IV"; 
			case 5: return "V"; 
			case 6: return "VI"; 
			case 7: return "VII"; 
			case 8: return "VIII"; 
			case 9: return "IX";
			case 10: return "X";
		}
		return "X";
	}
	
	static float toneStep = Mathf.Pow(2.0f, 1.0f / 12.0f);
	
	public static int GetNoteIndexNearestToFrequency(float frequency)
	{
		float result = Mathf.Log(frequency/440.0f, toneStep) + 69.0f;
		
		return (int) Mathf.Round(result);
	}
}

