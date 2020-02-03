using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace Lighthouse.UI.Logging
{
    class InMemorySink : ILogEventSink
    {
        private readonly ITextFormatter _textFormatter =
            new MessageTemplateTextFormatter("[{Level}] {Message}{Exception}",
                CultureInfo.InvariantCulture);

        public ObservableCollection<string> Events { get; } = new ObservableCollection<string>();

        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null) 
                throw new ArgumentNullException(nameof(logEvent));

            var renderSpace = new StringWriter();
            _textFormatter.Format(logEvent, renderSpace);
            Events.Add(renderSpace.ToString());
        }
    }
}