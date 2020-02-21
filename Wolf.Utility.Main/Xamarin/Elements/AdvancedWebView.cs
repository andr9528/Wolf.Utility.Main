﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Wolf.Utility.Main.Logging.Enum;
using Wolf.Utility.Main.Xamarin.Elements.Events;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace Wolf.Utility.Main.Xamarin.Elements
{
    

    public class AdvancedWebView : HybridWebView
    {
        public event EventHandler<ClickEvent> Clicked;

        public static readonly BindableProperty ClickCommandProperty =
            BindableProperty.Create("ClickCommand", typeof(ICommand), typeof(AdvancedWebView), null);

        public ICommand ClickCommand
        {
            get { return (ICommand)GetValue(ClickCommandProperty); }
            set { SetValue(ClickCommandProperty, value); }
        }

        public AdvancedWebView()
        {
            LoadFinished += (sender, e) =>
            {
                InjectJavaScript(GetJsEnterKeyPressEvent());
            };

            RegisterCallback("invokeClick", el => {
                var args = new ClickEvent { Element = el };

                Clicked?.Invoke(this, args);
                ClickCommand?.Execute(args);
            });

        }

        public void AddClickEvent(string elementId = "")
        {
            var js = string.IsNullOrEmpty(elementId) ? GetJsBodyClickEvent() : GetJsElementClickEvent(elementId);

            InjectJavaScript(js);

            // Logging.Logging.Log(LogType.Information, $"Injected Javascript => {js}");
        }

        public void SetInputFocus(string inputId)
        {
            var js = GetJsSetFocus(inputId);

            InjectJavaScript(js);

            // Logging.Logging.Log(LogType.Information, $"Injected Javascript => {js}");
        }

        public void PressEnter(string inputId)
        {
            var js = GetJsEnterKeyPressDispatch(inputId);

            InjectJavaScript(js);

            // Logging.Logging.Log(LogType.Information, $"Injected Javascript => {js}");
        }

        public void WriteToField(string inputId, string value)
        {
            var js = GetJsSetInputField(inputId, value);

            InjectJavaScript(js);

            // Logging.Logging.Log(LogType.Information, $"Injected Javascript => {js}");
        }

        public void LinkInputAndButton(string inputId, string buttonId)
        {
            var js = GetJsLinkInputAndButton(inputId, buttonId);

            InjectJavaScript(js);

            // Logging.Logging.Log(LogType.Information, $"Injected Javascript => {js}");
        }


        #region JS Strings
        // Source: https://stackoverflow.com/questions/3276794/jquery-or-pure-js-simulate-enter-key-pressed-for-testing
        private string GetJsEnterKeyPressDispatch(string inputId)
        {
            var builder = new StringBuilder();

            builder.Append($"if (document.getElementById('{inputId}'))");
            builder.Append("{");
            builder.Append($"document.getElementById('{inputId}').dispatchEvent(ke);");
            builder.Append("}");
            
            return builder.ToString();
        }

        private string GetJsEnterKeyPressEvent()
        {
            var builder = new StringBuilder();

            builder.Append("const ke = new KeyboardEvent('keyup', {bubbles: true, cancelable: true, key: 'Enter', code: '13'});");

            return builder.ToString();
        }
        // Source: https://stackoverflow.com/questions/155188/trigger-a-button-click-with-javascript-on-the-enter-key-in-a-text-box
        private string GetJsLinkInputAndButton(string inputId, string buttonId)
        {
            var builder = new StringBuilder();
            
            builder.Append($"if (document.getElementById('{inputId}'))");
            builder.Append("{");
            builder.Append($"document.getElementById('{inputId}').addEventListener('keyup', function(event) ");
            builder.Append("{");
            //builder.Append("event.preventDefault();");
            builder.Append("if (event.key == 'Enter')");
            builder.Append("{");
            builder.Append($"document.getElementById('{buttonId}').click();");
            builder.Append("}");
            builder.Append("});");
            builder.Append("}");

            return builder.ToString();
        }
        // Source: https://stackoverflow.com/questions/17500704/javascript-set-focus-to-html-form-element
        private string GetJsSetFocus(string inputId)
        {
            var builder = new StringBuilder();

            builder.Append($"if (document.getElementById('{inputId}'))");
            builder.Append("{");
            builder.Append($"document.getElementById('{inputId}').focus();");
            builder.Append("}");

            return builder.ToString();
        }
        // Source: https://stackoverflow.com/questions/30397090/xamarin-forms-handle-clicked-event-on-webview
        private string GetJsBodyClickEvent()
        {
            var builder = new StringBuilder();

            builder.Append("var x = document.body.addEventListener('click', function(e) ");
            builder.Append("{");
            builder.Append("e = e || window.event;");
            builder.Append("var target = e.target || e.srcElement;");
            builder.Append("Native('invokeClick', 'tag='+target.tagName+' id='+target.id+' name='+target.name);");
            builder.Append("}, ");
            builder.Append("true /* to ensure we capture it first*/);");

            return builder.ToString();
        }

        private string GetJsElementClickEvent(string elementId)
        {
            var builder = new StringBuilder();

            builder.Append($"if (document.getElementById('{elementId}'))");
            builder.Append("{");
            builder.Append($"var x = document.getElementById('{elementId}').addEventListener('click', function(e) ");
            builder.Append("{");
            builder.Append("e = e || window.event;");
            builder.Append("var target = e.target || e.srcElement;");
            builder.Append("Native('invokeClick', 'tag='+target.tagName+' id='+target.id+' name='+target.name);");
            builder.Append("}, ");
            builder.Append("true /* to ensure we capture it first*/);");
            builder.Append("}");
            
            return builder.ToString();
        }

        private string GetJsSetInputField(string inputId, string value)
        {
            var builder = new StringBuilder();

            builder.Append($"if (document.getElementById('{inputId}'))");
            builder.Append("{");
            builder.Append($"var input = document.getElementById('{inputId}');");
            builder.Append($"input.setAttribute('value', '{value}');");
            builder.Append("}");

            return builder.ToString();
        }
        #endregion

    }
}