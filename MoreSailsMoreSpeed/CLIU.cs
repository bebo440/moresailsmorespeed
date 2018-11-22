using System;
using System.Linq;
using UnityEngine.SceneManagement;

internal class CLIU
{
    public static bool InterpretBoolFromLastCommand(bool current)
    {
        string[] array = CLIU.SplitLastCommand();
        if (array.Length > 1)
        {
            string a = array[1].ToLower();
            return a == "1" || a == "t" || a == "y" || a == "true" || a == "yes";
        }
        return !current;
    }

    public static bool InterpretIntervalFromLastCommand(out Interval_Float interval, float lowerBound, float upperBound, float lowerDefault, float upperDefault, bool optionalMax = true, float upperBoundGap = 1f)
    {
        string[] array = CLIU.SplitLastCommand();
        try
        {
            if (array.Length <= (optionalMax ? 1 : 2))
            {
                throw new FormatException();
            }
            float num = Convert.ToSingle(array[1]);
            if (num < lowerBound || num > upperBound)
            {
                throw new OverflowException(num.ToString());
            }
            float num2 = num + upperBoundGap;
            if (array.Length > 2)
            {
                num2 = Convert.ToSingle(array[2]);
                if (num2 < lowerBound + upperBoundGap || num2 > upperBound)
                {
                    throw new OverflowException(num2.ToString());
                }
            }
            if (num2 <= num)
            {
                num2 = num + upperBoundGap;
            }
            interval = new Interval_Float
            {
                minValue = num,
                maxValue = num2
            };
            return true;
        }
        catch (FormatException)
        {
            CLIU.Echo(CLIU.Red("Invalid values, expected a minimum and" + (optionalMax ? " optional " : " ") + "maximum, eg: ") + CLIU.Blue(lowerDefault.ToString() + " " + upperDefault.ToString()));
        }
        catch (OverflowException)
        {
            CLIU.Echo(CLIU.Red("Values are outside the accepted range of ") + CLIU.ColorizeRange(lowerBound.ToString(), upperBound.ToString()));
        }
        interval = null;
        return false;
    }

    public static bool InterpretFloatFromLastCommand(out float value, float lowerBound, float upperBound, float defaultValue)
    {
        string[] array = CLIU.SplitLastCommand();
        try
        {
            if (array.Length <= 1)
            {
                throw new FormatException();
            }
            value = Convert.ToSingle(array[1]);
            if (value < lowerBound || value > upperBound)
            {
                throw new OverflowException(value.ToString());
            }
            return true;
        }
        catch (FormatException)
        {
            CLIU.Echo(CLIU.Red("Invalid value, expected a decimal number, eg: ") + CLIU.Blue(defaultValue.ToString()));
        }
        catch (OverflowException)
        {
            CLIU.Echo(CLIU.Red("Value is outside the accepted range of ") + CLIU.ColorizeRange(lowerBound.ToString(), upperBound.ToString()));
        }
        value = -1f;
        return false;
    }

    public static bool InterpretIntegerFromLastCommand(out int value, int lowerBound, int upperBound, int defaultValue)
    {
        string[] array = CLIU.SplitLastCommand();
        try
        {
            if (array.Length <= 1)
            {
                throw new FormatException();
            }
            value = Convert.ToInt32(array[1]);
            if (value < lowerBound || value > upperBound)
            {
                throw new OverflowException(value.ToString());
            }
            return true;
        }
        catch (FormatException)
        {
            CLIU.Echo(CLIU.Red("Invalid value, expected a number, eg: ") + CLIU.Blue(defaultValue.ToString()));
        }
        catch (OverflowException)
        {
            CLIU.Echo(CLIU.Red("Value is outside the accepted range of ") + CLIU.ColorizeRange(lowerBound.ToString(), upperBound.ToString()));
        }
        value = -1;
        return false;
    }

    public static bool IsCommandUsable()
    {
        if (SceneManager.GetActiveScene().name != Semih_Network.GameSceneName)
        {
            CLIU.Echo(CLIU.Red("This command only works inside a loaded world."));
            return false;
        }
        return true;
    }

    public static string[] SplitLastCommand()
    {
        return RConsole.lastCommands.LastOrDefault<string>().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
    }

    public static string Red(string text)
    {
        return CLIU.Colorize(CLIU.RED, text);
    }

    public static string Green(string text)
    {
        return CLIU.Colorize(CLIU.GREEN, text);
    }

    public static string White(string text)
    {
        return CLIU.Colorize(CLIU.WHITE, text);
    }

    public static string Orange(string text)
    {
        return CLIU.Colorize(CLIU.ORANGE, text);
    }

    public static string Blue(string text)
    {
        return CLIU.Colorize(CLIU.BLUE, text);
    }

    public static string Cyan(string text)
    {
        return CLIU.Colorize(CLIU.CYAN, text);
    }

    public static string ColorizeBool(bool setting)
    {
        if (!setting)
        {
            return CLIU.Red("false");
        }
        return CLIU.Green("TRUE");
    }

    public static string ColorizeRange(string min, string max)
    {
        return string.Concat(new string[]
        {
            CLIU.Blue(min),
            CLIU.White(" <= "),
            "x",
            CLIU.White(" <= "),
            CLIU.Blue(max)
        });
    }

    public static string Colorize(string color, string text)
    {
        return string.Concat(new string[]
        {
            "<color=",
            color,
            ">",
            text,
            "</color>"
        });
    }

    public static void Echo(string msg)
    {
        RConsole.Log(CLIU.CONSOLE_PREFIX + msg);
    }

    public static void EchoSetting(string name, bool value, bool def)
    {
        CLIU.Echo(string.Concat(new string[]
        {
            name,
            " ",
            CLIU.ColorizeBool(value),
            " (default: ",
            CLIU.ColorizeBool(def),
            ")"
        }));
    }

    public static void EchoSetting(string name, float value, float def)
    {
        CLIU.Echo(string.Concat(new string[]
        {
            name,
            " ",
            CLIU.Blue(value.ToString()),
            " (default: ",
            CLIU.Blue(def.ToString()),
            ")"
        }));
    }

    public static void EchoSetting(string name, int value, int def)
    {
        CLIU.Echo(string.Concat(new string[]
        {
            name,
            " ",
            CLIU.Blue(value.ToString()),
            " (default: ",
            CLIU.Blue(def.ToString()),
            ")"
        }));
    }

    public static string WHITE = "#ffffff";

    public static string ORANGE = "#ff8000";

    public static string CYAN = "00ffff";

    public static string BLUE = "#0080ff";

    public static string GREEN = "#009926";

    public static string RED = "#ff0000";

    public static string CONSOLE_PREFIX = "";
}