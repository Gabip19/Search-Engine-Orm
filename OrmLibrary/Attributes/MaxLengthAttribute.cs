namespace OrmLibrary.Attributes;

public class MaxLengthAttribute : AbstractColumnAttribute
{
    public uint Length { get; set; }
    public MaxLengthAttribute(uint length) => Length = length;
}