using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

public enum PlayerInput
{
    Gamepad,
    Keyboard
}

public class PlayerButtons : MonoBehaviour
{
    public PlayerInput playerInput;

    public GamepadButton attackButton;
    public Key attackKey;

    public GamepadButton rollButton;
    public Key rollKey;

    public GamepadButton throwButton;
    public Key throwKey;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        attackButton = GamepadButton.West;
        throwButton = GamepadButton.North;
        rollButton = GamepadButton.East;

        attackKey = Key.J;
        throwKey = Key.U;
        rollKey = Key.Space;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
