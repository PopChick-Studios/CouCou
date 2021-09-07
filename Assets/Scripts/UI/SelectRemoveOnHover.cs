using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectRemoveOnHover : MonoBehaviour
{
    private PlayerInputActions playerInputActions;

    private bool changeInput;
    public Button defaultButton;


    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.UI.Navigate.performed += x => OnNavigate(false);
        playerInputActions.UI.Point.performed += x => OnNavigate(true);
    }

    public void OnNavigate(bool mouse)
    {
        if (changeInput != mouse)
        {
            if (mouse)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
            else
            {
                defaultButton.Select();
            }
        }
        changeInput = mouse;
    }

    #region - Enable / Disable -

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }

    #endregion
}
