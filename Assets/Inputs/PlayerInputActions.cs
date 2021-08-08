// GENERATED AUTOMATICALLY FROM 'Assets/Inputs/PlayerInputActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerInputActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputActions"",
    ""maps"": [
        {
            ""name"": ""Wandering"",
            ""id"": ""6bca3e22-c38c-4b5a-8def-ee36467eb328"",
            ""actions"": [
                {
                    ""name"": ""CrouchStart"",
                    ""type"": ""Button"",
                    ""id"": ""724ec2f6-2fcd-4655-ab56-4d6f206655ad"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CrouchFinish"",
                    ""type"": ""Button"",
                    ""id"": ""594e3fb6-020f-4b5c-8598-cd8a75c696d2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""3b64ec8d-ad15-4d15-a16e-54ebdf56a63b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""9afc9d90-2729-4b5b-b6c6-633049c35447"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CameraMouse"",
                    ""type"": ""Value"",
                    ""id"": ""ebccaa18-fd45-4e87-9890-24b3e66204fa"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CameraJoystick"",
                    ""type"": ""Value"",
                    ""id"": ""01c66133-4e94-4774-9b2c-5226f466a482"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Value"",
                    ""id"": ""219ceb21-b594-4e20-b21b-8da2ad3b362a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""9ca58a6f-7c3e-45cb-a5ad-7a2822134315"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""CameraMouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""346c749b-2dd9-45d1-9161-ad4169d4952b"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""17973f5f-e47a-4c7b-99a5-d25928f2e0b2"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""d4fe9d3b-63fa-4626-bb95-36d16ab2f08d"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f7f8fe11-183b-47e5-9ef8-3f7053a54233"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""8ed9fe99-8164-48b4-af8f-6ad142ecb12a"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8e84c724-b110-4e0e-be89-bc9c5afb3427"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""12bfe15d-b0d8-455a-be97-216620f3d7c9"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""f9060acc-68bc-4f5d-a328-94b411abf9df"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8eda96aa-fbb9-437c-84ab-81d9b838ae45"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""CrouchStart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""259cbcd6-2949-4040-9bfb-418da1c277b4"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""744161b0-e952-487f-b9f4-114a2b90adb0"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""70549e0d-677c-42c7-8ce5-56cab361ecd5"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""CameraJoystick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""69f663fa-da59-45f1-8192-8c5ba61ac14b"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": ""Press(behavior=1)"",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""CrouchFinish"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KeyboardAndMouse"",
            ""bindingGroup"": ""KeyboardAndMouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Controller"",
            ""bindingGroup"": ""Controller"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Wandering
        m_Wandering = asset.FindActionMap("Wandering", throwIfNotFound: true);
        m_Wandering_CrouchStart = m_Wandering.FindAction("CrouchStart", throwIfNotFound: true);
        m_Wandering_CrouchFinish = m_Wandering.FindAction("CrouchFinish", throwIfNotFound: true);
        m_Wandering_Movement = m_Wandering.FindAction("Movement", throwIfNotFound: true);
        m_Wandering_Interact = m_Wandering.FindAction("Interact", throwIfNotFound: true);
        m_Wandering_CameraMouse = m_Wandering.FindAction("CameraMouse", throwIfNotFound: true);
        m_Wandering_CameraJoystick = m_Wandering.FindAction("CameraJoystick", throwIfNotFound: true);
        m_Wandering_Pause = m_Wandering.FindAction("Pause", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Wandering
    private readonly InputActionMap m_Wandering;
    private IWanderingActions m_WanderingActionsCallbackInterface;
    private readonly InputAction m_Wandering_CrouchStart;
    private readonly InputAction m_Wandering_CrouchFinish;
    private readonly InputAction m_Wandering_Movement;
    private readonly InputAction m_Wandering_Interact;
    private readonly InputAction m_Wandering_CameraMouse;
    private readonly InputAction m_Wandering_CameraJoystick;
    private readonly InputAction m_Wandering_Pause;
    public struct WanderingActions
    {
        private @PlayerInputActions m_Wrapper;
        public WanderingActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @CrouchStart => m_Wrapper.m_Wandering_CrouchStart;
        public InputAction @CrouchFinish => m_Wrapper.m_Wandering_CrouchFinish;
        public InputAction @Movement => m_Wrapper.m_Wandering_Movement;
        public InputAction @Interact => m_Wrapper.m_Wandering_Interact;
        public InputAction @CameraMouse => m_Wrapper.m_Wandering_CameraMouse;
        public InputAction @CameraJoystick => m_Wrapper.m_Wandering_CameraJoystick;
        public InputAction @Pause => m_Wrapper.m_Wandering_Pause;
        public InputActionMap Get() { return m_Wrapper.m_Wandering; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(WanderingActions set) { return set.Get(); }
        public void SetCallbacks(IWanderingActions instance)
        {
            if (m_Wrapper.m_WanderingActionsCallbackInterface != null)
            {
                @CrouchStart.started -= m_Wrapper.m_WanderingActionsCallbackInterface.OnCrouchStart;
                @CrouchStart.performed -= m_Wrapper.m_WanderingActionsCallbackInterface.OnCrouchStart;
                @CrouchStart.canceled -= m_Wrapper.m_WanderingActionsCallbackInterface.OnCrouchStart;
                @CrouchFinish.started -= m_Wrapper.m_WanderingActionsCallbackInterface.OnCrouchFinish;
                @CrouchFinish.performed -= m_Wrapper.m_WanderingActionsCallbackInterface.OnCrouchFinish;
                @CrouchFinish.canceled -= m_Wrapper.m_WanderingActionsCallbackInterface.OnCrouchFinish;
                @Movement.started -= m_Wrapper.m_WanderingActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_WanderingActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_WanderingActionsCallbackInterface.OnMovement;
                @Interact.started -= m_Wrapper.m_WanderingActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_WanderingActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_WanderingActionsCallbackInterface.OnInteract;
                @CameraMouse.started -= m_Wrapper.m_WanderingActionsCallbackInterface.OnCameraMouse;
                @CameraMouse.performed -= m_Wrapper.m_WanderingActionsCallbackInterface.OnCameraMouse;
                @CameraMouse.canceled -= m_Wrapper.m_WanderingActionsCallbackInterface.OnCameraMouse;
                @CameraJoystick.started -= m_Wrapper.m_WanderingActionsCallbackInterface.OnCameraJoystick;
                @CameraJoystick.performed -= m_Wrapper.m_WanderingActionsCallbackInterface.OnCameraJoystick;
                @CameraJoystick.canceled -= m_Wrapper.m_WanderingActionsCallbackInterface.OnCameraJoystick;
                @Pause.started -= m_Wrapper.m_WanderingActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_WanderingActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_WanderingActionsCallbackInterface.OnPause;
            }
            m_Wrapper.m_WanderingActionsCallbackInterface = instance;
            if (instance != null)
            {
                @CrouchStart.started += instance.OnCrouchStart;
                @CrouchStart.performed += instance.OnCrouchStart;
                @CrouchStart.canceled += instance.OnCrouchStart;
                @CrouchFinish.started += instance.OnCrouchFinish;
                @CrouchFinish.performed += instance.OnCrouchFinish;
                @CrouchFinish.canceled += instance.OnCrouchFinish;
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @CameraMouse.started += instance.OnCameraMouse;
                @CameraMouse.performed += instance.OnCameraMouse;
                @CameraMouse.canceled += instance.OnCameraMouse;
                @CameraJoystick.started += instance.OnCameraJoystick;
                @CameraJoystick.performed += instance.OnCameraJoystick;
                @CameraJoystick.canceled += instance.OnCameraJoystick;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
            }
        }
    }
    public WanderingActions @Wandering => new WanderingActions(this);
    private int m_KeyboardAndMouseSchemeIndex = -1;
    public InputControlScheme KeyboardAndMouseScheme
    {
        get
        {
            if (m_KeyboardAndMouseSchemeIndex == -1) m_KeyboardAndMouseSchemeIndex = asset.FindControlSchemeIndex("KeyboardAndMouse");
            return asset.controlSchemes[m_KeyboardAndMouseSchemeIndex];
        }
    }
    private int m_ControllerSchemeIndex = -1;
    public InputControlScheme ControllerScheme
    {
        get
        {
            if (m_ControllerSchemeIndex == -1) m_ControllerSchemeIndex = asset.FindControlSchemeIndex("Controller");
            return asset.controlSchemes[m_ControllerSchemeIndex];
        }
    }
    public interface IWanderingActions
    {
        void OnCrouchStart(InputAction.CallbackContext context);
        void OnCrouchFinish(InputAction.CallbackContext context);
        void OnMovement(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnCameraMouse(InputAction.CallbackContext context);
        void OnCameraJoystick(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
    }
}
