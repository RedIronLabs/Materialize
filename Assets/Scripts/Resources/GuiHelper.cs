#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using OBJ_IO.Plugins.Extension;
using UnityEngine;

// ReSharper disable ExplicitCallerInfoArgument

#endregion

public static class GuiHelper
{
    private static readonly Dictionary<string, string> TextFields = new();

    public static bool Slider(Rect rect, int value, out int outValue, int minValue, int maxValue,
        [CallerMemberName]
        string memberName = "",
        [CallerFilePath]
        string sourceFilePath = "",
        [CallerLineNumber]
        int sourceLineNumber = 0)
    {
        var res = Slider(rect, null, value, out var outValueFloat, minValue, maxValue, memberName, sourceFilePath,
            sourceLineNumber);
        outValue = outValueFloat;
        return res;
    }

    public static bool Slider(Rect rect, float value, out float outValue, float minValue, float maxValue,
        [CallerMemberName]
        string memberName = "",
        [CallerFilePath]
        string sourceFilePath = "",
        [CallerLineNumber]
        int sourceLineNumber = 0)
    {
        var res = Slider(rect, null, value, out var outValue2, minValue, maxValue, memberName, sourceFilePath,
            sourceLineNumber);
        outValue = outValue2;
        return res;
    }

    public static bool Slider(Rect rect, string title, int value, out int outValue, int minValue, int maxValue,
        [CallerMemberName]
        string memberName = "",
        [CallerFilePath]
        string sourceFilePath = "",
        [CallerLineNumber]
        int sourceLineNumber = 0)
    {
        var res = Slider(rect, title, value, out float outValueFloat, minValue, maxValue, memberName, sourceFilePath,
            sourceLineNumber);

        outValue = (int) outValueFloat;
        return res;
    }

    public static bool Slider(Rect rect, string title, int value, string inText, out int outValue,
        out string outText, int minValue, int maxValue, [CallerMemberName]
        string memberName = "",
        [CallerFilePath]
        string sourceFilePath = "",
        [CallerLineNumber]
        int sourceLineNumber = 0)
    {
        var ret = Slider(rect, title, value, out outValue, minValue, maxValue, memberName, sourceFilePath,
            sourceLineNumber);
        outText = outValue.ToString();

        return ret;
    }

    public static bool Slider(Rect rect, [CanBeNull] string title, float value, out float outValue, float minValue,
        float maxValue, [CallerMemberName]
        string memberName = "",
        [CallerFilePath]
        string sourceFilePath = "",
        [CallerLineNumber]
        int sourceLineNumber = 0)
    {
        const float tolerance = 0.001f;
        var offsetX = (int) rect.x;
        var offsetY = (int) rect.y;
        var startValue = value;

        if (!title.IsNullOrEmpty()) GUI.Label(new Rect(rect.x, rect.y, 250, 30), title);

        offsetY += 25;

        var isChanged = false;

        value = GUI.HorizontalSlider(new Rect(offsetX, offsetY, rect.width - 60, 10), value, minValue, maxValue);

        var handle = memberName + sourceFilePath + sourceLineNumber;

        GUI.SetNextControlName(handle);
        string textValue;
        if (TextFields.ContainsKey(handle))
        {
            TextFields.TryGetValue(handle, out textValue);
        }
        else
        {
            textValue = value.ToString(CultureInfo.CurrentCulture);
            TextFields.Add(handle, textValue);
        }

        textValue = GUI.TextField(new Rect(offsetX + rect.width - 50, offsetY - 5, 50, 20), textValue);

        if (GUI.GetNameOfFocusedControl().Contains(handle))
        {
            if (Event.current.isKey &&
                (Event.current.keyCode.Equals(KeyCode.Return) || Event.current.keyCode.Equals(KeyCode.KeypadEnter)))
            {
                if (textValue.Contains(".")) textValue = textValue.Replace(".", ",");

                float.TryParse(textValue, out var result);
                if (Math.Abs(result - value) > tolerance)
                {
                    value = Mathf.Clamp(result, minValue, maxValue);
                    textValue = value.ToString(CultureInfo.CurrentCulture);
                }
            }
        }
        else
        {
            textValue = value.ToString(CultureInfo.CurrentCulture); 
        }


        if (TextFields.ContainsKey(handle)) TextFields[handle] = textValue;

        if (Math.Abs(value - startValue) > tolerance) isChanged = true;

        outValue = value;

        return isChanged;
    }

    public static bool Toggle(Rect rect, bool value, out bool outValue, string text, bool doStuff)
    {
        var isChanged = false;

        var tempValue = value;
        value = GUI.Toggle(rect, value, text);
        if (value != tempValue || doStuff) isChanged = true;

        outValue = value;

        return isChanged;
    }


    public static bool VerticalSlider(Rect rect, float value, out float outValue, float minValue, float maxValue,
        bool doStuff)
    {
        var isChanged = false;

        var tempValue = value;
        value = GUI.VerticalSlider(rect, value, minValue, maxValue);
        if (Math.Abs(value - tempValue) > 0.0001f || doStuff) isChanged = true;

        outValue = value;

        return isChanged;
    }
}