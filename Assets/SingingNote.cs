using System;

[Serializable]
public class SingingNote
{
    public int midiNote;
    public float startTime;
    public float duration;
    public string lyric;

    public SingingNote(
        int midiNote,
        float startTime,
        float duration,
        string lyric = "")
    {
        this.midiNote = midiNote;
        this.startTime = startTime;
        this.duration = duration;
        this.lyric = lyric;
    }
}
