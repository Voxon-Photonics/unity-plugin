
namespace Voxon
{
    /// <summary>  
    ///  Voxon.Input is a Unity input replacement. Utilises Keybindings as set in Capture Volume
    ///  </summary>
    ///  <remarks>
    ///  Unity.Input does not allow input via -Batchmode (required for VX1), thus requiring the use of Voxon.Input
    ///  For single player simply replace Input with Voxon.Input and ensure binding strings are available in Input Controller (found on Capture Volume)
    ///  For multiplayer games; use GetXY(BindingName, PlayerNumber). Players are numbered 0-3.
    /// </remarks>  
    public class Input
    {
        // Keyboard Input
        public static bool GetKey(string key_name)
        {
            return VXProcess.runtime.GetKey((int)InputController.GetKey(key_name));
        }

        public static bool GetKeyUp(string key_name)
        {
            return VXProcess.runtime.GetKeyUp((int)InputController.GetKey(key_name));
        }

        public static bool GetKeyDown(string key_name)
        {
            return VXProcess.runtime.GetKeyDown((int)InputController.GetKey(key_name));
        }

        // Player 1 Default Input
        public static bool GetButton(string button_name)
        {
            return GetButton(button_name, 0);
        }

        public static bool GetButtonDown(string button_name)
        {
            return GetButtonDown(button_name, 0);
        }

        public static bool GetButtonUp(string button_name)
        {
            return GetButtonUp(button_name, 0);
        }

        public static float GetAxis(string axis_name)
        {
            return GetAxis(axis_name, 0);
        }

        // Multiplayer Input
        public static bool GetButton(string button_name, int player)
        {
            return VXProcess.runtime.GetButton((int)InputController.GetButton(button_name, player), player);
        }

        public static bool GetButtonDown(string button_name, int player)
        {
            return VXProcess.runtime.GetButtonDown((int)InputController.GetButton(button_name, player), player);
        }

        public static bool GetButtonUp(string button_name, int player)
        {
            return VXProcess.runtime.GetButtonUp((int)InputController.GetButton(button_name, player), player);
        }

        public static float GetAxis(string axis_name, int player)
        {
            return VXProcess.runtime.GetAxis((int)InputController.GetAxis(axis_name, player), player);
        }

        public static bool GetMouseButtonDown(string button_name)
        {
            return VXProcess.runtime.GetMouseButtonDown((int)InputController.GetMouseButton(button_name));
        }

        public static bool GetMouseButton(string button_name)
        {
            return VXProcess.runtime.GetMouseButton((int)InputController.GetMouseButton(button_name));
        }

        public static Mouse_Position GetMousePos()
        {
            return new Mouse_Position(VXProcess.runtime.GetMousePosition());
        }
    }
}