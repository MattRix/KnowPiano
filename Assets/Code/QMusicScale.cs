using System;
using System.Text;
using UnityEngine;

public class QMusicScale
{
	//basic scale intervals (they should always add up to 12, in theory)
	public static int[] MAJOR = {2,2,1,2,2,2,1};
	public static int[] NATURAL_MINOR = {2,1,2,2,1,2,2};
	public static int[] MELODIC_MINORA = {2,1,2,2,2,2,1};
	public static int[] MELODIC_MINORB = {2,2,1,2,2,1,2};
	public static int[] CHROMATIC = {1,1,1,1,1,1,1,1,1,1,1,1};
	public static int[] WHOLE_TONE = {2,2,2,2,2,2};
	public static int[] PENTATONIC_MAJOR = {2,3,2,2,3};
	public static int[] PENTATONIC_MINOR = {2,3,2,3,2};
	public static int[] PENTATONIC_ALT = {2,2,3,2,3};
	public static int[] OCTAVES = {12};
	public static int[] BLUES = {3,2,1,1,3,2};
	
	public static int[] GetSingleScaleNotes(int noteIndex,int[] intervals)
	{
		int[] result = new int[intervals.Length];
		result[0] = noteIndex;
		
		for(int i = 0; i<intervals.Length; i++)
		{
			noteIndex += intervals[i];
			result[i+1] = noteIndex;
		}
		return result;
	}
	
	public static int[] GetFullRangeNotes(int noteIndex,int[] intervals)
	{
		//check if the total is a multiple of 12, if not, add an interval to make it add up to 12!
		int total = 0;
		
		for (int i = 0; i<intervals.Length; i++)
		{
			total += intervals[i];	
		}
		
		if(total % 12 != 0)
		{
			Array.Resize(ref intervals,intervals.Length+1);
	
			intervals[intervals.Length-1] = 12 - (total % 12);
		}
		
		int[] allNotes = new int[200];
		
		int totalAdded = 0;
		
		noteIndex = noteIndex % 12; //bring it down to the lowest octave
		
		allNotes[0] = noteIndex;
		totalAdded++;
		
		int currentInterval = 0;
		
		while(noteIndex < 128) //the top note will be 128
		{
			noteIndex += intervals[currentInterval % intervals.Length];
			allNotes[totalAdded] = noteIndex;
			totalAdded++;
			currentInterval++;
		}

		Array.Resize(ref allNotes,totalAdded);
		
		return allNotes;
	}
	
	public static int GetRelativeNoteIndex(int[] fullRangeNotes, int baseNoteIndex, int delta)
	{
		for(int r = 0; r<fullRangeNotes.Length; r++)
		{
			if(fullRangeNotes[r] == baseNoteIndex)
			{
				return fullRangeNotes[r+delta];
			}
		}
		
		return fullRangeNotes[0];
	}

	public static void LogScaleNoteNames(int rootNoteIndex, int[] scaleIntervals, int count, int divider)
	{

		int[] fullRangeNotes = GetFullRangeNotes(rootNoteIndex, scaleIntervals);

//		Debug.Log("root is " + QNoteHelper.GetNameForIndex(rootNoteIndex));
//
//		Debug.Log("now root is " + QNoteHelper.GetNameForIndex(rootNoteIndex));
//
//		Debug.Log("root note index " + rootNoteIndex);

		StringBuilder sb = new StringBuilder();
		for(int c = 0; c<count; c++)
		{
			int relativeNoteIndex = GetRelativeNoteIndex(fullRangeNotes,rootNoteIndex,c);

			if(c != 0)
			{
				if(c % divider == 0) 
				{
					sb.Append("|");
				}
				else 
				{
					sb.Append(',');
				}
			}

			sb.Append(QNoteHelper.GetNameForIndex(relativeNoteIndex));
		}
		Debug.Log(sb.ToString());
	}
}
