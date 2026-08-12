using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace LB.TweenHelper.Automation.Editor
{
    public sealed class CanonicalJsonWriter
    {
        private enum ContainerKind
        {
            Object,
            Array
        }

        private sealed class ContainerState
        {
            public ContainerKind Kind;
            public int ValueCount;
            public bool ExpectsValue;
        }

        private readonly StringBuilder _builder = new StringBuilder();
        private readonly Stack<ContainerState> _containers = new Stack<ContainerState>();
        private bool _hasRootValue;

        public void BeginObject()
        {
            BeforeValue();
            _builder.Append('{');
            _containers.Push(new ContainerState { Kind = ContainerKind.Object });
        }

        public void EndObject()
        {
            ContainerState state = RequireContainer(ContainerKind.Object);
            if (state.ExpectsValue) throw new InvalidOperationException("A canonical JSON object property is missing its value.");
            _containers.Pop();
            _builder.Append('}');
        }

        public void BeginArray()
        {
            BeforeValue();
            _builder.Append('[');
            _containers.Push(new ContainerState { Kind = ContainerKind.Array });
        }

        public void EndArray()
        {
            RequireContainer(ContainerKind.Array);
            _containers.Pop();
            _builder.Append(']');
        }

        public void WritePropertyName(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            ContainerState state = RequireContainer(ContainerKind.Object);
            if (state.ExpectsValue) throw new InvalidOperationException("Write the current canonical JSON property value before starting another property.");
            if (state.ValueCount > 0) _builder.Append(',');
            WriteEscapedString(name);
            _builder.Append(':');
            state.ExpectsValue = true;
        }

        public void WriteString(string value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }

            BeforeValue();
            WriteEscapedString(value.Normalize(NormalizationForm.FormC));
        }

        public void WriteBoolean(bool value)
        {
            BeforeValue();
            _builder.Append(value ? "true" : "false");
        }

        public void WriteInt32(int value)
        {
            BeforeValue();
            _builder.Append(value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteInt64(long value)
        {
            BeforeValue();
            _builder.Append(value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteSingle(float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) throw new ArgumentOutOfRangeException(nameof(value), "Canonical JSON rejects NaN and infinity.");
            BeforeValue();
            _builder.Append(value == 0f ? "0" : value.ToString("R", CultureInfo.InvariantCulture));
        }

        public void WriteDouble(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) throw new ArgumentOutOfRangeException(nameof(value), "Canonical JSON rejects NaN and infinity.");
            BeforeValue();
            _builder.Append(value == 0d ? "0" : value.ToString("R", CultureInfo.InvariantCulture));
        }

        public void WriteNull()
        {
            BeforeValue();
            _builder.Append("null");
        }

        public string Complete()
        {
            if (_containers.Count > 0) throw new InvalidOperationException("Canonical JSON contains an unclosed container.");
            if (!_hasRootValue) throw new InvalidOperationException("Canonical JSON has no root value.");
            return _builder.ToString();
        }

        private void BeforeValue()
        {
            if (_containers.Count == 0)
            {
                if (_hasRootValue) throw new InvalidOperationException("Canonical JSON may contain only one root value.");
                _hasRootValue = true;
                return;
            }

            ContainerState state = _containers.Peek();
            if (state.Kind == ContainerKind.Array)
            {
                if (state.ValueCount > 0) _builder.Append(',');
                state.ValueCount++;
                return;
            }

            if (!state.ExpectsValue) throw new InvalidOperationException("Write a canonical JSON property name before its value.");
            state.ExpectsValue = false;
            state.ValueCount++;
        }

        private ContainerState RequireContainer(ContainerKind kind)
        {
            if (_containers.Count == 0 || _containers.Peek().Kind != kind)
            {
                throw new InvalidOperationException($"Canonical JSON is not currently inside a {kind.ToString().ToLowerInvariant()}.");
            }

            return _containers.Peek();
        }

        private void WriteEscapedString(string value)
        {
            _builder.Append('"');
            for (int i = 0; i < value.Length; i++)
            {
                char character = value[i];
                switch (character)
                {
                    case '"':
                        _builder.Append("\\\"");
                        break;
                    case '\\':
                        _builder.Append("\\\\");
                        break;
                    case '\b':
                        _builder.Append("\\b");
                        break;
                    case '\f':
                        _builder.Append("\\f");
                        break;
                    case '\n':
                        _builder.Append("\\n");
                        break;
                    case '\r':
                        _builder.Append("\\r");
                        break;
                    case '\t':
                        _builder.Append("\\t");
                        break;
                    default:
                        if (character < 0x20)
                        {
                            _builder.Append("\\u");
                            _builder.Append(((int)character).ToString("x4", CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            _builder.Append(character);
                        }
                        break;
                }
            }
            _builder.Append('"');
        }
    }

    public static class CanonicalHash
    {
        public static string Compute(Action<CanonicalJsonWriter> write)
        {
            if (write == null) throw new ArgumentNullException(nameof(write));
            var writer = new CanonicalJsonWriter();
            write(writer);
            byte[] bytes = Encoding.UTF8.GetBytes(writer.Complete());
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] digest = sha256.ComputeHash(bytes);
                var hex = new StringBuilder(digest.Length * 2);
                for (int i = 0; i < digest.Length; i++) hex.Append(digest[i].ToString("x2", CultureInfo.InvariantCulture));
                return "sha256:" + hex;
            }
        }
    }
}
