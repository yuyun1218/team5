using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private bool hasTriggered = false;

    private void Update()
    {
        if (Input.GetKey(KeyCode.E) && !hasTriggered)
        {
            CameraManager.Instance.ToggleNPCView();
            hasTriggered = true;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            hasTriggered = false;
        }
    }
}