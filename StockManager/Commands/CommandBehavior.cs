using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace StockManager.Commands
{
    public static class CommandBehavior
    {
        #region Command

        /// <summary>
        /// The actual ICommand to run
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command",
                typeof(ICommand),
                typeof(CommandBehavior),
                new FrameworkPropertyMetadata((ICommand)null));

        /// <summary>
        /// Gets the Command property.
        /// </summary>
        public static ICommand GetCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(CommandProperty);
        }

        /// <summary>
        /// Sets the Command property.
        /// </summary>
        public static void SetCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(CommandProperty, value);
        }

        #endregion

        #region CommandParameter

        /// <summary>
        /// The parameter for Execute method of ICommand
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter",
                typeof(object),
                typeof(CommandBehavior),
                new FrameworkPropertyMetadata((object)null));

        /// <summary>
        /// Gets the CommandParameter property.
        /// </summary>
        public static object GetCommandParameter(DependencyObject d)
        {
            return d.GetValue(CommandParameterProperty);
        }

        /// <summary>
        /// Sets the CommandParameter property.
        /// </summary>
        public static void SetCommandParameter(DependencyObject d, object value)
        {
            d.SetValue(CommandParameterProperty, value);
        }

        #endregion

        #region RoutedEventName

        /// <summary>
        /// The event that should actually execute the ICommand
        /// </summary>
        public static readonly DependencyProperty RoutedEventNameProperty =
            DependencyProperty.RegisterAttached("RoutedEventName",
                typeof(string),
                typeof(CommandBehavior),
                new FrameworkPropertyMetadata(string.Empty,
                new PropertyChangedCallback(OnRoutedEventNameChanged)));

        /// <summary>
        /// Gets the RoutedEventName property.
        /// </summary>
        public static string GetRoutedEventName(DependencyObject d)
        {
            return (string)d.GetValue(RoutedEventNameProperty);
        }

        /// <summary>
        /// Sets the RoutedEventName property.
        /// </summary>
        public static void SetRoutedEventName(DependencyObject d, string value)
        {
            d.SetValue(RoutedEventNameProperty, value);
        }

        /// <summary>
        /// Hooks up a Dynamically created EventHandler (by using the
        /// <see cref="EventHooker">EventHooker</see> class) that when
        /// run will run the associated ICommand
        /// </summary>
        private static void OnRoutedEventNameChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            string routedEvent = (string)e.NewValue;

            //If the RoutedEvent string is not null, create a new
            //dynamically created EventHandler that when run will execute
            //the actual bound ICommand instance (usually in the ViewModel)
            if (!string.IsNullOrEmpty(routedEvent))
            {
                EventHooker eventHooker = new EventHooker
                {
                    ObjectWithAttachedCommand = d
                };

                EventInfo eventInfo = d.GetType().GetEvent(routedEvent,
                    BindingFlags.Public | BindingFlags.Instance);

                //Hook up Dynamically created event handler
                if (eventInfo != null)
                {
                    eventInfo.AddEventHandler(d,
                        eventHooker.GetNewEventHandlerToRunCommand(eventInfo));
                }
            }
        }

        #endregion
    }

}
