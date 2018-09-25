using System.IO;
using UnityEngine;

/// <summary>  
///  Keycodings for Keyboard Input
///  </summary>
///  <remarks>
///  Default keybinding file name is default.json (clearing the file name without entering a new will load this file)
///  Default return value from key or button functions is Keys._ or Buttons._ .
/// </remarks>
public enum Keys
{
    _ = 0x00,
    Escape = 0x01,
    _1 = 0x02,
    _2 = 0x03,
    _3 = 0x04,
    _4 = 0x05,
    _5 = 0x06,
    _6 = 0x07,
    _7 = 0x08,
    _8 = 0x09,
    _9 = 0x0A,
    _0 = 0x0B,

    A = 0x1E,
    B = 0x30,
    C = 0x2E,
    D = 0x20,
    E = 0x12,
    F = 0x21,
    G = 0x22,
    H = 0x23,
    I = 0x17,
    J = 0x24,
    K = 0x25,
    L = 0x26,
    M = 0x32,
    N = 0x31,
    O = 0x18,
    P = 0x19,
    Q = 0x10,
    R = 0x13,
    S = 0x1F,
    T = 0x14,
    U = 0x16,
    V = 0x2F,
    W = 0x11,
    X = 0x2D,
    Y = 0x15,
    Z = 0x2C,

    Alt_Left = 0x38,
    Alt_Right = 0xB8,
    Backspace = 0x0E,
    CapsLock = 0x3A,
    Comma = 0x33,
    Control_Left = 0x1D,
    Control_Right = 0x9D,
    Delete = 0xD3,
    Divide = 0x35,
    Dot = 0x34,
    End = 0xCF,
    Enter = 0x1C,
    Equals = 0x0D,
    Home = 0xC7,
    Insert = 0xD2,
    Minus = 0x0C,
    NumLock = 0x45,
    PageDown = 0xD1,
    PageUp = 0xC9,
    Pause = 0xC5,
    PrintScreen = 0xB7,
    SecondaryAction = 0xDD,
    SemiColon = 0x27,
    ScrollLock = 0x46,
    Shift_Left = 0x2A,
    Shift_Right = 0x36,
    SingleQuote = 0x28,
    Space = 0x39,
    SquareBracket_Open = 0x1A,
    SquareBracket_Close = 0x1B,
    Tab = 0x0F,
    Tilde = 0x29,
    //BackSlash = 0x2B, (Owned by VX1)
    
    F1 = 0x3B,
    F2 = 0x3C,
    F3 = 0x3D,
    F4 = 0x3E,
    F5 = 0x3F,
    F6 = 0x40,
    F7 = 0x41,
    F8 = 0x42,
    F9 = 0x43,
    F10 = 0x44,
    F11 = 0x57,
    F12 = 0x58,

    NUMPAD_Divide = 0xB5,
    NUMPAD_Multiply = 0x37,
    NUMPAD_Minux = 0x4A,
    NUMPAD_Plus = 0x4E,
    NUMPAD_Enter = 0x9C,

    NUMPAD_0 = 0x52,
    NUMPAD_1 = 0x4F,
    NUMPAD_2 = 0x50,
    NUMPAD_3 = 0x51,
    NUMPAD_4 = 0x4B,
    NUMPAD_5 = 0x4C,
    NUMPAD_6 = 0x4D,
    NUMPAD_7 = 0x47,
    NUMPAD_8 = 0x48,
    NUMPAD_9 = 0x49,
    NUMPAD_Dot = 0x53,

};

/// <summary>  
///  Keycoding for XBox controller buttons
///  </summary>
public enum Buttons
{
    _,
    DPad_Up = 0x0001,
    DPad_Down = 0x0002,
    DPad_Left = 0x0004,
    DPad_Right = 0x0008,
    Start = 0x0010,
    Back = 0x0020,
    Left_Thumb = 0x0040,
    Right_Thumb = 0x0080,
    Left_Shoulder = 0x0100,
    Right_Shoulder = 0x0200,
    A = 0x1000,
    B = 0x2000,
    X = 0x4000,
    Y = 0x8000
};

/// <summary>  
///  Keycoding for XBox controller Axis
///  </summary>
public enum Axis
{
    _,
    LeftTrigger = 1,
    RightTrigger = 2,
    LeftStickX = 3,
    LeftStickY = 4,
    RightStickX = 5,
    RightStickY = 6
}

public enum Mouse_Button
{
    _,
    Mouse_1 = 0x1,
    Mouse_2 = 0x2,
    Mouse_3 = 0x4
}


public struct Mouse_Position
{
    public float x;
    public float y;
    public float z;

    public Mouse_Position(Vector3 pos)
    {
        this.x = pos.x / 100;
        this.y = pos.y / 100;
        this.z = pos.z / 100;
    }
    public Mouse_Position(float[] pos)
    {
        this.x = pos[0] / 100;
        this.y = pos[1] / 100;
        this.z = pos[2] / 100;
    }
}

[System.Serializable] public class KeyBindings : SerializableDictionary<string, Keys> { public KeyBindings():base(new System.Collections.Generic.StaticStringComparer()) { } }
[System.Serializable] public class MouseBindings : SerializableDictionary<string, Mouse_Button> { public MouseBindings() : base(new System.Collections.Generic.StaticStringComparer()) { } }
[System.Serializable] public class ButtonBindings : SerializableDictionary<string, Buttons> { public ButtonBindings() : base(new System.Collections.Generic.StaticStringComparer()) { } }
[System.Serializable] public class AxisBindings : SerializableDictionary<string, Axis> { public AxisBindings() : base(new System.Collections.Generic.StaticStringComparer()) { } }

/// <summary>  
///  Input Controller handles keybindings, Allows Saving and Loading of keybinding.json files
///  </summary>
public class InputController : Singleton<InputController>
{
    [Header("Load / Save")]
    public string filename = "default.json";

    [Header("Keyboard")]
    public KeyBindings Keyboard = new KeyBindings();

    [Header("Mouse")]
    public MouseBindings Mouse = new MouseBindings();

    [Header("Joy 1")]
    public ButtonBindings J1Buttons = new ButtonBindings();
    public AxisBindings J1Axis = new AxisBindings();
    [Header("Joy 2")]
    public ButtonBindings J2Buttons = new ButtonBindings();
    public AxisBindings J2Axis = new AxisBindings();
    [Header("Joy 3")]
    public ButtonBindings J3Buttons = new ButtonBindings();
    public AxisBindings J3Axis = new AxisBindings();
    [Header("Joy 4")]
    public ButtonBindings J4Buttons = new ButtonBindings();
    public AxisBindings J4Axis = new AxisBindings();


    // Use this for initialization
    void Start()
    {
        LoadData();
    }

    public static void LoadData()
    {
        Debug.Log("Load: " + Instance.filename);
        
        string filePath = Path.Combine(Application.streamingAssetsPath, Instance.filename);
        if (File.Exists(filePath))
        {
            // Read the JSON from the file into a string
            string dataAsJson = File.ReadAllText(filePath);
            // Pass the JSON to JSON Utility, and tell it to make a gameobject from it
            InputData Loaded = JsonUtility.FromJson<InputData>(dataAsJson);

            Loaded.To_IC();
        }
        else
        {
            Debug.Log("Cannot load input data. Creating: " + filePath);
            VXProcess.Instance.add_log_line("Cannot load input data. Creating: " + filePath);
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                file.Directory.Create();
            }
            SaveData();
        }
    }

    public static void SaveData()
    {
        InputData Save = new InputData();
        Save.From_IC();
        
        string dataAsJson = JsonUtility.ToJson(Save, true);
        string filePath = Path.Combine(Application.streamingAssetsPath, Instance.filename);

        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            file.Directory.Create();
        }

        File.WriteAllText(filePath, dataAsJson);
    }

    public static Keys GetKey(string key)
    {
        if (Instance.Keyboard.ContainsKey(key))
        {
            return Instance.Keyboard[key];
        }
        else
        {
            UnityEngine.Debug.Log("Does not contain this string: " + key);
            VXProcess.Instance.add_log_line("Does not contain this string: " + key);
            return Keys._;
        }
    }

    public static Buttons GetButton(string button, int joystick)
    {


        switch (joystick)
        {
            case 0:
                if (Instance.J1Buttons.ContainsKey(button))
                {
                    return Instance.J1Buttons[button];
                }
                else
                {
                    return Buttons._;
                }

            case 1:
                if (Instance.J2Buttons.ContainsKey(button))
                {
                    return Instance.J2Buttons[button];
                }
                else
                {
                    return Buttons._;
                }

            case 2:
                if (Instance.J3Buttons.ContainsKey(button))
                {
                    return Instance.J3Buttons[button];
                }
                else
                {
                    return Buttons._;
                }

            case 3:
                if (Instance.J4Buttons.ContainsKey(button))
                {
                    return Instance.J4Buttons[button];
                }
                else
                {
                    return Buttons._;
                }

            default:
                break;
        }
        return Buttons._;
    }

    public static Axis GetAxis(string axis, int joystick)
    {


        switch (joystick)
        {
            case 0:
                if (Instance.J1Axis.ContainsKey(axis))
                {
                    return Instance.J1Axis[axis];
                }
                else
                {
                    return Axis._;
                }

            case 1:
                if (Instance.J2Axis.ContainsKey(axis))
                {
                    return Instance.J2Axis[axis];
                }
                else
                {
                    return Axis._;
                }

            case 2:
                if (Instance.J3Axis.ContainsKey(axis))
                {
                    return Instance.J3Axis[axis];
                }
                else
                {
                    return Axis._;
                }

            case 3:
                if (Instance.J4Axis.ContainsKey(axis))
                {
                    return Instance.J4Axis[axis];
                }
                else
                {
                    return Axis._;
                }

            default:
                break;
        }
        return Axis._;
    }

    public static Mouse_Button GetMouseButton(string key)
    {


        if (Instance.Mouse.ContainsKey(key))
        {
            return Instance.Mouse[key];
        }
        else
        {
            UnityEngine.Debug.Log("Does not contain this string: " + key);
            return Mouse_Button._;
        }
    }

    [ExecuteInEditMode]
    void OnEnable()
    {
        if (filename == "")
        {
            filename = "default.json";
        }
    }

    void OnValidate()
    {
        if (filename == "")
        {
            filename = "default.json";
        }
    }
}