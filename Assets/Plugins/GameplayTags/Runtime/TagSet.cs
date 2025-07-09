using System;

namespace GameplayTags.Runtime
{
    public readonly struct TagSet
    {
        public readonly Tag[] Tags;
        public bool IsEmpty => Tags.Length == 0;
        
        public TagSet(string[] tagNames)
        {
            Tags = new Tag[tagNames.Length];
            for (var i = 0; i < tagNames.Length; i++)
            {
                Tags[i] = new Tag(tagNames[i]);
            }
        }
        
        public TagSet(params Tag[] tags)
        {
            Tags = tags?? Array.Empty<Tag>();
        }
        
        public bool HasTag(Tag tag)
        {
            foreach (var t in Tags)
            {
                if (t.HasTag(tag)) return true;
            }
            return false;
        }
        
        public bool HasAllTags(TagSet other)
        {
            return HasAllTags(other.Tags);
        }
        
        public bool HasAllTags(params Tag[] tags)
        {
            foreach (var tag in tags)
            {
                if (!HasTag(tag)) return false;
            }

            return true;
        }
        
        public bool HasAnyTags(TagSet other)
        {
            return HasAnyTags(other.Tags);
        }

        public bool HasAnyTags(params Tag[] tags)
        {
            foreach (var tag in tags)
            {
                if (HasTag(tag)) return true;
            }

            return false;
        }
        
        public bool HasNoneTags(TagSet other)
        {
            return HasNoneTags(other.Tags);
        }
        
        public bool HasNoneTags(params Tag[] tags)
        {
            foreach (var tag in tags)
            {
                if (HasTag(tag)) return false;
            }
            return true;
        }
    }
}