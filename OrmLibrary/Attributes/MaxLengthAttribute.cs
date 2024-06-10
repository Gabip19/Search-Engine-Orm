namespace OrmLibrary.Attributes;

public class MaxLengthAttribute : AbstractColumnAttribute
{
    public int Length { get; set; }
    public MaxLengthAttribute(int length) => Length = length;
}