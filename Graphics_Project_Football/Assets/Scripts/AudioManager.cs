using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private AudioSource secondarySource;
    public AudioClip buttonClick,goalH,kickSound;
    private void Awake() {
        if(instance==null)
        {
            instance=this;
        }
        else
        {
            Destroy(gameObject);
        }
        var audioSources=GetComponents<AudioSource>();
        secondarySource=audioSources[1];
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void playButtonClick()
    {
        secondarySource.PlayOneShot(buttonClick);
    }
    public void playGoalH()
    {
        secondarySource.PlayOneShot(goalH);
    }
    public void playKickSound()
    {
        secondarySource.PlayOneShot(kickSound);
    }
}
