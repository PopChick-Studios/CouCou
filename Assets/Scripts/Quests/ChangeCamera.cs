using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
    public Animator playerAnimator;
    public GameObject mainCamera;
    public GameObject tortureRoomDoorCamera;
    public GameObject docksCamera;
    public Animator crossfadeAnimator;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        //StartCoroutine(SwitchToCamera(diggleTownCamera));
    }

    public IEnumerator SwitchToCamera(GameObject camera)
    {
        playerAnimator.SetBool("isCrouching", false);
        playerAnimator.SetBool("isRunning", false);
        playerAnimator.SetBool("isWalking", false);
        gameManager.SetState(GameManager.GameState.Dialogue);
        crossfadeAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(1.5f);
        mainCamera.SetActive(false);
        camera.SetActive(true);
        crossfadeAnimator.SetTrigger("Reset");
        yield return new WaitForSeconds(3f);
        crossfadeAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(1.5f);
        camera.SetActive(false);
        mainCamera.SetActive(true);
        crossfadeAnimator.SetTrigger("Reset");
        gameManager.SetState(GameManager.GameState.Wandering);
    }
}
