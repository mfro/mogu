using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string name;

    [TextArea(3, 10)] public string[] sentences;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager dialogueManager;

    private Queue<Dialogue> dialogues;

    // Start is called before the first frame update
    void Start()
    {
        if (dialogueManager == null)
        {
            dialogueManager = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        dialogues = new Queue<Dialogue>();
    }

    public void AddDialogueToQueue(Dialogue dialogue)
    {
        dialogues.Enqueue(dialogue);
    }


}
