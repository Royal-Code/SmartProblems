using System.Text;

namespace RoyalCode.SmartProblems.Conversions;

internal sealed class DefaultPointerParser : IPointerParser
{
    /// <inheritdoc />
    public string? PointerToProperty(string? pointer)
    {
        if (pointer == null)
            return null;

        int pointerLength = pointer.Length;
        if (pointerLength == 0)
            return pointer;

        int startIndex = 0;
        if (pointer[startIndex] == '#')
        {
            if (pointerLength == 1)
                return string.Empty;

            startIndex = 1;
        }
        if (pointer[startIndex] == '/')
        {
            startIndex++;
        }

        if (startIndex == pointerLength)
            return string.Empty;

        StringBuilder propertyBuffer = new(pointerLength);

        int slash = 0;
        int chars = 0;
        bool isDigit = true;

        for (int i = startIndex, j = 0; i < pointerLength; i++, j++)
        {
            char c = pointer[i];
            if (c == '/')
            {
                if (chars is not 0)
                {
                    startIndex = slash is 0 ? startIndex : slash + 1;
                    if (isDigit)
                    {
                        propertyBuffer.Append('[')
                            .Append(pointer, startIndex, chars)
                            .Append(']');
                    }
                    else
                    {
                        if (slash is not 0)
                            propertyBuffer.Append('.');

                        propertyBuffer.Append(pointer, startIndex, chars);
                    }
                }

                slash = i;
                isDigit = true;
                chars = 0;
            }
            else
            {
                chars++;
                isDigit = isDigit && char.IsDigit(c);
            }
        }

        if (chars is not 0)
        {
            startIndex = slash is 0 ? startIndex : slash + 1;
            if (isDigit)
            {
                propertyBuffer.Append('[')
                    .Append(pointer, startIndex, chars)
                    .Append(']');
            }
            else
            {
                if (slash is not 0)
                    propertyBuffer.Append('.');

                propertyBuffer.Append(pointer, startIndex, chars);
            }
        }

        return propertyBuffer.ToString();
    }

    /// <inheritdoc />
    public string? PropertyToPointer(string? property)
    {
        if (property == null)
            return null;

        int pointerLength = property.Length;
        if (pointerLength == 0)
            return "#/";

        StringBuilder pointerBuffer = new(pointerLength);
        pointerBuffer.Append("#/");

        bool lastIsSlash = true;

        for (int i = 0; i < pointerLength; i++)
        {
            char c = property[i];
            if (c == '.' || c == '[' || c == ']')
            {
                if (!lastIsSlash)
                    pointerBuffer.Append('/');

                lastIsSlash = true;
            }
            else
            {
                pointerBuffer.Append(c);
                lastIsSlash = false;
            }
        }

        if (lastIsSlash)
            pointerBuffer.Length--;

        return pointerBuffer.ToString();
    }
}
