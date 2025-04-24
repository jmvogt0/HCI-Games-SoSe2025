using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    public ChatHandler ChatHandler;
    private ChatContext _chatContext;


    // Start is called before the first frame update
    void Start()
    {
        _chatContext = ChatHandler.StartConversation();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == this.transform)
            {
                _chatContext.EnqueueCompletion("Hello", _handleOpenAiResult);
            }
        }
    }

    private void _handleOpenAiResult(string result)
    {
        Debug.Log(result);
    }
}
