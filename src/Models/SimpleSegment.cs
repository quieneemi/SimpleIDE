using AvaloniaEdit.Document;

namespace SimpleWebStudio.Models;

public struct SimpleSegment : ISegment
{
    public int Offset { get; }
    public int Length { get; }
    public int EndOffset { get; }

    public SimpleSegment(ISegment completionSegment)
    {
        Offset = completionSegment.Offset;
        Length = completionSegment.Length;
        EndOffset = completionSegment.EndOffset;

        if (Offset == 0) return;

        Offset -= 1;
        Length += 1;
    }
}