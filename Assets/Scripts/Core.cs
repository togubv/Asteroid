using UnityEngine;

public class Core : MonoBehaviour
{ 
    public PlayerControl PlayerControl => playerControl;

    public delegate void ChangePlayerControlHandler(PlayerControl type);
    public event ChangePlayerControlHandler ChangePlayerControlHandlerEvent;

    private PlayerControl playerControl;

    private void Start()
    {
        PauseGame(true);
    }

    public void ChangePlayerControl(PlayerControl type)
    {
        playerControl = type;
        ChangePlayerControlHandlerEvent?.Invoke(type);
    }

    public void PauseGame(bool toggle)
    {
        if (toggle == true)
        {
            Time.timeScale = 0;
            return;
        }
        Time.timeScale = 1;
    }
}

public enum BorderAxis
{
    x,
    y
}

public enum PlayerControl
{
    Keyboard,
    MouseAndKeyboard,
}

public interface IKillable
{
    void Kill(Sender sender);
}

public enum Sender
{
    None,
    Player,
}

public enum AsteroidSize
{
    Large,
    Medium,
    Small
}