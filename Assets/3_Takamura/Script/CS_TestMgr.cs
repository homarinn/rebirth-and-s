using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_TestMgr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //! test : �{�^���N���b�N�V�[���J��
    public void OnClickButton()
    {
        SceneManager.LoadScene("Stage01Scene");
    }
}