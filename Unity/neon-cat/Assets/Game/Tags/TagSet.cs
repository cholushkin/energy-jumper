using System;
using System.Collections.Generic;
using System.Diagnostics;

[Serializable]
public class TagSet
{
    [Serializable]
    public class Tag
    {
        public string Name;
        public string Value;
    }
    public List<Tag> Tags;

    public bool IsEmpty()
    {
        return (Tags == null || Tags.Count == 0);
    }

    public override string ToString()
    {
        if (Tags == null || Tags.Count == 0)
        {
            return "no tags";
        }
        var result = "";
        foreach (var tag in Tags)
        {
            if (string.IsNullOrEmpty(tag.Value))
                result += string.Format("[{0}]", tag.Name);
            else
                result += string.Format("[{0}:{1}]", tag.Name, tag.Value);
        }
        return result;
    }
}