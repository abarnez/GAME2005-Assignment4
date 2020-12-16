using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextScene : MonoBehaviour
{
    public Button playBtn;
    void Start()
    {
        playBtn.GetComponent<Button>().onClick.AddListener(OnClick);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnClick()
    {
        SceneManager.LoadScene("Main");
    }
}
