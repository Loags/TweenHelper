using System;
using TMPro;
using UnityEngine;

namespace LB.TweenHelper
{
    internal sealed class TMPTextElementMap
    {
        private readonly int _characterCount;
        private readonly int[] _visibleCharacterIndices;
        private readonly int[] _characterToWord;
        private readonly int[] _characterToLine;
        private readonly int[] _visibleToWordGroup;
        private readonly int[] _visibleToLineGroup;
        private readonly int[] _wordGroupEnds;
        private readonly int[] _lineGroupEnds;

        public TMPTextElementMap(TMP_TextInfo textInfo)
        {
            if (textInfo == null) throw new ArgumentNullException(nameof(textInfo));

            _characterCount = textInfo.characterCount;
            _characterToWord = CreateMembership(_characterCount);
            _characterToLine = CreateMembership(_characterCount);
            PopulateWordMembership(textInfo);
            PopulateLineMembership(textInfo);

            var visible = new int[_characterCount];
            int visibleCount = 0;
            for (int i = 0; i < _characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible) continue;
                visible[visibleCount++] = i;
            }

            _visibleCharacterIndices = new int[visibleCount];
            Array.Copy(visible, _visibleCharacterIndices, visibleCount);
            BuildVisibleGroups(textInfo, _characterToWord, true, out _visibleToWordGroup, out _wordGroupEnds);
            BuildVisibleGroups(textInfo, _characterToLine, false, out _visibleToLineGroup, out _lineGroupEnds);
        }

        public int CharacterCount => _characterCount;
        public int VisibleCharacterCount => _visibleCharacterIndices.Length;
        public int[] VisibleCharacterIndices => _visibleCharacterIndices;
        public int[] CharacterToWordMembership => _characterToWord;
        public int[] CharacterToLineMembership => _characterToLine;

        public int GetVisibleGroupCount(TextAnimationUnit unit)
        {
            switch (unit)
            {
                case TextAnimationUnit.Character: return _visibleCharacterIndices.Length;
                case TextAnimationUnit.Word: return _wordGroupEnds.Length;
                case TextAnimationUnit.Line: return _lineGroupEnds.Length;
                default: throw new ArgumentOutOfRangeException(nameof(unit), unit, "Unknown text animation unit.");
            }
        }

        public int GetVisibleGroupIndex(TextAnimationUnit unit, int visibleCharacterOrder)
        {
            if (visibleCharacterOrder < 0 || visibleCharacterOrder >= _visibleCharacterIndices.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(visibleCharacterOrder));
            }

            switch (unit)
            {
                case TextAnimationUnit.Character: return visibleCharacterOrder;
                case TextAnimationUnit.Word: return _visibleToWordGroup[visibleCharacterOrder];
                case TextAnimationUnit.Line: return _visibleToLineGroup[visibleCharacterOrder];
                default: throw new ArgumentOutOfRangeException(nameof(unit), unit, "Unknown text animation unit.");
            }
        }

        public int GetVisibilityGroupCount(TextAnimationUnit unit)
        {
            switch (unit)
            {
                case TextAnimationUnit.Character: return _characterCount;
                case TextAnimationUnit.Word: return _wordGroupEnds.Length;
                case TextAnimationUnit.Line: return _lineGroupEnds.Length;
                default: throw new ArgumentOutOfRangeException(nameof(unit), unit, "Unknown text animation unit.");
            }
        }

        public int GetVisibilityBoundary(TextAnimationUnit unit, int visibleGroupCount)
        {
            int groupCount = GetVisibilityGroupCount(unit);
            int count = Mathf.Clamp(visibleGroupCount, 0, groupCount);
            if (count <= 0) return 0;
            if (unit == TextAnimationUnit.Character) return count;

            int[] groupEnds = unit == TextAnimationUnit.Word ? _wordGroupEnds : _lineGroupEnds;
            return count >= groupEnds.Length ? _characterCount : groupEnds[count - 1];
        }

        public int CountVisibilityGroupsBefore(TextAnimationUnit unit, int characterBoundary)
        {
            if (characterBoundary <= 0) return 0;
            if (unit == TextAnimationUnit.Character) return Mathf.Clamp(characterBoundary, 0, _characterCount);

            int[] groupEnds = unit == TextAnimationUnit.Word ? _wordGroupEnds : _lineGroupEnds;
            if (characterBoundary == int.MaxValue) return groupEnds.Length;

            int count = 0;
            while (count < groupEnds.Length && groupEnds[count] <= characterBoundary) count++;
            return count;
        }

        private static int[] CreateMembership(int count)
        {
            var membership = new int[count];
            for (int i = 0; i < membership.Length; i++) membership[i] = -1;
            return membership;
        }

        private void PopulateWordMembership(TMP_TextInfo textInfo)
        {
            for (int wordIndex = 0; wordIndex < textInfo.wordCount; wordIndex++)
            {
                TMP_WordInfo word = textInfo.wordInfo[wordIndex];
                int end = Mathf.Min(_characterCount, word.firstCharacterIndex + word.characterCount);
                for (int characterIndex = Mathf.Max(0, word.firstCharacterIndex); characterIndex < end; characterIndex++)
                {
                    _characterToWord[characterIndex] = wordIndex;
                }
            }
        }

        private void PopulateLineMembership(TMP_TextInfo textInfo)
        {
            for (int lineIndex = 0; lineIndex < textInfo.lineCount; lineIndex++)
            {
                TMP_LineInfo line = textInfo.lineInfo[lineIndex];
                int start = Mathf.Clamp(line.firstCharacterIndex, 0, _characterCount);
                int end = Mathf.Clamp(line.lastCharacterIndex + 1, start, _characterCount);
                for (int characterIndex = start; characterIndex < end; characterIndex++)
                {
                    _characterToLine[characterIndex] = lineIndex;
                }
            }
        }

        private void BuildVisibleGroups(TMP_TextInfo textInfo, int[] membership, bool wordGroups, out int[] visibleToGroup, out int[] groupEnds)
        {
            visibleToGroup = new int[_visibleCharacterIndices.Length];
            var ends = new int[_visibleCharacterIndices.Length];
            int groupCount = 0;
            int previousMembership = int.MinValue;

            for (int visibleOrder = 0; visibleOrder < _visibleCharacterIndices.Length; visibleOrder++)
            {
                int characterIndex = _visibleCharacterIndices[visibleOrder];
                int currentMembership = membership[characterIndex];
                if (currentMembership < 0) currentMembership = -characterIndex - 2;

                if (visibleOrder == 0 || currentMembership != previousMembership)
                {
                    ends[groupCount] = ResolveGroupEnd(textInfo, characterIndex, currentMembership, wordGroups);
                    groupCount++;
                    previousMembership = currentMembership;
                }
                else
                {
                    ends[groupCount - 1] = Mathf.Max(ends[groupCount - 1], characterIndex + 1);
                }

                visibleToGroup[visibleOrder] = groupCount - 1;
            }

            groupEnds = new int[groupCount];
            Array.Copy(ends, groupEnds, groupCount);
        }

        private int ResolveGroupEnd(TMP_TextInfo textInfo, int characterIndex, int membership, bool wordGroup)
        {
            if (membership < 0) return characterIndex + 1;
            if (wordGroup)
            {
                TMP_WordInfo word = textInfo.wordInfo[membership];
                return Mathf.Clamp(word.firstCharacterIndex + word.characterCount, 0, _characterCount);
            }

            TMP_LineInfo line = textInfo.lineInfo[membership];
            return Mathf.Clamp(line.lastCharacterIndex + 1, 0, _characterCount);
        }
    }
}
