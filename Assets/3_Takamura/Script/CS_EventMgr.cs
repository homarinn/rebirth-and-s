using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_EventMgr : MonoBehaviour
{
    [SerializeField, Header("次のシーン名")]
    string nextScene;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }

    //! test : ボタンクリックシーン遷移
    public void OnClickButton()
    {
        SceneManager.LoadScene(nextScene);
    }

    //! @brief アプリケーション終了
    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
