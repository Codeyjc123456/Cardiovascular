using Cardio.CustomRule;
using HandyControl.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Design.Behavior;

namespace Cadio.CustomRule
{
     public class ValidationExceptionBehavior:Behavior<FrameworkElement>
    {// 实现你的行为逻辑
        protected override void OnAttached()
        {
            Console.WriteLine($"Behavior attached to {AssociatedObject}");
            //// 在此处理附加逻辑
            //// AssociatedObject 就是 行为的对象 FrameworkElement
            AssociatedObject.AddHandler(Validation.ErrorEvent, new EventHandler<ValidationErrorEventArgs>(OnValidationError), true);
            //AssociatedObject.Loaded += OnLoaded;
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine($"Behavior attached to {AssociatedObject}, only once after load.");
            AssociatedObject.AddHandler(Validation.ErrorEvent, new EventHandler<ValidationErrorEventArgs>(OnValidationError), true);
        }
        protected override void OnDetaching()
        {
            Console.WriteLine($"Behavior detached to {AssociatedObject}");
            //// 在此处理分离逻辑
            ////移除 Validation.Error 事件监听
            this.AssociatedObject.RemoveHandler(Validation.ErrorEvent, new EventHandler<ValidationErrorEventArgs>(OnValidationError));
            //AssociatedObject.Loaded -= OnLoaded;
        }

        private void OnValidationError(object sender, ValidationErrorEventArgs e)
        {
            IValidationExceptionHandler validationException = null;
            if (AssociatedObject.DataContext is IValidationExceptionHandler)
            {
                validationException = this.AssociatedObject.DataContext as IValidationExceptionHandler;
            }
            if (validationException == null) return;

            //OriginalSource 触发事件的元素
            var element = e.OriginalSource as UIElement;
            if (element == null) return;

            //ValidationErrorEventAction.Added
            if (e.Action == ValidationErrorEventAction.Added)
            {
                // EmptyValidationRule返回的结果字符串
                validationException.RemoveAndAdd++;
                string error = e.Error.ErrorContent.ToString();

                validationException.Message = error;
            }
            else if (e.Action == ValidationErrorEventAction.Removed)
            {
                validationException.RemoveAndAdd--;
                validationException.Message = string.Empty;
            }
        }
    }
}
