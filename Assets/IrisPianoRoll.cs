using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class IrisPianoRoll : VisualElement
{
    private const int LowestMidiNote = 36;
    private const int HighestMidiNote = 84;

    private const float PixelsPerBeat = 120f;
    private const float PixelsPerNote = 24f;

    private readonly List<SingingNote> notes = new();

    public IrisPianoRoll()
    {
        style.flexGrow = 1;

        pickingMode = PickingMode.Position;

        generateVisualContent += DrawPianoRoll;

        RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    private void DrawPianoRoll(MeshGenerationContext context)
    {
        var painter = context.painter2D;

        float width = contentRect.width;
        float height = contentRect.height;

        if (width <= 0 || height <= 0)
            return;

        // 背景
        painter.fillColor =
            new Color(0.055f, 0.055f, 0.07f);

        painter.BeginPath();

        painter.MoveTo(new Vector2(0, 0));
        painter.LineTo(new Vector2(width, 0));
        painter.LineTo(new Vector2(width, height));
        painter.LineTo(new Vector2(0, height));

        painter.ClosePath();
        painter.Fill();

        // 横方向：音程のグリッド
        for (int midi = LowestMidiNote;
             midi <= HighestMidiNote;
             midi++)
        {
            float y = MidiToY(midi);

            bool blackKey = IsBlackKey(midi);

            painter.lineWidth =
                blackKey ? 1f : 2f;

            painter.strokeColor =
                blackKey
                    ? new Color(0.13f, 0.13f, 0.16f)
                    : new Color(0.22f, 0.22f, 0.26f);

            painter.BeginPath();

            painter.MoveTo(
                new Vector2(0, y)
            );

            painter.LineTo(
                new Vector2(width, y)
            );

            painter.Stroke();
        }

        // 縦方向：拍のグリッド
        int beatCount =
            Mathf.CeilToInt(
                width / PixelsPerBeat
            );

        for (int beat = 0;
             beat <= beatCount;
             beat++)
        {
            float x =
                beat * PixelsPerBeat;

            bool isBar =
                beat % 4 == 0;

            painter.lineWidth =
                isBar ? 2f : 1f;

            painter.strokeColor =
                isBar
                    ? new Color(0.30f, 0.30f, 0.35f)
                    : new Color(0.15f, 0.15f, 0.18f);

            painter.BeginPath();

            painter.MoveTo(
                new Vector2(x, 0)
            );

            painter.LineTo(
                new Vector2(x, height)
            );

            painter.Stroke();
        }

        // 音符
        foreach (var note in notes)
        {
            DrawNote(painter, note);
        }
    }

    private void DrawNote(
        Painter2D painter,
        SingingNote note)
    {
        float x =
            note.startTime * PixelsPerBeat;

        float y =
            MidiToY(note.midiNote);

        float width =
            note.duration * PixelsPerBeat;

        float height =
            PixelsPerNote - 3f;

        painter.fillColor =
            new Color(0.55f, 0.75f, 1.0f);

        painter.strokeColor =
            new Color(0.75f, 0.88f, 1.0f);

        painter.lineWidth = 1.5f;

        painter.BeginPath();

        painter.MoveTo(
            new Vector2(x, y + 2f)
        );

        painter.LineTo(
            new Vector2(x + width, y + 2f)
        );

        painter.LineTo(
            new Vector2(x + width, y + height)
        );

        painter.LineTo(
            new Vector2(x, y + height)
        );

        painter.ClosePath();

        painter.Fill();
        painter.Stroke();
    }

    private void OnPointerDown(
        PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        Vector2 position =
            evt.localPosition;

        int midiNote =
            YToMidi(position.y);

        float time =
            XToTime(position.x);

        time =
            SnapTime(time, 0.25f);

        float duration = 1f;

        var note =
            new SingingNote(
                midiNote,
                time,
                duration
            );

        notes.Add(note);

        MarkDirtyRepaint();

        Debug.Log(
            $"Iris Piano Roll: " +
            $"MIDI={midiNote}, " +
            $"Time={time:0.00}, " +
            $"Duration={duration:0.00}"
        );
    }

    private float MidiToY(int midiNote)
    {
        return
            (HighestMidiNote - midiNote)
            * PixelsPerNote;
    }

    private int YToMidi(float y)
    {
        int midi =
            HighestMidiNote -
            Mathf.FloorToInt(
                y / PixelsPerNote
            );

        return Mathf.Clamp(
            midi,
            LowestMidiNote,
            HighestMidiNote
        );
    }

    private float XToTime(float x)
    {
        return Mathf.Max(
            0f,
            x / PixelsPerBeat
        );
    }

    private float SnapTime(
        float time,
        float interval)
    {
        return Mathf.Round(
            time / interval
        ) * interval;
    }

    private bool IsBlackKey(int midi)
    {
        int pitchClass = midi % 12;

        return pitchClass == 1 ||
               pitchClass == 3 ||
               pitchClass == 6 ||
               pitchClass == 8 ||
               pitchClass == 10;
    }

    public IReadOnlyList<SingingNote> Notes
        => notes;
}