using System;
using System.Linq;
using UnityEngine.SceneManagement;

// Token: 0x02000002 RID: 2
internal class CLIU
{
    // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
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

    // Token: 0x06000002 RID: 2 RVA: 0x000020BC File Offset: 0x000002BC
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

    // Token: 0x06000003 RID: 3 RVA: 0x000021EC File Offset: 0x000003EC
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

    // Token: 0x06000004 RID: 4 RVA: 0x000022A4 File Offset: 0x000004A4
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

    // Token: 0x06000005 RID: 5 RVA: 0x00002358 File Offset: 0x00000558
    public static bool IsCommandUsable()
    {
        if (SceneManager.GetActiveScene().name != Semih_Network.GameSceneName)
        {
            CLIU.Echo(CLIU.Red("This command only works inside a loaded world."));
            return false;
        }
        return true;
    }

    // Token: 0x06000006 RID: 6 RVA: 0x00002390 File Offset: 0x00000590
    public static string[] SplitLastCommand()
    {
        return RConsole.lastCommands.LastOrDefault<string>().Split(new[] { "" }, StringSplitOptions.RemoveEmptyEntries);
    }

    // Token: 0x06000007 RID: 7 RVA: 0x000023A3 File Offset: 0x000005A3
    public static string Red(string text)
	{
		return CLIU.Colorize(CLIU.RED, text);
	}

	// Token: 0x06000008 RID: 8 RVA: 0x000023B0 File Offset: 0x000005B0
	public static string Green(string text)
	{
		return CLIU.Colorize(CLIU.GREEN, text);
	}

	// Token: 0x06000009 RID: 9 RVA: 0x000023BD File Offset: 0x000005BD
	public static string White(string text)
	{
		return CLIU.Colorize(CLIU.WHITE, text);
	}

	// Token: 0x0600000A RID: 10 RVA: 0x000023CA File Offset: 0x000005CA
	public static string Orange(string text)
	{
		return CLIU.Colorize(CLIU.ORANGE, text);
	}

	// Token: 0x0600000B RID: 11 RVA: 0x000023D7 File Offset: 0x000005D7
	public static string Blue(string text)
	{
		return CLIU.Colorize(CLIU.BLUE, text);
	}

	// Token: 0x0600000C RID: 12 RVA: 0x000023E4 File Offset: 0x000005E4
	public static string ColorizeBool(bool setting)
	{
		if (!setting)
		{
			return CLIU.Red("false");
		}
		return CLIU.Green("TRUE");
	}

	// Token: 0x0600000D RID: 13 RVA: 0x00002400 File Offset: 0x00000600
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

	// Token: 0x0600000E RID: 14 RVA: 0x0000244C File Offset: 0x0000064C
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

	// Token: 0x0600000F RID: 15 RVA: 0x00002479 File Offset: 0x00000679
	public static void Echo(string msg)
	{
		RConsole.Log(CLIU.CONSOLE_PREFIX + msg);
	}

	// Token: 0x04000001 RID: 1
	public static string WHITE = "#ffffff";

	// Token: 0x04000002 RID: 2
	public static string ORANGE = "#ff8000";

	// Token: 0x04000003 RID: 3
	public static string BLUE = "#0080ff";

	// Token: 0x04000004 RID: 4
	public static string GREEN = "#009926";

	// Token: 0x04000005 RID: 5
	public static string RED = "#ff0000";

	// Token: 0x04000006 RID: 6
	public static string CONSOLE_PREFIX = "";
}
