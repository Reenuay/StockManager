using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace StockManager.Commands
{
    /// <summary>
    /// Предоставляет коммандное поведение для объектов не реализующих интерфейс <see cref="ICommand"/>
    /// </summary>
    public static class CommandBehavior
    {
        #region Command

        /// <summary>
        /// Проигрываемая комманда <see cref="ICommand"/>
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command",
                typeof(ICommand),
                typeof(CommandBehavior),
                new FrameworkPropertyMetadata((ICommand)null));

        /// <summary>
        /// Получает CommandProperty
        /// </summary>
        public static ICommand GetCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(CommandProperty);
        }

        /// <summary>
        /// Устанавливает CommandProperty
        /// </summary>
        public static void SetCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(CommandProperty, value);
        }

        #endregion

        #region CommandParameter

        /// <summary>
        /// Параметер для метода <see cref="ICommand.Execute(object)"/>
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter",
                typeof(object),
                typeof(CommandBehavior),
                new FrameworkPropertyMetadata((object)null));

        /// <summary>
        /// Получает CommandParameterProperty
        /// </summary>
        public static object GetCommandParameter(DependencyObject d)
        {
            return d.GetValue(CommandParameterProperty);
        }

        /// <summary>
        /// Устанавливает CommandParameterProperty
        /// </summary>
        public static void SetCommandParameter(DependencyObject d, object value)
        {
            d.SetValue(CommandParameterProperty, value);
        }

        #endregion

        #region RoutedEventName

        /// <summary>
        /// Cобытие, которое вызовет команду <see cref="ICommand"/>
        /// </summary>
        public static readonly DependencyProperty RoutedEventNameProperty =
            DependencyProperty.RegisterAttached("RoutedEventName",
                typeof(string),
                typeof(CommandBehavior),
                new FrameworkPropertyMetadata(string.Empty,
                new PropertyChangedCallback(OnRoutedEventNameChanged)));

        /// <summary>
        /// Получает RoutedEventNameProperty
        /// </summary>
        public static string GetRoutedEventName(DependencyObject d)
        {
            return (string)d.GetValue(RoutedEventNameProperty);
        }

        /// <summary>
        /// Устанавливает RoutedEventNameProperty
        /// </summary>
        public static void SetRoutedEventName(DependencyObject d, string value)
        {
            d.SetValue(RoutedEventNameProperty, value);
        }

        /// <summary>
        /// Перехватывает, динамически созданное, событие посредством
        /// класса <see cref="EventHooker"/>, который затем, при вызове
        /// события, обработает ассоциированную комманду
        /// </summary>
        private static void OnRoutedEventNameChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            string routedEvent = (string)e.NewValue;

            if (!string.IsNullOrEmpty(routedEvent))
            {
                EventHooker eventHooker = new EventHooker
                {
                    ObjectWithAttachedCommand = d
                };

                EventInfo eventInfo = d.GetType().GetEvent(routedEvent,
                    BindingFlags.Public | BindingFlags.Instance);

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
