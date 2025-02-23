using System.Globalization;
using Newtonsoft.Json;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;
using Serilog.Parsing;

namespace serilog_payload_demo.Configuration;

public class PayloadLogFormatter : ITextFormatter
{
    readonly JsonValueFormatter _valueFormatter;
    
    public PayloadLogFormatter(JsonValueFormatter? valueFormatter = null)
    {
        _valueFormatter = valueFormatter ?? new JsonValueFormatter(typeTagName: "$type");
    }
    
    public void Format(LogEvent logEvent, TextWriter output)
    {
        FormatEvent(logEvent, output, _valueFormatter);
        output.WriteLine();
    }
    
    public static void FormatEvent(LogEvent logEvent, TextWriter output, JsonValueFormatter valueFormatter)
    {
        if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
        if (output == null) throw new ArgumentNullException(nameof(output));
        if (valueFormatter == null) throw new ArgumentNullException(nameof(valueFormatter));

        output.Write("{\"@t\":\"");
        output.Write(logEvent.Timestamp.UtcDateTime.ToString("O"));
        output.Write("\",\"@m\":\"PayloadEvent\"");
        
        output.Write(",\"@l\":\"");
        output.Write(logEvent.Level);
        output.Write('\"');
        
        if (logEvent.TraceId != null)
        {
            output.Write(",\"@tr\":\"");
            output.Write(logEvent.TraceId.Value.ToHexString());
            output.Write('\"');
        }

        if (logEvent.SpanId != null)
        {
            output.Write(",\"@sp\":\"");
            output.Write(logEvent.SpanId.Value.ToHexString());
            output.Write('\"');
        }

        
        foreach (var property in logEvent.Properties)
        {
            var name = property.Key;
            if (name.Length > 0 && name[0] == '@')
            {
                // Escape first '@' by doubling
                name = '@' + name;
            }
            switch (name)
            {
                case "HttpLog":
                    break;
                case "SourceContext":
                    output.Write(',');
                    JsonValueFormatter.WriteQuotedJsonString("@sc", output);
                    output.Write(':');
                    valueFormatter.Format(property.Value, output);
                    break;
                case "Duration":
                    output.Write(',');
                    JsonValueFormatter.WriteQuotedJsonString("duration", output);
                    output.Write(':');
                    valueFormatter.Format(property.Value, output);
                    break;
                case "StatusCode":
                    output.Write(',');
                    JsonValueFormatter.WriteQuotedJsonString("statusCode", output);
                    output.Write(':');
                    valueFormatter.Format(property.Value, output);
                    break;
                case "ResponseBody":
                    output.Write(',');
                    JsonValueFormatter.WriteQuotedJsonString("responseBody", output);
                    output.Write(':');
                    output.Write(JsonConvert.DeserializeObject(property.Value.ToString()).ToString().Replace("\n", ""));
                    break;
                case "RequestBody":
                    output.Write(',');
                    JsonValueFormatter.WriteQuotedJsonString("requestBody", output);
                    output.Write(':');
                    output.Write(JsonConvert.DeserializeObject(property.Value.ToString()).ToString().Replace("\n", ""));
                    break;
                case "RequestPath":
                    output.Write(',');
                    JsonValueFormatter.WriteQuotedJsonString("requestPath", output);
                    output.Write(':');
                    valueFormatter.Format(property.Value, output);
                    break;
                case "Method":
                    output.Write(',');
                    JsonValueFormatter.WriteQuotedJsonString("requestMethod", output);
                    output.Write(':');
                    valueFormatter.Format(property.Value, output);
                    break;
                default:
                    output.Write(',');
                    JsonValueFormatter.WriteQuotedJsonString(name, output);
                    output.Write(':');
                    valueFormatter.Format(property.Value, output);
                    break;
            }
        }

        output.Write('}');
    }
}