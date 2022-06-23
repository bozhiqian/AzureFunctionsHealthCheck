using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DreamCloud.Functions.HealthCheck;

internal class HealthCheckDataLogValue : IReadOnlyList<KeyValuePair<string, object>>
{
    private readonly string _name;
    private readonly List<KeyValuePair<string, object>> _values;

    private string _formatted;

    public HealthCheckDataLogValue(string name, IReadOnlyDictionary<string, object> values)
    {
        _name = name;
        _values = values.ToList();

        // We add the name as a kvp so that you can filter by health check name in the logs.
        // This is the same parameter name used in the other logs.
        _values.Add(new KeyValuePair<string, object>("HealthCheckName", name));
    }

    public KeyValuePair<string, object> this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException(nameof(index));
            }

            return _values[index];
        }
    }

    public int Count => _values.Count;

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    public override string ToString()
    {
        if (_formatted == null)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Health check data for {_name}:");

            var values = _values;
            foreach (var kvp in values)
            {
                builder.Append("    ");
                builder.Append(kvp.Key);
                builder.Append(": ");

                builder.AppendLine(kvp.Value?.ToString());
            }

            _formatted = builder.ToString();
        }

        return _formatted;
    }
}
